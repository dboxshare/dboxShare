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


    public class ListAttributeJsonController : ApiController
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
        /// 获取列表属性数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/list-attribute-json")]
        [HttpGet]
        public HttpResponseMessage ListAttributeJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            bool Folder = Context.Request.QueryString["Folder"].TypeString() == "true" ? true : false;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Purview = AppCommon.PurviewRole(Id, Folder, ref Conn);

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock, DS_Recycle From DS_File Where DS_Folder = " + (Folder == true ? 1 : 0) + " And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int Lock = FileTable["DS_Lock"].TypeInt();
                int Recycle = FileTable["DS_Recycle"].TypeInt();

                FileTable.Clear();

                return AppCommon.ResponseMessage("{'purview':'" + Purview + "','lock':'" + Lock + "','recycle':'" + Recycle + "'}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
