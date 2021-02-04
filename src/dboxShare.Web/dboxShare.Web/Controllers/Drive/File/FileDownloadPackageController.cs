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


namespace dboxShare.Web.Drive.Controllers
{


    public class FileDownloadPackageController : ApiController
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
        [Route("api/drive/file/download-package")]
        [HttpGet]
        public HttpResponseMessage DownloadPackage()
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

                Context.Response.Cookies["File-Download-Package-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
            }

            if (Context.Request.QueryString.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            string PackageId = System.Guid.NewGuid().ToString();

            string PackageFolderPath = Base.Common.PathCombine(AppConfig.TempStoragePath, PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            string PackageFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, "" + PackageId + ".zip");

            string PackageZipName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (int i = 0; i < Context.Request.QueryString.GetValues("Id").Length; i++)
                {
                    if (Context.Response.IsClientConnected == false)
                    {
                        if (Directory.Exists(PackageFolderPath) == true)
                        {
                            Directory.Delete(PackageFolderPath, true);
                        }

                        return AppCommon.ResponseMessage("client disconnect");
                    }

                    int Id = Context.Request.QueryString.GetValues("Id")[i].TypeInt();

                    Hashtable FileTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Recycle From DS_File Where DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    int Folder = FileTable["DS_Folder"].TypeInt();
                    string FolderPath = FileTable["DS_FolderPath"].TypeString();
                    string CodeId = FileTable["DS_CodeId"].TypeString();
                    string Name = FileTable["DS_Name"].TypeString();
                    string Extension = FileTable["DS_Extension"].TypeString();

                    FileTable.Clear();

                    if (AppCommon.PurviewCheck(Id, Folder == 1 ? true : false, "downloader", ref Conn) == false)
                    {
                        continue;
                    }

                    if (Folder == 1)
                    {
                        // 导出文件夹
                        ExportFolder(Id, PackageFolderPath);
                    }
                    else
                    {
                        // 导出文件
                        string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                        if (File.Exists(SourceFilePath) == false)
                        {
                            continue;
                        }

                        string ExportFilePath = Base.Common.PathCombine(PackageFolderPath, Name + Extension);

                        Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                        AppCommon.Log(Id, "file-download", ref Conn);
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

                return AppCommon.ResponseMessage("folder is empty");
            }

            SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

            try
            {
                // 压缩文件夹
                SevenZipCompressor Compressor = new SevenZipCompressor();

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
                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Name, DS_Recycle From DS_File Where DS_Folder = 1 And DS_Recycle = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return;
                }

                string Name = FileTable["DS_Name"].TypeString();

                FileTable.Clear();

                ExportFolderPath = Base.Common.PathCombine(ExportFolderPath, Name);

                if (Directory.Exists(ExportFolderPath) == false)
                {
                    Directory.CreateDirectory(ExportFolderPath);
                }
            }
            catch (Exception) { }

            // 导出当前目录属下文件夹和文件
            ArrayList FileArray = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_VersionId, DS_FolderId, DS_Recycle From DS_File Where DS_VersionId = 0 And DS_Recycle = 0 And DS_FolderId = " + FolderId, ref Conn, ref FileArray);

            for (int i = 0; i < FileArray.Count; i++)
            {
                if (Context.Response.IsClientConnected == false)
                {
                    return;
                }

                int Id = FileArray[i].TypeInt();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Recycle From DS_File Where DS_Recycle = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    continue;
                }

                int Folder = FileTable["DS_Folder"].TypeInt();
                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string CodeId = FileTable["DS_CodeId"].TypeString();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                if (AppCommon.PurviewCheck(Id, Folder == 1 ? true : false, "downloader", ref Conn) == false)
                {
                    continue;
                }

                if (Folder == 1)
                // 递归导出文件夹
                {
                    ExportFolder(Id, ExportFolderPath);
                }
                else
                // 导出文件
                {
                    string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                    if (File.Exists(SourceFilePath) == false)
                    {
                        continue;
                    }

                    string ExportFilePath = Base.Common.PathCombine(ExportFolderPath, Name + Extension);

                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                    AppCommon.Log(Id, "file-download", ref Conn);
                }
            }
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

            DirectoryInfo DI = new DirectoryInfo(Path);

            foreach (FileInfo FileItem in DI.GetFiles())
            {
                if (FileItem.Length > 0)
                {
                    return false;
                }
            }

            bool IsEmpty = true;

            foreach (DirectoryInfo DirectoryItem in DI.GetDirectories())
            {
                IsEmpty = FolderIsEmpty(DirectoryItem.FullName);
            }

            return IsEmpty;
        }


    }


}
