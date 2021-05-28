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


namespace dboxShare.Web.Share.Controllers
{


    public class Share_FileDownloadPackageController : ApiController
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
        /// 文件打包下载
        /// </summary>
        [Route("api/link/file-download-package")]
        [HttpGet]
        public HttpResponseMessage DownloadPackage()
        {
            var LinkId = Context.Session["LinkId"].TypeInt();

            if (LinkId == 0)
            {
                return AppCommon.ResponseMessage("link-login-failed");
            }

            var State = Context.Request.QueryString["State"].TypeString();

            // 设置状态
            if (string.IsNullOrEmpty(State) == false)
            {
                if (Base.Common.StringCheck(State, @"^[\w]{1,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }

                Context.Response.Cookies["File-Download-Package-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
            }

            if (Context.Request.QueryString.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            var PackageId = System.Guid.NewGuid().ToString();

            var PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            var PackageFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, "" + PackageId + ".zip");

            var PackageZipName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var ShareLink = Base.Data.SqlScalar("Select Count(*) From DS_Link Where DS_Deadline > '" + DateTime.Now.ToString() + "' And DS_Revoke = 0 And DS_Id = " + LinkId, ref Conn);

                // 判断分享链接合法性
                if (ShareLink == 0)
                {
                    return AppCommon.ResponseMessage("invalid-shared-link");
                }

                // 判断打包文件总大小是否超过限制
                if (DownloadTotalSize() > (long)AppConfig.UserDownloadSize * 1024 * 1024)
                {
                    if (Directory.Exists(PackageFolderPath) == true)
                    {
                        Directory.Delete(PackageFolderPath, true);
                    }

                    return AppCommon.ResponseMessage("download-size-limit");
                }

                for (var i = 0; i < Context.Request.QueryString.GetValues("Id").Length; i++)
                {
                    if (Context.Response.IsClientConnected == false)
                    {
                        if (Directory.Exists(PackageFolderPath) == true)
                        {
                            Directory.Delete(PackageFolderPath, true);
                        }

                        return AppCommon.ResponseMessage("client-disconnect");
                    }

                    var Id = Context.Request.QueryString.GetValues("Id")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Recycle From DS_File Where DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var Folder = FileTable["DS_Folder"].TypeInt();
                    var FolderId = FileTable["DS_FolderId"].TypeInt();
                    var FolderPath = FileTable["DS_FolderPath"].TypeString();
                    var CodeId = FileTable["DS_CodeId"].TypeString();
                    var Name = FileTable["DS_Name"].TypeString();
                    var Extension = FileTable["DS_Extension"].TypeString();

                    FileTable.Clear();

                    var ShareFile = Base.Data.SqlScalar("Select Count(*) From DS_Link_File Where DS_LinkId = " + LinkId + " And DS_FileId = " + Id, ref Conn);

                    // 判断分享文件合法性
                    if (ShareFile == 0)
                    {
                        if (FolderId == 0)
                        {
                            continue;
                        }
                        else
                        {
                            ShareFile = Base.Data.SqlScalar("Select Count(*) From DS_Link_File Where DS_LinkId = " + LinkId + " And DS_FileId = " + FolderId, ref Conn);

                            if (ShareFile == 0)
                            {
                                continue;
                            }
                        }
                    }

                    if (Folder == 1)
                    {
                        // 导出文件夹
                        ExportFolder(Id, PackageFolderPath);
                    }
                    else
                    {
                        // 导出文件
                        var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                        if (File.Exists(SourceFilePath) == false)
                        {
                            continue;
                        }

                        var ExportFilePath = Base.Common.PathCombine(PackageFolderPath, Name + Extension);

                        if (AppConfig.StorageEncryption == true)
                        {
                            Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);
                        }
                        else
                        {
                            File.Copy(SourceFilePath, ExportFilePath);
                        }
                    }
                }
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }

            if (FolderIsEmpty(PackageFolderPath) == true)
            {
                if (Directory.Exists(PackageFolderPath) == true)
                {
                    Directory.Delete(PackageFolderPath, true);
                }

                return AppCommon.ResponseMessage("folder-is-empty");
            }

            SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

            try
            {
                // 压缩文件夹
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
                Context.Response.Cookies["File-Download-Package-State"].Value = "complete";

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

            Context.Response.Cookies["File-Download-Package-State"].Value = "complete";

            // 输出 Zip 文件
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
        /// 导出文件夹
        /// </summary>
        private void ExportFolder(int FolderId, string ExportFolderPath)
        {
            try
            {
                // 生成文件夹
                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Name, DS_Recycle From DS_File Where DS_Folder = 1 And DS_Recycle = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return;
                }

                var Name = FileTable["DS_Name"].TypeString();

                FileTable.Clear();

                ExportFolderPath = Base.Common.PathCombine(ExportFolderPath, Name);

                if (Directory.Exists(ExportFolderPath) == false)
                {
                    Directory.CreateDirectory(ExportFolderPath);
                }
            }
            catch (Exception) { }

            // 导出当前目录属下文件夹和文件
            var FileArray = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_VersionId, DS_FolderId, DS_Recycle From DS_File Where DS_VersionId = 0 And DS_Recycle = 0 And DS_FolderId = " + FolderId, ref Conn, ref FileArray);

            for (var i = 0; i < FileArray.Count; i++)
            {
                if (Context.Response.IsClientConnected == false)
                {
                    return;
                }

                var Id = FileArray[i].TypeInt();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    continue;
                }

                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var CodeId = FileTable["DS_CodeId"].TypeString();
                var Name = FileTable["DS_Name"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                // 导出文件
                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    continue;
                }

                var ExportFilePath = Base.Common.PathCombine(ExportFolderPath, Name + Extension);

                if (AppConfig.StorageEncryption == true)
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);
                }
                else
                {
                    File.Copy(SourceFilePath, ExportFilePath);
                }
            }
        }


        /// <summary>
        /// 统计下载文件总大小
        /// </summary>
        private long DownloadTotalSize()
        {
            var TotalSize = 0L;

            for (var i = 0; i < Context.Request.QueryString.GetValues("Id").Length; i++)
            {
                var Id = Context.Request.QueryString.GetValues("Id")[i].TypeInt();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Size, DS_Recycle From DS_File Where DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    continue;
                }

                var Folder = FileTable["DS_Folder"].TypeInt();
                var Size = FileTable["DS_Size"].TypeLong();

                FileTable.Clear();

                if (Folder == 1)
                {
                    var FolderIdPath = AppCommon.FolderIdPath(Id, ref Conn);

                    TotalSize += Base.Data.SqlScalar("Select Sum(DS_Size) As Total From DS_File Where DS_Version = 0 And DS_FolderPath Like '" + FolderIdPath + "%'", ref Conn);
                }
                else
                {
                    TotalSize += Size;
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
