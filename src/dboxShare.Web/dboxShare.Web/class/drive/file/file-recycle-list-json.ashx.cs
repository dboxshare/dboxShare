using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class FileRecycleListJson : IHttpHandler, IReadOnlySessionState
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

            ListDataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取文件回收站数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            string Timestamp = "";
            int Page = 0;
            int Day = 0;
            string Query = "";
            string Json = "";

            Timestamp = Context.Request.QueryString["Timestamp"].TypeString();

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                if (Base.Common.StringCheck(Timestamp, @"^[\w\-]+$") == false)
                {
                    return;
                }
            }

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                Day = (int)DateTime.Now.DayOfWeek;

                switch (Timestamp)
                {
                    case "this-day":
                        Query += "DBS_RemoveTime > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "1-day-ago":
                        Query += "DBS_RemoveTime > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DBS_RemoveTime < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "2-day-ago":
                        Query += "DBS_RemoveTime > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DBS_RemoveTime < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "this-week":
                        Query += "DBS_RemoveTime > '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "1-week-ago":
                        Query += "DBS_RemoveTime < '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "2-week-ago":
                        Query += "DBS_RemoveTime < '" + DateTime.Now.AddDays(((7 - Day) - 7 - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                        break;
                }
            }

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_Recycle = 1 And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " And " + 
                     "Not Exists (Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = A1.DBS_FolderId And " + 
                     "A2.DBS_Recycle = 1) Union All ";

            // 创建者查询
            Query += "Select B1.DBS_Id From DBS_File As B1 Where " + 
                     "B1.DBS_Id = DBS_File.DBS_Id And " + 
                     "B1.DBS_Recycle = 1 And " + 
                     "B1.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' And " + 
                     "Not Exists (Select B2.DBS_Id From DBS_File As B2 Where " + 
                     "B2.DBS_Id = B1.DBS_FolderId And " + 
                     "B2.DBS_Recycle = 1) Union All ";

            // 移除者查询
            Query += "Select C1.DBS_Id From DBS_File As C1 Where " + 
                     "C1.DBS_Id = DBS_File.DBS_Id And " + 
                     "C1.DBS_Recycle = 1 And " + 
                     "C1.DBS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "' And " + 
                     "Not Exists (Select C2.DBS_Id From DBS_File As C2 Where " + 
                     "C2.DBS_Id = C1.DBS_FolderId And " + 
                     "C2.DBS_Recycle = 1)";

            Query += ")";

            Json = Base.Data.SqlPageToJson("DBS_File", "DBS_Id, DBS_UserId, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Size, DBS_Share, DBS_Lock, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime, DBS_RemoveUsername, DBS_RemoveTime", "DBS_RemoveTime Desc, DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
