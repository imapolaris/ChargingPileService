using CPS.Communication.Service;
using CPS.Communication.Service.DataPackets;
using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPS.Communication.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Service.Client client = null;
        IPEndPoint ep;
        private bool stopHeartbeat = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Client_ErrorOccurred(object sender, Service.Events.ErrorEventArgs args)
        {
            appendText("error", $"####{args}");
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new Service.Client(socket);
            string txtip = this.txtIP.Text.Trim();
            string txtport = this.txtPort.Text.Trim();
            IPAddress ip = IPAddress.Parse(txtip);
            ep = new IPEndPoint(ip, int.Parse(txtport));
            client.ReceiveCompleted += Client_ReceiveCompleted;
            client.SendCompleted += Client_SendCompleted;
            client.ErrorOccurred += Client_ErrorOccurred;
            client.ClientClosed += Client_ClientClosed;
            client.WorkSocket.Connect(ep);
        }

        private void Client_ClientClosed(object sender, Service.Events.ClientClosedEventArgs args)
        {
            appendText("close", "客户端关闭连接...");
        }

        private void Client_SendCompleted(object sender, Service.Events.SendCompletedEventArgs args)
        {
            appendText("send", "已发送...");
        }

        Random random = new Random();
        private void Client_ReceiveCompleted(object sender, Service.Events.ReceiveCompletedEventArgs args)
        {
            PacketBase packet = PacketAnalyzer.AnalysePacket(args.ReceivedBytes);
            if (packet.Command == PacketTypeEnum.HeartBeatServer)
            {
                HeartBeatPacket lrpacket = packet as HeartBeatPacket;
                appendText("Heartbeat", $"sn:{packet.SerialNumber}, timestamp:{lrpacket.TimeStamp}");
            }
            else if (packet.Command == PacketTypeEnum.LoginResult)
            {
                LoginResultPacket lrpacket = packet as LoginResultPacket;
                appendText("LoginResult", $"sn:{packet.SerialNumber}, result:{lrpacket.ResultEnum.ToString()}, timestamp:{lrpacket.TimeStamp}");
            }
            else if (packet.Command == PacketTypeEnum.Reboot)
            {
                RebootPacket rpacket = packet as RebootPacket;
                appendText("reboot", $"正在重启充电桩...");
                
                byte number = (byte)(random.Next() % 2 + 1);
                appendText("rebootResult", $"重启结果：" + number);
                client.Send(new RebootResultPacket()
                {
                    SerialNumber = rpacket.SerialNumber,
                    OperType = rpacket.OperType,
                    Result = number
                });
            }

            if (packet.Command == PacketTypeEnum.SetElecPrice)
            {
                SetElecPricePacket seppacket = packet as SetElecPricePacket;
                appendText("设置电价：", $"尖：{seppacket.SharpRate}峰：{seppacket.PeakRate}平：{seppacket.FlatRate}谷：{seppacket.Valleyrate}");
                client.Send(new byte[4]);
            }
        }

        private void btnDisConnect_Click(object sender, RoutedEventArgs e)
        {
            client.Close();
        }

        private void appendText(string msgType, string txt)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                document.Document.Blocks.Add(new Paragraph(new Run(string.Format("{0}:{1}", msgType, txt))));
                document.ScrollToEnd();
            }), null);
        }

        /// <summary>
        /// 登录
        /// </summary>
        private void login_Click(object sender, RoutedEventArgs e)
        {
            LoginPacket packet = new LoginPacket()
            {
                SerialNumber = "1234567890AbcBCa",
                TimeStamp = DateTime.Now.ConvertToTimeStampX(),
                Username = "alex",
                Pwd = "123"
            };
            client.Send(PacketAnalyzer.GeneratePacket(packet));

            client.Receive();
        }

        /// <summary>
        /// 心跳
        /// </summary>
        private void heartBeat_Click(object sender, RoutedEventArgs e)
        {
            if (this.heartBeat.Content.ToString() == "开启心跳")
            {
                this.heartBeat.Content = "结束心跳";

                HeartBeatPacket packet = new HeartBeatPacket(PacketTypeEnum.HeartBeatClient)
                {
                    SerialNumber = "1234567890AbcBCa",
                    TimeStamp = DateTime.Now.ConvertToTimeStampX(),
                };

                new Thread(() =>
                {
                    while (true)
                    {
                        if (stopHeartbeat)
                            break;

                        client.Send(packet);
                        Thread.Sleep(15 * 1000);
                    }
                })
                { IsBackground = true }
                .Start();
            }
            else
            {
                stopHeartbeat = true;
                this.heartBeat.Content = "开启心跳";
            }
        }

        /// <summary>
        /// 返回重启结果
        /// </summary>
        private void rebootResult_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 上报电价
        /// </summary>
        private void elecPrice_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 上报服务费
        /// </summary>
        private void servicePrice_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 上报上报间隔
        /// </summary>
        private void reportInterval_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 上报尖峰平谷信息
        /// </summary>
        private void elecPeriod_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 密钥信息
        /// </summary>
        private void secretKey_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 软件版本
        /// </summary>
        private void ver_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 枪口互联互通二维码
        /// </summary>
        private void barCode_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 状态信息
        /// </summary>
        private void state_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 起停结果
        /// </summary>
        private void startResult_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 实时充电数据
        /// </summary>
        private void rtData_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 上报账单信息
        /// </summary>
        private void bill_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 确认升级
        /// </summary>
        private void upgrade_Click(object sender, RoutedEventArgs e)
        {

        }

        private void heartBeatServer_Click(object sender, RoutedEventArgs e)
        {
            client.Receive();
        }
    }
}
