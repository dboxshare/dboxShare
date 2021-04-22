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


    public class ActivityLogListJsonController : ApiController
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
        /// 读取日志数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/activity/log-list-json")]
        [HttpGet]
        public HttpResponseMessage LogListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit 50 pages");
            }

            string Query = "DS_FileId = " + Id;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_Log", "DS_Id, DS_FileId, DS_FileName, DS_FileVersion, DS_UserId, DS_Username, DS_Action, DS_IP, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
