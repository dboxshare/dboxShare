using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.User
{


    public class ForgotPassword : IHttpHandler, IRequiresSessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        private dynamic Conn;


        public void ProcessRequest(HttpContext Context)
        {
            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            switch (Context.Request.QueryString["Action"].TypeString())
            {
                case "get-vericode":
                    GetVericode(Context);
                    break;

                case "reset-password":
                    ResetPassword(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 获取验证码
        /// </summary>
        private void GetVericode(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            string Username = "";
            string Email = "";
            string LockId = "";
            string Vericode = "";

            Username = Context.Request.Form["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return;
            }

            Email = Context.Request.Form["Email"].TypeString();

            if (Base.Common.StringCheck(Email, @"^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
            {
                return;
            }

            LockId = "Forgot-Lock-IP-" + Username + "";

            if (Context.Cache[LockId].TypeInt() == 5)
            {
                Context.Response.Write("forgot-lock-ip");
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Username, DBS_Password, DBS_Email, DBS_Status, DBS_Recycle From DBS_User Where DBS_Username = '" + Username + "' And DBS_Email = '" + Email + "' And DBS_Status = 1 And DBS_Recycle = 0", ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                if (Base.Common.IsNothing(Context.Cache[LockId]) == true)
                {
                    Context.Cache.Insert(LockId, 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                }
                else
                {
                    Context.Cache.Insert(LockId, Context.Cache[LockId].TypeInt() + 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                }

                Context.Response.Write("user-info-error");
                return;
            }

            UserTable.Clear();

            Vericode = new Random().Next(100000, 999999).ToString();

            Context.Session["Username"] = Username;
            Context.Session["Vericode"] = Vericode;

            VericodeMail(Username, Email, Vericode, Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 重设登录密码
        /// </summary>
        private void ResetPassword(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            string Username = "";
            string Password = "";
            string Vericode = "";

            Vericode = Context.Request.Form["Vericode"].TypeString();

            if (Context.Session["Vericode"].TypeString() != Vericode)
            {
                Context.Response.Write("vericode-error");
                return;
            }

            Username = Context.Session["Username"].TypeString();

            if (string.IsNullOrEmpty(Username) == true)
            {
                return;
            }

            Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\S]{6,16}$") == true)
            {
                Password = Base.Common.StringCrypto(Password, "MD5");
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Username, DBS_Status, DBS_Recycle From DBS_User Where DBS_Username = '" + Username + "' And DBS_Status = 1 And DBS_Recycle = 0", ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                Context.Response.Write("user-unexist");
                return;
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Update DBS_User Set DBS_Password = '" + Password + "' Where DBS_Username = '" + Username + "'", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 发送验证码邮件
        /// </summary>
        private void VericodeMail(string Username, string Email, string Vericode, HttpContext Context)
        {
            Hashtable MailTable = new Hashtable();
            string AppName = "";
            string Subject = "";
            string Content = "";

            AppCommon.XmlDataReader(Context.Server.MapPath("/storage/config/mail-template.xml"), "/template/forgot-password", MailTable);

            if (MailTable.Count == 0)
            {
                return;
            }

            AppName = ConfigurationManager.AppSettings["AppName"].TypeString();

            Subject = MailTable["subject"].TypeString();
            Subject = Subject.Replace("{appname}", AppName);
            Subject = Subject.Replace("{username}", Username);

            Content = MailTable["content"].TypeString();
            Content = Content.Replace("{appname}", AppName);
            Content = Content.Replace("{username}", Username);
            Content = Content.Replace("{vericode}", Vericode);

            AppCommon.SendMail(Email, Subject, Content);
        }


    }


}
