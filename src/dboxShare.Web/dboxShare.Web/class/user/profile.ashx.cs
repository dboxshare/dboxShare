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


    public class Profile : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            ProcessProfile(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 用户资料设置
        /// </summary>
        private void ProcessProfile(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;
            string Password = "";
            string Email = "";
            string Phone = "";
            string Tel = "";
            string Sql = "";

            if (Base.Common.IsNumeric(Context.Session["UserId"]) == true)
            {
                Id = Context.Session["UserId"].TypeInt();
            }
            else
            {
                return;
            }

            Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\S]{6,16}$") == true)
            {
                Password = Base.Common.StringCrypto(Password, "MD5");
            }

            Email = Context.Request.Form["Email"].TypeString();

            if (Base.Common.StringCheck(Email, @"^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
            {
                return;
            }

            Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return;
                }
            }

            Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return;
                }
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Email From DBS_User Where DBS_Email = '" + Email + "'", ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == true)
            {
                if (UserTable["DBS_Id"].TypeInt() != Id)
                {
                    Context.Response.Write("email-existed");
                    return;
                }
            }

            UserTable.Clear();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Phone From DBS_User Where DBS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == true)
                {
                    if (UserTable["DBS_Id"].TypeInt() != Id)
                    {
                        Context.Response.Write("phone-existed");
                        return;
                    }
                }

                UserTable.Clear();
            }

            if (string.IsNullOrEmpty(Password) == true)
            {
                Sql = "Update DBS_User Set DBS_Email = '" + Email + "', DBS_Phone = '" + Phone + "', DBS_Tel = '" + Tel + "' Where DBS_Id = " + Id;
            }
            else
            {
                Sql = "Update DBS_User Set DBS_Password = '" + Password + "', DBS_Email = '" + Email + "', DBS_Phone = '" + Phone + "', DBS_Tel = '" + Tel + "' Where DBS_Id = " + Id;
            }

            Base.Data.SqlQuery(Sql, ref Conn);

            Context.Response.Write("complete");
        }


    }


}
