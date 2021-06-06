using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class Admin_LinkListJsonController : ApiController
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
        /// 读取链接数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/link/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Keyword = Context.Request.QueryString["Keyword"].TypeString();

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Keyword = Base.Common.InputFilter(Keyword);
            }

            var Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit-50-pages");
            }

            var Query = "";

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Query += "Exists (";

                // 用户账号查询
                Query += "Select A.DS_Id From DS_Link As A Where " +
                         "A.DS_Id = DS_Link.DS_Id And " +
                         "A.DS_Username = '" + Keyword + "'";

                Query += ")";
            }
            else
            {
                Query = "DS_Id > 0";
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlPageToJson("DS_Link", "DS_Id, DS_UserId, DS_Username, DS_Title, DS_Deadline, DS_Count, DS_Revoke, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
