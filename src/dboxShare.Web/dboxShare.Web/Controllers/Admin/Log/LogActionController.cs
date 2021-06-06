using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class Admin_LogActionController : ApiController
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
        /// 日志导出
        /// </summary>
        [Route("api/admin/log/export")]
        [HttpGet]
        public HttpResponseMessage Export()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var LogFileName = "" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

            var LogFilePath = Base.Common.PathCombine(AppConfig.TempStoragePath, LogFileName);;

            var Action = Context.Request.QueryString["Action"].TypeString();

            if (string.IsNullOrEmpty(Action) == false)
            {
                if (Base.Common.StringCheck(Action, @"^[\w\-]+$") == false)
                {
                    Action = "";
                }
            }

            var Time = Context.Request.QueryString["Time"].TypeString();

            if (string.IsNullOrEmpty(Time) == false)
            {
                if (Base.Common.StringCheck(Time, @"^[\w\-]+$") == false)
                {
                    Time = "";
                }
            }

            var TimeStart = Context.Request.QueryString["TimeStart"].TypeString();

            if (string.IsNullOrEmpty(TimeStart) == false)
            {
                if (Base.Common.StringCheck(TimeStart, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    TimeStart = "";
                }
            }

            var TimeEnd = Context.Request.QueryString["TimeEnd"].TypeString();

            if (string.IsNullOrEmpty(TimeEnd) == false)
            {
                if (Base.Common.StringCheck(TimeEnd, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}$") == false)
                {
                    TimeEnd = "";
                }
            }

            var Keyword = Context.Request.QueryString["Keyword"].TypeString();

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Keyword = Base.Common.InputFilter(Keyword);
            }

            var Query = "";

            if (string.IsNullOrEmpty(Action) == false)
            {
                Query += "DS_Action = '" + Action + "' And ";
            }

            if (string.IsNullOrEmpty(TimeStart) == true && string.IsNullOrEmpty(TimeEnd) == true)
            {
                if (string.IsNullOrEmpty(Time) == false)
                {
                    var Day = (int)DateTime.Now.DayOfWeek;

                    switch (Time)
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

            var LogList = new List<Hashtable>();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Sql = "Select Top 1000 DS_Id, DS_FileId, DS_FileName, DS_FileExtension, DS_FileVersion, DS_UserId, DS_Username, DS_Action, DS_IP, DS_Time From DS_Log Where " + Query + " Order By DS_Id Desc";

                Base.Data.SqlListToTable(Sql, ref Conn, ref LogList);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }

            using (var FStream = File.OpenWrite(LogFilePath))
            {
                using (var SWriter = new StreamWriter(FStream, Encoding.UTF8))
                {
                    SWriter.WriteLine("operation time, operation action, file id, file name, file version, user id, user name, user ip");

                    for (var i = 0; i < LogList.Count; i++)
                    {
                        var Log = new ArrayList();

                        Log.Add("" + LogList[i]["DS_Time"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_Action"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_FileId"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_FileName"].TypeString() + "" + LogList[i]["DS_FileExtension"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_FileVersion"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_UserId"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_Username"].TypeString() + "");
                        Log.Add("" + LogList[i]["DS_IP"].TypeString() + "");

                        SWriter.WriteLine(string.Join(", ", Log.ToArray()));
                    }
                }
            }

            // 输出日志文件
            try
            {
                Context.Response.Clear();
                Context.Response.Buffer = false;
                Context.Response.BufferOutput = false;
                Context.Response.AddHeader("Content-Type", "application/octet-stream");
                Context.Response.AddHeader("Content-Length", new FileInfo(LogFilePath).Length.ToString());
                Context.Response.AddHeader("Content-Transfer-Encoding", "binary");
                Context.Response.AddHeader("Content-Disposition", "attachment; filename=" + LogFileName + "");
                Context.Response.TransmitFile(LogFilePath);
                Context.Response.Close();
                Context.Response.End();
            }
            catch (Exception)
            {
                Context.Response.End();
            }
            finally
            {
                if (File.Exists(LogFilePath) == true)
                {
                    File.Delete(LogFilePath);
                }
            }

            return AppCommon.ResponseMessage(HttpStatusCode.OK);
        }


    }


}
