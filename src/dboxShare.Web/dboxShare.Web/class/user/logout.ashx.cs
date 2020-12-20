using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.User
{


    public class Logout : IHttpHandler, IRequiresSessionState
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
            ProcessLogout(Context);
        }


        /// <summary>
        /// 用户注销登录
        /// </summary>
        private void ProcessLogout(HttpContext Context)
        {
            Context.Cache.Remove("Login-Token-Web-" + Context.Session["UserId"].TypeString() + "");

            Context.Session.Abandon();

            Context.Response.Redirect("/");
        }


    }


}
