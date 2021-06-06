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


    public class Drive_ZipFileDownloadController : ApiController
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
        /// 文件打包下载(提取)
        /// </summary>
        [Route("api/drive/file/zip-file-download")]
        [HttpGet]
        public HttpResponseMessage ZipFileDownload()
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

                Context.Response.Cookies["Zip-File-Download-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
            }

            var PackageId = System.Guid.NewGuid().ToString();

            var PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            var PackageFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, "" + PackageId + ".zip");

            var PackageZipName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Password = Context.Request.QueryString["Password"].TypeString();

            if (string.IsNullOrEmpty(Password) == false)
            {
                if (Base.Common.StringCheck(Password, @"^[\S]{1,32}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            if (Context.Request.QueryString.GetValues("Item").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Share, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

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

                AppCommon.Log(Id, "file-download", ref Conn);

                Conn.Close();
                Conn.Dispose();

                if (AppConfig.StorageEncryption == true)
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);
                }
                else
                {
                    ExportFilePath = SourceFilePath;
                }

                if (DownloadTotalSize(ExportFilePath) > (long)AppConfig.UserDownloadSize * 1024 * 1024)
                {
                    if (AppConfig.StorageEncryption == true)
                    {
                        if (File.Exists(ExportFilePath) == true)
                        {
                            File.Delete(ExportFilePath);
                        }
                    }

                    if (Directory.Exists(PackageFolderPath) == true)
                    {
                        Directory.Delete(PackageFolderPath, true);
                    }

                    return AppCommon.ResponseMessage("download-size-limit");
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
                                Extractor.ExtractFiles(PackageFolderPath, Extractor.ArchiveFileData[Item].Index);
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
            }
            finally
            {
                if (Base.Common.IsNothing(Conn) == false)
                {
                    Conn.Close();
                    Conn.Dispose();
                }

                if (Context.Response.IsClientConnected == false)
                {
                    if (Directory.Exists(PackageFolderPath) == true)
                    {
                        Directory.Delete(PackageFolderPath, true);
                    }
                }
            }

            if (FolderIsEmpty(PackageFolderPath) == true)
            {
                if (Directory.Exists(PackageFolderPath) == true)
                {
                    Directory.Delete(PackageFolderPath, true);
                }

                return AppCommon.ResponseMessage("folder-is-empty");
            }

            try
            {
                // 打包文件(压缩)
                var Compressor = new SevenZipCompressor();

                Compressor.ArchiveFormat = OutArchiveFormat.Zip;
                Compressor.CompressionMethod = CompressionMethod.Copy;
                Compressor.CompressionLevel = CompressionLevel.None;
                Compressor.FastCompression = true;
                Compressor.CompressDirectory(PackageFolderPath, PackageFilePath);

                Compressor = null;
            }
            catch (Exception)
            {
                Context.Response.Cookies["Zip-File-Download-State"].Value = "complete";

                return AppCommon.ResponseMessage("compression-failed");
            }
            finally
            {
                if (Directory.Exists(PackageFolderPath) == true)
                {
                    Directory.Delete(PackageFolderPath, true);
                }

                if (Context.Response.IsClientConnected == false)
                {
                    if (File.Exists(PackageFilePath) == true)
                    {
                        File.Delete(PackageFilePath);
                    }
                }
            }

            Context.Response.Cookies["Zip-File-Download-State"].Value = "complete";

            // 输出文件
            try
            {
                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.AddHeader("Content-Type", "application/octet-stream");
                Context.Response.AddHeader("Content-Length", new FileInfo(PackageFilePath).Length.ToString());
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + PackageZipName + "");
                Context.Response.TransmitFile(PackageFilePath);
                Context.Response.Close();
                Context.Response.End();
            }
            catch (Exception)
            {
                Context.Response.End();
            }
            finally
            {
                if (File.Exists(PackageFilePath) == true)
                {
                    File.Delete(PackageFilePath);
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


        /// <summary>
        /// 统计下载文件总大小
        /// </summary>
        private long DownloadTotalSize(string ExportFilePath)
        {
            var TotalSize = 0L;

            using (var FStream = File.Open(ExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var Extractor = new SevenZipExtractor(FStream))
                {
                    for (var i = 0; i < Context.Request.QueryString.GetValues("Item").Length; i++)
                    {
                        var Item = Context.Request.QueryString.GetValues("Item")[i].TypeInt();

                        var ArchiveSize = Extractor.ArchiveFileData[Item].Size.TypeLong();

                        TotalSize += ArchiveSize;
                    }
                }
            }

            return TotalSize;
        }


        /// <summary>
        /// 判断文件夹是否为空
        /// </summary>
        private bool FolderIsEmpty(string Path)
        {
            if (Directory.Exists(Path) == false)
            {
                return true;
            }

            if (Directory.GetDirectories(Path).Length == 0 && Directory.GetFiles(Path).Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }


}
