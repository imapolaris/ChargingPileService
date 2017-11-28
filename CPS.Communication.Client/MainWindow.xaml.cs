using CPS.Communication.Service;
using CPS.Communication.Service.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        Server.Client client = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client = new Server.Client(socket);
            string txtip = this.txtIP.Text.Trim();
            string txtport = this.txtPort.Text.Trim();
            IPAddress ip = IPAddress.Parse(txtip);
            IPEndPoint ep = new IPEndPoint(ip, int.Parse(txtport));
            client.ReceiveCompleted += Client_ReceiveCompleted;
            client.SendCompleted += Client_SendCompleted;
            client.WorkSocket.Connect(ep);
        }

        private void Client_SendCompleted(object sender, Service.Events.SendCompletedEventArgs args)
        {
            appendText("send", "已发送...");
        }

        private void Client_ReceiveCompleted(object sender, Service.Events.ReceiveCompletedEventArgs args)
        {
            LoginResultPacket packet = PacketAnalyzer.AnalysePacket(args.ReceivedBytes) as LoginResultPacket;

            appendText("receive", $"sn:{packet.SerialNumber}, result:{packet.ResultEnum.ToString()}, timestamp:{packet.TimeStamp}");
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
                TimeStamp = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
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
    }
}
