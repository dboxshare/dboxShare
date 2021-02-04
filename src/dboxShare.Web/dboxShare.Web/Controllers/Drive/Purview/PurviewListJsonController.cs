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


    public class PurviewListJsonController : ApiController
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
        /// 读取权限数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/purview/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            string Sql = "Select DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview From DS_File_Purview Where DS_FileId = " + Id + " Order By DS_DepartmentId Asc, DS_RoleId Asc, DS_UserId Asc";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

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
