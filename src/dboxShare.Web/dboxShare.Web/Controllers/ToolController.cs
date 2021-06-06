using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;
using SevenZip;


namespace dboxShare.Web.Controllers
{


    public class ToolController : ApiController
    {


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 同步工具打包下载
        /// </summary>
        [Route("api/tool/dboxsyncer-download")]
        [HttpGet]
        public HttpResponseMessage DownloadPackage()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var AppLanguage = Context.Request.QueryString["AppLanguage"].TypeString();

            if (Base.Common.StringCheck(AppLanguage, @"^[\w\-]+$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var ServerUrl = "" + Context.Request.Url.Scheme + "://" + Context.Request.Url.Host + "";

            var LoginId = Context.Session["Username"].TypeString();

            var dboxSyncerFolderPath = Base.Common.PathCombine(AppConfig.StoragePath, "download", "dboxsyncer");

            if (Directory.Exists(dboxSyncerFolderPath) == false)
            {
                return AppCommon.ResponseMessage("dboxsyncer-folder-not-exist");
            }

            var PackageId = System.Guid.NewGuid().ToString();

            var PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            var PackageFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, "" + PackageId + ".zip");

            SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

            try
            {
                // 复制文件夹
                CopyFolder(dboxSyncerFolderPath, PackageFolderPath);

                // 修改配置文件
                var ConfigPath = Base.Common.PathCombine(PackageFolderPath, "dboxSyncer.exe.config");
                var ConfigContent = File.ReadAllText(ConfigPath);

                ConfigContent = ConfigContent.Replace("{@applanguage}", AppLanguage);
                ConfigContent = ConfigContent.Replace("{@serverurl}", ServerUrl);
                ConfigContent = ConfigContent.Replace("{@loginid}", LoginId);

                File.WriteAllText(ConfigPath, ConfigContent, Encoding.UTF8);

                // 压缩文件夹
                var Compressor = new SevenZipCompressor();

                Compressor.ArchiveFormat = OutArchiveFormat.Zip;
                Compressor.CompressionMethod = CompressionMethod.Copy;
                Compressor.CompressionLevel = CompressionLevel.None;
                Compressor.FastCompression = true;
                Compressor.CompressDirectory(PackageFolderPath, PackageFilePath);

                Compressor = null;
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

            // 输出 Zip 文件
            try
            {
                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.AddHeader("Content-Type", "application/octet-stream");
                Context.Response.AddHeader("Content-Length", new FileInfo(PackageFilePath).Length.ToString());
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=dboxSyncer.zip");
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
        /// 复制文件夹
        /// </summary>
        private void CopyFolder(string SourceFolderPath, string TargetFolderPath)
        {
            if (Directory.Exists(TargetFolderPath) == false)
            {
                Directory.CreateDirectory(TargetFolderPath);
            }

            var Folders = Directory.GetDirectories(SourceFolderPath);

            foreach (var FolderPath in Folders)
            {
                CopyFolder(FolderPath, Base.Common.PathCombine(TargetFolderPath, Path.GetFileName(FolderPath)));
            }

            var Files = Directory.GetFiles(SourceFolderPath);

            foreach (var FilePath in Files)
            {
                File.Copy(FilePath, Base.Common.PathCombine(TargetFolderPath, Path.GetFileName(FilePath)));
            }
        }


    }


}
