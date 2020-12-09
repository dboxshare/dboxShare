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


    public class FileUpload : IHttpHandler, IReadOnlySessionState
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
        /// 文件上传
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
            int Id = 0;
            int FolderId = 0;
            string FolderPath = "";
            string CodeId = "";
            string Hash = "";
            string FileName = "";
            string FileExtension = "";
            int FileSize = 0;
            string FileType = "";
            int FolderUserId = 0;
            string FolderUsername = "";
            int FolderShare = 0;
            string TempStoragePath = "";
            string TempFilePath = "";
            string SaveStoragePath = "";
            string SaveFilePath = "";
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

                if (Base.Common.IsNumeric(Context.Request.Form["FolderId"]) == true)
                {
                    FolderId = Context.Request.Form["FolderId"].TypeInt();
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

                if (ExtensionCheck(FileExtension) == false)
                {
                    return;
                }

                TempStoragePath = Context.Server.MapPath("/storage/file/temp/");

                if (Directory.Exists(TempStoragePath) == false)
                {
                    Directory.CreateDirectory(TempStoragePath);
                }

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
                    if (FolderId == 0)
                    {
                        FolderPath = "/0/";
                        FolderUserId = Context.Session["UserId"].TypeInt();
                        FolderUsername = Context.Session["Username"].TypeString();
                    }
                    else
                    {
                        FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);

                        if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                        {
                            return;
                        }

                        Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Username, DBS_Folder, DBS_Share, DBS_Lock, DBS_Recycle From DBS_File Where DBS_Folder = 1 And DBS_Lock = 0 And DBS_Recycle = 0 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

                        if (FileTable["Exist"].TypeBool() == false)
                        {
                            return;
                        }
                        else
                        {
                            FolderUserId = FileTable["DBS_UserId"].TypeInt();
                            FolderUsername = FileTable["DBS_Username"].TypeString();
                            FolderShare = FileTable["DBS_Share"].TypeInt();
                        }

                        FileTable.Clear();
                    }

                    CodeId = AppCommon.CodeId();

                    FileName = AppCommon.FileName(FolderId, FileName, FileExtension, ref Conn);

                    FileType = AppCommon.FileType(FileExtension);

                    SaveStoragePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1));

                    SaveFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + FileExtension);

                    if (Directory.Exists(SaveStoragePath) == false)
                    {
                        Directory.CreateDirectory(SaveStoragePath);
                    }

                    if (File.Exists(TempFilePath) == false)
                    {
                        return;
                    }
                    else
                    {
                        File.Move(TempFilePath, SaveFilePath);
                    }

                    Hash = AppCommon.FileHash(SaveFilePath);

                    Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
                    Sql += "Values(" + FolderUserId + ", '" + FolderUsername + "', 1, 0, 0, " + FolderId + ", '" + FolderPath + "', '" + CodeId + "', '" + Hash + "', '" + FileName + "', '" + FileExtension + "', " + FileSize + ", '" + FileType + "', 'null', " + FolderShare + ", 0, 1, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                    Id = Base.Data.SqlInsert(Sql, ref Conn);

                    if (Id == 0)
                    {
                        return;
                    }

                    Base.Data.SqlQuery("Insert Into DBS_File_Process(DBS_FileId, DBS_Convert, DBS_Index) Values(" + Id + ", 1, 'add')", ref Conn);

                    AppCommon.FileProcessTrigger();

                    AppCommon.Log(Id, "file-upload", ref Conn);
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


        /// <summary>
        /// 文件扩展名校验
        /// </summary>
        private bool ExtensionCheck(string Extension)
        {
            string Extensions = "";
            string[] Items = {};
            int Index = 0;

            Extensions = ConfigurationManager.AppSettings["UploadExtension"].TypeString();

            if (string.IsNullOrEmpty(Extensions) == true)
            {
                return true;
            }

            Items = Extensions.Split(',');

            for (Index = 0; Index < Items.Length; Index++)
            {
                if (Items[Index] == Extension.Substring(1))
                {
                    return true;
                }
            }

            return false;
        }


    }


}
