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


    public class FolderDataJsonController : ApiController
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
        /// 读取文件夹数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/folder/data-json")]
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

                if (Context.Session["Admin"].TypeInt() == 0)
                {
                    if (AppCommon.PurviewCheck(Id, true, "viewer", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no operation permission");
                    }
                }

                string Json = Base.Data.SqlDataToJson("Select DS_Id, DS_Username, DS_Folder, DS_FolderId, DS_Name, DS_Remark, DS_Share, DS_Lock, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime, DS_RemoveUsername, DS_RemoveTime From DS_File Where DS_Folder = 1 And DS_Id = " + Id, ref Conn);

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
