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


    public class DepartmentListJson : IHttpHandler, IReadOnlySessionState
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

            ListDataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取部门数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            int DepartmentId = 0;
            string Sql = "";
            string Json = "";

            DepartmentId = Context.Request.QueryString["DepartmentId"].TypeInt();

            Sql = "Select DBS_Id, DBS_DepartmentId, DBS_Name, DBS_Sequence From DBS_Department Where DBS_DepartmentId = " + DepartmentId + " Order By DBS_Sequence Desc, DBS_Id Asc";

            Json = Base.Data.SqlListToJson(Sql, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
