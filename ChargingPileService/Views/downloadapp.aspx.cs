using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChargingPileService.Views
{
    public partial class downloadapp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        long sum = 0;
        protected void Unnamed_Click(object sender, EventArgs e)
        {
            sum += 1;

            var path = "http://mp.weixin.qq.com/mp/redirect?url=http://192.168.0.201//Res/apks/app-release.apk#weixin.qq.com#wechat_redirect";
            Response.Redirect(path);
        }
    }
}