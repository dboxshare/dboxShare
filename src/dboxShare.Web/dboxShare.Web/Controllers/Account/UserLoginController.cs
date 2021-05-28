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


    public class Account_UserLoginController : ApiController
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
        /// 用户登录
        /// </summary>
        [Route("api/account/user-login")]
        [HttpPost]
        public HttpResponseMessage Login()
        {
            var LoginId = Context.Request.Form["LoginId"].TypeString();

            if (Base.Common.StringCheck(LoginId, @"^([^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}|[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?|\+?([\d]{2,4}\-?)?[\d]{6,11})$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var LockId = "Login-Lock-IP-" + Base.Common.ClientIP() + "";

            if (Context.Cache[LockId].TypeInt() == 5)
            {
                return AppCommon.ResponseMessage("login-lock-ip");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                // 本地用户登录
                var UserId = LocalUser(LoginId, Password);

                if (UserId == 0)
                {
                    if (Base.Common.IsNothing(Context.Cache[LockId]) == true)
                    {
                        Context.Cache.Insert(LockId, 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }
                    else
                    {
                        Context.Cache.Insert(LockId, Context.Cache[LockId].TypeInt() + 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }

                    return AppCommon.ResponseMessage("login-failed");
                }

                var Query = "";

                Query += "DS_Id = " + UserId + " And ";
                Query += "DS_Freeze = 0 And ";
                Query += "DS_Recycle = 0";

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_DepartmentId, DS_RoleId, DS_Username, DS_Email, DS_Phone, DS_UploadSize, DS_DownloadSize, DS_Admin, DS_Freeze, DS_Recycle From DS_User Where " + Query + "", ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("login-failed");
                }

                var Id = UserTable["DS_Id"].TypeInt();
                var Username = UserTable["DS_Username"].TypeString();
                var DepartmentId = UserTable["DS_DepartmentId"].TypeString();
                var RoleId = UserTable["DS_RoleId"].TypeInt();
                var UploadSize = UserTable["DS_UploadSize"].TypeInt();
                var DownloadSize = UserTable["DS_DownloadSize"].TypeInt();
                var Admin = UserTable["DS_Admin"].TypeInt();

                UserTable.Clear();

                Base.Data.SqlQuery("Update DS_User Set DS_LoginIP = '" + Base.Common.ClientIP() + "', DS_LoginTime = '" + DateTime.Now.ToString() + "' Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Insert Into DS_User_Log(DS_UserId, DS_Username, DS_IP, DS_Time) Values(" + Id + ", '" + Username + "', '" + Base.Common.ClientIP() + "', '" + DateTime.Now.ToString() + "')", ref Conn);

                Context.Session["UserId"] = Id;
                Context.Session["Username"] = Username;
                Context.Session["DepartmentId"] = DepartmentId;
                Context.Session["RoleId"] = RoleId == 0 ? -1 : RoleId;
                Context.Session["UploadSize"] = UploadSize;
                Context.Session["DownloadSize"] = DownloadSize;
                Context.Session["Admin"] = Admin;
                Context.Session["LoginToken"] = Base.Crypto.TextEncrypt(Context.Session.SessionID, AppConfig.SecurityKey);

                Context.Cache.Insert("Login-Token-Web-" + Id + "", Context.Session["LoginToken"].TypeString(), null, DateTime.MaxValue, TimeSpan.FromHours(12));

                Context.Response.Cookies["User-Login-Id"].Value = Context.Server.UrlEncode(LoginId);
                Context.Response.Cookies["User-Login-Id"].Expires = DateTime.MaxValue;

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 本地用户验证
        /// </summary>
        private int LocalUser(string LoginId, string Password)
        {
            var PasswordMD5 = Base.Common.StringHash(Password, "MD5");

            var Query = "";

            Query += "Exists (";

            // 用户账号查询
            Query += "Select A.DS_Id From DS_User As A Where " +
                     "A.DS_Id = DS_User.DS_Id And " +
                     "A.DS_Username = '" + LoginId + "' And " +
                     "A.DS_Password = '" + PasswordMD5 + "' And " +
                     "A.DS_Freeze = 0 And " +
                     "A.DS_Recycle = 0 Union All ";

            // 电子邮箱查询
            Query += "Select B.DS_Id From DS_User As B Where " +
                     "B.DS_Id = DS_User.DS_Id And " +
                     "B.DS_Email = '" + LoginId + "' And " +
                     "B.DS_Password = '" + PasswordMD5 + "' And " +
                     "B.DS_Freeze = 0 And " +
                     "B.DS_Recycle = 0 Union All ";

            // 手机号码查询
            Query += "Select C.DS_Id From DS_User As C Where " +
                     "C.DS_Id = DS_User.DS_Id And " +
                     "C.DS_Phone = '" + LoginId + "' And " +
                     "C.DS_Password = '" + PasswordMD5 + "' And " +
                     "C.DS_Freeze = 0 And " +
                     "C.DS_Recycle = 0";

            Query += ")";

            var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Base.Data.SqlDataToTable("Select DS_Id, DS_Username, DS_Password, DS_Email, DS_Phone, DS_Admin, DS_Freeze, DS_Recycle From DS_User Where " + Query + "", ref Conn, ref UserTable);

            var UserId = 0;

            if (UserTable["Exists"].TypeBool() == true)
            {
                UserId = UserTable["DS_Id"].TypeInt();
            }

            UserTable.Clear();

            return UserId;
        }


    }


}
