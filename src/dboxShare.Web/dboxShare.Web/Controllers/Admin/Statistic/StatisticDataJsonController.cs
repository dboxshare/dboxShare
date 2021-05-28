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


    public class Admin_StatisticDataJsonController : ApiController
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
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = new ArrayList();

                Json.Add("'user_total':'" + UserTotal() + "'");
                Json.Add("'folder_total':'" + FolderTotal() + "'");
                Json.Add("'file_total':'" + FileTotal() + "'");
                Json.Add("'occupy_total':'" + OccupyTotal(AppConfig.FileStoragePath) + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 上传统计数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/statistic/upload-data-json")]
        [HttpGet]
        public HttpResponseMessage UploadDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = new ArrayList();

                Json.Add("'today_upload_total':'" + UploadTotal("today") + "'");
                Json.Add("'yesterday_upload_total':'" + UploadTotal("yesterday") + "'");
                Json.Add("'week_upload_total':'" + UploadTotal("week") + "'");
                Json.Add("'month_upload_total':'" + UploadTotal("month") + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 下载统计数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/statistic/download-data-json")]
        [HttpGet]
        public HttpResponseMessage DownloadDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = new ArrayList();

                Json.Add("'today_download_total':'" + DownloadTotal("today") + "'");
                Json.Add("'yesterday_download_total':'" + DownloadTotal("yesterday") + "'");
                Json.Add("'week_download_total':'" + DownloadTotal("week") + "'");
                Json.Add("'month_download_total':'" + DownloadTotal("month") + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 用户数量统计
        /// </summary>
        private int UserTotal()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_User", ref Conn);
        }


        /// <summary>
        /// 文件夹数量统计
        /// </summary>
        private int FolderTotal()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_Folder = 1", ref Conn);
        }


        /// <summary>
        /// 文件数量统计
        /// </summary>
        private int FileTotal()
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_VersionId = 0 And DS_Folder = 0", ref Conn);
        }


        /// <summary>
        /// 占用空间统计
        /// </summary>
        private long OccupyTotal(string Path)
        {
            if (Directory.Exists(Path) == false)
            {
                return 0;
            }

            var DI = new DirectoryInfo(Path);
            var Bytes = 0L;

            foreach (var DirectoryItem in DI.GetDirectories())
            {
                Bytes += OccupyTotal(DirectoryItem.FullName);
            }

            foreach (var FileItem in DI.GetFiles())
            {
                Bytes += FileItem.Length;
            }

            return Bytes;
        }


        /// <summary>
        /// 上传统计
        /// </summary>
        private int UploadTotal(string Time)
        {
            var Query = "";

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
        private int DownloadTotal(string Time)
        {
            var Query = "";

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
