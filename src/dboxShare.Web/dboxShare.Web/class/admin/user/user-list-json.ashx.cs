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


    public class UserListJson : IHttpHandler, IReadOnlySessionState
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
        /// 读取用户数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            int DepartmentId = 0;
            int RoleId = 0;
            string Status = "";
            string Keyword = "";
            int Page = 0;
            string Query = "";
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["DepartmentId"]) == true)
            {
                DepartmentId = Context.Request.QueryString["DepartmentId"].TypeInt();
            }

            if (Base.Common.IsNumeric(Context.Request.QueryString["RoleId"]) == true)
            {
                RoleId = Context.Request.QueryString["RoleId"].TypeInt();
            }

            Status = Context.Request.QueryString["Status"].TypeString();

            if (Base.Common.StringCheck(Status, @"^(job-on|job-off)$") == false)
            {
                Status = "";
            }
            else
            {
                Status = Status == "job-on" ? "1" : "0";
            }

            Keyword = Base.Common.InputFilter(Context.Request.QueryString["Keyword"].TypeString());

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            if (DepartmentId > 0)
            {
                Query += "DBS_DepartmentId Like '" + AppCommon.DepartmentIdPath(DepartmentId, ref Conn) + "%' And ";
            }

            if (RoleId > 0)
            {
                Query += "DBS_RoleId = " + RoleId + " And ";
            }

            if (string.IsNullOrEmpty(Status) == false)
            {
                Query += "DBS_Status = " + Status + " And ";
            }

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Query += "Exists (";

                // 用户账号查询
                Query += "Select A.DBS_Id From DBS_User As A Where " + 
                         "A.DBS_Id = DBS_User.DBS_Id And " + 
                         "A.DBS_Username = '" + Keyword + "' Union All ";

                // 电子邮箱查询
                Query += "Select B.DBS_Id From DBS_User As B Where " + 
                         "B.DBS_Id = DBS_User.DBS_Id And " + 
                         "B.DBS_Email = '" + Keyword + "' Union All ";

                // 手机号码查询
                Query += "Select C.DBS_Id From DBS_User As C Where " + 
                         "C.DBS_Id = DBS_User.DBS_Id And " + 
                         "C.DBS_Phone = '" + Keyword + "'";

                Query += ") And ";
            }

            Query += "DBS_Recycle = 0";

            Json = Base.Data.SqlPageToJson("DBS_User", "DBS_Id, DBS_DepartmentId, DBS_RoleId, DBS_Username, DBS_Position, DBS_Email, DBS_Phone, DBS_Status, DBS_Recycle", "DBS_Username Asc, DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
