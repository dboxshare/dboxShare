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


    public class RoleListJsonController : ApiController
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
        /// 读取角色数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/role/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Sql = "Select DS_Id, DS_Name, DS_Sequence From DS_Role Order By DS_Sequence Desc, DS_Id Asc";

                string Json = Base.Data.SqlListToJson(Sql, ref Conn);

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
