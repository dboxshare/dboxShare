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


    public class TaskOutboxListJsonController : ApiController
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
        [Route("api/drive/task/outbox-list-json")]
        [HttpGet]
        public HttpResponseMessage OutboxListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Status = Context.Request.QueryString["Status"].TypeString();

            if (string.IsNullOrEmpty(Status) == false)
            {
                if (Base.Common.StringCheck(Status, @"^(processing|expired|revoked)$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            string Query = "";

            if (string.IsNullOrEmpty(Status) == false)
            {
                if (Status == "processing")
                {
                    Query += "DS_Deadline > '" + DateTime.Now.ToString() + "' And ";
                    Query += "DS_Revoke = 0 And ";
                }

                if (Status == "expired")
                {
                    Query += "DS_Deadline < '" + DateTime.Now.ToString() + "' And ";
                    Query += "DS_Revoke = 0 And ";
                }

                if (Status == "revoked")
                {
                    Query += "DS_Revoke = 1 And ";
                }
            }

            Query += "DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_Task", "DS_Id, DS_FileId, DS_FileName, DS_FileExtension, DS_IsFolder, DS_UserId, DS_Username, DS_Level, DS_Deadline, DS_Revoke, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
