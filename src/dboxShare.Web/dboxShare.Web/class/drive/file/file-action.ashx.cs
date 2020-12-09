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


    public class FileAction : IHttpHandler, IReadOnlySessionState
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

                case "replace":
                    Replace(Context);
                    break;

                case "copy":
                    Copy(Context);
                    break;

                case "move":
                    Move(Context);
                    break;

                case "moveall":
                    MoveAll(Context);
                    break;

                case "remove":
                    Remove(Context);
                    break;

                case "removeall":
                    RemoveAll(Context);
                    break;

                case "restore":
                    Restore(Context);
                    break;

                case "restoreall":
                    RestoreAll(Context);
                    break;

                case "delete":
                    Delete(Context);
                    break;

                case "deleteall":
                    DeleteAll(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 文件重命名
        /// </summary>
        private void Rename(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int FolderId = 0;
            string Name = "";
            string Extension = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,75}$") == false)
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderId = FileTable["DBS_FolderId"].TypeInt();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            if (FileExist(Id, FolderId, Name, Extension, Context) == true)
            {
                Context.Response.Write("existed");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Name = '" + Name + "' Where DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'update' Where DBS_FileId = " + Id, ref Conn);

            AppCommon.Log(Id, "file-rename", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件备注
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

            if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Remark = '" + Remark + "' Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "file-remark", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件锁定
        /// </summary>
        private void Lock(HttpContext Context)
        {
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 1 Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "file-lock", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件取消锁定
        /// </summary>
        private void Unlock(HttpContext Context)
        {
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlQuery("Update DBS_File Set DBS_Lock = 0 Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "file-unlock", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件替换(版本)
        /// </summary>
        private void Replace(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int VersionId = 0;
            int CurrentVersion = 0;
            string CurrentCodeId = "";
            string CurrentHash = "";
            int CurrentSize = 0;
            string CurrentRemark = "";
            string CurrentUpdateUsername = "";
            string CurrentUpdateTime = "";
            int ReplaceVersion = 0;
            string ReplaceCodeId = "";
            string ReplaceHash = "";
            int ReplaceSize = 0;
            string ReplaceRemark = "";
            string ReplaceUpdateUsername = "";
            string ReplaceUpdateTime = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["VersionId"]) == true)
            {
                VersionId = Context.Request.Form["VersionId"].TypeInt();
            }
            else
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            // 读取当前版本信息
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_CodeId, DBS_Hash, DBS_Size, DBS_Remark, DBS_Lock, DBS_UpdateUsername, DBS_UpdateTime From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                CurrentVersion = FileTable["DBS_Version"].TypeInt();
                CurrentCodeId = FileTable["DBS_CodeId"].TypeString();
                CurrentHash = FileTable["DBS_Hash"].TypeString();
                CurrentSize = FileTable["DBS_Size"].TypeInt();
                CurrentRemark = FileTable["DBS_Remark"].TypeString();
                CurrentUpdateUsername = FileTable["DBS_UpdateUsername"].TypeString();
                CurrentUpdateTime = FileTable["DBS_UpdateTime"].TypeString();
            }

            FileTable.Clear();

            // 读取替换版本信息
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_CodeId, DBS_Hash, DBS_Size, DBS_Remark, DBS_Lock, DBS_UpdateUsername, DBS_UpdateTime From DBS_File Where DBS_VersionId = " + Id + " And DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + VersionId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                ReplaceVersion = FileTable["DBS_Version"].TypeInt();
                ReplaceCodeId = FileTable["DBS_CodeId"].TypeString();
                ReplaceHash = FileTable["DBS_Hash"].TypeString();
                ReplaceSize = FileTable["DBS_Size"].TypeInt();
                ReplaceRemark = FileTable["DBS_Remark"].TypeString();
                ReplaceUpdateUsername = FileTable["DBS_UpdateUsername"].TypeString();
                ReplaceUpdateTime = FileTable["DBS_UpdateTime"].TypeString();
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_Version = " + CurrentVersion + ", DBS_CodeId = '" + CurrentCodeId + "', DBS_Hash = '" + CurrentHash + "', DBS_Size = " + CurrentSize + ", DBS_Remark = '" + CurrentRemark + "', DBS_UpdateUsername = '" + CurrentUpdateUsername + "', DBS_UpdateTime = '" + CurrentUpdateTime + "' Where DBS_Id = " + VersionId, ref Conn);
            Base.Data.SqlQuery("Update DBS_File Set DBS_Version = " + ReplaceVersion + ", DBS_CodeId = '" + ReplaceCodeId + "', DBS_Hash = '" + ReplaceHash + "', DBS_Size = " + ReplaceSize + ", DBS_Remark = '" + ReplaceRemark + "', DBS_UpdateUsername = '" + ReplaceUpdateUsername + "', DBS_UpdateTime = '" + ReplaceUpdateTime + "' Where DBS_Id = " + Id, ref Conn);

            Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'update' Where DBS_FileId = " + Id, ref Conn);

            AppCommon.Log(VersionId, "file-replace", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件复制
        /// </summary>
        private void Copy(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            byte[] Bytes = {};
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            int FolderId = 0;
            string FolderIdPath = "";
            int FolderUserId = 0;
            string FolderUsername = "";
            int FolderShare = 0;
            int FolderSync = 0;
            int NewId = 0;
            string NewCodeId = "";
            string NewName = "";
            string SourceFilePath = "";
            string TargetFilePath = "";
            string Sql = "";

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

            if (AppCommon.PurviewCheck(Id, false, "editor", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderPath = FileTable["DBS_FolderPath"].TypeString();
                CodeId = FileTable["DBS_CodeId"].TypeString();
                Name = FileTable["DBS_Name"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
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

                if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
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

            NewCodeId = AppCommon.CodeId();

            NewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

            SourceFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

            TargetFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderIdPath.Substring(1), NewCodeId + Extension);

            if (File.Exists(SourceFilePath) == false)
            {
                return;
            }
            else
            {
                Bytes = Base.Crypto.FileDecrypt(SourceFilePath, CodeId, true, false, true);

                if (Base.Common.IsNothing(Bytes) == true)
                {
                    return;
                }
                else
                {
                    File.WriteAllBytes(TargetFilePath, Bytes);
                }
            }

            Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
            Sql += "Select DBS_UserId, DBS_Username, 1, 0, 0, " + FolderId + ", '" + FolderIdPath + "', '" + NewCodeId + "', DBS_Hash, '" + NewName + "', DBS_Extension, DBS_Size, DBS_Type, 'null', " + FolderShare + ", 0, " + FolderSync + ", 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00' From DBS_File Where DBS_Id = " + Id;

            NewId = Base.Data.SqlInsert(Sql, ref Conn);

            if (NewId == 0)
            {
                return;
            }

            Base.Data.SqlQuery("Insert Into DBS_File_Process(DBS_FileId, DBS_Convert, DBS_Index) Values(" + NewId + ", 1, 'add')", ref Conn);

            AppCommon.FileProcessTrigger();

            AppCommon.Log(NewId, "file-copy", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件移动
        /// </summary>
        private void Move(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            int FolderId = 0;
            string FolderIdPath = "";
            string FolderNewName = "";
            int FolderUserId = 0;
            string FolderUsername = "";
            int FolderShare = 0;
            int FolderSync = 0;
            string SourceFilePath = "";
            string TargetFilePath = "";

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

            if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderPath = FileTable["DBS_FolderPath"].TypeString();
                CodeId = FileTable["DBS_CodeId"].TypeString();
                Name = FileTable["DBS_Name"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            if (FileTable["DBS_FolderId"].TypeInt() == FolderId)
            {
                return;
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

                if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
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

            FolderNewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

            SourceFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

            TargetFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderIdPath.Substring(1), CodeId + Extension);

            if (File.Exists(SourceFilePath) == false)
            {
                return;
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

            Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + FolderId + ", DBS_FolderPath = '" + FolderIdPath + "', DBS_Name = '" + FolderNewName + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Id = " + Id, ref Conn);

            Move_Version(Id, FolderPath, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync, Context);

            AppCommon.Log(Id, "file-move", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件批量移动
        /// </summary>
        private void MoveAll(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            int FolderId = 0;
            string FolderIdPath = "";
            string FolderNewName = "";
            int FolderUserId = 0;
            string FolderUsername = "";
            int FolderShare = 0;
            int FolderSync = 0;
            string SourceFilePath = "";
            string TargetFilePath = "";
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
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

            if (FolderId == 0)
            {
                FolderIdPath = "/0/";
                FolderUserId = Context.Session["UserId"].TypeInt();
                FolderUsername = Context.Session["Username"].TypeString();
            }
            else
            {
                FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);

                if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
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

            for (Index = 0; Index < Context.Request.Form.GetValues("Id").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("Id")[Index]) == true)
                {
                    Id = Context.Request.Form.GetValues("Id")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    continue;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    continue;
                }
                else
                {
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    CodeId = FileTable["DBS_CodeId"].TypeString();
                    Name = FileTable["DBS_Name"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                }

                if (FileTable["DBS_FolderId"].TypeInt() == FolderId)
                {
                    continue;
                }

                FileTable.Clear();

                FolderNewName = AppCommon.FileName(FolderId, Name, Extension, ref Conn);

                SourceFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                TargetFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderIdPath.Substring(1), CodeId + Extension);

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

                Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + FolderId + ", DBS_FolderPath = '" + FolderIdPath + "', DBS_Name = '" + FolderNewName + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Id = " + Id, ref Conn);

                Move_Version(Id, FolderPath, FolderId, FolderIdPath, FolderUserId, FolderUsername, FolderShare, FolderSync, Context);

                AppCommon.Log(Id, "file-move", ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件移动(版本)
        /// </summary>
        private void Move_Version(int Id, string FolderPath, int FolderId, string FolderIdPath, int FolderUserId, string FolderUsername, int FolderShare, int FolderSync, HttpContext Context)
        {
            List<Hashtable> FileList = new List<Hashtable>();
            int FileId = 0;
            string CodeId = "";
            string Extension = "";
            string SourceFilePath = "";
            string TargetFilePath = "";
            int Index = 0;

            Base.Data.SqlListToTable("Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_CodeId, DBS_Extension From DBS_File Where DBS_Folder = 0 And DBS_VersionId = " + Id, ref Conn, ref FileList);

            for (Index = 0; Index < FileList.Count; Index++)
            {
                FileId = FileList[Index]["DBS_Id"].TypeInt();
                CodeId = FileList[Index]["DBS_CodeId"].TypeString();
                Extension = FileList[Index]["DBS_Extension"].TypeString();

                SourceFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                TargetFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderIdPath.Substring(1), CodeId + Extension);

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

                Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + FolderUserId + ", DBS_Username = '" + FolderUsername + "', DBS_FolderId = " + FolderId + ", DBS_FolderPath = '" + FolderIdPath + "', DBS_Share = " + FolderShare + ", DBS_Sync = " + FolderSync + " Where DBS_Id = " + FileId, ref Conn);
            }
        }


        /// <summary>
        /// 文件移除
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

            if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 1, DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DBS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'delete' Where DBS_FileId = " + Id, ref Conn);

            AppCommon.Log(Id, "file-remove", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件批量移除
        /// </summary>
        private void RemoveAll(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("Id").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("Id")[Index]) == true)
                {
                    Id = Context.Request.Form.GetValues("Id")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    continue;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 1, DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "', DBS_RemoveTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'delete' Where DBS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-remove", ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件还原
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

            if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 0, DBS_RemoveUsername = 'null', DBS_RemoveTime = '1970/1/1 00:00:00' Where DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'add' Where DBS_Id = " + Id, ref Conn);

            AppCommon.Log(Id, "file-restore", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件批量还原
        /// </summary>
        private void RestoreAll(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("Id").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("Id")[Index]) == true)
                {
                    Id = Context.Request.Form.GetValues("Id")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    continue;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                FileTable.Clear();

                Base.Data.SqlQuery("Update DBS_File Set DBS_Recycle = 0, DBS_RemoveUsername = 'null', DBS_RemoveTime = '1970/1/1 00:00:00' Where DBS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Index = 'add' Where DBS_Id = " + Id, ref Conn);

                AppCommon.Log(Id, "file-restore", ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Extension = "";
            string FilePath = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
            {
                Context.Response.Write("no-permission");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderPath = FileTable["DBS_FolderPath"].TypeString();
                CodeId = FileTable["DBS_CodeId"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

            if (File.Exists(FilePath) == true)
            {
                File.Delete(FilePath);
                File.Delete("" + FilePath + ".pdf");
                File.Delete("" + FilePath + ".flv");
            }

            Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Id, ref Conn);
            Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);
            Base.Data.SqlQuery("Delete From DBS_File_Process Where DBS_FileId = " + Id, ref Conn);

            Delete_Version(Id, FolderPath, Context);

            AppCommon.Log(Id, "file-delete", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件批量删除
        /// </summary>
        private void DeleteAll(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Extension = "";
            string FilePath = "";
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("Id").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("Id")[Index]) == true)
                {
                    Id = Context.Request.Form.GetValues("Id")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                if (AppCommon.PurviewCheck(Id, false, "creator", ref Conn) == false)
                {
                    continue;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Extension, DBS_Lock From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    continue;
                }
                else
                {
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    CodeId = FileTable["DBS_CodeId"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                }

                FileTable.Clear();

                FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                    File.Delete("" + FilePath + ".pdf");
                    File.Delete("" + FilePath + ".flv");
                }

                Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DBS_File_Purview Where DBS_FileId = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DBS_File_Process Where DBS_FileId = " + Id, ref Conn);

                Delete_Version(Id, FolderPath, Context);

                AppCommon.Log(Id, "file-delete", ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 文件删除(版本)
        /// </summary>
        private void Delete_Version(int Id, string FolderPath, HttpContext Context)
        {
            List<Hashtable> FileList = new List<Hashtable>();
            int FileId = 0;
            string CodeId = "";
            string Extension = "";
            string FilePath = "";
            int Index = 0;

            Base.Data.SqlListToTable("Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_CodeId, DBS_Extension From DBS_File Where DBS_Folder = 0 And DBS_VersionId = " + Id, ref Conn, ref FileList);

            for (Index = 0; Index < FileList.Count; Index++)
            {
                FileId = FileList[Index]["DBS_Id"].TypeInt();
                CodeId = FileList[Index]["DBS_CodeId"].TypeString();
                Extension = FileList[Index]["DBS_Extension"].TypeString();

                FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == true)
                {
                    File.Delete(FilePath);
                    File.Delete("" + FilePath + ".pdf");
                    File.Delete("" + FilePath + ".flv");
                }

                Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + FileId, ref Conn);
            }
        }


        /// <summary>
        /// 判断文件名称是否存在
        /// </summary>
        private bool FileExist(int Id, int FolderId, string Name, string Extension, HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            bool Exist = false;
            string Query = "";
            string Sql = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_VersionId = 0 And " + 
                     "A1.DBS_Folder = 0 And " + 
                     "A1.DBS_FolderId = " + FolderId + " And " + 
                     "A1.DBS_Name = '" + Name + "' And " + 
                     "A1.DBS_Extension = '" + Extension + "' And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_VersionId = 0 And " + 
                     "A2.DBS_Folder = 0 And " + 
                     "A2.DBS_FolderId = " + FolderId + " And " + 
                     "A2.DBS_Name = '" + Name + "' And " + 
                     "A2.DBS_Extension = '" + Extension + "' And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File.DBS_Extension = '" + Extension + "' And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File.DBS_Extension = '" + Extension + "' And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + Name + "' And " + 
                     "DBS_File.DBS_Extension = '" + Extension + "' And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Extension From DBS_File Where " + Query + "";

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


    }


}
