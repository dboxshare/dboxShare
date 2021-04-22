using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.User.Controllers
{


    public class UserProfileController : ApiController
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
        /// 用户资料设置
        /// </summary>
        [Route("api/user/profile")]
        [HttpPost]
        public HttpResponseMessage Profile()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Session["UserId"].TypeInt();

            string Password = Context.Request.Form["Password"].TypeString();

            if (string.IsNullOrEmpty(Password) == false)
            {
                if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string Email = Context.Request.Form["Email"].TypeString();

            if (Base.Common.StringCheck(Email, @"^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable UserTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                UserTable.Clear();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Email From DS_User Where DS_Email = '" + Email + "'", ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == true)
                {
                    if (UserTable["DS_Id"].TypeInt() != Id)
                    {
                        return AppCommon.ResponseMessage("email-existed");
                    }
                }

                UserTable.Clear();

                if (string.IsNullOrEmpty(Phone) == false)
                {
                    Base.Data.SqlDataToTable("Select DS_Id, DS_Phone From DS_User Where DS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        if (UserTable["DS_Id"].TypeInt() != Id)
                        {
                            return AppCommon.ResponseMessage("phone-existed");
                        }
                    }

                    UserTable.Clear();
                }

                string Sql = "";

                if (string.IsNullOrEmpty(Password) == true)
                {
                    Sql = "Update DS_User Set DS_Email = '" + Email + "', DS_Phone = '" + Phone + "', DS_Tel = '" + Tel + "' Where DS_Id = " + Id;
                }
                else
                {
                    string PasswordMD5 = Base.Common.StringHash(Password, "MD5");

                    Sql = "Update DS_User Set DS_Password = '" + PasswordMD5 + "', DS_Email = '" + Email + "', DS_Phone = '" + Phone + "', DS_Tel = '" + Tel + "' Where DS_Id = " + Id;
                }

                Base.Data.SqlQuery(Sql, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
