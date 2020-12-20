using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class TaskOutboxListJson : IHttpHandler, IReadOnlySessionState
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
        /// 读取任务数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            string Status = "";
            int Page = 0;
            string Query = "";
            string Json = "";

            Status = Context.Request.QueryString["Status"].TypeString();

            if (string.IsNullOrEmpty(Status) == false)
            {
                if (Base.Common.StringCheck(Status, @"^(processing|expired|revoked)$") == false)
                {
                    return;
                }
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

            if (string.IsNullOrEmpty(Status) == false)
            {
                if (Status == "processing")
                {
                    Query += "DBS_Deadline > '" + DateTime.Now.ToString() + "' And ";
                }

                if (Status == "expired")
                {
                    Query += "DBS_Deadline < '" + DateTime.Now.ToString() + "' And ";
                }

                if (Status == "revoked")
                {
                    Query += "DBS_Revoke = 1 And ";
                }
            }

            Query += "DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Json = Base.Data.SqlPageToJson("DBS_Task", "DBS_Id, DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Level, DBS_Deadline, DBS_Revoke, DBS_Time", "DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
