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


    public class Drive_FileActionController : ApiController
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\/\:\*\?\""\<\>\|]{1,75}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var FolderId = FileTable["DS_FolderId"].TypeInt();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                if (FileExists(Id, FolderId, Name, Extension, Share) == true)
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var Remark = Context.Request.Form["Remark"].TypeString();

            if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,100}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }
            else
            {
                Remark = Base.Common.InputFilter(Remark);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var CurrentId = Context.Request.Form["Id"].TypeInt();

            var VersionId = Context.Request.Form["VersionId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 读取文件正本信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_CodeId, DS_Hash, DS_Size, DS_Remark, DS_Share, DS_Lock, DS_UpdateUsername, DS_UpdateTime From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + CurrentId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var CurrentVersion = FileTable["DS_Version"].TypeInt();
                var CurrentCodeId = FileTable["DS_CodeId"].TypeString();
                var CurrentHash = FileTable["DS_Hash"].TypeString();
                var CurrentSize = FileTable["DS_Size"].TypeLong();
                var CurrentRemark = FileTable["DS_Remark"].TypeString();
                var CurrentShare = FileTable["DS_Share"].TypeInt();
                var CurrentUpdateUsername = FileTable["DS_UpdateUsername"].TypeString();
                var CurrentUpdateTime = FileTable["DS_UpdateTime"].TypeString();

                FileTable.Clear();

                if (CurrentShare == 1)
                {
                    if (AppCommon.PurviewCheck(CurrentId, false, "creator", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                // 读取文件版本信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_CodeId, DS_Hash, DS_Size, DS_Remark, DS_Lock, DS_UpdateUsername, DS_UpdateTime From DS_File Where DS_VersionId = " + CurrentId + " And DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + VersionId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var VersionVersion = FileTable["DS_Version"].TypeInt();
                var VersionCodeId = FileTable["DS_CodeId"].TypeString();
                var VersionHash = FileTable["DS_Hash"].TypeString();
                var VersionSize = FileTable["DS_Size"].TypeLong();
                var VersionRemark = FileTable["DS_Remark"].TypeString();
                var VersionUpdateUsername = FileTable["DS_UpdateUsername"].TypeString();
                var VersionUpdateTime = FileTable["DS_UpdateTime"].TypeString();

                FileTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_Version = " + CurrentVersion + ", DS_CodeId = '" + CurrentCodeId + "', DS_Hash = '" + CurrentHash + "', DS_Size = " + CurrentSize + ", DS_Remark = '" + CurrentRemark + "', DS_UpdateUsername = '" + CurrentUpdateUsername + "', DS_UpdateTime = '" + CurrentUpdateTime + "' Where DS_Id = " + VersionId, ref Conn);
                Base.Data.SqlQuery("Update DS_File Set DS_Version = " + VersionVersion + ", DS_CodeId = '" + VersionCodeId + "', DS_Hash = '" + VersionHash + "', DS_Size = " + VersionSize + ", DS_Remark = '" + VersionRemark + "', DS_UpdateUsername = '" + VersionUpdateUsername + "', DS_UpdateTime = '" + VersionUpdateTime + "' Where DS_Id = " + CurrentId, ref Conn);

                Base.Data.SqlQuery("Update DS_File_Task Set DS_Index = 'update' Where DS_FileId = " + CurrentId, ref Conn);

                AppCommon.Log(CurrentId, "file-replace", ref Conn);

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var CopyId = Context.Request.Form["Id"].TypeInt();

            var FolderId = Context.Request.Form["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 读取复制文件信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + CopyId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var CopyFolderPath = FileTable["DS_FolderPath"].TypeString();
                var CopyCodeId = FileTable["DS_CodeId"].TypeString();
                var CopyName = FileTable["DS_Name"].TypeString();
                var CopyExtension = FileTable["DS_Extension"].TypeString();
                var CopyShare = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (CopyShare == 1)
                {
                    if (AppCommon.PurviewCheck(CopyId, false, "editor", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                var FolderUserId = Context.Session["UserId"].TypeInt();
                var FolderUsername = Context.Session["Username"].TypeString();
                var FolderIdPath = "/0/";
                var FolderShare = 0;
                var FolderSync = 0;

                if (FolderId > 0)
                {
                    // 读取目标文件夹信息
                    Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share, DS_Lock, DS_Sync From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data-not-exist");
                    }
                    else
                    {
                        FolderUserId = FileTable["DS_UserId"].TypeInt();
                        FolderUsername = FileTable["DS_Username"].TypeString();
                        FolderShare = FileTable["DS_Share"].TypeInt();
                        FolderSync = FileTable["DS_Sync"].TypeInt();
                    }

                    FileTable.Clear();

                    if (FolderShare == 1)
                    {
                        if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                        {
                            return AppCommon.ResponseMessage("no-permission");
                        }
                    }

                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, CopyFolderPath.Substring(1), CopyCodeId + CopyExtension);

                var TargetFileCodeId = AppCommon.FileCodeId();

                var TargetFileName = AppCommon.FileName(FolderId, FolderShare, CopyName, CopyExtension, ref Conn);

                var TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), TargetFileCodeId + CopyExtension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file-not-exist");
                }
                else
                {
                    if (AppConfig.StorageEncryption == true)
                    {
                        Base.Crypto.FileDecrypt(SourceFilePath, TargetFilePath, CopyCodeId);
                    }
                    else
                    {
                        File.Copy(SourceFilePath, TargetFilePath);
                    }
                }

                var Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                          "Select DS_UserId, DS_Username, 1, 0, 0, " + FolderId + ", '" + FolderIdPath + "', '" + TargetFileCodeId + "', DS_Hash, '" + TargetFileName + "', DS_Extension, DS_Size, DS_Type, 'null', " + FolderShare + ", 0, " + FolderSync + ", 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00' From DS_File Where DS_Id = " + CopyId;

                var NewId = Base.Data.SqlInsert(Sql, ref Conn);

                if (NewId == 0)
                {
                    return AppCommon.ResponseMessage("data-insertion-failed");
                }

                var IsProcess = 1;

                if (AppConfig.StorageEncryption == false)
                {
                    IsProcess = AppCommon.IsConvert(CopyExtension) == true ? 1 : 0;
                }

                Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + NewId + ", " + IsProcess + ", 'add')", ref Conn);

                AppCommon.FileProcessorTrigger();

                AppCommon.Log(CopyId, "file-copy", ref Conn);
                AppCommon.Log(NewId, "file-copy", ref Conn);

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var MoveId = Context.Request.Form["Id"].TypeInt();

            var FolderId = Context.Request.Form["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 读取移动文件信息
                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + MoveId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var MoveFolderPath = FileTable["DS_FolderPath"].TypeString();
                var MoveCodeId = FileTable["DS_CodeId"].TypeString();
                var MoveName = FileTable["DS_Name"].TypeString();
                var MoveExtension = FileTable["DS_Extension"].TypeString();
                var MoveShare = FileTable["DS_Share"].TypeInt();

                if (FileTable["DS_FolderId"].TypeInt() == FolderId)
                {
                    return AppCommon.ResponseMessage("operation-forbidden");
                }

                FileTable.Clear();

                if (MoveShare == 1)
                {
                    if (AppCommon.PurviewCheck(MoveId, false, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                var FolderUserId = Context.Session["UserId"].TypeInt();
                var FolderUsername = Context.Session["Username"].TypeString();
                var FolderIdPath = "/0/";
                var FolderShare = 0;
                var FolderSync = 0;

                if (FolderId > 0)
                {
                    // 读取目标文件夹信息
                    Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share, DS_Lock, DS_Sync From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data-not-exist");
                    }
                    else
                    {
                        FolderUserId = FileTable["DS_UserId"].TypeInt();
                        FolderUsername = FileTable["DS_Username"].TypeString();
                        FolderShare = FileTable["DS_Share"].TypeInt();
                        FolderSync = FileTable["DS_Sync"].TypeInt();
                    }

                    FileTable.Clear();

                    if (FolderShare == 1)
                    {
                        if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                        {
                            return AppCommon.ResponseMessage("no-permission");
                        }
                    }

                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, MoveFolderPath.Substring(1), MoveCodeId + MoveExtension);

                var TargetFileName = AppCommon.FileName(FolderId, FolderShare, MoveName, MoveExtension, ref Conn);

                var TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), MoveCodeId + MoveExtension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file-not-exist");
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

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Name = '" + TargetFileName + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + MoveId, ref Conn);

                Move_Version(MoveId, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync);

                AppCommon.Log(MoveId, "file-move", ref Conn);

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            var FolderId = Context.Request.Form["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FolderUserId = Context.Session["UserId"].TypeInt();
                var FolderUsername = Context.Session["Username"].TypeString();
                var FolderIdPath = "/0/";
                var FolderShare = 0;
                var FolderSync = 0;

                if (FolderId > 0)
                {
                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    // 读取目标文件夹信息
                    Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share, DS_Lock, DS_Sync From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data-not-exist");
                    }
                    else
                    {
                        FolderUserId = FileTable["DS_UserId"].TypeInt();
                        FolderUsername = FileTable["DS_Username"].TypeString();
                        FolderShare = FileTable["DS_Share"].TypeInt();
                        FolderSync = FileTable["DS_Sync"].TypeInt();
                    }

                    FileTable.Clear();

                    if (FolderShare == 1)
                    {
                        if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                        {
                            return AppCommon.ResponseMessage("no-permission");
                        }
                    }

                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var MoveId = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    // 读取移动文件信息
                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + MoveId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var MoveFolderPath = FileTable["DS_FolderPath"].TypeString();
                    var MoveCodeId = FileTable["DS_CodeId"].TypeString();
                    var MoveName = FileTable["DS_Name"].TypeString();
                    var MoveExtension = FileTable["DS_Extension"].TypeString();
                    var MoveShare = FileTable["DS_Share"].TypeInt();

                    if (FileTable["DS_FolderId"].TypeInt() == FolderId)
                    {
                        continue;
                    }

                    FileTable.Clear();

                    if (MoveShare == 1)
                    {
                        if (AppCommon.PurviewCheck(MoveId, false, "manager", ref Conn) == false)
                        {
                            continue;
                        }
                    }

                    var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, MoveFolderPath.Substring(1), MoveCodeId + MoveExtension);

                    var TargetFileName = AppCommon.FileName(FolderId, FolderShare, MoveName, MoveExtension, ref Conn);

                    var TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), MoveCodeId + MoveExtension);

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

                    Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Name = '" + TargetFileName + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + MoveId, ref Conn);

                    Move_Version(MoveId, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync);

                    AppCommon.Log(MoveId, "file-move", ref Conn);
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
        private void Move_Version(int Id, int FolderId, string FolderIdPath, int FolderUserId, string FolderUsername, int FolderShare, int FolderSync)
        {
            var FileList = new List<Hashtable>();

            Base.Data.SqlListToTable("Select DS_Id, DS_VersionId, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension From DS_File Where DS_Folder = 0 And DS_VersionId = " + Id, ref Conn, ref FileList);

            for (var i = 0; i < FileList.Count; i++)
            {
                var MoveId = FileList[i]["DS_Id"].TypeInt();
                var MoveFolderPath = FileList[i]["DS_FolderPath"].TypeString();
                var MoveCodeId = FileList[i]["DS_CodeId"].TypeString();
                var MoveExtension = FileList[i]["DS_Extension"].TypeString();

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, MoveFolderPath.Substring(1), MoveCodeId + MoveExtension);

                var TargetFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1), MoveCodeId + MoveExtension);

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

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + FolderUserId + ", DS_Username = '" + FolderUsername + "', DS_FolderId = " + FolderId + ", DS_FolderPath = '" + FolderIdPath + "', DS_Share = " + FolderShare + ", DS_Sync = " + FolderSync + " Where DS_Id = " + MoveId, ref Conn);
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var Share = FileTable["DS_Share"].TypeInt();

                    FileTable.Clear();

                    if (Share == 1)
                    {
                        if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                        {
                            continue;
                        }
                    }

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var Share = FileTable["DS_Share"].TypeInt();

                    FileTable.Clear();

                    if (Share == 1)
                    {
                        if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                        {
                            continue;
                        }
                    }

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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var CodeId = FileTable["DS_CodeId"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                var FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                }

                var PDFFilePath = "" + FilePath + ".pdf";

                if (File.Exists(PDFFilePath) == true)
                {
                    File.Delete(PDFFilePath);
                }

                var FLVFilePath = "" + FilePath + ".flv";

                if (File.Exists(FLVFilePath) == true)
                {
                    File.Delete(FLVFilePath);
                }

                Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Collect Where DS_FileId = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_File_Follow Where DS_FileId = " + Id, ref Conn);
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Share, DS_Lock From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var FolderPath = FileTable["DS_FolderPath"].TypeString();
                    var CodeId = FileTable["DS_CodeId"].TypeString();
                    var Extension = FileTable["DS_Extension"].TypeString();
                    var Share = FileTable["DS_Share"].TypeInt();

                    FileTable.Clear();

                    if (Share == 1)
                    {
                        if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                        {
                            continue;
                        }
                    }

                    var FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                    if (File.Exists(FilePath) == true)
                    {
                        File.Delete(FilePath);
                    }

                    var PDFFilePath = "" + FilePath + ".pdf";

                    if (File.Exists(PDFFilePath) == true)
                    {
                        File.Delete(PDFFilePath);
                    }

                    var FLVFilePath = "" + FilePath + ".flv";

                    if (File.Exists(FLVFilePath) == true)
                    {
                        File.Delete(FLVFilePath);
                    }

                    Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
                    Base.Data.SqlQuery("Delete From DS_File_Collect Where DS_FileId = " + Id, ref Conn);
                    Base.Data.SqlQuery("Delete From DS_File_Follow Where DS_FileId = " + Id, ref Conn);
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
            var FileList = new List<Hashtable>();

            Base.Data.SqlListToTable("Select DS_Id, DS_VersionId, DS_Folder, DS_CodeId, DS_Extension From DS_File Where DS_Folder = 0 And DS_VersionId = " + Id, ref Conn, ref FileList);

            for (var i = 0; i < FileList.Count; i++)
            {
                var FileId = FileList[i]["DS_Id"].TypeInt();
                var CodeId = FileList[i]["DS_CodeId"].TypeString();
                var Extension = FileList[i]["DS_Extension"].TypeString();

                var FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                }

                var PDFFilePath = "" + FilePath + ".pdf";

                if (File.Exists(PDFFilePath) == true)
                {
                    File.Delete(PDFFilePath);
                }

                var FLVFilePath = "" + FilePath + ".flv";

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
        private bool FileExists(int Id, int FolderId, string Name, string Extension, int Share)
        {
            var Query = "";

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
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "'";

            if (Share == 1)
            {
                Query += " Union All ";

                // 共享部门查询
                Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = B.DS_FolderId And " +
                         "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Where " +
                         "B.DS_Id = DS_File.DS_Id And " +
                         "B.DS_VersionId = 0 And " +
                         "B.DS_Folder = 0 And " +
                         "B.DS_FolderId = " + FolderId + " And " +
                         "B.DS_Share = 1 And " +
                         "B.DS_Name = '" + Name + "' And " +
                         "B.DS_Extension = '" + Extension + "' Union All ";

                // 共享角色查询
                Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = C.DS_FolderId And " +
                         "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Where " +
                         "C.DS_Id = DS_File.DS_Id And " +
                         "C.DS_VersionId = 0 And " +
                         "C.DS_Folder = 0 And " +
                         "C.DS_FolderId = " + FolderId + " And " +
                         "C.DS_Share = 1 And " +
                         "C.DS_Name = '" + Name + "' And " +
                         "C.DS_Extension = '" + Extension + "' Union All ";

                // 共享用户查询
                Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = D.DS_FolderId And " +
                         "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Where " +
                         "D.DS_Id = DS_File.DS_Id And " +
                         "D.DS_VersionId = 0 And " +
                         "D.DS_Folder = 0 And " +
                         "D.DS_FolderId = " + FolderId + " And " +
                         "D.DS_Share = 1 And " +
                         "D.DS_Name = '" + Name + "' And " +
                         "D.DS_Extension = '" + Extension + "'";
            }

            Query += ")";

            var Sql = "Select DS_Id, DS_Folder, DS_FolderId, DS_Name, DS_Extension From DS_File Where " + Query + "";

            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Base.Data.SqlDataToTable(Sql, ref Conn, ref FileTable);

            var Exists = false;

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
