using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class LogListJson : IHttpHandler, IReadOnlySessionState
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

            ListDataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取日志数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            string Action = "";
            string Timestamp = "";
            string TimeStart = "";
            string TimeEnd = "";
            string Keyword = "";
            int Page = 0;
            int Day = 0;
            string Query = "";
            string Json = "";

            Action = Context.Request.QueryString["Action"].TypeString();

            if (string.IsNullOrEmpty(Action) == false)
            {
                if (Base.Common.StringCheck(Action, @"^[\w\-]+$") == false)
                {
                    return;
                }
            }

            Timestamp = Context.Request.QueryString["Timestamp"].TypeString();

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                if (Base.Common.StringCheck(Timestamp, @"^[\w\-]+$") == false)
                {
                    return;
                }
            }

            TimeStart = Context.Request.QueryString["TimeStart"].TypeString();

            if (string.IsNullOrEmpty(TimeStart) == false)
            {
                if (Base.Common.StringCheck(TimeStart, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    return;
                }
            }

            TimeEnd = Context.Request.QueryString["TimeEnd"].TypeString();

            if (string.IsNullOrEmpty(TimeEnd) == false)
            {
                if (Base.Common.StringCheck(TimeEnd, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    return;
                }
            }

            Keyword = Base.Common.InputFilter(Context.Request.QueryString["Keyword"].TypeString());

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            if (Page > 50)
            {
                return;
            }

            if (string.IsNullOrEmpty(Action) == false)
            {
                Query += "DBS_Action = '" + Action + "' And ";
            }

            if (string.IsNullOrEmpty(TimeStart) == true && string.IsNullOrEmpty(TimeEnd) == true)
            {
                if (string.IsNullOrEmpty(Timestamp) == false)
                {
                    Day = (int)DateTime.Now.DayOfWeek;

                    switch (Timestamp)
                    {
                        case "this-day":
                            Query += "DBS_Time > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-day-ago":
                            Query += "DBS_Time > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DBS_Time < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-day-ago":
                            Query += "DBS_Time > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DBS_Time < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-week":
                            Query += "DBS_Time > '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-week-ago":
                            Query += "DBS_Time < '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-week-ago":
                            Query += "DBS_Time < '" + DateTime.Now.AddDays(((7 - Day) - 7 - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;
                    }
                }
            }
            else
            {
                Query += "DBS_Time > '" + TimeStart + ":00' And DBS_Time < '" + TimeEnd + ":00' And ";
            }

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Query += "Exists (";

                // 用户账号查询
                Query += "Select A.DBS_Id From DBS_Log As A Where " + 
                         "A.DBS_Id = DBS_Log.DBS_Id And " + 
                         "A.DBS_Username = '" + Keyword + "' Union All ";

                // 文件名称查询
                Query += "Select B.DBS_Id From DBS_Log As B Where " + 
                         "B.DBS_Id = DBS_Log.DBS_Id And " + 
                         "B.DBS_FileName Like '" + Keyword + "%'";

                Query += ") And ";
            }

            if (string.IsNullOrEmpty(Query) == true)
            {
                Query += "DBS_Id > 0";
            }
            else
            {
                Query = Query.Substring(0, Query.Length - 5);
            }

            Json = Base.Data.SqlPageToJson("DBS_Log", "DBS_Id, DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_FileVersion, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Action, DBS_IP, DBS_Time", "DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
