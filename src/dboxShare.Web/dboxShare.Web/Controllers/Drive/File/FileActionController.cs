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


    public class FileActionController : ApiController
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
        /// 文件重命名
        /// </summary>
        [Route("api/drive/file/rename")]
        [HttpPost]
        public HttpResponseMessage Rename()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\/\:\*\?\""\<\>\|]{1,75}$") == false)
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

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int FolderId = FileTable["DS_FolderId"].TypeInt();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                if (FileExists(Id, FolderId, Name, Extension) == true)
                {
                    return AppCommon.ResponseMessage("existed");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Name = '" + Name + "' Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'update' Where DS_FileId = " + Id, ref Conn);

                AppCommon.Log(Id, "file-rename", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件备注
        /// </summary>
        [Route("api/drive/file/remark")]
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

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Remark = '" + Remark + "' Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-remark", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件锁定
        /// </summary>
        [Route("api/drive/file/lock")]
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

                if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 1 Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-lock", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件取消锁定
        /// </summary>
        [Route("api/drive/file/unlock")]
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

                if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Base.Data.SqlQuery("Update DS_File Set DS_Lock = 0 Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-unlock", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件版本替换
        /// </summary>
        [Route("api/drive/file/replace")]
        [HttpPost]
        public HttpResponseMessage Replace()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            int VersionId = Context.Request.Form["VersionId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                // 读取当前版本信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_CodeId, DS_Hash, DS_Size, DS_Remark, DS_Lock, DS_UpdateUsername, DS_UpdateTime From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int CurrentVersion = FileTable["DS_Version"].TypeInt();
                string CurrentCodeId = FileTable["DS_CodeId"].TypeString();
                string CurrentHash = FileTable["DS_Hash"].TypeString();
                int CurrentSize = FileTable["DS_Size"].TypeInt();
                string CurrentRemark = FileTable["DS_Remark"].TypeString();
                string CurrentUpdateUsername = FileTable["DS_UpdateUsername"].TypeString();
                string CurrentUpdateTime = FileTable["DS_UpdateTime"].TypeString();

                FileTable.Clear();

                // 读取替换版本信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_CodeId, DS_Hash, DS_Size, DS_Remark, DS_Lock, DS_UpdateUsername, DS_UpdateTime From DS_File Where DS_VersionId = " + Id + " And DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + VersionId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int ReplaceVersion = FileTable["DS_Version"].TypeInt();
                string ReplaceCodeId = FileTable["DS_CodeId"].TypeString();
                string ReplaceHash = FileTable["DS_Hash"].TypeString();
                int ReplaceSize = FileTable["DS_Size"].TypeInt();
                string ReplaceRemark = FileTable["DS_Remark"].TypeString();
                string ReplaceUpdateUsername = FileTable["DS_UpdateUsername"].TypeString();
                string ReplaceUpdateTime = FileTable["DS_UpdateTime"].TypeString();

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Version = " + CurrentVersion + ", DS_CodeId = '" + CurrentCodeId + "', DS_Hash = '" + CurrentHash + "', DS_Size = " + CurrentSize + ", DS_Remark = '" + CurrentRemark + "', DS_UpdateUsername = '" + CurrentUpdateUsername + "', DS_UpdateTime = '" + CurrentUpdateTime + "' Where DS_Id = " + VersionId, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Version = " + ReplaceVersion + ", DS_CodeId = '" + ReplaceCodeId + "', DS_Hash = '" + ReplaceHash + "', DS_Size = " + ReplaceSize + ", DS_Remark = '" + ReplaceRemark + "', DS_UpdateUsername = '" + ReplaceUpdateUsername + "', DS_UpdateTime = '" + ReplaceUpdateTime + "' Where DS_Id = " + Id, ref Conn);

                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'update' Where DS_FileId = " + Id, ref Conn);

                AppCommon.Log(Id, "file-replace", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件复制
        /// </summary>
        [Route("api/drive/file/copy")]
        [HttpPost]
        public HttpResponseMessage Copy()
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

                if (AppCommon.PurviewCheck(Id, false, "editor", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string CodeId = FileTable["DS_CodeId"].TypeString();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                int FolderUserId = Context.Session["UserId"].TypeInt();
                string FolderUsername = Context.Session["Username"].TypeString();
                string FolderIdPath = "/0/";
                int FolderShare = 0;
                int FolderSync = 0;

                if (FolderId > 0)
                {
                    if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
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
                }

                string NewCodeId = AppCommon.FileCodeId();

                string NewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

                string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                string TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), NewCodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file does not exist");
                }
                else
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, TargetFilePath, CodeId);
                }

                string Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                             "Select DS_UserId, DS_Username, 1, 0, 0, " + FolderId + ", '" + FolderIdPath + "', '" + NewCodeId + "', DS_Hash, '" + NewName + "', DS_Extension, DS_Size, DS_Type, 'null', " + FolderShare + ", 0, " + FolderSync + ", 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00' From DS_File Where DS_Id = " + Id;

                int NewId = Base.Data.SqlInsert(Sql, ref Conn);

                if (NewId == 0)
                {
                    return AppCommon.ResponseMessage("data insertion failed");
                }

                Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + NewId + ", 1, 'add')", ref Conn);

                AppCommon.FileProcessorTrigger();

                AppCommon.Log(Id, "file-copy", ref Conn);
                AppCommon.Log(NewId, "file-add", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件移动
        /// </summary>
        [Route("api/drive/file/move")]
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

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string CodeId = FileTable["DS_CodeId"].TypeString();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                if (FileTable["DS_FolderId"].TypeInt() == FolderId)
                {
                    return AppCommon.ResponseMessage("operation has been banned");
                }

                FileTable.Clear();

                int FolderUserId = Context.Session["UserId"].TypeInt();
                string FolderUsername = Context.Session["Username"].TypeString();
                string FolderIdPath = "/0/";
                int FolderShare = 0;
                int FolderSync = 0;

                if (FolderId > 0)
                {
                    if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
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
                }

                string FolderNewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

                string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                string TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file does not exist");
                }
                else
                {
                    File.Move(SourceFilePath, TargetFilePath);

                    if (File.Exists("" + SourceFilePath + ".pdf") == true)
                    {
                        File.Move("" + SourceFilePath + ".pdf", "" + TargetFilePath + ".pdf");
                    }

                    if (File.Exists("" + SourceFilePath + ".flv") == true)
                    {
                        File.Move("" + SourceFilePath + ".flv", "" + TargetFilePath + ".flv");
                    }
                }

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Name = '" + FolderNewName + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + Id, ref Conn);

                Move_Version(Id, FolderPath, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync);

                AppCommon.Log(Id, "file-move", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件批量移动
        /// </summary>
        [Route("api/drive/file/move-all")]
        [HttpPost]
        public HttpResponseMessage MoveAll()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            int FolderId = Context.Request.Form["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                int FolderUserId = Context.Session["UserId"].TypeInt();
                string FolderUsername = Context.Session["Username"].TypeString();
                string FolderIdPath = "/0/";
                int FolderShare = 0;
                int FolderSync = 0;

                if (FolderId > 0)
                {
                    if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }

                    Hashtable FileTable = new Hashtable();

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
                }

                for (int i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    int Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        continue;
                    }

                    Hashtable FileTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    string FolderPath = FileTable["DS_FolderPath"].TypeString();
                    string CodeId = FileTable["DS_CodeId"].TypeString();
                    string Name = FileTable["DS_Name"].TypeString();
                    string Extension = FileTable["DS_Extension"].TypeString();

                    if (FileTable["DS_FolderId"].TypeInt() == FolderId)
                    {
                        continue;
                    }

                    FileTable.Clear();

                    string FolderNewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

                    string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                    string TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), CodeId + Extension);

                    if (File.Exists(SourceFilePath) == false)
                    {
                        continue;
                    }
                    else
                    {
                        File.Move(SourceFilePath, TargetFilePath);

                        if (File.Exists("" + SourceFilePath + ".pdf") == true)
                        {
                            File.Move("" + SourceFilePath + ".pdf", "" + TargetFilePath + ".pdf");
                        }

                        if (File.Exists("" + SourceFilePath + ".flv") == true)
                        {
                            File.Move("" + SourceFilePath + ".flv", "" + TargetFilePath + ".flv");
                        }
                    }

                    Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Name = '" + FolderNewName + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + Id, ref Conn);

                    Move_Version(Id, FolderPath, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync);

                    AppCommon.Log(Id, "file-move", ref Conn);
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
        /// 文件版本移动
        /// </summary>
        private void Move_Version(int Id, string FolderPath, int FolderId, string FolderIdPath, int FolderUserId, string FolderUsername, int FolderShare, int FolderSync)
        {
            List<Hashtable> FileList = new List<Hashtable>();

            Base.Data.SqlListToTable("Select DS_Id, DS_VersionId, DS_Folder, DS_CodeId, DS_Extension From DS_File Where DS_Folder = 0 And DS_VersionId = " + Id, ref Conn, ref FileList);

            for (int i = 0; i < FileList.Count; i++)
            {
                int FileId = FileList[i]["DS_Id"].TypeInt();
                string CodeId = FileList[i]["DS_CodeId"].TypeString();
                string Extension = FileList[i]["DS_Extension"].TypeString();

                string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                string TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    continue;
                }
                else
                {
                    File.Move(SourceFilePath, TargetFilePath);

                    if (File.Exists("" + SourceFilePath + ".pdf") == true)
                    {
                        File.Move("" + SourceFilePath + ".pdf", "" + TargetFilePath + ".pdf");
                    }

                    if (File.Exists("" + SourceFilePath + ".flv") == true)
                    {
                        File.Move("" + SourceFilePath + ".flv", "" + TargetFilePath + ".flv");
                    }
                }

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + FileId, ref Conn);
            }
        }


        /// <summary>
        /// 文件移除
        /// </summary>
        [Route("api/drive/file/remove")]
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

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 1, DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'delete' Where DS_FileId = " + Id, ref Conn);

                AppCommon.Log(Id, "file-remove", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件批量移除
        /// </summary>
        [Route("api/drive/file/remove-all")]
        [HttpPost]
        public HttpResponseMessage RemoveAll()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (int i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    int Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        continue;
                    }

                    Hashtable FileTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    FileTable.Clear();

                    Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 1, DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Id, ref Conn);
                    Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'delete' Where DS_Id = " + Id, ref Conn);

                    AppCommon.Log(Id, "file-remove", ref Conn);
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
        /// 文件还原
        /// </summary>
        [Route("api/drive/file/restore")]
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

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 0, DS_RemoveUsername = 'null', DS_RemoveTime = '1970/1/1 00:00:00' Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'add' Where DS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-restore", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件批量还原
        /// </summary>
        [Route("api/drive/file/restore-all")]
        [HttpPost]
        public HttpResponseMessage RestoreAll()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (int i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    int Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        continue;
                    }

                    Hashtable FileTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    FileTable.Clear();

                    Base.Data.SqlQuery("Update DS_File Set DS_Recycle = 0, DS_RemoveUsername = 'null', DS_RemoveTime = '1970/1/1 00:00:00' Where DS_Id = " + Id, ref Conn);
                    Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'add' Where DS_Id = " + Id, ref Conn);

                    AppCommon.Log(Id, "file-restore", ref Conn);
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
        /// 文件删除
        /// </summary>
        [Route("api/drive/file/delete")]
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

                if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string CodeId = FileTable["DS_CodeId"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                string FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                }

                string PDFFilePath = Base.Common.PathCombine(FilePath, ".pdf");

                if (File.Exists(PDFFilePath) == true)
                {
                    File.Delete(PDFFilePath);
                }

                string FLVFilePath = Base.Common.PathCombine(FilePath, ".flv");

                if (File.Exists(FLVFilePath) == true)
                {
                    File.Delete(FLVFilePath);
                }

                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Task Where DS_FileId = " + Id, ref Conn);

                Delete_Version(Id, FolderPath);

                AppCommon.Log(Id, "file-delete", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件批量删除
        /// </summary>
        [Route("api/drive/file/delete-all")]
        [HttpPost]
        public HttpResponseMessage DeleteAll()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (int i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    int Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                    {
                        continue;
                    }

                    Hashtable FileTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    string FolderPath = FileTable["DS_FolderPath"].TypeString();
                    string CodeId = FileTable["DS_CodeId"].TypeString();
                    string Extension = FileTable["DS_Extension"].TypeString();

                    FileTable.Clear();

                    string FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                    if (File.Exists(FilePath) == true)
                    {
                        File.Delete(FilePath);
                    }

                    string PDFFilePath = Base.Common.PathCombine(FilePath, ".pdf");

                    if (File.Exists(PDFFilePath) == true)
                    {
                        File.Delete(PDFFilePath);
                    }

                    string FLVFilePath = Base.Common.PathCombine(FilePath, ".flv");

                    if (File.Exists(FLVFilePath) == true)
                    {
                        File.Delete(FLVFilePath);
                    }

                    Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
                    Base.Data.SqlQuery("Delete From DS_File_Purview Where DS_FileId = " + Id, ref Conn);
                    Base.Data.SqlQuery("Delete From DS_File_Task Where DS_FileId = " + Id, ref Conn);

                    Delete_Version(Id, FolderPath);

                    AppCommon.Log(Id, "file-delete", ref Conn);
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
        /// 文件版本删除
        /// </summary>
        private void Delete_Version(int Id, string FolderPath)
        {
            List<Hashtable> FileList = new List<Hashtable>();

            Base.Data.SqlListToTable("Select DS_Id, DS_VersionId, DS_Folder, DS_CodeId, DS_Extension From DS_File Where DS_Folder = 0 And DS_VersionId = " + Id, ref Conn, ref FileList);

            for (int i = 0; i < FileList.Count; i++)
            {
                int FileId = FileList[i]["DS_Id"].TypeInt();
                string CodeId = FileList[i]["DS_CodeId"].TypeString();
                string Extension = FileList[i]["DS_Extension"].TypeString();

                string FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                }

                string PDFFilePath = Base.Common.PathCombine(FilePath, ".pdf");

                if (File.Exists(PDFFilePath) == true)
                {
                    File.Delete(PDFFilePath);
                }

                string FLVFilePath = Base.Common.PathCombine(FilePath, ".flv");

                if (File.Exists(FLVFilePath) == true)
                {
                    File.Delete(FLVFilePath);
                }

                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + FileId, ref Conn);
            }
        }


        /// <summary>
        /// 判断文件名称是否存在
        /// </summary>
        private bool FileExists(int Id, int FolderId, string Name, string Extension)
        {
            string Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_VersionId = 0 And " +
                     "A1.DS_Folder = 0 And " +
                     "A1.DS_FolderId = " + FolderId + " And " +
                     "A1.DS_Name = '" + Name + "' And " +
                     "A1.DS_Extension = '" + Extension + "' And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_VersionId = 0 And " +
                     "A2.DS_Folder = 0 And " +
                     "A2.DS_FolderId = " + FolderId + " And " +
                     "A2.DS_Name = '" + Name + "' And " +
                     "A2.DS_Extension = '" + Extension + "' And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B.DS_FolderId Where " +
                     "DS_File.DS_Id = B.DS_Id And " +
                     "DS_File.DS_VersionId = 0 And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File.DS_Extension = '" + Extension + "' And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_FolderId Where " +
                     "DS_File.DS_Id = C.DS_Id And " +
                     "DS_File.DS_VersionId = 0 And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File.DS_Extension = '" + Extension + "' And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_FolderId Where " +
                     "DS_File.DS_Id = D.DS_Id And " +
                     "DS_File.DS_VersionId = 0 And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Name = '" + Name + "' And " +
                     "DS_File.DS_Extension = '" + Extension + "' And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            string Sql = "Select DS_Id, DS_Folder, DS_FolderId, DS_Name, DS_Extension From DS_File Where " + Query + "";

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


    }


}
