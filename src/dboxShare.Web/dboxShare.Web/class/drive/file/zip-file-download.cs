using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;
using SevenZip;


namespace dboxShare.Web.Drive
{


    public class ZipFileDownload : IHttpHandler, IReadOnlySessionState
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
        /// 文件打包下载(提取)
        /// </summary>
        private void DownloadPackage(HttpContext Context)
        {
            MemoryStream MS = default(MemoryStream);
            SevenZipCompressor Compressor = default(SevenZipCompressor);
            SevenZipExtractor Extractor = default(SevenZipExtractor);
            Hashtable ExportTable = new Hashtable();
            Hashtable FileTable = new Hashtable();
            string PackageId = "";
            string PackageFolderPath = "";
            string PackageFilePath = "";
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Extension = "";
            string FilePath = "";
            string Password = "";
            byte[] Bytes = {};
            int Item = 0;
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
                if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
                {
                    Id = Context.Request.QueryString["Id"].TypeInt();
                }
                else
                {
                    return;
                }

                Password = Context.Request.QueryString["Password"].TypeString();

                if (string.IsNullOrEmpty(Password) == false)
                {
                    if (Base.Common.StringCheck(Password, @"^[\S]{1,32}$") == false)
                    {
                        return;
                    }
                }

                if (Context.Request.QueryString.GetValues("Item").Length == 0)
                {
                    return;
                }

                if (AppCommon.PurviewCheck(Id, false, "manager", ref Conn) == false)
                {
                    Context.Response.Write("no-permission");
                    return;
                }

                // 保存提取文件索引
                for (Index = 0; Index < Context.Request.QueryString.GetValues("Item").Length; Index++)
                {
                    if (Base.Common.IsNumeric(Context.Request.QueryString.GetValues("Item")[Index]) == true)
                    {
                        Item = Context.Request.QueryString.GetValues("Item")[Index].TypeInt();
                    }
                    else
                    {
                        continue;
                    }

                    ExportTable.Add(Item, true);
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Folder = 0 And DBS_Recycle = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    return;
                }
                else
                {
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    CodeId = FileTable["DBS_CodeId"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                }

                FileTable.Clear();

                if (Extension != ".7z" && Extension != ".rar" && Extension != ".zip")
                {
                    return;
                }

                FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == false)
                {
                    return;
                }

                Bytes = ReadFileBytes(FilePath, CodeId);

                if (Bytes.Length == 0)
                {
                    return;
                }

                MS = new MemoryStream(Bytes);

                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                if (string.IsNullOrEmpty(Password) == true)
                {
                    Extractor = new SevenZipExtractor(MS);
                }
                else
                {
                    Extractor = new SevenZipExtractor(MS, Password);
                }

                // 提取文件(解压)
                for (Index = 0; Index < Extractor.ArchiveFileData.Count; Index++)
                {
                    if (Context.Response.IsClientConnected == false)
                    {
                        return;
                    }

                    if (ExportTable[Index].TypeBool() == true)
                    {
                        try
                        {
                            Extractor.ExtractFiles(PackageFolderPath, Extractor.ArchiveFileData[Index].Index);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                if (Extractor.Check() == false)
                {
                    Context.Response.Write("wrong-password");
                    return;
                }

                Extractor = null;

                MS.Dispose();

                Compressor = new SevenZipCompressor();

                Compressor.ArchiveFormat = OutArchiveFormat.Zip;
                Compressor.CompressionLevel = CompressionLevel.High;
                Compressor.CompressDirectory(PackageFolderPath, PackageFilePath);

                Compressor = null;

                AppCommon.Log(Id, "file-download", ref Conn);

                OutputZip(PackageFilePath, Context);
            }
            catch (Exception ex)
            {
                AppCommon.Error(ex);

                Context.Response.StatusCode = 500;
            }
            finally
            {
                if (Base.Common.IsNothing(MS) == false)
                {
                    MS.Dispose();
                }

                if (Base.Common.IsNothing(Compressor) == false)
                {
                    Compressor = null;
                }

                if (Base.Common.IsNothing(Extractor) == false)
                {
                    Extractor = null;
                }

                Directory.Delete(PackageFolderPath, true);

                File.Delete(PackageFilePath);

                GC.Collect();
                GC.WaitForPendingFinalizers();
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
