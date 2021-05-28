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


namespace dboxShare.Web.Sync.Controllers
{


    public class Sync_FileDownloadController : ApiController
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
        /// 文件下载
        /// </summary>
        [Route("api/sync/file-download")]
        [HttpGet]
        public HttpResponseMessage FileDownload()
        {
            if (AppCommon.LoginAuth("PC") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var CodeId = Context.Request.QueryString["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\d]{8}-[\d]{6}-[\d]{6}-[\d]{4}-[\d]{8}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var DownloadFileName = "";
            var DownloadFilePath = "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Share, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id + " And DS_CodeId = '" + CodeId + "'", ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var Name = FileTable["DS_Name"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-operation-permission");
                    }
                }

                DownloadFileName = Name + Extension;

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                var ExportFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, System.Guid.NewGuid().ToString());

                if (Directory.Exists(AppConfig.TempStoragePath) == false)
                {
                    Directory.CreateDirectory(AppConfig.TempStoragePath);
                }

                AppCommon.Log(Id, "file-download", ref Conn);

                Conn.Close();
                Conn.Dispose();

                if (AppConfig.StorageEncryption == true)
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                    DownloadFilePath = ExportFilePath;
                }
                else
                {
                    DownloadFilePath = SourceFilePath;
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
                    if (AppConfig.StorageEncryption == true)
                    {
                        if (File.Exists(DownloadFilePath) == true)
                        {
                            File.Delete(DownloadFilePath);
                        }
                    }
                }
            }

            // 输出文件
            try
            {
                var AsyncStream = new Drive.AsyncFileStream(DownloadFilePath, true);

                Action<Stream, HttpContent, TransportContext> OutputStream = AsyncStream.Output;

                var Response = new HttpResponseMessage(HttpStatusCode.OK);

                Response.Content = new PushStreamContent(OutputStream);

                Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                Response.Content.Headers.ContentLength = new FileInfo(DownloadFilePath).Length;
                Response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = DownloadFileName };

                return Response;
            }
            catch (Exception)
            {
                if (AppConfig.StorageEncryption == true)
                {
                    if (File.Exists(DownloadFilePath) == true)
                    {
                        File.Delete(DownloadFilePath);
                    }
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


    }


}
