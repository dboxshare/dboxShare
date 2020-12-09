using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class FolderAction : IHttpHandler, IReadOnlySessionState
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

                case "rename":
                    Rename(Context);
                    break;

                case "remark":
                    Remark(Context);
                    break;

                case "lock":
                    Lock(Context);
                    break;

                case "unlock":
                    Unlock(Context);
                    break;

                case "move":
                    Move(Context);
                    break;

                case "remove":
                    Remove(Context);
                    break;

                case "restore":
                    Restore(Context);
                    break;

                case "delete":
                    Delete(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 文件夹添加
        /// </summary>
        private void Add(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int UserId = 0;
            string Username = "";
            int FolderId = 0;
            string FolderPath = "";
            string Name = "";
            string Remark = "";
            int Inherit = 0;
            int Share = 0;
            string Sql = "";

            if (Base.Common.IsNumeric(Context.Request.Form["FolderId"]) == true)
            {
                FolderId = Context.Request.Form["FolderId"].TypeInt();
            }

            FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,50}$") == false)
            {
                return;
            }

            Remark = Base.Common.InputFilter(Context.Request.Form["Remark"].TypeString());

            if (string.IsNullOrEmpty(Remark) == false)
            {
                if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,100}$") == false)
                {
                    return;
                }
            }

            Inherit = Context.Request.Form["Inherit"].TypeString() == "true" ? 1 : 0;

            Share = Context.Request.Form["Share"].TypeString() == "true" ? 1 : 0;

            if (FolderId > 0)
            {
                if (AppCommon.PurviewCheck(FolderId, true, "manager", ref Conn) == false)
                {
                    Context.Response.Write("no-permission");
                    return;
                }

                if (Inherit == 1)
                {
                    Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Username, DBS_Folder, DBS_Share From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exist"].TypeBool() == false)
                    {
                        return;
                    }
                    else
                    {
                        UserId = FileTable["DBS_UserId"].TypeInt();
                        Username = FileTable["DBS_Username"].TypeString();
                        Share = FileTable["DBS_Share"].TypeInt();
                    }

                    FileTable.Clear();
                }
            }

            if (FolderExist(0, FolderId, Name, Context) == true)
            {
                Context.Response.Write("existed");
                return;
            }

            Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
            Sql += "Values(" + (UserId == 0 ? Context.Session["UserId"].TypeString() : UserId.ToString()) + ", '" + (UserId == 0 ? Context.Session["Username"].TypeString() : Username) + "', 0, 0, 1, " + FolderId + ", '" + (FolderId == 0 ? "/0/" : FolderPath) + "', 'null', 'null', '" + Name + "', 'null', 0, 'folder', '" + Remark + "', " + Share + ", 0, " + Inherit + ", 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

            Id = Base.Data.SqlInsert(Sql, ref Conn);

            if (Id == 0)
            {
                return;
            }

            if (FolderPath.Length == 0)
            {
                Directory.CreateDirectory(Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), Id.ToString()));
            }
            else
            {
                Directory.CreateDirectory(Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), Id.ToString()));
            }

            if (FolderId > 0 && Inherit == 1)
            {
                PurviewInherit(Id, FolderId);
            }

            AppCommon.Log(Id, "folder-add", ref Conn);

            if (Share == 1)
            {
                Context.Response.Write(Id.ToString());
                return;
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹重命名
        /// </summary>
        private void Rename(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int FolderId = 0;
            string Name = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,50}$") == false)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderId = FileTable["DBS_FolderId"].TypeInt();
            }

            FileTable.Clear();

            if (FolderExist(Id, FolderId, Name, Context) == true)
            {
                Context.Response.Write("existed");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Name = '" + Name + "' Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "folder-rename", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹备注
        /// </summary>
        private void Remark(HttpContext Context)
        {
            int Id = 0;
            string Remark = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Remark = Base.Common.InputFilter(Context.Request.Form["Remark"].TypeString());

            if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,100}$") == false)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, true, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Remark = '" + Remark + "' Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "folder-remark", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹锁定
        /// </summary>
        private void Lock(HttpContext Context)
        {
            int Id = 0;
            string FolderPath = "";

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

            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 1 Where DBS_VersionId = 0 And DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 1 Where DBS_VersionId = 0 And DBS_FolderPath Like '" + FolderPath + "%'", ref Conn);

            AppCommon.Log(Id, "folder-lock", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹取消锁定
        /// </summary>
        private void Unlock(HttpContext Context)
        {
            int Id = 0;
            string FolderPath = "";

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

            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 0 Where DBS_VersionId = 0 And DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 0 Where DBS_VersionId = 0 And DBS_FolderPath Like '" + FolderPath + "%'", ref Conn);

            AppCommon.Log(Id, "folder-unlock", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹移动
        /// </summary>
        private void Move(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string Name = "";
            int FolderId = 0;
            string FolderIdPath = "";
            string FolderNewName = "";
            int FolderUserId = 0;
            string FolderUsername = "";
            int FolderShare = 0;
            int FolderSync = 0;
            string SourceDirectoryPath = "";
            string TargetDirectoryPath = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["FolderId"]) == true)
            {
                FolderId = Context.Request.Form["FolderId"].TypeInt();
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

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Name, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Name = FileTable["DBS_Name"].TypeString();
            }

            FileTable.Clear();

            if (FolderId == 0)
            {
                FolderIdPath = "/0/";
                FolderUserId = Context.Session["UserId"].TypeInt();
                FolderUsername = Context.Session["Username"].TypeString();
            }
            else
            {
                FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);

                // 判断是否移动到子文件夹
                if (FolderIdPath.IndexOf("/" + Id + "/") > -1)
                {
                    if (FolderIdPath.Substring(FolderIdPath.IndexOf("/" + Id + "/")).IndexOf("/" + FolderId + "/") > -1)
                    {
                        return;
                    }
                }

                if (AppCommon.PurviewCheck(FolderId, true, "manager", ref Conn) == false)
                {
                    Context.Response.Write("no-permission");
                    return;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Username, DBS_Folder, DBS_Share, DBS_Lock, DBS_Sync From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    return;
                }
                else
                {
                    FolderUserId = FileTable["DBS_UserId"].TypeInt();
                    FolderUsername = FileTable["DBS_Username"].TypeString();
                    FolderShare = FileTable["DBS_Share"].TypeInt();
                    FolderSync = FileTable["DBS_Sync"].TypeInt();
                }

                FileTable.Clear();
            }

            SourceDirectoryPath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1));

            if (FolderId == 0)
            {
                TargetDirectoryPath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), Id.ToString());
            }
            else
            {
                TargetDirectoryPath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderIdPath.Substring(1), Id.ToString());
            }

            if (SourceDirectoryPath == TargetDirectoryPath)
            {
                return;
            }

            if (Directory.Exists(SourceDirectoryPath) == true)
            {
                Directory.Move(SourceDirectoryPath, TargetDirectoryPath);
            }

            FolderNewName = AppCommon.FolderName(FolderId, Name, ref Conn);

            Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + FolderId + ", DBS_FolderPath = '" + FolderIdPath + "', DBS_Name = '" + FolderNewName + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Id = " + Id, ref Conn);

            Move_Subfolder(Id, FolderUserId, FolderUsername, FolderShare, FolderSync);

            PurviewSync(Id, FolderId);

            AppCommon.Log(Id, "folder-move", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹移动(子文件夹)
        /// </summary>
        private void Move_Subfolder(int Id, int FolderUserId, string FolderUsername, int FolderShare, int FolderSync)
        {
            ArrayList Folders = new ArrayList();
            ArrayList Files = new ArrayList();
            string FolderPath = "";
            int Index = 0;

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_FolderId = " + Id, ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + Id + ", DBS_FolderPath = '" + FolderPath + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Id = " + Folders[Index], ref Conn);

                Move_Subfolder(Folders[Index].TypeInt(), FolderUserId, FolderUsername, FolderShare, FolderSync);
            }

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 0 And DBS_FolderId = " + Id, ref Conn, ref Files);

            for (Index = 0; Index < Files.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + Id + ", DBS_FolderPath = '" + FolderPath + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Folder = 0 And DBS_Id = " + Files[Index], ref Conn);
                Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + Id + ", DBS_FolderPath = '" + FolderPath + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Folder = 0 And DBS_VersionId = " + Files[Index], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹移除
        /// </summary>
        private void Remove(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;

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

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 1, DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DBS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + Id, ref Conn);

            Remove_Subfolder(Id, Context);

            AppCommon.Log(Id, "folder-remove", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹移除(子文件夹)
        /// </summary>
        private void Remove_Subfolder(int Id, HttpContext Context)
        {
            ArrayList Folders = new ArrayList();
            ArrayList Files = new ArrayList();
            int Index = 0;

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_FolderId = " + Id, ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 1, DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DBS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + Folders[Index], ref Conn);

                Remove_Subfolder(Folders[Index].TypeInt(), Context);
            }

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_FolderId From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0 And DBS_FolderId = " + Id, ref Conn, ref Files);

            for (Index = 0; Index < Files.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 1, DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DBS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + Files[Index], ref Conn);
                Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'delete' Where DBS_FileId = " + Files[Index], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹还原
        /// </summary>
        private void Restore(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;

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

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 0, DBS_RemoveUsername = 'null', DBS_RemoveTime = '1970/1/1 00:00:00' Where DBS_Id = " + Id, ref Conn);

            Restore_Subfolder(Id);

            AppCommon.Log(Id, "folder-restore", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹还原(子文件夹)
        /// </summary>
        private void Restore_Subfolder(int Id)
        {
            ArrayList Folders = new ArrayList();
            ArrayList Files = new ArrayList();
            int Index = 0;

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_FolderId = " + Id, ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 0, DBS_RemoveUsername = 'null', DBS_RemoveTime = '1970/1/1 00:00:00' Where DBS_Id = " + Folders[Index], ref Conn);

                Restore_Subfolder(Folders[Index].TypeInt());
            }

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_FolderId From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0 And DBS_FolderId = " + Id, ref Conn, ref Files);

            for (Index = 0; Index < Files.Count; Index++)
            {
                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 0, DBS_RemoveUsername = 'null', DBS_RemoveTime = '1970/1/1 00:00:00' Where DBS_Id = " + Files[Index], ref Conn);
                Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'add' Where DBS_FileId = " + Files[Index], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string DirectoryPath = "";

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

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }

            FileTable.Clear();

            DirectoryPath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1));

            if (Directory.Exists(DirectoryPath) == true)
            {
                Directory.Delete(DirectoryPath, true);
            }

            Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);

            Delete_Subfolder(Id);

            AppCommon.Log(Id, "folder-delete", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件夹删除(子文件夹)
        /// </summary>
        private void Delete_Subfolder(int Id)
        {
            ArrayList Folders = new ArrayList();
            ArrayList Files = new ArrayList();
            int Index = 0;

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_FolderId = " + Id, ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Folders[Index], ref Conn);
                Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Folders[Index], ref Conn);

                Delete_Subfolder(Folders[Index].TypeInt());
            }

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_FolderId From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0 And DBS_FolderId = " + Id, ref Conn, ref Files);

            for (Index = 0; Index < Files.Count; Index++)
            {
                Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Files[Index], ref Conn);
                Base.Data.SqlQuery("Delete From DBS_File Where DBS_VersionId = " + Files[Index], ref Conn);
                Base.Data.SqlQuery("Delete From DBS_File_Process Where DBS_FileId = " + Files[Index], ref Conn);
            }
        }


        /// <summary>
        /// 判断文件夹名称是否存在
        /// </summary>
        private bool FolderExist(int Id, int FolderId, string Name, HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            bool Exist = false;
            string Query = "";
            string Sql = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_Folder = 1 And " + 
                     "A1.DBS_FolderId = " + FolderId + " And " + 
                     "A1.DBS_Name = '" + Name + "' And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_Folder = 1 And " + 
                     "A2.DBS_FolderId = " + FolderId + " And " + 
                     "A2.DBS_Name = '" + Name + "' And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_Id Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_Id Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_Id Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Name From DBS_File Where " + Query + "";

            Base.Data.SqlDataToTable(Sql, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                Exist = false;
            }
            else
            {
                if (Id == 0)
                {
                    Exist = true;
                }
                else
                {
                    if (FileTable["DBS_Id"].TypeInt() == Id)
                    {
                        Exist = false;
                    }
                    else
                    {
                        Exist = true;
                    }
                }
            }

            FileTable.Clear();

            return Exist;
        }


        /// <summary>
        /// 文件夹权限继承
        /// </summary>
        private void PurviewInherit(int Id, int FolderId)
        {
            Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);
            Base.Data.SqlQuery("Insert Into DBS_File_Purview(DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview) Select " + Id + ", DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_FileId = " + FolderId, ref Conn);
        }


        /// <summary>
        /// 文件夹权限同步
        /// </summary>
        private void PurviewSync(int Id, int FolderId)
        {
            ArrayList Folders = new ArrayList();
            string FolderPath = "";
            int Index = 0;

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_Sync From DBS_File Where DBS_Folder = 1 And DBS_Sync = 1 And DBS_FolderPath Like '" + FolderPath + "%'", ref Conn, ref Folders);

            for (Index = 0; Index < Folders.Count; Index++)
            {
                Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Folders[Index], ref Conn);
                Base.Data.SqlQuery("Insert Into DBS_File_Purview(DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview) Select " + Folders[Index] + ", DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_FileId = " + FolderId, ref Conn);
            }

            Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);
            Base.Data.SqlQuery("Insert Into DBS_File_Purview(DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview) Select " + Id + ", DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_FileId = " + FolderId, ref Conn);
        }


    }


}
