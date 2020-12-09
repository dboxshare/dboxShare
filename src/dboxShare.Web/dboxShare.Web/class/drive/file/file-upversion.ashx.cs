using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class FileUpversion : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        private dynamic Conn;


        public void ProcessRequest(System.Web.HttpContext Context)
        {
            bool Connect = false;
            int Chunk = 0;
            int Chunks = 0;

            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Context.Response.ContentType = "text/plain";

            if (Context.Request.Form.AllKeys.Any(Mode => Mode == "chunk"))
            {
                if (Base.Common.IsNumeric(Context.Request.Form["Chunk"]) == true)
                {
                    Chunk = Context.Request.Form["Chunk"].TypeInt();
                }
                else
                {
                    return;
                }

                if (Base.Common.IsNumeric(Context.Request.Form["Chunks"]) == true)
                {
                    Chunks = Context.Request.Form["Chunks"].TypeInt();
                }
                else
                {
                    return;
                }

                if (Chunk == Chunks - 1)
                {
                    Connect = true;
                }
                else
                {
                    Connect = false;
                }
            }
            else
            {
                Connect = true;
            }

            if (Connect == true)
            {
                Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

                Conn.Open();
            }

            Upload(Context);

            if (Connect == true)
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件版本上传
        /// </summary>
        private void Upload(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            HttpPostedFile UploadFile = default(HttpPostedFile);
            FileStream FileStream = default(FileStream);
            Stream Stream = default(Stream);
            byte[] ByteBuffer = {};
            int ByteRead = 0;
            int Chunk = 0;
            int Chunks = 0;
            string Guid = "";
            int FileId = 0;
            string Remark = "";
            string FolderPath = "";
            string Name = "";
            string Extension = "";
            string FileName = "";
            string FileExtension = "";
            int FileSize = 0;
            string TempStoragePath = "";
            string TempFilePath = "";
            string SaveStoragePath = "";
            string SaveFilePath = "";
            int NewId = 0;
            int NewVersion = 0;
            string NewCodeId = "";
            string NewHash = "";
            int VersionCount = 0;
            string Sql = "";

            try
            {
                if (Base.Common.IsNumeric(Context.Request.Form["Chunk"]) == true)
                {
                    Chunk = Context.Request.Form["Chunk"].TypeInt();
                }
                else
                {
                    return;
                }

                if (Base.Common.IsNumeric(Context.Request.Form["Chunks"]) == true)
                {
                    Chunks = Context.Request.Form["Chunks"].TypeInt();
                }
                else
                {
                    return;
                }

                Guid = Context.Request.Form["Guid"].TypeString();

                if (Base.Common.StringCheck(Guid, @"^[\d\.]+$") == false)
                {
                    return;
                }

                if (Base.Common.IsNumeric(Context.Request.Form["FileId"]) == true)
                {
                    FileId = Context.Request.Form["FileId"].TypeInt();
                }
                else
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

                UploadFile = Context.Request.Files[0];

                if (Base.Common.IsNothing(UploadFile) == true || string.IsNullOrEmpty(UploadFile.FileName) == true || UploadFile.ContentLength == 0)
                {
                    return;
                }

                FileName = Path.GetFileNameWithoutExtension(UploadFile.FileName);

                FileExtension = Path.GetExtension(UploadFile.FileName).ToString().ToLower();

                if (Base.Common.IsNumeric(Context.Request.Form["Size"]) == true)
                {
                    FileSize = Context.Request.Form["Size"].TypeInt();
                }
                else
                {
                    return;
                }

                if (FileSize > ConfigurationManager.AppSettings["UploadSize"].TypeInt() * 1024 * 1024)
                {
                    return;
                }

                TempStoragePath = Context.Server.MapPath("/storage/file/temp/");

                TempFilePath = Base.Common.PathCombine(TempStoragePath, Guid);

                Stream = UploadFile.InputStream;

                FileStream = new FileStream(TempFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true);

                ByteBuffer = new byte[(int)Stream.Length];

                ByteRead = Stream.Read(ByteBuffer, 0, (int)Stream.Length);

                FileStream.Write(ByteBuffer, 0, ByteRead);

                FileStream.Close();
                FileStream.Dispose();
                Stream.Close();
                Stream.Dispose();

                if (Chunk == (Chunks == 0 ? 0 : Chunks - 1))
                {
                    if (AppCommon.PurviewCheck(FileId, false, "editor", ref Conn) == false)
                    {
                        return;
                    }

                    Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Lock, DBS_Recycle From DBS_File Where DBS_Folder = 0 And DBS_Lock = 0 And DBS_Recycle = 0 And DBS_Id = " + FileId, ref Conn, ref FileTable);

                    if (FileTable["Exist"].TypeBool() == false)
                    {
                        return;
                    }
                    else
                    {
                        FolderPath = FileTable["DBS_FolderPath"].TypeString();
                        Name = FileTable["DBS_Name"].TypeString();
                        Extension = FileTable["DBS_Extension"].TypeString();
                    }

                    FileTable.Clear();

                    if (Extension != FileExtension)
                    {
                        return;
                    }

                    NewVersion = AppCommon.FileVersionNumber(FileId, ref Conn);

                    NewCodeId = AppCommon.CodeId();

                    SaveStoragePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1));

                    SaveFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), NewCodeId + FileExtension);

                    if (File.Exists(TempFilePath) == false)
                    {
                        return;
                    }
                    else
                    {
                        File.Move(TempFilePath, SaveFilePath);
                    }

                    VersionCount = Base.Data.SqlScalar("Select Count(*) From DBS_File Where DBS_VersionId = " + FileId, ref Conn);

                    // 文件旧版本清理
                    if (VersionCount >= ConfigurationManager.AppSettings["VersionCount"].TypeInt())
                    {
                        AppCommon.FileVersionCleanup(FileId, ref Conn);
                    }

                    NewHash = AppCommon.FileHash(SaveFilePath);

                    Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
                    Sql += "Select DBS_UserId, DBS_Username, " + NewVersion + ", " + FileId + ", DBS_Folder, DBS_FolderId, DBS_FolderPath, '" + NewCodeId + "', '" + NewHash + "', '" + Name + "', DBS_Extension, " + FileSize + ", DBS_Type, '" + Remark + "', DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', DBS_RemoveUsername, DBS_RemoveTime From DBS_File Where DBS_Id = " + FileId;

                    NewId = Base.Data.SqlInsert(Sql, ref Conn);

                    if (NewId == 0)
                    {
                        return;
                    }

                    Base.Data.SqlQuery("Insert Into DBS_File_Process(DBS_FileId, DBS_Convert, DBS_Index) Values(" + NewId + ", 1, 'null')", ref Conn);

                    AppCommon.FileProcessTrigger();

                    AppCommon.Log(NewId, "file-upversion", ref Conn);
                }

                Context.Response.Write("success");
            }
            catch (Exception ex)
            {
                File.Delete(TempFilePath);

                AppCommon.Error(ex);

                Context.Response.Write(ex.Message);
            }
            finally
            {
                if (Base.Common.IsNothing(FileStream) == false)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                }

                if (Base.Common.IsNothing(Stream) == false)
                {
                    Stream.Close();
                    Stream.Dispose();
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }


}
