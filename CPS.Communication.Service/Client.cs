using CPS.Communication.Service.DataPackets;
using CPS.Communication.Service.Events;
using CPS.Communication.Service.Exceptions;
using CPS.Communication.Service.State;
using CPS.Infrastructure.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.Communication.Service
{
    public class Client : IDisposable
    {
        public Client() { }

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

        private string _serialNumber = "";
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
        private object closeLocker = new object();
        public void Close()
        {
            new Thread(() =>
            {
                lock (closeLocker)
                {
                    if (_socket != null)
                    {
                        var id = this.ID;
                        _socket.Close();
                        OnClientClosed(new ClientClosedEventArgs(id));
                    }
                    _socket = null;
                }
            })
            { IsBackground = true }.Start();
            _closed = true;
        }

        public bool Send(PacketBase packet)
        {
            if (packet == null) return false;
            byte[] buffer = PacketAnalyzer.GeneratePacket(packet);
            return Send(buffer);
        }

        public bool Send(byte[] buffer)
        {
            SendState state = new SendState()
            {
                WorkSocket = _socket
            };
            try
            {
                if (!IsConnected)
                    throw new UnConnectException("充电桩未连接");

                WorkSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, sendCallback, state);
                return true;
            }
            catch (SocketException se)
            {
                OnSendDataException(new SendDataExceptionEventArgs(this));
                handleSocketException(se, ErrorTypes.Send);
            }
            catch (ObjectDisposedException ode)
            {
                Console.WriteLine(ode.Message);
            }
            catch (UnConnectException)
            {
                //OnClientClosed(new ClientClosedEventArgs(""));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
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
                OnSendDataException(new SendDataExceptionEventArgs(this));
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
                            Close();
                            OnErrorOccurred(new ErrorEventArgs("连续接收到无效的空白信息，可能由于远端连接已异常关闭，连接自动退出！", ErrorTypes.SocketAccept));
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
                        Close();
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
                    if (_socket != null)
                        _socket.Close();
                    _socket = null;
                }
                disposed = true;
            }
        }

        #region 【事件定义】
        public event ErrorOccurredHandler ErrorOccurred;
        public event SendCompletedHandler SendCompleted;
        public event SendDataExceptionHandler SendDataException;
        public event ReceiveCompletedHandler ReceiveCompleted;
        public event ClientClosedHandler ClientClosed;

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

        public void ClearClient()
        {
            if (this._clients != null && this._clients.Count > 0)
            {
                foreach (var item in this._clients)
                {
                    if (item != null)
                        item.Close();
                }

                this._clients.Clear();
            }
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
            if (client == null) return;

            _mEventClients.Reset();

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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ClearClient();
                    this._clients = null;
                }
                disposed = true;
            }
        }
    }
}
