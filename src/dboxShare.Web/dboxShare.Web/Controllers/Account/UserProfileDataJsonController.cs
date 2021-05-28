using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Account.Controllers
{


    public class Account_UserProfileDataJsonController : ApiController
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
        /// 读取用户数据记录返回 JSON 格式字符串
        /// </summary>
        [Route("api/account/user-profile-data-json")]
        [HttpGet]
        public HttpResponseMessage ProfileDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Session["UserId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlDataToJson("Select DS_Id, DS_Username, DS_Name, DS_Email, DS_Phone, DS_Tel From DS_User Where DS_Id = " + Id, ref Conn);

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
