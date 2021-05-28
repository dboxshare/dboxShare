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


    public class Drive_ZipDataJsonController : ApiController
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Json = new ArrayList();

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
                    if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-operation-permission");
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

                SevenZipCompressor.SetLibraryPath(Context.Server.MapPath("/bin/7z64.dll"));

                try
                {
                    using (var FStream = File.Open(ExportFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var Extractor = new SevenZipExtractor(FStream))
                        {
                            for (var i = 0; i < Extractor.ArchiveFileData.Count; i++)
                            {
                                if (Context.Response.IsClientConnected == false)
                                {
                                    return AppCommon.ResponseMessage("client-disconnect");
                                }

                                var ArchivePath = Extractor.ArchiveFileData[i].FileName.ToString().Replace("\\", "/") + (Extractor.ArchiveFileData[i].IsDirectory == true ? "/" : "");
                                var ArchiveSize = Extractor.ArchiveFileData[i].Size.ToString();

                                Json.Add("{'path':'" + Base.Common.JsonEscape(ArchivePath) + "','size':'" + ArchiveSize + "'}");
                            }
                        }
                    }
                }
                finally
                {
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
            }

            return AppCommon.ResponseMessage("[" + string.Join(",", Json.ToArray()) + "]");
        }


    }


}
