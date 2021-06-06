using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Share.Controllers
{


    public class Share_FileListJsonController : ApiController
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
        /// 读取分享文件数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/link/file-list-json")]
        [HttpGet]
        public HttpResponseMessage FileListJson()
        {
            var LinkId = Context.Session["LinkId"].TypeInt();

            if (LinkId.TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("link-login-failed");
            }

            var FolderId = Context.Request.QueryString["FolderId"].TypeInt();

            var Query = "";

            Query += "Exists (";

            // 分享文件查询
            if (FolderId == 0)
            {
                Query += "Select A.DS_Id From DS_File As A Inner Join DS_Link_File On " +
                         "DS_Link_File.DS_FileId = A.DS_Id And " +
                         "DS_Link_File.DS_LinkId = " + LinkId + " Where " +
                         "A.DS_Id = DS_File.DS_Id And " +
                         "A.DS_Recycle = 0";
            }
            else
            {
                Query += "Select A.DS_Id From DS_File As A Inner Join DS_Link_File On " +
                         "DS_Link_File.DS_FileId = A.DS_FolderId And " +
                         "DS_Link_File.DS_LinkId = " + LinkId + " Where " +
                         "A.DS_Id = DS_File.DS_Id And " +
                         "A.DS_Folder = 0 And " +
                         "A.DS_FolderId = " + FolderId + " And " +
                         "A.DS_Recycle = 0";
            }

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Sql = "Select DS_Id, DS_Version, DS_Folder, DS_FolderId, DS_CodeId, DS_Name, DS_Extension, DS_Size, DS_Recycle, DS_CreateTime, DS_UpdateTime From DS_File Where " + Query + " Order By DS_Folder Desc, DS_Name Asc, DS_Id Desc";

                var Json = Base.Data.SqlListToJson(Sql, ref Conn);

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
