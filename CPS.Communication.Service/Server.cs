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

        public Server()
        {
            _clients = new ClientCollection();

            StartHeartbeatCheck();
            StartHeartbeatFromServer();
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

        private void StartHeartbeatFromServer()
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

                foreach (var item in this._clients)
                {
                    if (item != null)
                    {
                        item.Close();
                    }
                }
                this._clients.Clear();
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
                client.ClientDisconnected += Client_ClientDisconnected;
                client.Receive();

                OnClientAccepted(new ClientAcceptedEventArgs(client));
                // 保存到客户端列表
                _clients.AddClient(client);

                s.BeginAccept(acceptCallback, s);
            }
            catch (SocketException se)
            {
                if (!IsRunning)
                    return;
                handleSocketException(se, ErrorTypes.SocketAccept);
            }
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

        }

        private void Client_SendDataException(object sender, SendDataExceptionEventArgs args)
        {
            try
            {
                Server.Client client = sender as Client;
                if (this._clients != null)
                {
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
                Server.Client client = sender as Client;
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
            
        }

        private void Client_ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            if (args != null && args.CurClient != null)
            {
                Console.WriteLine($"----Client {args.CurClient.ID} has disconnected!");
            }
        }

        private void handleSocketException(SocketException se, ErrorTypes eType)
        {
            OnErrorOccurred(new ErrorEventArgs(se.Message, eType, se.ErrorCode, se));
        }

        public override string ToString()
        {
            string info = $"本地端点：{this._listener.LocalEndPoint}\n";
            if (this._clients != null)
            {
                info += $"当前客户端数：{this._clients.Count} 个\n";
                int normal = 0, unnormal = 0;
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

                        cInfo += $"客户端{index++}：\n";
                        cInfo += item.ToString();
                    }
                }

                info += $"正常客户端数：{normal} / 非正常客户端数：{unnormal}\n";
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


        #region 【客户端】
        public class Client : IDisposable
        {
            public Client(){}

            public Client(Socket socket)
            {
                _socket = socket;
            }

            private Socket _socket;

            public Socket WorkSocket
            {
                get { return _socket; }
                set { _socket = value; }
            }

            private string _serialNumber="";
            /// <summary>
            /// 充电桩序列号
            /// </summary>
            public string SerialNumber
            {
                get { return _serialNumber; }
                set { _serialNumber = value; }
            }

            public EndPoint LocalEndPoint
            {
                get
                {
                    return _socket.LocalEndPoint;
                }
            }

            public EndPoint RemoteEndPoint
            {
                get
                {
                    return _socket.RemoteEndPoint;
                }
            }

            public DateTime ActiveDate { get; set; }

            public string ID
            {
                get
                {
                    return $"{RemoteEndPoint}:{SerialNumber}";
                }
            }

            private bool hasLogined;
            /// <summary>
            /// 是否已登陆
            /// </summary>
            public bool HasLogined
            {
                get { return hasLogined; }
                set { hasLogined = value; }
            }

            public int Handle
            {
                get
                {
                    if (_socket == null)
                        return int.MinValue;
                    return _socket.Handle.ToInt32();
                }
            }

            public bool IsConnected
            {
                get
                {
                    return _socket != null && _socket.Connected && !_closed;
                }
            }

            public string IsConnectedDesc
            {
                get
                {
                    return this.IsConnected ? "已连接" : "未连接";
                }
            }

            private bool _closed = false;
            private int _emptyTimes;
            private ManualResetEvent _mre = new ManualResetEvent(true);

            public void Disconnect()
            {
                new Thread(() =>
                {
                    _mre.Reset();
                    {
                        OnClientDisconnected(new ClientDisconnectedEventArgs(this));

                        if (_socket != null && IsConnected)
                        {
                            _socket.Disconnect(false);
                        }
                    }
                    _mre.Set();
                })
                { IsBackground = true }.Start();
            }

            public void Close()
            {
                new Thread(() =>
                {
                    _mre.Reset();
                    {
                        OnClientClosed(new ClientClosedEventArgs(this));

                        if (_socket != null)
                        {
                            _socket.Close();
                        }
                        _socket = null;
                    }
                    _mre.Set();
                })
                { IsBackground = true }.Start();
                _closed = true;
            }

            public void Send(PacketBase packet)
            {
                if (packet == null) return;
                byte[] buffer = PacketAnalyzer.GeneratePacket(packet);
                Send(buffer);
            }

            public void Send(byte[] buffer)
            {
                SendState state = new SendState()
                {
                    WorkSocket = _socket
                };
                try
                {
                    if (IsConnected)
                        WorkSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, sendCallback, state);
                }
                catch (SocketException se)
                {
                    handleSocketException(se, ErrorTypes.Send);
                }
                catch (ObjectDisposedException ode)
                {
                    Console.WriteLine(ode.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            private void sendCallback(IAsyncResult ar)
            {
                SendState state = ar.AsyncState as SendState;
                try
                {
                    if (!state.WorkSocket.Connected)
                        throw new UnConnectException("客户端未连接...");

                    int len = state.WorkSocket.EndSend(ar);
                    OnSendCompleted(new SendCompletedEventArgs(len));
                }
                catch (UnConnectException uce)
                {
                    Console.WriteLine(uce.Message);
                }
                catch (SocketException se)
                {
                    //OnSendDataException(new SendDataExceptionEventArgs());
                    handleSocketException(se, ErrorTypes.Send);
                }
                catch (ObjectDisposedException ode)
                {
                    Console.WriteLine(ode.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public void Receive()
            {
                ReceiveState state = new ReceiveState()
                {
                    WorkSocket = _socket
                };
                try
                {
                    if (IsConnected)
                    {
                        WorkSocket.BeginReceive(state.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, receiveCallback, state);
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

            private void receiveCallback(IAsyncResult ar)
            {
                if (IsConnected)
                {
                    ReceiveState state = ar.AsyncState as ReceiveState;
                    Socket s = state.WorkSocket;
                    try
                    {
                        int byteLen = s.EndReceive(ar);
                        if (byteLen == 0)
                        {
                            _emptyTimes++;
                            if (_emptyTimes == 100)
                            {
                                //OnErrorOccurred(new ErrorEventArgs("连续接收到无效的空白信息，可能由于远端连接已异常关闭，连接自动退出！", ErrorTypes.SocketAccept));
                                Close();
                                return;
                            }
                        }
                        else
                        {
                            _emptyTimes = 0;
                        }

                        byte[] buffer = state.Buffer;
                        if (state.UnhandledBytes != null)
                        {
                            buffer = BytesHelper.Combine(state.UnhandledBytes, buffer);
                            byteLen += state.UnhandledBytes.Length;
                            state.UnhandledBytes = null;
                        }

                        Queue<ReceiveState> sQueue = new Queue<ReceiveState>();
                        processReceived(sQueue, state, buffer, 0, byteLen);

                        ReceiveState workingRS = null;
                        while (sQueue.Count > 0)
                        {
                            ReceiveState rs = sQueue.Dequeue();
                            if (rs.Completed)
                            {
                                OnReceiveCompleted(new ReceiveCompletedEventArgs(rs.Received));
                            }
                            else
                            {
                                workingRS = rs;
                                break;
                            }
                        }

                        if (sQueue.Count > 0)
                        {
                            //Close();
                            OnErrorOccurred(new ErrorEventArgs("由于数据包丢失，导致接收数据不完整，连接已关闭！", ErrorTypes.Receive));
                            return;
                        }

                        if (workingRS != null)
                            s.BeginReceive(workingRS.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, receiveCallback, workingRS);
                        else
                            Receive();
                    }
                    catch (SocketException se)
                    {
                        handleSocketException(se, ErrorTypes.Receive);
                    }
                    catch (ObjectDisposedException ode)
                    {
                        Console.WriteLine(ode.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            private void processReceived(Queue<ReceiveState> stateQueue, ReceiveState state, byte[] buffer, int start, int byteLen)
            {
                if (state.IsNew)
                {
                    if (byteLen >= PacketBase.HeaderLen)
                    {
                        int len = BitConverter.ToInt32(buffer, PacketBase.BodyLenIndex);
                        state.TotalBytes = len + PacketBase.HeaderLen;
                    }
                    else
                    {
                        // 当发送端多次发送的字节流拥堵时，接收端会接收到连续的字节数据。
                        // 所以单次接收的数据有可能不完整。
                        byte[] sub = BytesHelper.SubArray(buffer, start, byteLen);
                        state.UnhandledBytes = sub;
                        stateQueue.Enqueue(state);
                        return;
                    }
                }

                int missing = state.TotalBytes - state.ReceivedBytes;
                if (missing >= byteLen)
                {
                    state.AppendBytes(buffer, start, byteLen);
                    stateQueue.Enqueue(state);
                }
                else
                {
                    state.AppendBytes(buffer, start, missing);
                    stateQueue.Enqueue(state);

                    start += missing;
                    byteLen -= missing;
                    ReceiveState newState = new ReceiveState()
                    {
                        WorkSocket = state.WorkSocket
                    };
                    processReceived(stateQueue, newState, buffer, start, byteLen);
                }
            }

            private void handleSocketException(SocketException se, ErrorTypes eType)
            {
                OnErrorOccurred(new ErrorEventArgs(se.Message, eType, se.ErrorCode, se));
            }

            public override string ToString()
            {
                if (this.WorkSocket != null)
                    return $"状态：{this.IsConnectedDesc}，ID：{this.ID}\n";
                else
                    return $"状态：{this.IsConnectedDesc}，ID：<空>\n";
            }

            public void Dispose()
            {
                
            }

            #region 【事件定义】
            public event ErrorOccurredHandler ErrorOccurred;
            public event SendCompletedHandler SendCompleted;
            public event SendDataExceptionHandler SendDataException;
            public event ReceiveCompletedHandler ReceiveCompleted;
            public event ClientClosedHandler ClientClosed;
            public event ClientDisconnectedHandler ClientDisconnected;

            private void OnErrorOccurred(ErrorEventArgs args)
            {
                ErrorOccurredHandler handler = ErrorOccurred;
                if (handler != null)
                    handler(this, args);
            }

            private void OnSendCompleted(SendCompletedEventArgs args)
            {
                SendCompletedHandler handler = SendCompleted;
                if (handler != null)
                    handler(this, args);
            }

            private void OnSendDataException(SendDataExceptionEventArgs args)
            {
                SendDataExceptionHandler handler = SendDataException;
                if (handler != null)
                    handler(this, args);
            }

            private void OnReceiveCompleted(ReceiveCompletedEventArgs args)
            {
                ReceiveCompletedHandler handler = ReceiveCompleted;
                if (handler != null)
                    handler(this, args);
            }

            private void OnClientClosed(ClientClosedEventArgs args)
            {
                ClientClosedHandler handler = ClientClosed;
                if (handler != null)
                    handler(this, args);
            }

            private void OnClientDisconnected(ClientDisconnectedEventArgs args)
            {
                ClientDisconnectedHandler handler = ClientDisconnected;
                if (handler != null)
                    handler(this, args);
            }
            #endregion 【事件定义】
        }

        public class ClientCollection : IEnumerable<Client>, IDisposable
        {
            private List<Client> _clients = null;
            private ManualResetEvent _mEventClients = new ManualResetEvent(true);
            
            public ClientCollection()
            {
                _clients = new List<Client>();
            }

            protected List<Client> Clients { get { return _clients; } }
            public void CloseClient(Client client)
            {
                client.Close();
            }

            public int Count
            {
                get
                {
                    return this._clients == null ? 0 : this._clients.Count;
                }
            }

            public void Clear()
            {
                if (this._clients != null && this._clients.Count > 0)
                    this._clients.Clear();
            }

            public void AddClient(Client client)
            {
                _mEventClients.Reset();

                var handle = client.Handle;
                var result = _clients.Exists(_ => _.Handle == handle);
                if (result)
                {
                    foreach (var item in _clients)
                    {
                        if (item.Handle == handle)
                        {
                            item.Close();
                            _clients.Remove(item);
                        }
                    }
                }
                _clients.Add(client);

                _mEventClients.Set();
            }

            public void RemoveClient(Client client)
            {
                _mEventClients.Reset();

                if (client == null) return;
                client.Close();
                _clients.Remove(client);

                _mEventClients.Set();
            }

            public IEnumerator<Client> GetEnumerator()
            {
                return this._clients.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this._clients.GetEnumerator();
            }

            public void Dispose()
            {

            }
        }
        #endregion 【客户端】
    }
}
