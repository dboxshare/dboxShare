using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Controllers
{


    public class ScriptController : ApiController
    {


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 用户登录验证返回 JS 脚本
        /// </summary>
        [Route("api/script/login-auth")]
        [HttpGet]
        public HttpResponseMessage LoginAuth()
        {
            var Response = new HttpResponseMessage(HttpStatusCode.OK);

            if (AppCommon.LoginAuth("Web") == true)
            {
                Response.Content = new StringContent("/* login authentication successful */");
            }
            else
            {
                // 验证失败加载重新登录脚本
                var ScriptPath = Context.Server.MapPath("/web/account/js/login-box.js");
                var ScriptContent = File.ReadAllText(ScriptPath);

                Response.Content = new StringContent(ScriptContent);
            }

            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/javascript");
            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


        /// <summary>
        /// App 数据返回 JS 脚本
        /// </summary>
        [Route("api/script/app-data")]
        [HttpGet]
        public HttpResponseMessage AppData()
        {
            var Response = new HttpResponseMessage(HttpStatusCode.OK);

            if (AppCommon.LoginAuth("Web") == false)
            {
                Response.Content = new StringContent("/* login-authentication-failed */");
            }
            else
            {
                var ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "app-data.js");
                var ScriptContent = File.ReadAllText(ScriptPath);

                ScriptContent = ScriptContent.Replace("{@myid}", Context.Session["UserId"].TypeString());
                ScriptContent = ScriptContent.Replace("{@myusername}", Context.Session["Username"].TypeString());
                ScriptContent = ScriptContent.Replace("{@myadmin}", Context.Session["Admin"].TypeString());
                ScriptContent = ScriptContent.Replace("{@myuploadsize}", AppConfig.UserUploadSize.TypeString());
                ScriptContent = ScriptContent.Replace("{@mydownloadsize}", AppConfig.UserDownloadSize.TypeString());
                ScriptContent = ScriptContent.Replace("{@uploadextension}", AppConfig.UploadExtension.TypeString());
                ScriptContent = ScriptContent.Replace("{@retentiondays}", AppConfig.RetentionDays.TypeString());
                ScriptContent = ScriptContent.Replace("{@systemdate}", DateTime.Now.ToString("yyyy-MM-dd"));

                Response.Content = new StringContent(ScriptContent);
            }

            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/javascript");
            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


    }


}