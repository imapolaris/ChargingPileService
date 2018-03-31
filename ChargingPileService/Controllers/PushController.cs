using CPS.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ChargingPileService.Controllers
{
    using CPS.Infrastructure.Utils;
    using CPS.PushService;
    using Models;

    /// <summary>
    /// 消息推送控制器
    /// </summary>
    [RoutePrefix("api/pushmessage")]
    public class PushController : ApiController
    {
        [HttpPost]
        [Route("notification")]
        public IHttpActionResult PostNotification(dynamic obj)
        {
            PlatformTypeEnum platform = obj.platform;
            string title = obj.title;
            string content = obj.content;
            object extrasObj = obj.extras;
            Dictionary<string, object> extras = null;
            if (extrasObj != null)
            {
                extras = JsonHelper.Deserialize<Dictionary<string, object>>(extrasObj.ToString());
            }

            var result = PushMessage.Instance.PushNotification(platform, title, content, extras);
            if (result)
            {
                return Ok(SimpleResult.Succeed("消息推送成功！"));
            }
            else
            {
                return Ok(SimpleResult.Failed("消息推送失败！"));
            }
        }
    }
}
