using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class PurviewListJson : IHttpHandler, IReadOnlySessionState
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

            ListDataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 读取权限数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            int Id = 0;
            string Sql = "";
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Sql = "Select DBS_FileId, DBS_DepartmentId, DBS_RoleId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_FileId = " + Id + " Order By DBS_DepartmentId Asc, DBS_RoleId Asc, DBS_UserId Asc";

            Json = Base.Data.SqlListToJson(Sql, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
