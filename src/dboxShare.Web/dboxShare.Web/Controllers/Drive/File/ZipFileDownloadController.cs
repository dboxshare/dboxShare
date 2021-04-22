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


    public class ZipFileDownloadController : ApiController
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
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string State = Context.Request.QueryString["State"].TypeString();

            // 设置状态
            if (string.IsNullOrEmpty(State) == false)
            {
                if (Base.Common.StringCheck(State, @"^[\w]{1,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }

                Context.Response.Cookies["Zip-File-Download-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
            }

            string PackageId = System.Guid.NewGuid().ToString();

            string PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            string PackageFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, "" + PackageId + ".zip");

            string PackageZipName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

            int Id = Context.Request.QueryString["Id"].TypeInt();

            string Password = Context.Request.QueryString["Password"].TypeString();

            if (string.IsNullOrEmpty(Password) == false)
            {
                if (Base.Common.StringCheck(Password, @"^[\S]{1,32}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            if (Context.Request.QueryString.GetValues("Item").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no-permission");
                }

                // 保存提取文件索引
                Hashtable ExportTable = new Hashtable();

                for (int i = 0; i < Context.Request.QueryString.GetValues("Item").Length; i++)
                {
                    int Item = Context.Request.QueryString.GetValues("Item")[i].TypeInt();

                    ExportTable.Add(Item, true);
                }

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string CodeId = FileTable["DS_CodeId"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                if (Extension != ".7z" && Extension != ".rar" && Extension != ".zip")
                {
                    return AppCommon.ResponseMessage("operation has been banned");
                }

                string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage("file does not exist");
                }

                string ExportFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, System.Guid.NewGuid().ToString());

                if (Directory.Exists(AppConfig.TempStoragePath) == false)
                {
                    Directory.CreateDirectory(AppConfig.TempStoragePath);
                }

                AppCommon.Log(Id, "file-download", ref Conn);

                Conn.Close();
                Conn.Dispose();

                Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                SevenZipExtractor Extractor = null;

                try
                {
                    using (FileStream FStream = File.Open(ExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                        for (int i = 0; i < Extractor.ArchiveFileData.Count; i++)
                        {
                            if (Context.Response.IsClientConnected == false)
                            {
                                return AppCommon.ResponseMessage("client disconnect");
                            }

                            if (ExportTable[i].TypeBool() == true)
                            {
                                try
                                {
                                    Extractor.ExtractFiles(PackageFolderPath, Extractor.ArchiveFileData[i].Index);
                                }
                                catch (Exception) { }
                            }
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

                    if (File.Exists(ExportFilePath) == true)
                    {
                        File.Delete(ExportFilePath);
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

                return AppCommon.ResponseMessage("folder is empty");
            }

            try
            {
                // 打包文件(压缩)
                SevenZipCompressor Compressor = new SevenZipCompressor();

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

                return AppCommon.ResponseMessage(HttpStatusCode.OK);
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
