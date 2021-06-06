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


    public class Share_LinkDataJsonController : ApiController
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
        /// 读取分享链接数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/link/link-data-json")]
        [HttpGet]
        public HttpResponseMessage LinkDataJson()
        {
            var UserId = Context.Request.QueryString["UserId"].TypeInt();

            var CodeId = Context.Request.QueryString["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\w]{16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlDataToJson("Select DS_Id, DS_CodeId, DS_UserId, DS_Deadline, DS_Revoke From DS_Link Where DS_UserId = " + UserId + " And DS_CodeId = '" + CodeId + "'", ref Conn);

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
