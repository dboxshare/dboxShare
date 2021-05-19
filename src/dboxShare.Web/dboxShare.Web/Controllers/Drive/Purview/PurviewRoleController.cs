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


    public class PurviewRoleController : ApiController
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
        /// 读取权限角色数据
        /// </summary>
        [Route("api/drive/purview/role")]
        [HttpGet]
        public HttpResponseMessage Role()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            bool Folder = Context.Request.QueryString["Folder"].TypeString() == "true" ? true : false;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Role = AppCommon.PurviewRole(Id, Folder, ref Conn);

                return AppCommon.ResponseMessage(Role);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
