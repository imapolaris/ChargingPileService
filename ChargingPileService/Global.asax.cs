using CPS.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace ChargingPileService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // 注册并启动短信服务
            SmsServiceConfig.Instance.Register();

            // 注册并启动消息队列服务
            SessionServiceConfig.Instance.Register();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            Logger.Error(ex);
            Server.ClearError();
        }
    }
}
