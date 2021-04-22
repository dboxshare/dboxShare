using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class FileRecycleListJsonController : ApiController
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
        /// 读取文件回收站数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/recycle-list-json")]
        [HttpGet]
        public HttpResponseMessage RecycleListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Timestamp = Context.Request.QueryString["Timestamp"].TypeString();

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                if (Base.Common.StringCheck(Timestamp, @"^[\w\-]+$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            string Query = "";

            if (string.IsNullOrEmpty(Timestamp) == false)
            {
                int Day = (int)DateTime.Now.DayOfWeek;

                switch (Timestamp)
                {
                    case "this-day":
                        Query += "DS_RemoveTime > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "1-day-ago":
                        Query += "DS_RemoveTime > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DS_RemoveTime < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "2-day-ago":
                        Query += "DS_RemoveTime > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DS_RemoveTime < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "this-week":
                        Query += "DS_RemoveTime > '" + DateTime.Now.AddDays((7 - Day) - 14).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "1-week-ago":
                        Query += "DS_RemoveTime < '" + DateTime.Now.AddDays(((7 - Day) - 14) + 1).ToShortDateString() + " 00:00:00' And ";
                        break;

                    case "2-week-ago":
                        Query += "DS_RemoveTime < '" + DateTime.Now.AddDays(((7 - Day) - 21) + 1).ToShortDateString() + " 00:00:00' And ";
                        break;
                }
            }

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Recycle = 1 And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " And " +
                     "Not Exists (Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = A1.DS_FolderId And " +
                     "A2.DS_Recycle = 1) Union All ";

            // 创建者查询
            Query += "Select B1.DS_Id From DS_File As B1 Where " +
                     "B1.DS_Id = DS_File.DS_Id And " +
                     "B1.DS_Recycle = 1 And " +
                     "B1.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' And " +
                     "Not Exists (Select B2.DS_Id From DS_File As B2 Where " +
                     "B2.DS_Id = B1.DS_FolderId And " +
                     "B2.DS_Recycle = 1) Union All ";

            // 移除者查询
            Query += "Select C1.DS_Id From DS_File As C1 Where " +
                     "C1.DS_Id = DS_File.DS_Id And " +
                     "C1.DS_Recycle = 1 And " +
                     "C1.DS_RemoveUsername = '" + Context.Session["Username"].TypeString() + "' And " +
                     "Not Exists (Select C2.DS_Id From DS_File As C2 Where " +
                     "C2.DS_Id = C1.DS_FolderId And " +
                     "C2.DS_Recycle = 1)";

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_File", "DS_Id, DS_UserId, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_CodeId, DS_Name, DS_Extension, DS_Size, DS_Share, DS_Lock, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime", "DS_RemoveTime Desc, DS_Id Desc", Query, 50, Page, ref Conn);

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
