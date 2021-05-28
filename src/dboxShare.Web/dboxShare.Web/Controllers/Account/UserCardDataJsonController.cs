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


    public class Account_UserCardDataJsonController : ApiController
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
        [Route("api/account/user-card-data-json")]
        [HttpGet]
        public HttpResponseMessage CardDataJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Username = Context.Request.QueryString["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Json = Base.Data.SqlDataToJson("Select DS_Id, DS_DepartmentId, DS_RoleId, DS_Username, DS_Name, DS_Code, DS_Title, DS_Email, DS_Phone, DS_Tel, DS_Admin, DS_Freeze, DS_LoginIP, DS_LoginTime From DS_User Where DS_Username = '" + Username + "'", ref Conn);

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