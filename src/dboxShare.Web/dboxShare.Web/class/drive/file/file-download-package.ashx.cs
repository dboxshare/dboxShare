using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;
using SevenZip;


namespace dboxShare.Web.Drive
{


    public class FileDownloadPackage : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                Context.Response.StatusCode = 500;
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            DownloadPackage(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 文件打包下载
        /// </summary>
        private void DownloadPackage(HttpContext Context)
        {
            SevenZipCompressor Compressor = default(SevenZipCompressor);
            Hashtable FileTable = new Hashtable();
            string PackageId = "";
            string PackageFolderPath = "";
            string PackageFilePath = "";
            int Id = 0;
            int Folder = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            string StoragePath = "";
            string FilePath = "";
            int Index = 0;

            PackageId = Guid.NewGuid().ToString();

            PackageFolderPath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/temp/"), PackageId);

            if (Directory.Exists(PackageFolderPath) == false)
            {
                Directory.CreateDirectory(PackageFolderPath);
            }

            PackageFilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/temp/"), "" + PackageId + ".zip");

            try
            {
                if (Context.Request.QueryString.GetValues("Id").Length == 0)
                {
                    return;
                }

                for (Index = 0; Index < Context.Request.QueryString.GetValues("Id").Length; Index++)
                {
                    if (Context.Response.IsClientConnected == false)
                    {
                        return;
                    }

                    if (Base.Common.IsNumeric(Context.Request.QueryString.GetValues("Id")[Index]) == true)
                    {
                        Id = Context.Request.QueryString.GetValues("Id")[Index].TypeInt();
                    }
                    else
                    {
                        continue;
                    }

                    Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Recycle = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                    if (FileTable["Exist"].TypeBool() == false)
                    {
                        continue;
                    }
                    else
                    {
                        Folder = FileTable["DBS_Folder"].TypeInt();
                        FolderPath = FileTable["DBS_FolderPath"].TypeString();
                        CodeId = FileTable["DBS_CodeId"].TypeString();
                        Name = FileTable["DBS_Name"].TypeString();
                        Extension = FileTable["DBS_Extension"].TypeString();
                    }

                    FileTable.Clear();

                    if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                    {
                        continue;
                    }

                    StoragePath = Context.Server.MapPath("/storage/file/");

                    if (Folder == 1)
                    // 导出文件夹
                    {
                        ExportFolder(Id, StoragePath, PackageFolderPath, Context);
                    }
                    else
                    // 导出文件
                    {
                        FilePath = Base.Common.PathCombine(StoragePath, FolderPath.Substring(1), CodeId + Extension);

                        if (File.Exists(FilePath) == false)
                        {
                            continue;
                        }

                        File.WriteAllBytes(Base.Common.PathCombine(PackageFolderPath, Name + Extension), ReadFileBytes(FilePath, CodeId));

                        AppCommon.Log(Id, "file-download", ref Conn);
                    }
                }

                // 压缩文件夹
                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                Compressor = new SevenZipCompressor();

                Compressor.ArchiveFormat = OutArchiveFormat.Zip;
                Compressor.CompressionLevel = CompressionLevel.High;
                Compressor.CompressDirectory(PackageFolderPath, PackageFilePath);

                Compressor = null;

                // 输出zip文件
                OutputZip(PackageFilePath, Context);
            }
            catch (Exception ex)
            {
                AppCommon.Error(ex);

                Context.Response.StatusCode = 500;
            }
            finally
            {
                if (Base.Common.IsNothing(Compressor) == false)
                {
                    Compressor = null;
                }

                Directory.Delete(PackageFolderPath, true);

                File.Delete(PackageFilePath);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// 导出文件夹
        /// </summary>
        private void ExportFolder(int FolderId, string StoragePath, string ExportFolderPath, HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            ArrayList FileArray = new ArrayList();
            int Id = 0;
            int Folder = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            string FilePath = "";
            int Index = 0;

            // 生成文件夹
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Name, DBS_Recycle From DBS_File Where DBS_Folder = 1 And DBS_Recycle = 0 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Name = FileTable["DBS_Name"].TypeString();
            }

            FileTable.Clear();

            ExportFolderPath = Base.Common.PathCombine(ExportFolderPath, Name);

            if (Directory.Exists(ExportFolderPath) == false)
            {
                Directory.CreateDirectory(ExportFolderPath);
            }

            // 导出当前目录属下文件夹和文件
            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_VersionId, DBS_FolderId, DBS_Recycle From DBS_File Where DBS_VersionId = 0 And DBS_Recycle = 0 And DBS_FolderId = " + FolderId, ref Conn, ref FileArray);

            for (Index = 0; Index < FileArray.Count; Index++)
            {
                if (Context.Response.IsClientConnected == false)
                {
                    return;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Recycle = 0 And DBS_Id = " + FileArray[Index], ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    continue;
                }
                else
                {
                    Folder = FileTable["DBS_Folder"].TypeInt();
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    CodeId = FileTable["DBS_CodeId"].TypeString();
                    Name = FileTable["DBS_Name"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                }

                FileTable.Clear();

                if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                {
                    continue;
                }

                if (Folder == 1)
                // 递归导出文件夹
                {
                    ExportFolder(FileArray[Index].TypeInt(), StoragePath, ExportFolderPath, Context);
                }
                else
                // 导出文件
                {
                    FilePath = Base.Common.PathCombine(StoragePath, FolderPath.Substring(1), CodeId + Extension);

                    if (File.Exists(FilePath) == false)
                    {
                        continue;
                    }

                    File.WriteAllBytes(Base.Common.PathCombine(ExportFolderPath, Name + Extension), ReadFileBytes(FilePath, CodeId));

                    AppCommon.Log(Id, "file-download", ref Conn);
                }
            }
            
        }


        /// <summary>
        /// 读取文件字节流
        /// </summary>
        private byte[] ReadFileBytes(string FilePath, string Key)
        {
            byte[] Bytes = {};

            Bytes = Base.Crypto.FileDecrypt(FilePath, Key, true, false, true);

            if (Base.Common.IsNothing(Bytes) == true)
            {
                Bytes = File.ReadAllBytes(FilePath);
            }

            return Bytes;
        }


        /// <summary>
        /// 打包文件流输出
        /// </summary>
        private void OutputZip(string PackageFilePath, HttpContext Context)
        {
            string FileName = "";

            try
            {
                FileName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";

                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.ContentType = "application/octet-stream";
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + "");
                Context.Response.TransmitFile(PackageFilePath);
                Context.Response.Flush();
            }
            catch (Exception ex)
            {
                AppCommon.Error(ex);

                Context.Response.StatusCode = 500;
            }
            finally
            {
                File.Delete(PackageFilePath);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }


}
