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


    public class FileViewController : ApiController
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
        /// 文件查看(验证是否可以预览)
        /// </summary>
        [Route("api/drive/file/view")]
        [HttpHead]
        public HttpResponseMessage View_Head()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_FileId, DS_Process From DS_File_Task Where DS_FileId = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                int Process = TaskTable["DS_Process"].TypeInt();

                TaskTable.Clear();

                if (Process == 0)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.Forbidden);
                }
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 文件查看(输出文件流)
        /// </summary>
        [Route("api/drive/file/view")]
        [HttpGet]
        public HttpResponseMessage View_Get()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            string CodeId = Context.Request.QueryString["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\d]{8}-[\d]{6}-[\d]{6}-[\d]{4}-[\d]{8}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string OutputFilePath = "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension From DS_File Where DS_Folder = 0 And DS_Id = " + Id + " And DS_CodeId = '" + CodeId + "'", ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                string FolderPath = FileTable["DS_FolderPath"].TypeString();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                string SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(SourceFilePath) == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                if (Extension != ".pdf")
                {
                    if (File.Exists("" + SourceFilePath + ".pdf") == true)
                    {
                        SourceFilePath += ".pdf";
                    }
                }

                if (Extension != ".flv")
                {
                    if (File.Exists("" + SourceFilePath + ".flv") == true)
                    {
                        SourceFilePath += ".flv";
                    }
                }

                string ExportFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, System.Guid.NewGuid().ToString());

                if (Directory.Exists(AppConfig.TempStoragePath) == false)
                {
                    Directory.CreateDirectory(AppConfig.TempStoragePath);
                }

                OutputFilePath = ExportFilePath;

                int FileViewed = Base.Data.SqlScalar("Select Count(*) From DS_Log Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And DS_FileId = " + Id, ref Conn);

                if (FileViewed == 0)
                {
                    AppCommon.Log(Id, "file-view", ref Conn);
                }

                Conn.Close();
                Conn.Dispose();

                Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);
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
                    if (File.Exists(OutputFilePath) == true)
                    {
                        File.Delete(OutputFilePath);
                    }
                }
            }

            // 输出文件
            try
            {
                var AsyncStream = new AsyncFileStream(OutputFilePath, true);

                Action<Stream, HttpContent, TransportContext> OutputStream = AsyncStream.Output;

                HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.OK);

                Response.Content = new PushStreamContent(OutputStream);

                Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return Response;
            }
            catch (Exception)
            {
                if (File.Exists(OutputFilePath) == true)
                {
                    File.Delete(OutputFilePath);
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


    }


}
