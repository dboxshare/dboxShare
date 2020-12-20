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


    public class TaskOutboxDataJson : IHttpHandler, IReadOnlySessionState
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

            Json = Base.Data.SqlDataToJson("Select DBS_Id, DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Content, DBS_Level, DBS_Deadline, DBS_Revoke, DBS_Cause, DBS_Time From DBS_Task Where DBS_Id = " + Id, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
