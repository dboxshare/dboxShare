using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.User
{


    public class LoginAuthJS : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        public void ProcessRequest(HttpContext Context)
        {
            LoginAuth(Context);
        }


        /// <summary>
        /// 用户登录验证返回js脚本
        /// </summary>
        private void LoginAuth(HttpContext Context)
        {
            if (AppCommon.LoginAuth("Web") == true)
            {
                Context.Response.ContentType = "text/javascript";
            }
            else
            {
                // 验证失败转到重新登录js
                Context.Response.Redirect("/web/user/js/re-login.js");
            }
        }


    }


}