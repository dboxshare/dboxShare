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


    public class TaskMemberListJsonController : ApiController
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
        /// 读取任务成员数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/task/member-list-json")]
        [HttpGet]
        public HttpResponseMessage MemberListJson()
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

                string Sql = "Select DS_Id, DS_UserId From DS_Task Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id + " Union All " + 
                             "Select DS_TaskId, DS_UserId From DS_Task_Member Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id;

                Base.Data.SqlDataToTable(Sql, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                TaskTable.Clear();

                string Json = Base.Data.SqlListToJson("Select DS_TaskId, DS_UserId, DS_Username, DS_Reason, DS_Remark, DS_Status, DS_AcceptedTime, DS_RejectedTime, DS_CompletedTime From DS_Task_Member Where DS_TaskId = " + Id, ref Conn);

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
