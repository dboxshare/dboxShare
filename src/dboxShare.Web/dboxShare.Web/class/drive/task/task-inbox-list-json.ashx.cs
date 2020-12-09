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


    public class TaskInboxListJson : IHttpHandler, IReadOnlySessionState
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
            string Join = "";
            string Query = "";
            string Json = "";

            Status = Context.Request.QueryString["Status"].TypeString();

            if (string.IsNullOrEmpty(Status) == false)
            {
                switch (Status)
                {
                    case "unprocessed":
                        Status = "0";
                        break;

                    case "accepted":
                        Status = "1";
                        break;

                    case "rejected":
                        Status = "-1";
                        break;

                    case "completed":
                        Status = "2";
                        break;

                    default:
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

            Join += "Inner Join DBS_Task_Member On DBS_Task_Member.DBS_TaskId = DBS_Task.DBS_Id";

            if (string.IsNullOrEmpty(Status) == false)
            {
                Query += "DBS_Task_Member.DBS_Status = " + Status + " And ";
            }

            Query += "DBS_Task_Member.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Json = Base.Data.SqlPageToJson("DBS_Task", "DBS_Task.DBS_Id, DBS_Task.DBS_FileId, DBS_Task.DBS_FileName, DBS_Task.DBS_FileExtension, DBS_Task.DBS_IsFolder, DBS_Task.DBS_UserId, DBS_Task.DBS_Username, DBS_Task.DBS_Level, DBS_Task.DBS_Deadline, DBS_Task.DBS_Revoke, DBS_Task.DBS_Time, DBS_Task_Member.DBS_Status", "DBS_Task.DBS_Id Desc", Join, Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
