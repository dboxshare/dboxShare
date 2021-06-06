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


    public class Drive_PurviewActionController : ApiController
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
        /// 权限添加
        /// </summary>
        [Route("api/drive/purview/add")]
        [HttpPost]
        public HttpResponseMessage Add()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            if (Context.Request.Form.GetValues("TypeId").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                for (var i = 0; i < Context.Request.Form.GetValues("TypeId").Length; i++)
                {
                    var TypeId = Context.Request.Form.GetValues("TypeId")[i].TypeInt();

                    var TypeIdPath = "";

                    if (Type == "department")
                    {
                        TypeIdPath = AppCommon.DepartmentIdPath(TypeId, ref Conn);
                    }

                    var Sql = "";

                    switch (Type)
                    {
                        case "department":
                            Sql = "Select DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId From DS_File_Purview Where DS_FileId = " + Id + " And DS_DepartmentId = '" + TypeIdPath + "'";
                            break;

                        case "role":
                            Sql = "Select DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId From DS_File_Purview Where DS_FileId = " + Id + " And DS_RoleId = " + TypeId;
                            break;

                        case "user":
                            Sql = "Select DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId From DS_File_Purview Where DS_FileId = " + Id + " And DS_UserId = " + TypeId;
                            break;
                    }

                    var PurviewTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable(Sql, ref Conn, ref PurviewTable);

                    if (PurviewTable["Exists"].TypeBool() == true)
                    {
                        continue;
                    }

                    PurviewTable.Clear();

                    Base.Data.SqlQuery("Insert Into DS_File_Purview(DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview) Values(" + Id + ", '" + (Type == "department" ? TypeIdPath : "null") + "', " + (Type == "role" ? TypeId : 0) + ", " + (Type == "user" ? TypeId : 0) + ", 'viewer')", ref Conn);
                }

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 权限修改
        /// </summary>
        [Route("api/drive/purview/modify")]
        [HttpPost]
        public HttpResponseMessage Modify()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var TypeId = Context.Request.Form["TypeId"].TypeString();

            if (Base.Common.StringCheck(TypeId, @"^[\d\/]+$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Purview = Context.Request.Form["Purview"].TypeString();

            if (Base.Common.StringCheck(Purview, @"^[\w]+$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var Sql = "";

                switch (Type)
                {
                    case "department":
                        Sql = "Update DS_File_Purview Set DS_Purview = '" + Purview + "' Where DS_FileId = " + Id + " And DS_DepartmentId = '" + TypeId + "'";
                        break;

                    case "role":
                        Sql = "Update DS_File_Purview Set DS_Purview = '" + Purview + "' Where DS_FileId = " + Id + " And DS_RoleId = " + TypeId;
                        break;

                    case "user":
                        Sql = "Update DS_File_Purview Set DS_Purview = '" + Purview + "' Where DS_FileId = " + Id + " And DS_UserId = " + TypeId;
                        break;
                }

                Base.Data.SqlQuery(Sql, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 权限删除
        /// </summary>
        [Route("api/drive/purview/delete")]
        [HttpPost]
        public HttpResponseMessage Delete()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var TypeId = Context.Request.Form["TypeId"].TypeString();

            if (Base.Common.StringCheck(TypeId, @"^[\d\/]+$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var Sql = "";

                switch (Type)
                {
                    case "department":
                        Sql = "Delete From DS_File_Purview Where DS_FileId = " + Id + " And DS_DepartmentId = '" + TypeId + "'";
                        break;

                    case "role":
                        Sql = "Delete From DS_File_Purview Where DS_FileId = " + Id + " And DS_RoleId = " + TypeId;
                        break;

                    case "user":
                        Sql = "Delete From DS_File_Purview Where DS_FileId = " + Id + " And DS_UserId = " + TypeId;
                        break;
                }

                Base.Data.SqlQuery(Sql, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 权限更改
        /// </summary>
        [Route("api/drive/purview/change")]
        [HttpPost]
        public HttpResponseMessage Change()
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

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                var Folders = new ArrayList();

                Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderPath, DS_Sync From DS_File Where DS_Folder = 1 And DS_Sync = 1 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn, ref Folders);

                for (var i = 0; i < Folders.Count; i++)
                {
                    Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Folders[i], ref Conn);
                    Base.Data.SqlQuery("Insert Into DS_File_Purview(DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview) Select " + Folders[i] + ", DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview From DS_File_Purview Where DS_FileId = " + Id, ref Conn);
                }

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 共享切换
        /// </summary>
        [Route("api/drive/purview/share")]
        [HttpPost]
        public HttpResponseMessage Share()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Share = Context.Request.Form["Share"].TypeString() == "true" ? 1 : 0;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Base.Data.SqlQuery("Update DS_File Set DS_Share = " + Share + " Where DS_Folder = 1 And DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Share = " + Share + ", DS_Sync = " + Share + " Where DS_FolderPath Like '" + FolderPath + "%'", ref Conn);

                AppCommon.Log(Id, (Share == 1 ? "folder-share" : "folder-unshare"), ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 同步切换
        /// </summary>
        [Route("api/drive/purview/sync")]
        [HttpPost]
        public HttpResponseMessage Sync()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Sync = Context.Request.Form["Sync"].TypeString() == "true" ? 1 : 0;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                var FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Base.Data.SqlQuery("Update DS_File Set DS_Sync = " + Sync + " Where DS_Folder = 1 And DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Sync = " + Sync + " Where DS_FolderPath Like '" + FolderPath + "%'", ref Conn);

                AppCommon.Log(Id, (Sync == 1 ? "folder-sync" : "folder-unsync"), ref Conn);

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
