using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class PurviewRole : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        private dynamic Conn;


        public void ProcessRequest(HttpContext Context)
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            Role(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取权限角色数据
        /// </summary>
        private void Role(HttpContext Context)
        {
            int Id = 0;
            bool Folder = false;
            string Role = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Folder = Context.Request.QueryString["Folder"].TypeString() == "true" ? true : false;

            Role = AppCommon.PurviewRole(Id, Folder, ref Conn);

            Context.Response.Write(Role);
        }


    }


}
