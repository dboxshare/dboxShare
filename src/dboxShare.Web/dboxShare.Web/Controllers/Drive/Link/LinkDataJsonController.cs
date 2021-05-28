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


    public class Drive_LinkDataJsonController : ApiController
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
        /// 读取链接数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/link/data-json")]
        [HttpGet]
        public HttpResponseMessage DataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                if (Context.Session["Admin"].TypeInt() == 0)
                {
                    var LinkTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_UserId From DS_Link Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id, ref Conn, ref LinkTable);

                    if (LinkTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data-not-exist");
                    }

                    LinkTable.Clear();
                }

                var Json = Base.Data.SqlDataToJson("Select DS_Id, DS_CodeId, DS_UserId, DS_Username, DS_Title, DS_Deadline, DS_Password, DS_Count, DS_Revoke, DS_Time From DS_Link Where DS_Id = " + Id, ref Conn);

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
