using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;
using SevenZip;


namespace dboxShare.Web.Drive.Controllers
{


    public class ZipDataJsonController : ApiController
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
        /// 读取压缩文件数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/zip-data-json")]
        [HttpGet]
        public HttpResponseMessage ZipDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            ArrayList Json = new ArrayList();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

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

                if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no operation permission");
                }

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

                Conn.Close();
                Conn.Dispose();

                Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                try
                {
                    using (FileStream FStream = File.Open(ExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (SevenZipExtractor Extractor = new SevenZipExtractor(FStream))
                        {
                            for (int i = 0; i < Extractor.ArchiveFileData.Count; i++)
                            {
                                if (Context.Response.IsClientConnected == false)
                                {
                                    return AppCommon.ResponseMessage("client disconnect");
                                }

                                string ArchivePath = Extractor.ArchiveFileData[i].FileName.ToString().Replace("\\", "/") + (Extractor.ArchiveFileData[i].IsDirectory == true ? "/" : "");
                                string ArchiveSize = Extractor.ArchiveFileData[i].Size.ToString();

                                Json.Add("{'path':'" + Base.Common.JsonEscape(ArchivePath) + "','size':'" + ArchiveSize + "'}");
                            }
                        }
                    }
                }
                finally
                {
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
            }

            return AppCommon.ResponseMessage("[" + string.Join(",", Json.ToArray()) + "]");
        }


    }


}
