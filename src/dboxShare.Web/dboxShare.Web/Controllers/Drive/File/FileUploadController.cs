using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class FileUploadController : ApiController
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
        /// 文件上传
        /// </summary>
        [Route("api/drive/file/upload")]
        [HttpPost]
        public HttpResponseMessage Upload()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Chunk = Context.Request.Form["Chunk"].TypeInt();

            int Chunks = Context.Request.Form["Chunks"].TypeInt();

            string Guid = Context.Request.Form["Guid"].TypeString();

            if (Base.Common.StringCheck(Guid, @"^[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            int FolderId = Context.Request.Form["FolderId"].TypeInt();

            HttpPostedFile UploadFile = Context.Request.Files[0];

            if (Base.Common.IsNothing(UploadFile) == true || string.IsNullOrEmpty(UploadFile.FileName) == true || UploadFile.ContentLength == 0)
            {
                return AppCommon.ResponseMessage("invalid file");
            }

            string FileName = Path.GetFileNameWithoutExtension(UploadFile.FileName);

            string FileExtension = Path.GetExtension(UploadFile.FileName).ToString().ToLower();

            int FileSize = Context.Request.Form["Size"].TypeInt();

            if (FileSize > (long)AppConfig.UploadSize * 1024 * 1024)
            {
                return AppCommon.ResponseMessage("file length exceeds limit");
            }

            if (ExtensionCheck(FileExtension) == false)
            {
                return AppCommon.ResponseMessage("this file extension is not supported");
            }

            if (Directory.Exists(AppConfig.TempStoragePath) == false)
            {
                Directory.CreateDirectory(AppConfig.TempStoragePath);
            }

            string TempFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, Guid);

            try
            {
                using (Stream Stream = UploadFile.InputStream)
                {
                    using (FileStream FStream = new FileStream(TempFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                    {
                        byte[] Buffer = new byte[(int)Stream.Length];

                        int Read = Stream.Read(Buffer, 0, (int)Stream.Length);

                        FStream.Write(Buffer, 0, Read);
                    }
                }

                if (Chunk == (Chunks == 0 ? 0 : Chunks - 1))
                {
                    try
                    {
                        Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                        Conn.Open();

                        int FolderUserId = Context.Session["UserId"].TypeInt();
                        string FolderUsername = Context.Session["Username"].TypeString();
                        string FolderIdPath = "/0/";
                        int FolderShare = 0;

                        if (FolderId > 0)
                        {
                            if (AppCommon.PurviewCheck(FolderId, true, "uploader", ref Conn) == false)
                            {
                                return AppCommon.ResponseMessage("no operation permission");
                            }

                            Hashtable FileTable = new Hashtable();

                            Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Username, DS_Folder, DS_Share, DS_Lock, DS_Recycle From DS_File Where DS_Folder = 1 And DS_Lock = 0 And DS_Recycle = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                            if (FileTable["Exists"].TypeBool() == false)
                            {
                                return AppCommon.ResponseMessage("data does not exist or is invalid");
                            }
                            else
                            {
                                FolderUserId = FileTable["DS_UserId"].TypeInt();
                                FolderUsername = FileTable["DS_Username"].TypeString();
                                FolderShare = FileTable["DS_Share"].TypeInt();
                            }

                            FileTable.Clear();

                            FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                        }

                        string CodeId = AppCommon.FileCodeId();

                        string Hash = AppCommon.FileHash(TempFilePath);

                        string FileType = AppCommon.FileType(FileExtension);

                        string SaveStoragePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderIdPath.Substring(1));

                        string SaveFilePath = Base.Common.PathCombine(SaveStoragePath, CodeId + FileExtension);

                        if (Directory.Exists(SaveStoragePath) == false)
                        {
                            Directory.CreateDirectory(SaveStoragePath);
                        }

                        if (File.Exists(TempFilePath) == false)
                        {
                            return AppCommon.ResponseMessage("file does not exist");
                        }
                        else
                        {
                            File.Move(TempFilePath, SaveFilePath);
                        }

                        FileName = AppCommon.FileName(FolderId, FileName, FileExtension, ref Conn);

                        string Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                                     "Values(" + FolderUserId + ", '" + FolderUsername + "', 1, 0, 0, " + FolderId + ", '" + FolderIdPath + "', '" + CodeId + "', '" + Hash + "', '" + FileName + "', '" + FileExtension + "', " + FileSize + ", '" + FileType + "', 'null', " + FolderShare + ", 0, 1, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                        int Id = Base.Data.SqlInsert(Sql, ref Conn);

                        if (Id == 0)
                        {
                            return AppCommon.ResponseMessage("data insertion failed");
                        }

                        Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + Id + ", 1, 'add')", ref Conn);

                        AppCommon.FileProcessorTrigger();

                        AppCommon.Log(Id, "file-upload", ref Conn);

                        return AppCommon.ResponseMessage("success");
                    }
                    finally
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                if (File.Exists(TempFilePath) == true)
                {
                    File.Delete(TempFilePath);
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


        /// <summary>
        /// 文件扩展名校验
        /// </summary>
        private bool ExtensionCheck(string Extension)
        {
            if (string.IsNullOrEmpty(AppConfig.UploadExtension) == true)
            {
                return true;
            }

            string[] Items = AppConfig.UploadExtension.Split(',');

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == Extension.Substring(1))
                {
                    return true;
                }
            }

            return false;
        }


    }


}
