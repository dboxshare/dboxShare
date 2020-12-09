using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class StatisticDataJson : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            switch (Context.Request.QueryString["Type"].TypeString())
            {
                case "basis":
                    BasisDataToJson(Context);
                    break;

                case "time":
                    TimeDataToJson(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 基本统计数据返回json格式字符串
        /// </summary>
        private void BasisDataToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();

            JsonList.Add("'user_count':'" + UserCount() + "'");
            JsonList.Add("'folder_count':'" + FolderCount() + "'");
            JsonList.Add("'file_count':'" + FileCount() + "'");
            JsonList.Add("'occupy_space':'" + UsedSpace(Context.Server.MapPath("/storage/file/")) + "'");

            Context.Response.Write("{" + string.Join(",", JsonList.ToArray()) + "}");
        }


        /// <summary>
        /// 时间统计数据返回json格式字符串
        /// </summary>
        private void TimeDataToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();

            JsonList.Add("'today_upload':'" + UploadCount("today") + "'");
            JsonList.Add("'today_download':'" + DownloadCount("today") + "'");
            JsonList.Add("'yesterday_upload':'" + UploadCount("yesterday") + "'");
            JsonList.Add("'yesterday_download':'" + DownloadCount("yesterday") + "'");
            JsonList.Add("'week_upload':'" + UploadCount("week") + "'");
            JsonList.Add("'week_download':'" + DownloadCount("week") + "'");
            JsonList.Add("'month_upload':'" + UploadCount("month") + "'");
            JsonList.Add("'month_download':'" + DownloadCount("month") + "'");

            Context.Response.Write("{" + string.Join(",", JsonList.ToArray()) + "}");
        }


        /// <summary>
        /// 用户统计
        /// </summary>
        private string UserCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DBS_User", ref Conn).TypeString();
        }


        /// <summary>
        /// 文件夹统计
        /// </summary>
        private string FolderCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DBS_File Where DBS_Folder = 1", ref Conn).TypeString();
        }


        /// <summary>
        /// 文件统计
        /// </summary>
        private string FileCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0", ref Conn).TypeString();
        }


        /// <summary>
        /// 占用空间统计
        /// </summary>
        private long UsedSpace(string Path)
        {
            if (Directory.Exists(Path) == false)
            {
                return 0;
            }

            DirectoryInfo DI = new DirectoryInfo(Path);
            long Bytes = 0;

            foreach (DirectoryInfo DirectoryItem in DI.GetDirectories())
            {
                Bytes += UsedSpace(DirectoryItem.FullName);
            }

            foreach (FileInfo FileItem in DI.GetFiles())
            {
                Bytes += FileItem.Length;
            }

            return Bytes;
        }


        /// <summary>
        /// 上传统计
        /// </summary>
        private string UploadCount(string Time)
        {
            string Query = "";

            switch (Time)
            {
                case "today":
                    Query = "DBS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "yesterday":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DBS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "week":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + " 00:00:00'";
                    break;

                case "month":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(0 - DateTime.Now.Day).ToShortDateString() + " 00:00:00'";
                    break;
            }

            return Base.Data.SqlScalar("Select Count(*) From DBS_Log Where DBS_Action = 'file-upload' And " + Query + "", ref Conn).TypeString();
        }


        /// <summary>
        /// 下载统计
        /// </summary>
        private string DownloadCount(string Time)
        {
            string Query = "";

            switch (Time)
            {
                case "today":
                    Query = "DBS_Time = '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "yesterday":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DBS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "week":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + " 00:00:00'";
                    break;

                case "month":
                    Query = "DBS_Time > '" + DateTime.Now.AddDays(0 - DateTime.Now.Day).ToShortDateString() + " 00:00:00'";
                    break;
            }

            return Base.Data.SqlScalar("Select Count(*) From DBS_Log Where DBS_Action = 'file-download' And " + Query + "", ref Conn).TypeString();
        }


    }


}
