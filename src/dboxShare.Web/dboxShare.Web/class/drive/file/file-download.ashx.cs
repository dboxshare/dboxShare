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


namespace dboxShare.Web.Drive
{


    public class FileDownload : IHttpHandler, IReadOnlySessionState
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

            Download(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        private void Download(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int Version = 0;
            int VersionId = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            string FilePath = "";

            try
            {
                if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
                {
                    Id = Context.Request.QueryString["Id"].TypeInt();
                }
                else
                {
                    Context.Response.StatusCode = 500;
                    return;
                }

                CodeId = Context.Request.QueryString["CodeId"].TypeString();

                if (Base.Common.StringCheck(CodeId, @"^[\d]{8}-[\d]{6}-[\d]{7}-[\d]{3}$") == false)
                {
                    Context.Response.StatusCode = 500;
                    return;
                }

                if (AppCommon.PurviewCheck(Id, false, "downloader", ref Conn) == false)
                {
                    Context.Response.StatusCode = 500;
                    return;
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Folder = 0 And DBS_Id = " + Id + " And DBS_CodeId = '" + CodeId + "' And DBS_Recycle = 0", ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    Context.Response.StatusCode = 404;
                    return;
                }
                else
                {
                    Version = FileTable["DBS_Version"].TypeInt();
                    VersionId = FileTable["DBS_VersionId"].TypeInt();
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                    Name = FileTable["DBS_Name"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                }

                FileTable.Clear();

                if (VersionId > 0)
                {
                    Name += " v" + Version + "";
                }

                FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == false)
                {
                    Context.Response.StatusCode = 404;
                    return;
                }

                Output(CodeId, Name, Extension, FilePath, Context);

                AppCommon.Log(Id, "file-download", ref Conn);
            }
            catch (Exception ex)
            {
                AppCommon.Error(ex);

                Context.Response.StatusCode = 500;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// 文件流输出
        /// </summary>
        private void Output(string CodeId, string Name, string Extension, string FilePath, HttpContext Context)
        {
            MemoryStream MS = default(MemoryStream);
            string FileName = "";
            byte[] Bytes = {};
            byte[] Buffer = {};
            int Count = 0;

            try
            {
                if (Context.Request.UserAgent.ToString().ToLower().Contains("firefox") == true)
                {
                    FileName = Name + Extension;
                }
                else
                {
                    FileName = HttpUtility.UrlEncode(Name, Encoding.UTF8) + Extension;
                }

                Bytes = Base.Crypto.FileDecrypt(FilePath, CodeId, true, false, true);

                if (Base.Common.IsNothing(Bytes) == true)
                {
                    Bytes = File.ReadAllBytes(FilePath);
                }

                MS = new MemoryStream(Bytes);

                Buffer = new byte[1024];

                Count = MS.Read(Buffer, 0, 1024);

                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.ContentType = "application/octet-stream";

                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + "");

                while (Count > 0)
                {
                    Context.Response.OutputStream.Write(Buffer, 0, Count);

                    if (Context.Response.IsClientConnected == true)
                    {
                        Context.Response.Flush();
                    }
                    else
                    {
                        Context.Response.End();
                    }

                    Count = MS.Read(Buffer, 0, 1024);
                }

                Bytes = null;

                MS.Dispose();
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

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


    }


}
