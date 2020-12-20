using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class PurviewAction : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        private dynamic Conn;


        public void ProcessRequest(HttpContext Context)
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            switch (Context.Request.QueryString["Action"].TypeString())
            {
                case "add":
                    Add(Context);
                    break;

                case "modify":
                    Modify(Context);
                    break;

                case "delete":
                    Delete(Context);
                    break;

                case "change":
                    Change(Context);
                    break;

                case "share":
                    Share(Context);
                    break;

                case "sync":
                    Sync(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 权限添加
        /// </summary>
        private void Add(HttpContext Context)
        {
            Hashtable PurviewTable = new Hashtable();
            int Id = 0;
            string Type = "";
            int TypeId = 0;
            string TypeIdPath = "";
            string Sql = "";
            int Index = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return;
            }

            if (Context.Request.Form.GetValues("TypeId").Length == 0)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("TypeId").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("TypeId")[Index]) == true)
                {
                    TypeId = Context.Request.Form.GetValues("TypeId")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                if (Type == "department")
                {
                    TypeIdPath = AppCommon.DepartmentIdPath(TypeId, ref Conn);
                }

                switch (Type)
                {
                    case "department":
                        Sql = "Select DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_DepartmentId = '" + TypeIdPath + "'";
                        break;

                    case "role":
                        Sql = "Select DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_RoleId = " + TypeId;
                        break;

                    case "user":
                        Sql = "Select DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_UserId = " + TypeId;
                        break;
                }

                Base.Data.SqlDataToTable(Sql, ref Conn, ref PurviewTable);

                if (PurviewTable["Exist"].TypeBool() == true)
                {
                    continue;
                }

                PurviewTable.Clear();

                Base.Data.SqlQuery("Insert Into DBS_File_Purview(DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview) Values(" + Id + ", '" + (Type == "department" ? TypeIdPath : "null") + "', " + (Type == "role" ? TypeId : 0) + ", " + (Type == "user" ? TypeId : 0) + ", 'viewer')", ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 权限修改
        /// </summary>
        private void Modify(HttpContext Context)
        {
            int Id = 0;
            string Type = "";
            string TypeId = "";
            string Purview = "";
            string Sql = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return;
            }

            TypeId = Context.Request.Form["TypeId"].TypeString();

            if (Base.Common.StringCheck(TypeId, @"^[\d\/]+$") == false)
            {
                return;
            }

            Purview = Context.Request.Form["Purview"].TypeString();

            if (Base.Common.StringCheck(Purview, @"^[\w]+$") == false)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            switch (Type)
            {
                case "department":
                    Sql = "Update DBS_File_Purview Set DBS_Purview = '" + Purview + "' Where DBS_FileId = " + Id + " And DBS_DepartmentId = '" + TypeId + "'";
                    break;

                case "role":
                    Sql = "Update DBS_File_Purview Set DBS_Purview = '" + Purview + "' Where DBS_FileId = " + Id + " And DBS_RoleId = " + TypeId;
                    break;

                case "user":
                    Sql = "Update DBS_File_Purview Set DBS_Purview = '" + Purview + "' Where DBS_FileId = " + Id + " And DBS_UserId = " + TypeId;
                    break;
            }

            Base.Data.SqlQuery(Sql, ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 权限删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            int Id = 0;
            string Type = "";
            string TypeId = "";
            string Sql = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Type = Context.Request.Form["Type"].TypeString();

            if (Base.Common.StringCheck(Type, @"^(department|role|user)$") == false)
            {
                return;
            }

            TypeId = Context.Request.Form["TypeId"].TypeString();

            if (Base.Common.StringCheck(TypeId, @"^[\d\/]+$") == false)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            switch (Type)
            {
                case "department":
                    Sql = "Delete From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_DepartmentId = '" + TypeId + "'";
                    break;

                case "role":
                    Sql = "Delete From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_RoleId = " + TypeId;
                    break;

                case "user":
                    Sql = "Delete From DBS_File_Purview Where DBS_FileId = " + Id + " And DBS_UserId = " + TypeId;
                    break;
            }

            Base.Data.SqlQuery(Sql, ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 权限更改
        /// </summary>
        private void Change(HttpContext Context)
        {
            ArrayList Folders = new ArrayList();
            int Id = 0;
            string FolderPath = "";
            int Index = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_Sync From DBS_File Where DBS_Folder = 1 And DBS_Sync = 1 And DBS_FolderPath Like '" + FolderPath + "%'", ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Folders[Index], ref Conn);
                Base.Data.SqlQuery("Insert Into DBS_File_Purview(DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview) Select " + Folders[Index] + ", DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 权限共享切换
        /// </summary>
        private void Share(HttpContext Context)
        {
            int Id = 0;
            int Share = 0;
            string FolderPath = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Share = Context.Request.Form["Share"].TypeString() == "true" ? 1 : 0;

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            Base.Data.SqlQuery("Update DBS_File Set DBS_Share = " + Share + " Where DBS_Folder = 1 And DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File Set DBS_Share = " + Share + ", DBS_Sync = " + Share + " Where DBS_FolderPath Like '" + FolderPath + "%'", ref Conn);

            AppCommon.Log(Id, (Share == 1 ? "folder-share" : "folder-unshare"), ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 权限同步切换
        /// </summary>
        private void Sync(HttpContext Context)
        {
            int Id = 0;
            int Sync = 0;
            string FolderPath = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Sync = Context.Request.Form["Sync"].TypeString() == "true" ? 1 : 0;

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            Base.Data.SqlQuery("Update DBS_File Set DBS_Sync = " + Sync + " Where DBS_Folder = 1 And DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File Set DBS_Sync = " + Sync + " Where DBS_FolderPath Like '" + FolderPath + "%'", ref Conn);

            AppCommon.Log(Id, (Sync == 1 ? "folder-sync" : "folder-unsync"), ref Conn);

            Context.Response.Write("complete");
        }


    }


}
