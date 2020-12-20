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


    public class ZipDataJson : IHttpHandler, IReadOnlySessionState
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
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            DataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取压缩文件数据返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();
            MemoryStream MS = default(MemoryStream);
            dynamic Zip = default(dynamic);
            string ArchivePath = "";
            string ArchiveSize = "";
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Extension = "";
            string FilePath = "";
            byte[] Bytes = {};
            int Index = 0;

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

                if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
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

                Zip = new SevenZipExtractor(MS);

                for (Index = 0; Index < Zip.ArchiveFileData.Count; Index++)
                {
                    if (Context.Response.IsClientConnected == false)
                    {
                        return;
                    }

                    ArchivePath = Zip.ArchiveFileData[Index].FileName.ToString().Replace("\\", "/") + (Zip.ArchiveFileData[Index].IsDirectory == true ? "/" : "");
                    ArchiveSize = Zip.ArchiveFileData[Index].Size.ToString();

                    JsonList.Add("{'path':'" + Base.Common.JsonEscape(ArchivePath) + "','size':'" + ArchiveSize + "'}");
                }

                Zip = null;

                MS.Dispose();

                Context.Response.Write("[" + string.Join(",", JsonList.ToArray()) + "]");
            }
            catch (Exception ex)
            {
                AppCommon.Error(ex);
            }
            finally
            {
                if (Base.Common.IsNothing(MS) == false)
                {
                    MS.Dispose();
                }

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


    }


}
