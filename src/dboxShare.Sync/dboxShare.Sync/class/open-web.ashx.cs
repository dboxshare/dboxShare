using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;
using dboxShare.Sync;


namespace dboxShare.Sync
{


    public class OpenWeb : IHttpHandler, IRequiresSessionState
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
            string LoginId = "";
            string Password = "";

            LoginId = Context.Request.QueryString["LoginId"].TypeString();

            if (string.IsNullOrEmpty(LoginId) == true)
            {
                return;
            }

            Password = Context.Request.QueryString["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\S]{6,16}$") == false)
            {
                return;
            }

            Context.Response.Redirect("/web/?loginid=" + HttpUtility.UrlEncode(LoginId) + "&password=" + HttpUtility.UrlEncode(Password) + "");
        }


    }


}