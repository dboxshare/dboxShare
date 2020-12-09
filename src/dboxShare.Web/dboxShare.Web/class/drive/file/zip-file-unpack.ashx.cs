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


    public class ZipFileUnpack : IHttpHandler, IReadOnlySessionState
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

            ZipUnpack(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 压缩文件解包
        /// </summary>
        private void ZipUnpack(HttpContext Context)
        {
            MemoryStream MS = default(MemoryStream);
            SevenZipExtractor Extractor = default(SevenZipExtractor);
            Hashtable ExportTable = new Hashtable();
            Hashtable FileTable = new Hashtable();
            string PackageId = "";
            string PackageFolderPath = "";
            string UnpackFolderPath = "";
            string StorageFolderPath = "";
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
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

            StorageFolderPath = Context.Server.MapPath("/storage/file/");

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

                if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                {
                    Context.Response.Write("no-permission");
                    return;
                }

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

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Folder = 0 And DBS_Recycle = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    return;
                }
                else
                {
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    CodeId = FileTable["DBS_CodeId"].TypeString();
                    Name = FileTable["DBS_Name"].TypeString();
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

                UnpackFolderPath = Base.Common.PathCombine(PackageFolderPath, Name);

                if (Directory.Exists(UnpackFolderPath) == false)
                {
                    Directory.CreateDirectory(UnpackFolderPath);
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
                            Extractor.ExtractFiles(UnpackFolderPath, Extractor.ArchiveFileData[Index].Index);
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

                ZipScan(0, PackageFolderPath, StorageFolderPath, Context);
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

                if (Base.Common.IsNothing(Extractor) == false)
                {
                    Extractor = null;
                }

                Directory.Delete(PackageFolderPath, true);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// 压缩目录扫描
        /// </summary>
        private void ZipScan(int FolderId, string PackageFolderPath, string StorageFolderPath, HttpContext Context)
        {
            if (Directory.Exists(PackageFolderPath) == false)
            {
                return;
            }

            DirectoryInfo DI = new DirectoryInfo(PackageFolderPath);

            foreach (DirectoryInfo DirectoryItem in DI.GetDirectories())
            {
                FolderStorage(FolderId, DirectoryItem.Name, DirectoryItem.FullName, StorageFolderPath, Context);
            }

            foreach (FileInfo FileItem in DI.GetFiles())
            {
               FileStorage(FolderId, Path.GetFileNameWithoutExtension(FileItem.Name), Path.GetExtension(FileItem.Name), FileItem.Length, FileItem.FullName, StorageFolderPath, Context);
            }
        }


        /// <summary>
        /// 文件夹存储(入库)
        /// </summary>
        private void FolderStorage(int FolderId, string FolderName, string PackageFolderPath, string StorageFolderPath, HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string Sql = "";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Folder, DBS_FolderId, DBS_Name From DBS_File Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Folder = 1 And DBS_FolderId = " + FolderId + " And DBS_Name = '" + FolderName + "'", ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DBS_Id"].TypeInt();
            }

            FileTable.Clear();

            if (Id == 0)
            {
                if (FolderId == 0)
                {
                    FolderPath = "/0/";
                }
                else
                {
                    FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
                Sql += "Values(" + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', 0, 0, 1, " + FolderId + ", '" + FolderPath + "', 'null', 'null', '" + FolderName + "', 'null', 0, 'folder', 'null', 0, 0, 0, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

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

            ZipScan(Id, PackageFolderPath, StorageFolderPath, Context);
        }


        /// <summary>
        /// 文件存储(入库)
        /// </summary>
        private void FileStorage(int FolderId, string FileName, string FileExtension, long FileSize, string FilePath, string StorageFolderPath, HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Hash = "";
            string FileType = "";
            string StorageFilePath = "";
            string Sql = "";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Extension From DBS_File Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Folder = 0 And DBS_FolderId = " + FolderId + " And DBS_Name = '" + FileName + "' And DBS_Extension = '" + FileExtension + "'", ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DBS_Id"].TypeInt();
            }

            FileTable.Clear();

            if (Id == 0)
            {
                if (FolderId == 0)
                {
                    FolderPath = "/0/";
                }
                else
                {
                    FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);
                }

                CodeId = AppCommon.CodeId();

                Hash = AppCommon.FileHash(FilePath);

                FileType = AppCommon.FileType(FileExtension);

                Sql = "Insert Into DBS_File(DBS_UserId, DBS_Username, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_FolderPath, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Sync, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime) ";
                Sql += "Values(" + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', 1, 0, 0, " + FolderId + ", '" + FolderPath + "', '" + CodeId + "', '" + Hash + "', '" + FileName + "', '" + FileExtension + "', " + FileSize + ", '" + FileType + "', 'null', 0, 0, 0, 0, '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', '" + Context.Session["Username"].TypeString() + "', '" + DateTime.Now.ToString() + "', 'null', '1970/1/1 00:00:00')";

                Id = Base.Data.SqlInsert(Sql, ref Conn);

                if (Id == 0)
                {
                    return;
                }

                StorageFilePath = Base.Common.PathCombine(StorageFolderPath, CodeId + FileExtension);

                File.Move(FilePath, StorageFilePath);

                Base.Data.SqlQuery("Insert Into DBS_File_Process(DBS_FileId, DBS_Convert, DBS_Index) Values(" + Id + ", 1, 'add')", ref Conn);

                AppCommon.FileProcessTrigger();

                AppCommon.Log(Id, "file-add", ref Conn);
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


    }


}
