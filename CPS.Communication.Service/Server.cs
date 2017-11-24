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

namespace CPS.Communication.Service
{
    public class Server
    {
        private Socket _listener;
        private bool _closing = false;
        private bool _closed = false;
        private ClientCollection _clients;

        public Server()
        {
            _clients = new ClientCollection();
        }

        public void Listen(int port)
        {
            Listen(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(string localIp, int port)
        {
            IPAddress ip = IPAddress.Parse(localIp);
            Listen(new IPEndPoint(ip, port));
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
                if (_listener != null)
                    _listener.Close();
                _listener = null;

                OnServerStopped(new ServerStoppedEventArgs());
            }
            _closing = false;
        }

        private void acceptCallback(IAsyncResult ar)
        {
            try
            {
                if (_closed || _closing)
                    return;
                Socket s = ar.AsyncState as Socket;
                Socket wSocket = s.EndAccept(ar);
                // 引发事件
                Client client = new Client(wSocket);
                client.ReceiveCompleted += Client_ReceiveCompleted;
                client.SendCompleted += Client_SendCompleted;
                client.ErrorOccurred += Client_ErrorOccurred;
                client.Receive();

                OnClientAccepted(new ClientAcceptedEventArgs());
                // 保存到客户端列表
                _clients.AddClient(client);

                s.BeginAccept(acceptCallback, s);
            }
            catch (SocketException se)
            {
                if (_closed || _closing)
                    return;
                handleSocketException(se, ErrorTypes.SocketAccept);
            }
        }

        private void Client_ErrorOccurred(object sender, ErrorEventArgs args)
        {
        }

        private void Client_SendCompleted(object sender, SendCompletedEventArgs args)
        {
        }

        private void Client_ReceiveCompleted(object sender, ReceiveCompletedEventArgs args)
        {
            PacketBase packet = new PacketBase();
            packet = packet.AnalysePacket(args.ReceivedBytes);

        }

        private void handleSocketException(SocketException se, ErrorTypes eType)
        {
            OnErrorOccurred(new ErrorEventArgs(se.Message, eType, se.ErrorCode, se));
        }

        public override string ToString()
        {
            //打印 IP/Port/在线客户端数 等信息
            return base.ToString();
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
        public class Client
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

            private string _serialNumber;
            /// <summary>
            /// 充电桩序列号
            /// </summary>
            public string SerialNumber
            {
                get { return _serialNumber; }
                set { _serialNumber = value; }
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

            private bool _closed = false;
            private object _closeObj = new object();
            private int _emptyTimes;

            public void Close()
            {
                new Thread(() =>
                {
                    lock (_closeObj)
                    {
                        if (_socket != null)
                        {
                            _socket.Close();
                        }
                        _socket = null;
                    }
                })
                { IsBackground = true }.Start();
                _closed = true;
            }

            public void Send(PacketBase packet)
            {

            }

            public void Send(byte[] buffer)
            {
                SendState state = new SendState()
                {
                    WorkSocket = _socket
                };
                try
                {
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
                    WorkSocket.BeginReceive(state.Buffer, 0, ReceiveState.BufferSize, SocketFlags.None, receiveCallback, state);
                }
                catch (SocketException se)
                {
                    handleSocketException(se, ErrorTypes.Receive);
                }
            }

            private void receiveCallback(IAsyncResult ar)
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
                            OnErrorOccurred(new ErrorEventArgs("连续接收到无效的空白信息，可能由于远端连接已异常关闭，连接自动退出！", ErrorTypes.SocketAccept));
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
                            // 解析数据包
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
                // 打印充电桩编号、充电桩机器 等信息
                return base.ToString();
            }

            #region 【事件定义】
            public event ErrorOccurredHandler ErrorOccurred;
            public event SendCompletedHandler SendCompleted;
            public event ReceiveCompletedHandler ReceiveCompleted;

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

            private void OnReceiveCompleted(ReceiveCompletedEventArgs args)
            {
                ReceiveCompletedHandler handler = ReceiveCompleted;
                if (handler != null)
                    handler(this, args);
            }
            #endregion 【事件定义】
        }

        public class ClientCollection
        {
            private List<Client> _clients = null;
            private ManualResetEvent _mEventClients = new ManualResetEvent(true);
            
            public ClientCollection()
            {
                _clients = new List<Client>();
            }

            public List<Client> Clients { get { return _clients; } }
            public void CloseClient(Client client)
            {
                client.Close();
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

                var handle = client.Handle;
                foreach (var item in _clients)
                {
                    if (item.Handle == handle)
                    {
                        _clients.Remove(item);
                    }
                }

                _mEventClients.Set();
            }
        }
        #endregion 【客户端】
    }
}
