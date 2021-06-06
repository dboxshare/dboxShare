using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;
using SevenZip;


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_ZipFileUnpackController : ApiController
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
        /// 压缩文件解包
        /// </summary>
        [Route("api/drive/file/zip-file-unpack")]
        [HttpGet]
        public HttpResponseMessage ZipFileUnpack()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var State = Context.Request.QueryString["State"].TypeString();

            // 设置状态
            if (string.IsNullOrEmpty(State) == false)
            {
                if (Base.Common.StringCheck(State, @"^[\w]{1,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }

                Context.Response.Cookies["Zip-File-Unpack-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
            }

            var PackageId = System.Guid.NewGuid().ToString();

            var PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Password = Context.Request.QueryString["Password"].TypeString();

            if (string.IsNullOrEmpty(Password) == false)
            {
                if (Base.Common.StringCheck(Password, @"^[\S]{1,32}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var CodeId = FileTable["DS_CodeId"].TypeString();
                var Name = FileTable["DS_Name"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-permission");
                    }
                }

                if (Extension != ".7z" && Extension != ".rar" && Extension != ".zip")
                {
                    return AppCommon.ResponseMessage("operation-forbidden");
                }

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file-not-exist");
                }

                var ExportFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, System.Guid.NewGuid().ToString());

                if (Directory.Exists(AppConfig.TempStoragePath) == false)
                {
                    Directory.CreateDirectory(AppConfig.TempStoragePath);
                }

                if (AppConfig.StorageEncryption == true)
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);
                }
                else
                {
                    ExportFilePath = SourceFilePath;
                }

                var UnpackFolderPath = Base.Common.PathCombine(PackageFolderPath, Name);

                if (Directory.Exists(UnpackFolderPath) == false)
                {
                    Directory.CreateDirectory(UnpackFolderPath);
                }

                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                var Extractor = (SevenZipExtractor)null;

                try
                {
                    using (var FStream = File.Open(ExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (string.IsNullOrEmpty(Password) == true)
                        {
                            Extractor = new SevenZipExtractor(FStream);
                        }
                        else
                        {
                            Extractor = new SevenZipExtractor(FStream, Password);
                        }

                        // 提取文件(解压)
                        for (var i = 0; i < Context.Request.QueryString.GetValues("Item").Length; i++)
                        {
                            if (Context.Response.IsClientConnected == false)
                            {
                                return AppCommon.ResponseMessage("client-disconnect");
                            }

                            var Item = Context.Request.QueryString.GetValues("Item")[i].TypeInt();

                            try
                            {
                                Extractor.ExtractFiles(UnpackFolderPath, Extractor.ArchiveFileData[Item].Index);
                            }
                            catch (Exception) { }
                        }

                        // 需要解压密码或密码错误
                        if (Extractor.Check() == false)
                        {
                            if (Directory.Exists(PackageFolderPath) == true)
                            {
                                Directory.Delete(PackageFolderPath, true);
                            }

                            return AppCommon.ResponseMessage("wrong-password");
                        }
                    }
                }
                finally
                {
                    if (Base.Common.IsNothing(Extractor) == false)
                    {
                        Extractor.Dispose();
                    }

                    if (AppConfig.StorageEncryption == true)
                    {
                        if (File.Exists(ExportFilePath) == true)
                        {
                            File.Delete(ExportFilePath);
                        }
                    }
                }

                try
                {
                    ZipScan(0, PackageFolderPath, AppConfig.FileStoragePath);
                }
                finally
                {
                    if (Directory.Exists(PackageFolderPath) == true)
                    {
                        Directory.Delete(PackageFolderPath, true);
                    }
                }
            }
            catch (Exception)
            {
                return AppCommon.ResponseMessage(HttpStatusCode.InternalServerError);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }

            Context.Response.Cookies["Zip-File-Unpack-State"].Value = "complete";

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


        /// <summary>
        /// 压缩目录扫描
        /// </summary>
        private void ZipScan(int FolderId, string PackageFolderPath, string StorageFolderPath)
        {
            if (Directory.Exists(PackageFolderPath) == false)
            {
                return;
            }

            var DI = new DirectoryInfo(PackageFolderPath);

            foreach (var DirectoryItem in DI.GetDirectories())
            {
                FolderStorage(FolderId, DirectoryItem.Name, DirectoryItem.FullName, StorageFolderPath);
            }

            foreach (var FileItem in DI.GetFiles())
            {
                FileStorage(FolderId, Path.GetFileNameWithoutExtension(FileItem.Name), Path.GetExtension(FileItem.Name), FileItem.Length, FileItem.FullName, StorageFolderPath);
            }
        }


        /// <summary>
        /// 文件夹存储(入库)
        /// </summary>
        private void FolderStorage(int FolderId, string FolderName, string PackageFolderPath, string StorageFolderPath)
        {
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var Id = 0;

            Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Folder, DS_FolderId, DS_Name From DS_File Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Folder = 1 And DS_FolderId = " + FolderId + " And DS_Name = '" + FolderName + "'", ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DS_Id"].TypeInt();
            }

            FileTable.Clear();

            if (Id == 0)
            {
                var FolderIdPath = "/0/";

                if (FolderId > 0)
                {
                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                var Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                          "Values(" + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', 0, 0, 1, " + FolderId + ", '" + FolderIdPath + "', 'null', 'null', '" + FolderName + "', 'null', 0, 'folder', 'null', 0, 0, 0, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                Id = Base.Data.SqlInsert(Sql, ref Conn);

                if (Id == 0)
                {
                    return;
                }

                StorageFolderPath = Base.Common.PathCombine(StorageFolderPath, Id.ToString());

                Directory.CreateDirectory(StorageFolderPath);

                AppCommon.Log(Id, "folder-add", ref Conn);
            }
            else
            {
                StorageFolderPath = Base.Common.PathCombine(StorageFolderPath, Id.ToString());
            }

            ZipScan(Id, PackageFolderPath, StorageFolderPath);
        }


        /// <summary>
        /// 文件存储(入库)
        /// </summary>
        private void FileStorage(int FolderId, string FileName, string FileExtension, long FileSize, string FilePath, string StorageFolderPath)
        {
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var Id = 0;

            Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Folder, DS_FolderId, DS_Name, DS_Extension From DS_File Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Folder = 0 And DS_FolderId = " + FolderId + " And DS_Name = '" + FileName + "' And DS_Extension = '" + FileExtension + "'", ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DS_Id"].TypeInt();
            }

            FileTable.Clear();

            if (Id == 0)
            {
                var FolderIdPath = "/0/";

                if (FolderId > 0)
                {
                    FolderIdPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                var CodeId = AppCommon.FileCodeId();

                var Hash = AppCommon.FileHash(FilePath);

                var FileType = AppCommon.FileType(FileExtension);

                var Sql = "Insert Into DS_File(DS_UserId, DS_Username, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Sync, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime) " +
                          "Values(" + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', 1, 0, 0, " + FolderId + ", '" + FolderIdPath + "', '" + CodeId + "', '" + Hash + "', '" + FileName + "', '" + FileExtension + "', " + FileSize + ", '" + FileType + "', 'null', 0, 0, 0, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                Id = Base.Data.SqlInsert(Sql, ref Conn);

                if (Id == 0)
                {
                    return;
                }

                var StorageFilePath = Base.Common.PathCombine(StorageFolderPath, CodeId + FileExtension);

                File.Move(FilePath, StorageFilePath);

                var IsProcess = 1;

                if (AppConfig.StorageEncryption == false)
                {
                    IsProcess = AppCommon.IsConvert(FileExtension) == true ? 1 : 0;
                }

                Base.Data.SqlQuery("Insert Into DS_File_Task(DS_FileId, DS_Process, DS_Index) Values(" + Id + ", " + IsProcess + ", 'add')", ref Conn);

                AppCommon.FileProcessorTrigger();

                AppCommon.Log(Id, "file-add", ref Conn);
            }
        }


    }


}
