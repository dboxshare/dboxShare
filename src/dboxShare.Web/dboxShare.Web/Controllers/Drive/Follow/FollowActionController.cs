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


    public class Drive_FollowActionController : ApiController
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
        /// 加入关注
        /// </summary>
        [Route("api/drive/file/follow")]
        [HttpPost]
        public HttpResponseMessage Follow()
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

                var FollowTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId From DS_File_Follow Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn, ref FollowTable);

                if (FollowTable["Exists"].TypeBool() == true)
                {
                    return AppCommon.ResponseMessage("already-exists");
                }

                FollowTable.Clear();

                Base.Data.SqlQuery("Insert Into DS_File_Follow(DS_FileId, DS_UserId, DS_Time) Values(" + Id + ", " + Context.Session["UserId"].TypeString() + ", '" + DateTime.Now.ToString() + "')", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 取消关注
        /// </summary>
        [Route("api/drive/file/unfollow")]
        [HttpPost]
        public HttpResponseMessage Unfollow()
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

                var FollowTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId From DS_File_Follow Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn, ref FollowTable);

                if (FollowTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("no-permissions-or-not-exist");
                }

                FollowTable.Clear();

                Base.Data.SqlQuery("Delete From DS_File_Follow Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + Id, ref Conn);

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
