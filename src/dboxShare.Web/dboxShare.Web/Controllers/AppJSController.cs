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


    public class AppJSController : ApiController
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
        [Route("api/app-js/login-auth")]
        [HttpGet]
        public HttpResponseMessage LoginAuth()
        {
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

            if (AppCommon.LoginAuth("Web") == true)
            {
                Response.Content = new StringContent("/* login authentication successful */");
            }
            else
            {
                // 验证失败加载重新登录脚本
                string ScriptPath = Context.Server.MapPath("/web/user/js/re-login.js");
                string ScriptContent = File.ReadAllText(ScriptPath);

                Response.Content = new StringContent(ScriptContent);
            }

            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/javascript");
            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


        /// <summary>
        /// App 数据返回 JS 脚本
        /// </summary>
        [Route("api/app-js/app-data")]
        [HttpGet]
        public HttpResponseMessage AppData()
        {
            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

            if (AppCommon.LoginAuth("Web") == false)
            {
                Response.Content = new StringContent("/* login authentication failed */");
            }
            else
            {
                string ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "app-data.js");
                string ScriptContent = File.ReadAllText(ScriptPath);

                ScriptContent = ScriptContent.Replace("{@userid}", Context.Session["UserId"].TypeString());
                ScriptContent = ScriptContent.Replace("{@username}", Context.Session["Username"].TypeString());
                ScriptContent = ScriptContent.Replace("{@useradmin}", Context.Session["Admin"].TypeString());
                ScriptContent = ScriptContent.Replace("{@systemdate}", DateTime.Now.ToString("yyyy-MM-dd"));
                ScriptContent = ScriptContent.Replace("{@uploadextension}", AppConfig.UploadExtension.TypeString());
                ScriptContent = ScriptContent.Replace("{@uploadsize}", AppConfig.UploadSize.TypeString());

                Response.Content = new StringContent(ScriptContent);
            }

            Response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/javascript");
            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


    }


}