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


    public class FileDataJsonController : ApiController
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
        /// 读取文件数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/data-json")]
        [HttpGet]
        public HttpResponseMessage DataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no operation permission");
                }

                string Json = Base.Data.SqlDataToJson("Select DS_Id, DS_Username, DS_Version, DS_Folder, DS_FolderId, DS_Name, DS_Extension, DS_Size, DS_Remark, DS_Share, DS_Lock, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime From DS_File Where DS_Folder = 0 And DS_Id = " + Id, ref Conn);

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
