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


    public class DiscussListJson : IHttpHandler, IReadOnlySessionState
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

            ListDataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取讨论数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            int Id = 0;
            int Page = 0;
            string Query = "";
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            Query += "DBS_FileId = " + Id + "";

            Json = Base.Data.SqlPageToJson("DBS_Discuss", "DBS_Id, DBS_FileId, DBS_FileName, DBS_UserId, DBS_Username, DBS_Content, DBS_Revoke, DBS_Time", "DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
