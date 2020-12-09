using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class DepartmentDataJson : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
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
        /// 读取部门数据记录返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            int Id = 0;
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Json = Base.Data.SqlDataToJson("Select DBS_Id, DBS_DepartmentId, DBS_Name From DBS_Department Where DBS_Id = " + Id, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
