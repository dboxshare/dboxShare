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


    public class Account_ForgotPasswordController : ApiController
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
        /// 获取验证码
        /// </summary>
        [Route("api/account/user-forgot/get-vericode")]
        [HttpPost]
        public HttpResponseMessage GetVericode()
        {
            var Username = Context.Request.Form["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Email = Context.Request.Form["Email"].TypeString();

            if (Base.Common.StringCheck(Email, @"^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var LockId = "Forgot-Lock-IP-" + Base.Common.ClientIP() + "";

            if (Context.Cache[LockId].TypeInt() == 5)
            {
                return AppCommon.ResponseMessage("forgot-lock-ip");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Username, DS_Password, DS_Email, DS_Freeze, DS_Recycle From DS_User Where DS_Username = '" + Username + "' And DS_Email = '" + Email + "' And DS_Freeze = 0 And DS_Recycle = 0", ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    if (Base.Common.IsNothing(Context.Cache[LockId]) == true)
                    {
                        Context.Cache.Insert(LockId, 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }
                    else
                    {
                        Context.Cache.Insert(LockId, Context.Cache[LockId].TypeInt() + 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }

                    return AppCommon.ResponseMessage("username-email-error");
                }

                UserTable.Clear();

                var Vericode = new Random().Next(100000, 999999).ToString();

                Context.Session["Username"] = Username;
                Context.Session["Vericode"] = Vericode;

                VericodeMail(Username, Email, Vericode);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 重设登录密码
        /// </summary>
        [Route("api/account/user-forgot/reset-password")]
        [HttpPost]
        public HttpResponseMessage ResetPassword()
        {
            var Vericode = Context.Request.Form["Vericode"].TypeString();

            if (Context.Session["Vericode"].TypeString() != Vericode)
            {
                return AppCommon.ResponseMessage("vericode-error");
            }

            var Username = Context.Session["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Username, DS_Freeze, DS_Recycle From DS_User Where DS_Username = '" + Username + "' And DS_Freeze = 0 And DS_Recycle = 0", ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("user-unexist");
                }

                UserTable.Clear();

                var PasswordMD5 = Base.Common.StringHash(Password, "MD5");

                Base.Data.SqlQuery("Update DS_User Set DS_Password = '" + PasswordMD5 + "' Where DS_Username = '" + Username + "'", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 发送验证码邮件
        /// </summary>
        private void VericodeMail(string Username, string Email, string Vericode)
        {
            var MailTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            AppCommon.XmlDataReader(Base.Common.PathCombine(AppConfig.DataStoragePath, "mail-template.xml"), "/template/forgot-password", MailTable);

            if (MailTable.Count == 0)
            {
                return;
            }

            var Subject = MailTable["Subject"].TypeString();

            Subject = Subject.Replace("{@appname}", AppConfig.AppName);
            Subject = Subject.Replace("{@username}", Username);

            var Content = MailTable["Content"].TypeString();

            Content = Content.Replace("{@appname}", AppConfig.AppName);
            Content = Content.Replace("{@username}", Username);
            Content = Content.Replace("{@vericode}", Vericode);

            Base.Common.SendMail(AppConfig.AppName, AppConfig.MailAddress, AppConfig.MailUsername, AppConfig.MailPassword, AppConfig.MailServer, AppConfig.MailServerPort, AppConfig.MailServerSSL, Email, Subject, Content, true);
        }


    }


}
