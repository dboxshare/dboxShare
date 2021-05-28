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


    public class Drive_FileRecentListJsonController : ApiController
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
        /// 读取收藏数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/recent-list-json")]
        [HttpGet]
        public HttpResponseMessage RecentListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit-50-pages");
            }

            var Join = "Inner Join DS_Log On " + 
                       "DS_Log.DS_FileId = DS_File.DS_Id And " + 
                       "DS_Log.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            var Query = "";

            Query += "Exists (";

            Query += "Select A.DS_Id From DS_Log As A Where " +
                     "A.DS_Id = DS_Log.DS_Id And " +
                     "A.DS_Action = 'file-view'";

            Query += ") And ";

            Query += "DS_File.DS_VersionId = 0 And ";
            Query += "DS_File.DS_Recycle = 0";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlPageToJson("DS_File", "DS_File.DS_Id, DS_File.DS_UserId, DS_File.DS_Version, DS_File.DS_VersionId, DS_File.DS_Folder, DS_File.DS_FolderId, DS_File.DS_CodeId, DS_File.DS_Name, DS_File.DS_Extension, DS_File.DS_Size, DS_File.DS_Type, DS_File.DS_Remark, DS_File.DS_Share, DS_File.DS_Lock, DS_File.DS_Recycle, DS_Log.DS_Id, DS_Log.DS_Time", "DS_Log.DS_Id Desc, DS_File.DS_Id Desc", Join, Query, 50, Page, ref Conn);

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
