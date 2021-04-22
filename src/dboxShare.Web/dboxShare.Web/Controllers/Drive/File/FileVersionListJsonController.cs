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


    public class FileVersionListJsonController : ApiController
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
        /// 读取文件版本数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/version-list-json")]
        [HttpGet]
        public HttpResponseMessage VersionListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            bool Myself = Context.Request.QueryString["Myself"].TypeString() == "true" ? true : false;

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            string Query = "";

            Query += "DS_VersionId = " + Id + " And ";

            if (Myself == true)
            {
                Query += "Exists (";

                // 创建者查询
                Query += "Select A.DS_Id From DS_File As A Where " +
                         "A.DS_Id = DS_File.DS_Id And " +
                         "A.DS_Version = 1 And " +
                         "A.DS_VersionId = " + Id + " And " +
                         "A.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                // 更新者查询
                Query += "Select B.DS_Id From DS_File As B Where " +
                         "B.DS_Id = DS_File.DS_Id And " +
                         "B.DS_VersionId = " + Id + " And " +
                         "B.DS_UpdateUsername = '" + Context.Session["Username"].TypeString() + "'";

                Query += ") And ";
            }

            Query += "DS_Recycle = 0";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_File", "DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_CodeId, DS_Name, DS_Extension, DS_Remark, DS_Share, DS_Lock, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime", "DS_Id Desc", Query, 50, Page, ref Conn);

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
