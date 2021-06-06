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


    public class Drive_FileViewController : ApiController
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
                return AppCommon.ResponseMessage(HttpStatusCode.Unauthorized);
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Extension = Context.Request.QueryString["Extension"].TypeString();

            if (Base.Common.StringCheck(Extension, @"^\.[\w]{1,8}$") == false)
            {
                return AppCommon.ResponseMessage(HttpStatusCode.BadRequest);
            }

            if (AppConfig.StorageEncryption == true)
            {
                if (AppCommon.IsConvert(Extension) == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.OK);
                }
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var TaskTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_FileId, DS_Process From DS_File_Task Where DS_FileId = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                var Process = TaskTable["DS_Process"].TypeInt();

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
                return AppCommon.ResponseMessage(HttpStatusCode.Unauthorized);
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var CodeId = Context.Request.QueryString["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\d]{8}-[\d]{6}-[\d]{6}-[\d]{4}-[\d]{8}$") == false)
            {
                return AppCommon.ResponseMessage(HttpStatusCode.BadRequest);
            }

            var OutputFileType = "";
            var OutputFilePath = "";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Name, DS_Extension, DS_Type From DS_File Where DS_Folder = 0 And DS_Id = " + Id + " And DS_CodeId = '" + CodeId + "'", ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage(HttpStatusCode.NotFound);
                }

                var FolderPath = FileTable["DS_FolderPath"].TypeString();
                var Name = FileTable["DS_Name"].TypeString();
                var Extension = FileTable["DS_Extension"].TypeString();
                var Type = FileTable["DS_Type"].TypeString();

                FileTable.Clear();

                var SourceFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

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

                var ExportFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, System.Guid.NewGuid().ToString());

                if (Directory.Exists(AppConfig.TempStoragePath) == false)
                {
                    Directory.CreateDirectory(AppConfig.TempStoragePath);
                }

                // 查看日志 20 分钟记录一次
                var FileViewed = Base.Data.SqlScalar("Select Count(*) From DS_Log Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Action = 'file-view' And DS_Time > '" + DateTime.Now.AddMinutes(-20).ToString() + "' And DS_FileId = " + Id, ref Conn);

                if (FileViewed == 0)
                {
                    AppCommon.Log(Id, "file-view", ref Conn);
                }

                Conn.Close();
                Conn.Dispose();

                OutputFileType = Type;

                if (AppConfig.StorageEncryption == true)
                {
                    Base.Crypto.FileDecrypt(SourceFilePath, ExportFilePath, CodeId);

                    OutputFilePath = ExportFilePath;
                }
                else
                {
                    OutputFilePath = SourceFilePath;
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
                        if (File.Exists(OutputFilePath) == true)
                        {
                            File.Delete(OutputFilePath);
                        }
                    }
                }
            }

            // 输出文件
            try
            {
                var AsyncStream = new AsyncFileStream(OutputFilePath, AppConfig.StorageEncryption);

                Action<Stream, HttpContent, TransportContext> OutputStream = AsyncStream.Output;

                var Response = new HttpResponseMessage(HttpStatusCode.OK);

                Response.Content = new PushStreamContent(OutputStream);

                Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                if (OutputFileType == "text" || OutputFileType == "code")
                {
                    Response.Content.Headers.ContentType.CharSet = GetTextEncoding(OutputFilePath).HeaderName;
                }

                return Response;
            }
            catch (Exception)
            {
                if (AppConfig.StorageEncryption == true)
                {
                    if (File.Exists(OutputFilePath) == true)
                    {
                        File.Delete(OutputFilePath);
                    }
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


        /// <summary>
        /// 获取文本编码
        /// </summary>
        private Encoding GetTextEncoding(string FilePath)
        {
            using (var FStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader BReader = new BinaryReader(FStream);

                byte[] Bytes = BReader.ReadBytes((int)FStream.Length);

                BReader.Close();

                if (Bytes.Length > 3)
                {
                    if (Bytes[0] == 0xEF && Bytes[1] == 0xBB && Bytes[2] == 0xBF)
                    {
                        return Encoding.UTF8;
                    }
                    else if (Bytes[0] == 0xFE && Bytes[1] == 0xFF && Bytes[2] == 0x00)
                    {
                        return Encoding.BigEndianUnicode;
                    }
                    else if (Bytes[0] == 0xFF && Bytes[1] == 0xFE && Bytes[2] == 0x41)
                    {
                        return Encoding.Unicode;
                    }
                    else if (IsNoBOMUTF8(Bytes) == true)
                    {
                        return Encoding.UTF8;
                    }
                    else
                    {
                        return Encoding.Default;
                    }
                }
                else
                {
                    return Encoding.Default;
                }
            }
        }


        /// <summary>
        /// 判断是否 UTF-8 编码(没有 BOM)
        /// </summary>
        private bool IsNoBOMUTF8(byte[] Bytes)
        {
            for (var i = 0; i < Bytes.Length; i++)
            {
                if ((Bytes[i] & 0xE0) == 0xC0)
                {
                    if ((Bytes[i + 1] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((Bytes[i] & 0xF0) == 0xE0)
                {
                    if ((Bytes[i + 1] & 0x80) != 0x80 || (Bytes[i + 2] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((Bytes[i] & 0xF8) == 0xF0)
                {
                    if ((Bytes[i + 1] & 0x80) != 0x80 || (Bytes[i + 2] & 0x80) != 0x80 || (Bytes[i + 3] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


    }


}
