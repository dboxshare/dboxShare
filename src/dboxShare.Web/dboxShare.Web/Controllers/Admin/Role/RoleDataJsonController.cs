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


    public class Admin_RoleDataJsonController : ApiController
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
        /// 读取角色数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/role/data-json")]
        [HttpGet]
        public HttpResponseMessage DataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlDataToJson("Select DS_Id, DS_Name From DS_Role Where DS_Id = " + Id, ref Conn);

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
