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


    public class Drive_LinkLogListJsonController : ApiController
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
        /// 读取链接日志数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/link/log-list-json")]
        [HttpGet]
        public HttpResponseMessage LogListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit-50-pages");
            }

            var Query = "DS_LinkId = " + Id + "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlPageToJson("DS_Link_Log", "DS_Id, DS_LinkId, DS_IP, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
