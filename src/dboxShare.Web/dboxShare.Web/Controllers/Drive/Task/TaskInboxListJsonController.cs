using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class TaskInboxListJsonController : ApiController
    {


        private dynamic Conn;


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 读取任务数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/task/inbox-list-json")]
        [HttpGet]
        public HttpResponseMessage InboxListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Status = Context.Request.QueryString["Status"].TypeString();

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
                        return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            string Join = "Inner Join DS_Task_Member On DS_Task_Member.DS_TaskId = DS_Task.DS_Id";

            string Query = "";

            if (string.IsNullOrEmpty(Status) == false)
            {
                Query += "DS_Task_Member.DS_Status = " + Status + " And ";
            }

            Query += "DS_Task_Member.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_Task", "DS_Task.DS_Id, DS_Task.DS_FileId, DS_Task.DS_FileName, DS_Task.DS_FileExtension, DS_Task.DS_IsFolder, DS_Task.DS_UserId, DS_Task.DS_Username, DS_Task.DS_Level, DS_Task.DS_Deadline, DS_Task.DS_Revoke, DS_Task.DS_Time, DS_Task_Member.DS_Status", "DS_Task.DS_Id Desc", Join, Query, 50, Page, ref Conn);

                return AppCommon.ResponseMessage(Json);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
