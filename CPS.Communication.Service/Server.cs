using CPS.Communication.Service.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CPS.Communication.Service.Exceptions;
using CPS.Communication.Service.State;
using CPS.Infrastructure.Utils;
using CPS.Communication.Service.DataPackets;
using System.Collections;

namespace CPS.Communication.Service
{
    public class Server : IDisposable
    {
        private Socket _listener;
        private bool _closing = false;
        private bool _closed = false;
        private ClientCollection _clients;
        private IChargingPileService MyChargingService = ChargingService.Instance;

        #region 心跳检测
        private const int HeartBeatInterval = 15; // 心跳间隔15秒
        private const int HeartBeatCheckInterval = 60; // 心跳检测间隔60秒
        private object heartbeatCheckLocker = new object();
        private bool stopHeartbeatCheck = false;
        private object heartbeatServerLocker = new object();
        private bool stopHeartbeatServer = false;
        /// <summary>
        /// 心跳检测线程
        /// </summary>
        Thread ThreadHeartbeatDetection;
        /// <summary>
        /// 服务端发送心跳包线程
        /// </summary>
        Thread ThreadHeartbeatFromServer;

        #endregion 心跳检测

        public bool IsRunning
        {
            get
            {
                return !_closing && !_closed;
            }
        }

        public IPAddress IP { get; private set; }
        public int Port { get; private set; }
        public ClientCollection Clients
        {
            get
            {
                return _clients;
            }
        }

        public Server()
        {
            _clients = new ClientCollection();
            MyChargingService.MyServer = this;

            StartHeartbeatCheck();
            SendHeartbeatFromServer();
        }

