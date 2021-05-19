using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class StatisticDataJsonController : ApiController
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
        /// 基本统计数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/statistic/basis-data-json")]
        [HttpGet]
        public HttpResponseMessage BasisDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                ArrayList Json = new ArrayList();

                Json.Add("'user_count':'" + UserCount() + "'");
                Json.Add("'folder_count':'" + FolderCount() + "'");
                Json.Add("'file_count':'" + FileCount() + "'");
                Json.Add("'occupy_space':'" + UsedSpace(AppConfig.FileStoragePath) + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 时间统计数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/statistic/time-data-json")]
        [HttpGet]
        public HttpResponseMessage TimeDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                ArrayList Json = new ArrayList();

                Json.Add("'today_upload':'" + UploadCount("today") + "'");
                Json.Add("'today_download':'" + DownloadCount("today") + "'");
                Json.Add("'yesterday_upload':'" + UploadCount("yesterday") + "'");
                Json.Add("'yesterday_download':'" + DownloadCount("yesterday") + "'");
                Json.Add("'week_upload':'" + UploadCount("week") + "'");
                Json.Add("'week_download':'" + DownloadCount("week") + "'");
                Json.Add("'month_upload':'" + UploadCount("month") + "'");
                Json.Add("'month_download':'" + DownloadCount("month") + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 用户统计
        /// </summary>
        private int UserCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_User", ref Conn);
        }


        /// <summary>
        /// 文件夹统计
        /// </summary>
        private int FolderCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_Folder = 1", ref Conn);
        }


        /// <summary>
        /// 文件统计
        /// </summary>
        private int FileCount()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_VersionId = 0 And DS_Folder = 0", ref Conn);
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
        private int UploadCount(string Time)
        {
            string Query = "";

            switch (Time)
            {
                case "today":
                    Query = "DS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "yesterday":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "week":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + " 00:00:00'";
                    break;

                case "month":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(0 - DateTime.Now.Day).ToShortDateString() + " 00:00:00'";
                    break;
            }

            return Base.Data.SqlScalar("Select Count(*) From DS_Log Where DS_Action = 'file-upload' And " + Query + "", ref Conn);
        }


        /// <summary>
        /// 下载统计
        /// </summary>
        private int DownloadCount(string Time)
        {
            string Query = "";

            switch (Time)
            {
                case "today":
                    Query = "DS_Time = '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "yesterday":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00'";
                    break;

                case "week":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).ToShortDateString() + " 00:00:00'";
                    break;

                case "month":
                    Query = "DS_Time > '" + DateTime.Now.AddDays(0 - DateTime.Now.Day).ToShortDateString() + " 00:00:00'";
                    break;
            }

            return Base.Data.SqlScalar("Select Count(*) From DS_Log Where DS_Action = 'file-download' And " + Query + "", ref Conn);
        }


    }


}
