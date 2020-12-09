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


    public class CardDataJson : IHttpHandler, IReadOnlySessionState
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

            DataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取用户数据记录返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            string Username = "";
            string Sql = "";
            string Json = "";

            Username = Context.Request.QueryString["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return;
            }

            Sql = "Select DBS_Id, DBS_DepartmentId, DBS_RoleId, DBS_Username, DBS_Code, DBS_Position, DBS_Email, DBS_Phone, DBS_Tel, DBS_Admin, DBS_Status, DBS_LoginIP, DBS_LoginTime From DBS_User Where DBS_Username = '" + Username + "'";

            Json = Base.Data.SqlDataToJson(Sql, ref Conn);

            Context.Response.Write(Json);
        }


    }


}