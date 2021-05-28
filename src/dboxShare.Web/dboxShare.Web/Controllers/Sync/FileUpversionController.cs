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


    public class Sync_FileUpversionController : ApiController
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Guid = Context.Request.QueryString["Guid"].TypeString();

            if (Base.Common.StringCheck(Guid, @"^[\w]{8}-[\w]{4}-[\w]{4}-[\w]{4}-[\w]{12}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var FileId = Context.Request.QueryString["FileId"].TypeInt();

            var FilePath = Context.Request.QueryString["FilePath"].TypeString();

            if (string.IsNullOrEmpty(FilePath) == true)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var FileName = Path.GetFileNameWithoutExtension(FilePath);

            if (Base.Common.StringCheck(FileName, @"^[^\\/\:\*\?\""\<\>\|]{1,75}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var FileExtension = Path.GetExtension(FilePath).ToString().ToLower();

            var FileSize = Context.Request.QueryString["FileSize"].TypeLong();

            if (FileSize > (long)AppConfig.UserUploadSize * 1024 * 1024)
            {
                return AppCommon.ResponseMessage("file-length-exceeds-limit");
            }

            var FileHash = Context.Request.QueryString["FileHash"].TypeString();

            if (Base.Common.StringCheck(FileHash, @"^[\w]{32}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var TempFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, Guid);

            try
            {
                using (var Stream = Context.Request.InputStream)
                {
                    using (var FStream = new FileStream(TempFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                    {
                        var Buffer = new byte[(int)Stream.Length];

                        var Read = Stream.Read(Buffer, 0, (int)Stream.Length);

                        FStream.Write(Buffer, 0, Read);
                    }
                }

                if (AppCommon.FileHash(TempFilePath) == FileHash)
                {
                    try
                    {
                        Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                        Conn.Open();

                        var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                        Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Lock, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Lock = 0 And DS_Recycle = 0 And DS_Id = " + FileId, ref Conn, ref FileTable);

                        if (FileTable["Exists"].TypeBool() == false)
                        {
                            return AppCommon.ResponseMessage("data-not-exist");
                        }

                        var FolderPath = FileTable["DS_FolderPath"].TypeString();
                        var Name = FileTable["DS_Name"].TypeString();
                        var Extension = FileTable["DS_Extension"].TypeString();
                        var Share = FileTable["DS_Share"].TypeInt();

                        FileTable.Clear();

                        if (Share == 1)
                        {
                            if (AppCommon.PurviewCheck(FileId, false, "editor", ref Conn) == false)
                            {
                                return AppCommon.ResponseMessage("no-operation-permission");
                            }
                        }

                        if (Extension != FileExtension)
                        {
                            return AppCommon.ResponseMessage("invalid-file-extension");
                        }

                        var NewVersion = AppCommon.FileVersionNumber(FileId, ref Conn);

                        var NewCodeId = AppCommon.FileCodeId();

                        var NewHash = AppCommon.FileHash(TempFilePath);

                        var SaveStoragePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1));

                        var SaveFilePath = Base.Common.PathCombine(SaveStoragePath, NewCodeId + FileExtension);

                        if (File.Exists(TempFilePath) == false)
                        {
                            return AppCommon.ResponseMessage("file-not-exist");
                        }
                        else
                        {
                            File.Move(TempFilePath, SaveFilePath);
                        }

                        var VersionCount = Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_VersionId = " + FileId, ref Conn);

                        // 文件旧版本清理
                        if (VersionCount >= AppConfig.VersionCount)
                        {
                            AppCommon.FileVersionCleanup(FileId, ref Conn);
                        }

                        var Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                                     "Select DS_UserId, DS_Username, " + NewVersion + ", " + FileId + ", DS_Folder, DS_FolderId, DS_FolderPath, '" + NewCodeId + "', '" + NewHash + "', '" + Name + "', DS_Extension, " + FileSize + ", DS_Type, 'null', DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', DS_RemoveUsername, DS_RemoveTime From DS_File Where DS_Id = " + FileId;

                        var NewId = Base.Data.SqlInsert(Sql, ref Conn);

                        if (NewId == 0)
                        {
                            return AppCommon.ResponseMessage("data-insertion-failed");
                        }

                        var IsProcess = 1;

                        if (AppConfig.StorageEncryption == false)
                        {
                            IsProcess = AppCommon.IsConvert(FileExtension) == true ? 1 : 0;
                        }

                        Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + NewId + ", " + IsProcess + ", 'null')", ref Conn);

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
