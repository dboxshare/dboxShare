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


namespace dboxShare.Web.Sync.Controllers
{


    public class SyncFileUpversionController : ApiController
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
        /// 文件版本上传
        /// </summary>
        [Route("api/sync/file-upversion")]
        [HttpPost]
        public HttpResponseMessage FileUpversion()
        {
            if (AppCommon.LoginAuth("PC") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Guid = Context.Request.QueryString["Guid"].TypeString();

            if (Base.Common.StringCheck(Guid, @"^[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            int FileId = Context.Request.QueryString["FileId"].TypeInt();

            string FilePath = Context.Request.QueryString["FilePath"].TypeString();

            if (string.IsNullOrEmpty(FilePath) == true)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string FileName = Path.GetFileNameWithoutExtension(FilePath);

            if (Base.Common.StringCheck(FileName, @"^[^\\/\:\*\?\""\<\>\|]{1,75}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string FileExtension = Path.GetExtension(FilePath).ToString().ToLower();

            int FileSize = Context.Request.QueryString["FileSize"].TypeInt();

            if (FileSize > (long)AppConfig.UploadSize * 1024 * 1024)
            {
                return AppCommon.ResponseMessage("file length exceeds limit");
            }

            string FileHash = Context.Request.QueryString["FileHash"].TypeString();

            if (Base.Common.StringCheck(FileHash, @"^[\w]{32}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string TempFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, Guid);

            try
            {
                using (Stream Stream = Context.Request.InputStream)
                {
                    using (FileStream FStream = new FileStream(TempFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                    {
                        byte[] Buffer = new byte[(int)Stream.Length];

                        int Read = Stream.Read(Buffer, 0, (int)Stream.Length);

                        FStream.Write(Buffer, 0, Read);
                    }
                }

                if (AppCommon.FileHash(TempFilePath) == FileHash)
                {
                    try
                    {
                        Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                        Conn.Open();

                        if (AppCommon.PurviewCheck(FileId, false, "editor", ref Conn) == false)
                        {
                            return AppCommon.ResponseMessage("no operation permission");
                        }

                        Hashtable FileTable = new Hashtable();

                        Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Lock, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Recycle = 0 And DS_Id = " + FileId, ref Conn, ref FileTable);

                        if (FileTable["Exists"].TypeBool() == false)
                        {
                            return AppCommon.ResponseMessage("data does not exist or is invalid");
                        }

                        string FolderPath = FileTable["DS_FolderPath"].TypeString();
                        string Name = FileTable["DS_Name"].TypeString();
                        string Extension = FileTable["DS_Extension"].TypeString();

                        FileTable.Clear();

                        if (Extension != FileExtension)
                        {
                            return AppCommon.ResponseMessage("operation has been banned");
                        }

                        int NewVersion = AppCommon.FileVersionNumber(FileId, ref Conn);

                        string NewCodeId = AppCommon.FileCodeId();

                        string NewHash = AppCommon.FileHash(TempFilePath);

                        string SaveStoragePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1));

                        string SaveFilePath = Base.Common.PathCombine(SaveStoragePath, NewCodeId + FileExtension);

                        if (File.Exists(TempFilePath) == false)
                        {
                            return AppCommon.ResponseMessage("file does not exist");
                        }
                        else
                        {
                            File.Move(TempFilePath, SaveFilePath);
                        }

                        int VersionCount = Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_VersionId = " + FileId, ref Conn);

                        // 文件旧版本清理
                        if (VersionCount >= AppConfig.VersionCount)
                        {
                            AppCommon.FileVersionCleanup(FileId, ref Conn);
                        }

                        string Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                                     "Select DS_UserId, DS_Username, " + NewVersion + ", " + FileId + ", DS_Folder, DS_FolderId, DS_FolderPath, '" + NewCodeId + "', '" + NewHash + "', '" + Name + "', DS_Extension, " + FileSize + ", DS_Type, '', DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', DS_RemoveUsername, DS_RemoveTime From DS_File Where DS_Id = " + FileId;

                        int NewId = Base.Data.SqlInsert(Sql, ref Conn);

                        if (NewId == 0)
                        {
                            return AppCommon.ResponseMessage("data insertion failed");
                        }

                        Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + NewId + ", 1, 'null')", ref Conn);

                        AppCommon.FileProcessorTrigger();

                        AppCommon.Log(NewId, "file-upversion", ref Conn);
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


    }


}
