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


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_FileDownloadController : ApiController
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
        [Route("api/drive/file/download")]
        [HttpGet]
        public HttpResponseMessage Download()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var State = Context.Request.QueryString["State"].TypeString();

            // 设置状态
            if (string.IsNullOrEmpty(State) == false)
            {
                if (Base.Common.StringCheck(State, @"^[\w]{1,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }

                Context.Response.Cookies["File-Download-State"].Value = State;

                return AppCommon.ResponseMessage("complete");
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

                Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Size, DS_Share, DS_Recycle From DS_File Where DS_Folder = 0 And DS_Recycle = 0 And DS_Id = " + Id + " And DS_CodeId = '" + CodeId + "'", ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                var Version = FileTable["DS_Version"].TypeInt();
                var VersionId = FileTable["DS_VersionId"].TypeInt();
                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var Name = FileTable["DS_Name"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Size = FileTable["DS_Size"].TypeLong();
                var Share = FileTable["DS_Share"].TypeInt();

                FileTable.Clear();

                if (Share == 1)
                {
                    if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no-operation-permission");
                    }
                }

                if (Size > (long)AppConfig.UserDownloadSize * 1024 * 1024)
                {
                    return AppCommon.ResponseMessage("download-size-limit");
                }

                if (VersionId > 0)
                {
                    Name += " v" + Version + "";
                }

                if (Context.Request.UserAgent.ToString().ToLower().Contains("firefox") == true)
                {
                    DownloadFileName = Name + Extension;
                }
                else
                {
                    DownloadFileName = HttpUtility.UrlEncode(Name, Encoding.UTF8) + Extension;
                }

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

            Context.Response.Cookies["File-Download-State"].Value = "complete";

            // 输出文件
            try
            {
                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.AddHeader("Content-Type", "application/octet-stream");
                Context.Response.AddHeader("Content-Length", new FileInfo(DownloadFilePath).Length.ToString());
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + DownloadFileName + "");
                Context.Response.TransmitFile(DownloadFilePath);
                Context.Response.Close();
                Context.Response.End();
            }
            catch (Exception)
            {
                Context.Response.End();
            }
            finally
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
