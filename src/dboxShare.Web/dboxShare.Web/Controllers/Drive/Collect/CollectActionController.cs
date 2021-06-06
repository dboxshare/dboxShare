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


    public class Drive_CollectActionController : ApiController
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
        /// 加入收藏
        /// </summary>
        [Route("api/drive/file/collect")]
        [HttpPost]
        public HttpResponseMessage Collect()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var CollectTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId From DS_File_Collect Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn, ref CollectTable);

                if (CollectTable["Exists"].TypeBool() == true)
                {
                    return AppCommon.ResponseMessage("already-exists");
                }

                CollectTable.Clear();

                Base.Data.SqlQuery("Insert Into DS_File_Collect(DS_FileId, DS_UserId, DS_Time) Values(" + Id + ", " + Context.Session["UserId"].TypeString() + ", '" + DateTime.Now.ToString() + "')", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 取消收藏
        /// </summary>
        [Route("api/drive/file/uncollect")]
        [HttpPost]
        public HttpResponseMessage Uncollec()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var CollectTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId From DS_File_Collect Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn, ref CollectTable);

                if (CollectTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("no-permissions-or-not-exist");
                }

                CollectTable.Clear();

                Base.Data.SqlQuery("Delete From DS_File_Collect Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
