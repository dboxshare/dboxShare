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


    public class FileView : IHttpHandler, IReadOnlySessionState
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

            View(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 文件查看
        /// </summary>
        private void View(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            Hashtable TaskTable = new Hashtable();
            string Method = "";
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Name = "";
            string Extension = "";
            string FilePath = "";

            try
            {
                Method = Context.Request.RequestType;

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

                if (Method == "HEAD")
                {
                    Base.Data.SqlDataToTable("Select DBS_FileId, DBS_Convert From DBS_File_Process Where DBS_FileId = " + Id + " And DBS_Convert = 1", ref Conn, ref TaskTable);

                    if (TaskTable["Exist"].TypeBool() == true)
                    {
                        Context.Response.StatusCode = 403;
                        return;
                    }

                    TaskTable.Clear();
                }

                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Name, DBS_Extension From DBS_File Where DBS_Folder = 0 And DBS_Id = " + Id + " And DBS_CodeId = '" + CodeId + "'", ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    Context.Response.StatusCode = 404;
                    return;
                }
                else
                {
                    Name = FileTable["DBS_Name"].TypeString();
                    Extension = FileTable["DBS_Extension"].TypeString();
                    FolderPath = FileTable["DBS_FolderPath"].TypeString();
                }

                FileTable.Clear();

                FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

                if (File.Exists(FilePath) == false)
                {
                    Context.Response.StatusCode = 404;
                    return;
                }

                if (Extension != ".pdf")
                {
                    if (File.Exists("" + FilePath + ".pdf") == true)
                    {
                        FilePath = "" + FilePath + ".pdf";
                    }
                }

                if (Extension != ".flv")
                {
                    if (File.Exists("" + FilePath + ".flv") == true)
                    {
                        FilePath = "" + FilePath + ".flv";
                    }
                }

                if (Method == "GET")
                {
                    int Viewed = Base.Data.SqlScalar("Select Count(*) From DBS_Log Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And DBS_FileId = " + Id, ref Conn);

                    if (Viewed == 0)
                    {
                        AppCommon.Log(Id, "file-view", ref Conn);
                    }

                    Output(CodeId, FilePath, Context);
                }
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
        private void Output(string CodeId, string FilePath, HttpContext Context)
        {
            MemoryStream MS = default(MemoryStream);
            byte[] Bytes = {};
            byte[] Buffer = {};
            int Count = 0;

            try
            {
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
                Context.Response.AddHeader("Content-Length", Bytes.Length.ToString());
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");

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
