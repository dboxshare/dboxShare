using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class SystemConfigDataJsonController : ApiController
    {


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 读取 Web.config 配置信息返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/system/config-data-json")]
        [HttpGet]
        public HttpResponseMessage ConfigDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            ArrayList Json = new ArrayList();

            Json.Add("'appname':'" + AppConfig.AppName + "'");
            Json.Add("'uploadextension':'" + AppConfig.UploadExtension + "'");
            Json.Add("'uploadsize':'" + AppConfig.UploadSize + "'");
            Json.Add("'versioncount':'" + AppConfig.VersionCount + "'");
            Json.Add("'mailaddress':'" + AppConfig.MailAddress + "'");
            Json.Add("'mailusername':'" + AppConfig.MailUsername + "'");
            Json.Add("'mailpassword':'" + AppConfig.MailPassword + "'");
            Json.Add("'mailserver':'" + AppConfig.MailServer + "'");
            Json.Add("'mailserverport':'" + AppConfig.MailServerPort + "'");
            Json.Add("'mailserverssl':'" + AppConfig.MailServerSSL.ToString().ToLower() + "'");

            return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
        }


    }


}
