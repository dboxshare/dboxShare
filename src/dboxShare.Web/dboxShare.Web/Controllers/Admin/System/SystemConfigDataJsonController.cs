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


    public class Admin_SystemConfigDataJsonController : ApiController
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Json = new ArrayList();

            Json.Add("'appname':'" + AppConfig.AppName.TypeString() + "'");
            Json.Add("'uploadextension':'" + AppConfig.UploadExtension.TypeString() + "'");
            Json.Add("'uploadsize':'" + AppConfig.UploadSize.TypeString() + "'");
            Json.Add("'downloadsize':'" + AppConfig.DownloadSize.TypeString() + "'");
            Json.Add("'versioncount':'" + AppConfig.VersionCount.TypeString() + "'");
            Json.Add("'mailaddress':'" + AppConfig.MailAddress.TypeString() + "'");
            Json.Add("'mailusername':'" + AppConfig.MailUsername.TypeString() + "'");
            Json.Add("'mailpassword':'" + AppConfig.MailPassword.TypeString() + "'");
            Json.Add("'mailserver':'" + AppConfig.MailServer.TypeString() + "'");
            Json.Add("'mailserverport':'" + AppConfig.MailServerPort.TypeString() + "'");
            Json.Add("'mailserverssl':'" + AppConfig.MailServerSSL.TypeString().ToLower() + "'");

            return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
        }


    }


}
