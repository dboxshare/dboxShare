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


    public class Drive_ContextAttributeJsonController : ApiController
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
        /// 获取菜单属性数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/context-attribute-json")]
        [HttpGet]
        public HttpResponseMessage ContextAttributeJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Folder = Context.Request.QueryString["Folder"].TypeString() == "true" ? true : false;

            var Creator = Context.Request.QueryString["Creator"].TypeString() == "true" ? true : false;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Role = "creator";

                if (Creator == false)
                {
                    Role = AppCommon.PurviewRole(Id, Folder, ref Conn);
                }

                var Collect = 0;
                var Follow = 0;

                if (Folder == false)
                {
                    Collect = Base.Data.SqlScalar("Select Count(*) From DS_File_Collect Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn);
                    Follow = Base.Data.SqlScalar("Select Count(*) From DS_File_Follow Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn);
                }

                var Json = new ArrayList();

                Json.Add("'role':'" + Role + "'");
                Json.Add("'collect':'" + Collect + "'");
                Json.Add("'follow':'" + Follow + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
