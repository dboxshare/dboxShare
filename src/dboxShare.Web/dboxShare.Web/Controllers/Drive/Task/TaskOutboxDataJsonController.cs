using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class TaskOutboxDataJsonController : ApiController
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
        /// 读取任务数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/task/outbox-data-json")]
        [HttpGet]
        public HttpResponseMessage OutboxDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId From DS_Task Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                TaskTable.Clear();

                string Json = Base.Data.SqlDataToJson("Select DS_Id, DS_FileId, DS_FileName, DS_FileExtension, DS_IsFolder, DS_UserId, DS_Username, DS_Content, DS_Level, DS_Deadline, DS_Revoke, DS_Cause, DS_Time From DS_Task Where DS_Id = " + Id, ref Conn);

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
