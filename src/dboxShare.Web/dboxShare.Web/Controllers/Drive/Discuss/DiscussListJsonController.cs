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


    public class DiscussListJsonController : ApiController
    {


        private dynamic Conn;


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        [Route("api/drive/discuss/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.QueryString["Id"].TypeInt();

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit 50 pages");
            }

            string Query = "DS_FileId = " + Id;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder From DS_File Where DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int Folder = FileTable["DS_Folder"].TypeInt();

                FileTable.Clear();

                if (AppCommon.PurviewCheck(Id, Folder == 1 ? true : false, "viewer", ref Conn) == false)
                {
                    return AppCommon.ResponseMessage("no operation permission");
                }

                string Json = Base.Data.SqlPageToJson("DS_Discuss", "DS_Id, DS_FileId, DS_FileName, DS_UserId, DS_Username, DS_Content, DS_Revoke, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