        public void Listen(int port)
        {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(string localIp, int port)
        {
            try
            {
                IPAddress ip = IPAddress.Parse(localIp);

                IP = ip;
                Port = port;

                Listen(new IPEndPoint(ip, port));
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine($"无法启动服务器，IP地址不能为空...\n详细信息：{ane.Message}");
            }
            catch (FormatException fe)
            {
                Console.WriteLine($"无法启动服务器，非法的IP地址...\n详细信息：{fe.Message}");
            }
        }

        private void Listen(IPEndPoint ep)
        {
            try
            {
                StopListen();
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Bind(ep);
                s.Listen(int.MaxValue);
                _listener = s;
                _listener.BeginAccept(acceptCallback, _listener);

                OnServerStarted(new ServerStartedEventArgs());
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.ServerStart);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 启动心跳检测
        /// </summary>
        private void StartHeartbeatCheck()
        {
            ThreadHeartbeatDetection = new Thread(() =>
            {
                while (true)
                {
                    long time = DateTime.Now.Ticks;
                    lock (heartbeatCheckLocker)
                    {
                        if (!ThreadHeartbeatDetection.IsAlive || stopHeartbeatCheck)
                            break;
                        try
                        {
                            DateTime now = DateTime.Now;
                            List<Client> outdated = new List<Client>();
                            foreach (var item in this._clients)
                            {
                                if (item != null)
                                {
                                    DateTime t1 = item.ActiveDate;

                                    if ((now - t1).TotalSeconds > HeartBeatCheckInterval)
                                    {
                                        outdated.Add(item);
                                    }
                                }
                            }

                            foreach (var item in outdated)
                            {
                                this._clients.RemoveClient(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    Thread.Sleep(HeartBeatCheckInterval * 1000);
                }
            })
            { IsBackground = true };
            ThreadHeartbeatDetection.Start();
        }

        private void SendHeartbeatFromServer()
        {
            ThreadHeartbeatFromServer = new Thread(() =>
            {
                while (true)
                {
                    if (!ThreadHeartbeatFromServer.IsAlive || stopHeartbeatServer)
                        break;

                    lock (heartbeatServerLocker)
                    {
                        HeartBeatPacket packet = new HeartBeatPacket(PacketTypeEnum.HeartBeatServer);
                        foreach (var item in this._clients)
                        {
                            if (item != null)
                            {
                                packet.TimeStamp = DateTime.Now.ConvertToTimeStampX();
                                packet.SerialNumber = item.SerialNumber;

                                if (item.IsConnected)
                                    item.Send(packet);
                            }
                        }
                    }

                    Thread.Sleep(HeartBeatInterval * 1000);
                }
            })
            { IsBackground = true };
            ThreadHeartbeatFromServer.Start();
        }

        private object _closeObj = new object();
        public void Close()
        {
            new Thread(StopListen)
            {
                IsBackground = true
            }.Start();
            _closed = true;
        }

        private void StopListen()
        {
            _closing = true;
            lock (_closeObj)
            {
                if (_listener != null && IsRunning)
                {
                    _listener.Close();
                    _listener = null;

                    OnServerStopped(new ServerStoppedEventArgs());
                }
                
                this._clients.ClearClient();
            }
            _closing = false;
        }

        private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                if (!IsRunning)
                    return;
                Socket s = ar.AsyncState as Socket;
                Socket wSocket = s.EndAccept(ar);
                // 引发事件
                Client client = new Client(wSocket);
                client.ReceiveCompleted += Client_ReceiveCompleted;
                client.SendCompleted += Client_SendCompleted;
                client.SendDataException += Client_SendDataException;
                client.ErrorOccurred += Client_ErrorOccurred;
                client.ClientClosed += Client_ClientClosed;

                client.Receive();

                OnClientAccepted(new ClientAcceptedEventArgs(client));
                // 保存到客户端列表
                _clients.AddClient(client);

                s.BeginAccept(acceptCallback, s);
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.SocketAccept);
            }
        }

        public bool Send(Client client, PacketBase packet)
        {
            try
            {
                if (this._clients == null || this._clients.Count <= 0)
                    throw new ArgumentNullException("尚未有充电桩连接到服务器...");
                if (client == null)
                    throw new ArgumentNullException("充电桩不能为空...");

                return client.Send(packet);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine(ane.Message);
            }
            catch (UnConnectException uce)
            {
                Console.WriteLine(uce.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public void SendAll(PacketBase packet)
        {
            if (packet == null) return;
            byte[] buffer = PacketAnalyzer.GeneratePacket(packet);
            SendAll(buffer);
        }

        public void SendAll(byte[] buffer)
        {
            foreach (var item in this._clients)
            {
                item.Send(buffer);
            }
        }

        private void Client_ErrorOccurred(object sender, ErrorEventArgs args)
        {
            if (args != null)
            {
                //Logger.Instance.Error(args.ToString());
                Console.WriteLine($"####{args.ToString()}");
            }
        }

        private void Client_SendCompleted(object sender, SendCompletedEventArgs args)
        {
            //if (args != null)
            //{
            //    Console.WriteLine($"此次发送的数据长度：{args.Len}");
            //}
        }

        private void Client_SendDataException(object sender, SendDataExceptionEventArgs args)
        {
            try
            {
                Client client = sender as Client;
                if (this._clients != null)
                {
                    Console.WriteLine($"发送数据异常，关闭远程客户端：{client.ID}");
                    this._clients.RemoveClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Client_ReceiveCompleted(object sender, ReceiveCompletedEventArgs args)
        {
            try
            {
                Client client = sender as Client;
                if (client == null)
                    throw new ArgumentNullException("client is null...");
                // 解析数据包
                PacketBase packet = PacketAnalyzer.AnalysePacket(args.ReceivedBytes);

                DateTime now = DateTime.Now;
                if (packet.Command == PacketTypeEnum.HeartBeatClient)
                {
                    if (client != null)
                    {
                        client.ActiveDate = now;
                    }
                }
                else
                {
                    MyChargingService.ServiceFactory(client, packet);
                }
            }
            catch (SocketException se)
            {
                handleSocketException(se, ErrorTypes.Receive);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Client_ClientClosed(object sender, ClientClosedEventArgs args)
        {
            Console.WriteLine($"----客户端 {args.Id} 断开连接!");
        }

        private void handleSocketException(SocketException se, ErrorTypes eType)
        {
            OnErrorOccurred(new ErrorEventArgs(se.Message, eType, se.ErrorCode, se));
        }

        public Client FindClientBySerialNumber(string serialNumber)
        {
            try
            {
                return this._clients.Where(_ => _.SerialNumber == serialNumber).First();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine($"该充电桩不存在：{ane.Message}");
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public override string ToString()
        {
            string info = $"本地端点：{this._listener.LocalEndPoint}\n";
            if (this._clients != null)
            {
                info += $"当前客户端数：{this._clients.Count} 个\n";
                int normal = 0, unnormal = 0;
                int logined = 0, unlogined = 0;
                string cInfo = "";
                int index = 1;
                foreach (var item in this._clients)
                {
                    if (item != null)
                    {
                        if (item.IsConnected)
                            normal++;
                        else
                            unnormal++;

                        if (item.HasLogined)
                            logined++;
                        else
                            unlogined++;

                        cInfo += $"客户端{index++}：\n";
                        cInfo += item.ToString();
                    }
                }

                info += $"已连接客户端数：{normal} / 未连接客户端数：{unnormal}\n";
                info += $"已登录客户端数：{logined} / 未登录客户端数：{unlogined}\n";
                if (!string.IsNullOrEmpty(cInfo))
                {
                    info += $"客户端列表：\n";
                    info += cInfo;
                }
            }
            return info;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        StopListen();

                        stopHeartbeatCheck = true;
                        ThreadHeartbeatDetection = null;
                    }
                    catch (SocketException se)
                    {
                        handleSocketException(se, ErrorTypes.ServerStop);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                disposed = true;
            }
        }

        #region 【事件定义】
        public event ErrorOccurredHandler ErrorOccurred;
        public event ClientAcceptedHandler ClientAccepted;
        public event ServerStartedHandler ServerStarted;
        public event ServerStoppedHandler ServerStopped;

        private void OnErrorOccurred(ErrorEventArgs args)
        {
            ErrorOccurredHandler handler = ErrorOccurred;
            if (handler != null)
                handler(this, args);
        }

        private void OnClientAccepted(ClientAcceptedEventArgs args)
        {
            ClientAcceptedHandler handler = ClientAccepted;
            if (handler != null)
                handler(this, args);
        }

        private void OnServerStarted(ServerStartedEventArgs args)
        {
            ServerStartedHandler handler = ServerStarted;
            if (handler != null)
                handler(this, args);
        }

        private void OnServerStopped(ServerStoppedEventArgs args)
        {
            ServerStoppedHandler handler = ServerStopped;
            if (handler != null)
                handler(this, args);
        }
        #endregion 【事件定义】
    }
}
