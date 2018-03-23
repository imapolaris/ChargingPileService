using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace ChargingPileService.Views.AliPay
{
    using ChargingPileService.Common;

    public partial class ResultNotifyPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AliPayResultNotify resultNotify = new AliPayResultNotify(this);
            resultNotify.ProcessNotify();
        }       
    }
}