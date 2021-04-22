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


    public class UserRecycleListJsonController : ApiController
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
        /// 读取用户回收站数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/user/recycle-list-json")]
        [HttpGet]
        public HttpResponseMessage RecycleListJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            string Query = "DS_Recycle = 1";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_User", "DS_Id, DS_DepartmentId, DS_RoleId, DS_Username, DS_Title, DS_Email, DS_Phone, DS_Status, DS_Recycle", "DS_Id Desc", Query, 50, Page, ref Conn);

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
