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


    public class Login : IHttpHandler, IRequiresSessionState
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
            if (string.IsNullOrEmpty(Context.Session["UserId"].TypeString()) == false)
            {
                Context.Response.Write("logged-in");
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            ProcessLogin(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        private void ProcessLogin(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            string SecurityKey = "";
            string LoginId = "";
            string Password = "";
            string LockId = "";
            int UserId = 0;
            string Username = "";
            string DepartmentId = "";
            int RoleId = 0;
            int Admin = 0;
            string Query = "";

            SecurityKey = ConfigurationManager.AppSettings["SecurityKey"].TypeString();

            LoginId = Context.Request.Form["LoginId"].TypeString();

            if (string.IsNullOrEmpty(LoginId) == true)
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

            LockId = "Login-Lock-IP-" + Base.Common.ClientIP() + "";

            if (Context.Cache[LockId].TypeInt() == 5)
            {
                Context.Response.Write("login-lock-ip");
                return;
            }

            Query += "Exists (";

            // 用户账号查询
            Query += "Select A.DBS_Id From DBS_User As A Where " + 
                     "A.DBS_Id = DBS_User.DBS_Id And " + 
                     "A.DBS_Username = '" + LoginId + "' And " + 
                     "A.DBS_Password = '" + Password + "' And " + 
                     "A.DBS_Status = 1 And " + 
                     "A.DBS_Recycle = 0 Union All ";

            // 电子邮箱查询
            Query += "Select B.DBS_Id From DBS_User As B Where " + 
                     "B.DBS_Id = DBS_User.DBS_Id And " + 
                     "B.DBS_Email = '" + LoginId + "' And " + 
                     "B.DBS_Password = '" + Password + "' And " + 
                     "B.DBS_Status = 1 And " + 
                     "B.DBS_Recycle = 0 Union All ";

            // 手机号码查询
            Query += "Select C.DBS_Id From DBS_User As C Where " + 
                     "C.DBS_Id = DBS_User.DBS_Id And " + 
                     "C.DBS_Phone = '" + LoginId + "' And " + 
                     "C.DBS_Password = '" + Password + "' And " + 
                     "C.DBS_Status = 1 And " + 
                     "C.DBS_Recycle = 0";

            Query += ")";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_DepartmentId, DBS_RoleId, DBS_Username, DBS_Password, DBS_Email, DBS_Phone, DBS_Admin, DBS_Status, DBS_Recycle From DBS_User Where " + Query + "", ref Conn, ref UserTable);

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

                Context.Response.Write("failure");
                return;
            }
            else
            {
                UserId = UserTable["DBS_Id"].TypeInt();
                Username = UserTable["DBS_Username"].TypeString();
                DepartmentId = UserTable["DBS_DepartmentId"].TypeString();
                RoleId = UserTable["DBS_RoleId"].TypeInt();
                Admin = UserTable["DBS_Admin"].TypeInt();
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Update DBS_User Set DBS_LoginIP = '" + Base.Common.ClientIP() + "', DBS_LoginTime = '" + DateTime.Now.ToString() + "' Where DBS_Id = " + UserId, ref Conn);

            Context.Session["UserId"] = UserId;
            Context.Session["Username"] = Username;
            Context.Session["DepartmentId"] = DepartmentId;
            Context.Session["RoleId"] = RoleId == 0 ? -1 : RoleId;
            Context.Session["Admin"] = Admin;
            Context.Session["LoginToken"] = Base.Crypto.TextEncrypt(Context.Session.SessionID, SecurityKey);

            Context.Cache.Insert("Login-Token-Web-" + UserId.ToString() + "", Context.Session["LoginToken"].TypeString(), null, DateTime.MaxValue, TimeSpan.FromHours(12));

            Context.Response.Cookies["User-Login-Id"].Value = Context.Server.UrlEncode(LoginId);
            Context.Response.Cookies["User-Login-Id"].Expires = DateTime.MaxValue;

            Context.Response.Write("complete");
        }


    }


}
