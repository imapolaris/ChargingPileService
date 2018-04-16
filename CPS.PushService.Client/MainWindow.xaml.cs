using CPS.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CPS.PushService.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
            this.pushNofication.Click += PushNofication_Click;
            this.pushCustomMessage.Click += PushCustomMessage_Click;
            this.pushRichMedia.Click += PushRichMedia_Click;
        }

        private void PushRichMedia_Click(object sender, RoutedEventArgs e)
        {
            PushMessage.Instance.PushRichMedia();
        }

        private void PushCustomMessage_Click(object sender, RoutedEventArgs e)
        {
            PushMessage.Instance.PushCustomMessage();
        }

        private void PushNofication_Click(object sender, RoutedEventArgs e)
        {
            PushMessage.Instance.PushNotification(PlatformTypeEnum.All, "EV堡", "推送测试", new Dictionary<string, object> { { "url", "http://wwww.baidu.com"} });
        }
    }
}
