using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class FolderActionController : ApiController
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
        /// 文件夹添加
        /// </summary>
        [Route("api/drive/folder/add")]
        [HttpPost]
        public HttpResponseMessage Add()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int FolderId = Context.Request.Form["FolderId"].TypeInt();

            string Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\/\:\*\?\""\<\>\|]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            string Remark = Context.Request.Form["Remark"].TypeString();

            if (string.IsNullOrEmpty(Remark) == false)
            {
                Remark = Base.Common.InputFilter(Remark);

                if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,100}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            int Inherit = Context.Request.Form["Inherit"].TypeString() == "true" ? 1 : 0;

            int Share = Context.Request.Form["Share"].TypeString() == "true" ? 1 : 0;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                int UserId = Context.Session["UserId"].TypeInt();
                string Username = Context.Session["Username"].TypeString();
                string FolderIdPath = "/0/";

                if (FolderId > 0)
                {
                    if (AppCommon.PurviewCheck(FolderId, true, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }

                    if (Inherit == 1)
                    {
                        Hashtable FileTable = new Hashtable();

                        Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                        if (FileTable["Exists"].TypeBool() == false)
                        {
                            return AppCommon.ResponseMessage("data does not exist or is invalid");
                        }
                        else
                        {
                            UserId = FileTable["DS_UserId"].TypeInt();
                            Username = FileTable["DS_Username"].TypeString();
                            Share = FileTable["DS_Share"].TypeInt();
                        }

                        FileTable.Clear();
                    }

                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                if (FolderExists(0, FolderId, Name) == true)
                {
                    return AppCommon.ResponseMessage("existed");
                }

                string Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                             "Values(" + UserId + ", '" + Username + "', 0, 0, 1, " + FolderId + ", '" + FolderIdPath + "', 'null', 'null', '" + Name + "', 'null', 0, 'folder', '" + Remark + "', " + Share + ", 0, " + Inherit + ", 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                int Id = Base.Data.SqlInsert(Sql, ref Conn);

                if (Id == 0)
                {
                    return AppCommon.ResponseMessage("data insertion failed");
                }

                if (FolderId == 0)
                {
                    Directory.CreateDirectory(Base.Common.PathCombine(AppConfig.FileStoragePath, Id.ToString()));
                }
                else
                {
                    Directory.CreateDirectory(Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), Id.ToString()));

                    if (Inherit == 1)
                    {
                        PurviewInherit(Id, FolderId);
                    }
                }

                AppCommon.Log(Id, "folder-add", ref Conn);

                if (Share == 1)
                {
                    Context.Response.Cookies["New-Folder-Id"].Value = Id.ToString();
                }
                else
                {
                    Context.Response.Cookies.Remove("New-Folder-Id");
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
        /// 文件夹重命名
        /// </summary>
        [Route("api/drive/folder/rename")]
        [HttpPost]
        public HttpResponseMessage Rename()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\/\:\*\?\""\<\>\|]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_Lock From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int FolderId = FileTable["DS_FolderId"].TypeInt();

                FileTable.Clear();

                if (FolderExists(Id, FolderId, Name) == true)
                {
                    return AppCommon.ResponseMessage("existed");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Name = '" + Name + "' Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "folder-rename", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹备注
        /// </summary>
        [Route("api/drive/folder/remark")]
        [HttpPost]
        public HttpResponseMessage Remark()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Remark = Context.Request.Form["Remark"].TypeString();

            if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,100}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }
            else
            {
                Remark = Base.Common.InputFilter(Remark);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Remark = '" + Remark + "' Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "folder-remark", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹锁定
        /// </summary>
        [Route("api/drive/folder/lock")]
        [HttpPost]
        public HttpResponseMessage Lock()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 1 Where DS_VersionId = 0 And DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 1 Where DS_VersionId = 0 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn);

                AppCommon.Log(Id, "folder-lock", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹取消锁定
        /// </summary>
        [Route("api/drive/folder/unlock")]
        [HttpPost]
        public HttpResponseMessage Unlock()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 0 Where DS_VersionId = 0 And DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 0 Where DS_VersionId = 0 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn);

                AppCommon.Log(Id, "folder-unlock", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹移动
        /// </summary>
        [Route("api/drive/folder/move")]
        [HttpPost]
        public HttpResponseMessage Move()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            int FolderId = Context.Request.Form["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Name, DS_Lock From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                string Name = FileTable["DS_Name"].TypeString();

                FileTable.Clear();

                int FolderUserId = Context.Session["UserId"].TypeInt();
                string FolderUsername = Context.Session["Username"].TypeString();
                string FolderIdPath = "/0/";
                int FolderShare = 0;
                int FolderSync = 0;

                if (FolderId > 0)
                {
                    if (AppCommon.PurviewCheck(FolderId, true, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }

                    Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share, DS_Lock, DS_Sync From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data does not exist or is invalid");
                    }
                    else
                    {
                        FolderUserId = FileTable["DS_UserId"].TypeInt();
                        FolderUsername = FileTable["DS_Username"].TypeString();
                        FolderShare = FileTable["DS_Share"].TypeInt();
                        FolderSync = FileTable["DS_Sync"].TypeInt();
                    }

                    FileTable.Clear();

                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);

                    // 判断是否移动到子文件夹
                    if (FolderIdPath.IndexOf("/" + Id + "/") > -1)
                    {
                        if (FolderIdPath.Substring(FolderIdPath.IndexOf("/" + Id + "/")).IndexOf("/" + FolderId + "/") > -1)
                        {
                            return AppCommon.ResponseMessage("operation has been banned");
                        }
                    }
                }

                string SourceDirectoryPath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1));

                string TargetDirectoryPath = "";

                if (FolderId == 0)
                {
                    TargetDirectoryPath = Base.Common.PathCombine(AppConfig.FileStoragePath, Id.ToString());
                }
                else
                {
                    TargetDirectoryPath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), Id.ToString());
                }

                if (SourceDirectoryPath == TargetDirectoryPath)
                {
                    return AppCommon.ResponseMessage("operation has been banned");
                }

                if (Directory.Exists(SourceDirectoryPath) == true)
                {
                    Directory.Move(SourceDirectoryPath, TargetDirectoryPath);
                }

                string FolderNewName = AppCommon.FolderName(FolderId, Name, ref Conn);

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Name = '" + FolderNewName + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + Id, ref Conn);

                Move_Subfolder(Id, FolderUserId, FolderUsername, FolderShare, FolderSync);

                PurviewSync(Id, FolderId);

                AppCommon.Log(Id, "folder-move", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹批量移动
        /// </summary>
        [Route("api/drive/folder/move-all")]
        [HttpPost]
        public HttpResponseMessage MoveAll()
        {
            return Move();
        }


        /// <summary>
        /// 子文件夹移动
        /// </summary>
        private void Move_Subfolder(int Id, int FolderUserId, string FolderUsername, int FolderShare, int FolderSync)
        {
            string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            ArrayList Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_FolderId = " + Id, ref Conn, ref Folders);

            for (int i = 0; i < Folders.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + Id + ", DS_FolderPath = '" + FolderPath + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + Folders[i], ref Conn);

                Move_Subfolder(Folders[i].TypeInt(), FolderUserId, FolderUsername, FolderShare, FolderSync);
            }

            ArrayList Files = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 0 And DS_FolderId = " + Id, ref Conn, ref Files);

            for (int i = 0; i < Files.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + Id + ", DS_FolderPath = '" + FolderPath + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Folder = 0 And DS_Id = " + Files[i], ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + Id + ", DS_FolderPath = '" + FolderPath + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Folder = 0 And DS_VersionId = " + Files[i], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹移除
        /// </summary>
        [Route("api/drive/folder/remove")]
        [HttpPost]
        public HttpResponseMessage Remove()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 1, DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Id, ref Conn);

                Remove_Subfolder(Id);

                AppCommon.Log(Id, "folder-remove", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹批量移除
        /// </summary>
        [Route("api/drive/folder/remove-all")]
        [HttpPost]
        public HttpResponseMessage RemoveAll()
        {
            return Remove();
        }


        /// <summary>
        /// 子文件夹移除
        /// </summary>
        private void Remove_Subfolder(int Id)
        {
            ArrayList Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_FolderId = " + Id, ref Conn, ref Folders);

            for (int i = 0; i < Folders.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 1, DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Folders[i], ref Conn);

                Remove_Subfolder(Folders[i].TypeInt());
            }

            ArrayList Files = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_VersionId, DS_Folder, DS_FolderId From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_FolderId = " + Id, ref Conn, ref Files);

            for (int i = 0; i < Files.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 1, DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Files[i], ref Conn);
                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'delete' Where DS_FileId = " + Files[i], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹还原
        /// </summary>
        [Route("api/drive/folder/restore")]
        [HttpPost]
        public HttpResponseMessage Restore()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 0, DS_RemoveUsername = 'null', DS_RemoveTime = '1970/1/1 00:00:00' Where DS_Id = " + Id, ref Conn);

                Restore_Subfolder(Id);

                AppCommon.Log(Id, "folder-restore", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹批量还原
        /// </summary>
        [Route("api/drive/folder/restore-all")]
        [HttpPost]
        public HttpResponseMessage RestoreAll()
        {
            return Restore();
        }


        /// <summary>
        /// 子文件夹还原
        /// </summary>
        private void Restore_Subfolder(int Id)
        {
            ArrayList Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_FolderId = " + Id, ref Conn, ref Folders);

            for (int i = 0; i < Folders.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 0, DS_RemoveUsername = 'null', DS_RemoveTime = '1970/1/1 00:00:00' Where DS_Id = " + Folders[i], ref Conn);

                Restore_Subfolder(Folders[i].TypeInt());
            }

            ArrayList Files = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_VersionId, DS_Folder, DS_FolderId From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_FolderId = " + Id, ref Conn, ref Files);

            for (int i = 0; i < Files.Count; i++)
            {
                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 0, DS_RemoveUsername = 'null', DS_RemoveTime = '1970/1/1 00:00:00' Where DS_Id = " + Files[i], ref Conn);
                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'add' Where DS_FileId = " + Files[i], ref Conn);
            }
        }


        /// <summary>
        /// 文件夹删除
        /// </summary>
        [Route("api/drive/folder/delete")]
        [HttpPost]
        public HttpResponseMessage Delete()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, true, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                FileTable.Clear();

                string DirectoryPath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1));

                if (Directory.Exists(DirectoryPath) == true)
                {
                    Directory.Delete(DirectoryPath, true);
                }

                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Id, ref Conn);

                Delete_Subfolder(Id);

                AppCommon.Log(Id, "folder-delete", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件夹批量删除
        /// </summary>
        [Route("api/drive/folder/delete-all")]
        [HttpPost]
        public HttpResponseMessage DeleteAll()
        {
            return Delete();
        }


        /// <summary>
        /// 子文件夹删除
        /// </summary>
        private void Delete_Subfolder(int Id)
        {
            ArrayList Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_FolderId = " + Id, ref Conn, ref Folders);

            for (int i = 0; i < Folders.Count; i++)
            {
                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Folders[i], ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Folders[i], ref Conn);

                Delete_Subfolder(Folders[i].TypeInt());
            }

            ArrayList Files = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_VersionId, DS_Folder, DS_FolderId From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_FolderId = " + Id, ref Conn, ref Files);

            for (int i = 0; i < Files.Count; i++)
            {
                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Files[i], ref Conn);
                Base.Data.SqlQuery("Delete From DS_File Where DS_VersionId = " + Files[i], ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Task Where DS_FileId = " + Files[i], ref Conn);
            }
        }


        /// <summary>
        /// 判断文件夹名称是否存在
        /// </summary>
        private bool FolderExists(int Id, int FolderId, string Name)
        {
            string Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Folder = 1 And " +
                     "A1.DS_FolderId = " + FolderId + " And " +
                     "A1.DS_Name = '" + Name + "' And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_Folder = 1 And " +
                     "A2.DS_FolderId = " + FolderId + " And " +
                     "A2.DS_Name = '" + Name + "' And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B.DS_Id Where " +
                     "DS_File.DS_Id = B.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_Id Where " +
                     "DS_File.DS_Id = C.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_Id Where " +
                     "DS_File.DS_Id = D.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            string Sql = "Select DS_Id, DS_Folder, DS_FolderId, DS_Name From DS_File Where " + Query + "";

            Hashtable FileTable = new Hashtable();

            Base.Data.SqlDataToTable(Sql, ref Conn, ref FileTable);

            bool Exists = false;

            if (FileTable["Exists"].TypeBool() == false)
            {
                Exists = false;
            }
            else
            {
                if (Id == 0)
                {
                    Exists = true;
                }
                else
                {
                    if (FileTable["DS_Id"].TypeInt() == Id)
                    {
                        Exists = false;
                    }
                    else
                    {
                        Exists = true;
                    }
                }
            }

            FileTable.Clear();

            return Exists;
        }


        /// <summary>
        /// 文件夹权限继承
        /// </summary>
        private void PurviewInherit(int Id, int FolderId)
        {
            Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Id, ref Conn);
            Base.Data.SqlQuery("Insert Into DS_File_Purview(DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview) Select " + Id + ", DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview From DS_File_Purview Where DS_FileId = " + FolderId, ref Conn);
        }


        /// <summary>
        /// 文件夹权限同步
        /// </summary>
        private void PurviewSync(int Id, int FolderId)
        {
            string FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            ArrayList Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_Folder, DS_FolderPath, DS_Sync From DS_File Where DS_Folder = 1 And DS_Sync = 1 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn, ref Folders);

            for (int i = 0; i < Folders.Count; i++)
            {
                Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Folders[i], ref Conn);
                Base.Data.SqlQuery("Insert Into DS_File_Purview(DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview) Select " + Folders[i] + ", DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview From DS_File_Purview Where DS_FileId = " + FolderId, ref Conn);
            }

            Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Id, ref Conn);
            Base.Data.SqlQuery("Insert Into DS_File_Purview(DS_FileId, DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview) Select " + Id + ", DS_DepartmentId, DS_RoleId, DS_UserId, DS_Purview From DS_File_Purview Where DS_FileId = " + FolderId, ref Conn);
        }


    }


}
