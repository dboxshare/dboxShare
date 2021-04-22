using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class LogListJsonController : ApiController
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
        /// 读取日志数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/log/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Action = Context.Request.QueryString["Action"].TypeString();

            if (string.IsNullOrEmpty(Action) == false)
            {
                if (Base.Common.StringCheck(Action, @"^[\w\-]+$") == false)
                {
                    Action = "";
                }
            }

            string Timestamp = Context.Request.QueryString["Timestamp"].TypeString();

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                if (Base.Common.StringCheck(Timestamp, @"^[\w\-]+$") == false)
                {
                    Timestamp = "";
                }
            }

            string TimeStart = Context.Request.QueryString["TimeStart"].TypeString();

            if (string.IsNullOrEmpty(TimeStart) == false)
            {
                if (Base.Common.StringCheck(TimeStart, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    TimeStart = "";
                }
            }

            string TimeEnd = Context.Request.QueryString["TimeEnd"].TypeString();

            if (string.IsNullOrEmpty(TimeEnd) == false)
            {
                if (Base.Common.StringCheck(TimeEnd, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    TimeEnd = "";
                }
            }

            string Keyword = Context.Request.QueryString["Keyword"].TypeString();

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Keyword = Base.Common.InputFilter(Keyword);
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit 50 pages");
            }

            string Query = "";

            if (string.IsNullOrEmpty(Action) == false)
            {
                Query += "DS_Action = '" + Action + "' And ";
            }

            if (string.IsNullOrEmpty(TimeStart) == true && string.IsNullOrEmpty(TimeEnd) == true)
            {
                if (string.IsNullOrEmpty(Timestamp) == false)
                {
                    int Day = (int)DateTime.Now.DayOfWeek;

                    switch (Timestamp)
                    {
                        case "this-day":
                            Query += "DS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-day-ago":
                            Query += "DS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-day-ago":
                            Query += "DS_Time > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DS_Time < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-week":
                            Query += "DS_Time > '" + DateTime.Now.AddDays((7 - Day) - 14).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-week-ago":
                            Query += "DS_Time < '" + DateTime.Now.AddDays(((7 - Day) - 14) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-week-ago":
                            Query += "DS_Time < '" + DateTime.Now.AddDays(((7 - Day) - 21) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;
                    }
                }
            }
            else
            {
                Query += "DS_Time > '" + TimeStart + ":00' And DS_Time < '" + TimeEnd + ":00' And ";
            }

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Query += "Exists (";

                // 用户账号查询
                Query += "Select A.DS_Id From DS_Log As A Where " +
                         "A.DS_Id = DS_Log.DS_Id And " +
                         "A.DS_Username = '" + Keyword + "' Union All ";

                // 文件名称查询
                Query += "Select B.DS_Id From DS_Log As B Where " +
                         "B.DS_Id = DS_Log.DS_Id And " +
                         "B.DS_FileName Like '" + Keyword + "%'";

                Query += ") And ";
            }

            if (string.IsNullOrEmpty(Query) == true)
            {
                Query += "DS_Id > 0";
            }
            else
            {
                Query = Query.Substring(0, Query.Length - 5);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_Log", "DS_Id, DS_FileId, DS_FileName, DS_FileExtension, DS_FileVersion, DS_IsFolder, DS_UserId, DS_Username, DS_Action, DS_IP, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

                return AppCommon.ResponseMessage(Json);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
