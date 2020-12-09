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


    public class TaskInboxDataJson : IHttpHandler, IReadOnlySessionState
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

            DataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取任务数据记录返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            int Id = 0;
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Json = Base.Data.SqlDataToJson("Select DBS_Task.DBS_Id, DBS_Task.DBS_FileId, DBS_Task.DBS_FileName, DBS_Task.DBS_FileExtension, DBS_Task.DBS_IsFolder, DBS_Task.DBS_UserId, DBS_Task.DBS_Username, DBS_Task.DBS_Content, DBS_Task.DBS_Level, DBS_Task.DBS_Deadline, DBS_Task.DBS_Revoke, DBS_Task.DBS_Cause, DBS_Task.DBS_Time, DBS_Task_Member.DBS_Status From DBS_Task Inner Join DBS_Task_Member On DBS_Task_Member.DBS_TaskId = DBS_Task.DBS_Id Where DBS_Task_Member.DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Task.DBS_Id = " + Id, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
