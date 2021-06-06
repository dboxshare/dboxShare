using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class Admin_UserListJsonController : ApiController
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
        /// 读取用户数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/admin/user/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var DepartmentId = Context.Request.QueryString["DepartmentId"].TypeInt();

            var RoleId = Context.Request.QueryString["RoleId"].TypeInt();

            var Status = Context.Request.QueryString["Status"].TypeString();

            var Keyword = Context.Request.QueryString["Keyword"].TypeString();

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Keyword = Base.Common.InputFilter(Keyword);
            }

            var Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Query = "";

                if (DepartmentId > 0)
                {
                    var DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);

                    Query += "DS_DepartmentId Like '" + DepartmentIdPath + "%' And ";
                }

                if (RoleId > 0)
                {
                    Query += "DS_RoleId = " + RoleId + " And ";
                }

                if (string.IsNullOrEmpty(Status) == false)
                {
                    Query += "DS_Freeze = " + (Status == "on-job" ? 0 : 1) + " And ";
                }

                if (string.IsNullOrEmpty(Keyword) == false)
                {
                    Query += "Exists (";

                    // 用户账号查询
                    Query += "Select A.DS_Id From DS_User As A Where " +
                             "A.DS_Id = DS_User.DS_Id And " +
                             "A.DS_Username = '" + Keyword + "' Union All ";

                    // 电子邮箱查询
                    Query += "Select B.DS_Id From DS_User As B Where " +
                             "B.DS_Id = DS_User.DS_Id And " +
                             "B.DS_Email = '" + Keyword + "' Union All ";

                    // 手机号码查询
                    Query += "Select C.DS_Id From DS_User As C Where " +
                             "C.DS_Id = DS_User.DS_Id And " +
                             "C.DS_Phone = '" + Keyword + "'";

                    Query += ") And ";
                }

                Query += "DS_Recycle = 0";

                var Json = Base.Data.SqlPageToJson("DS_User", "DS_Id, DS_DepartmentId, DS_RoleId, DS_Username, DS_Title, DS_Email, DS_Phone, DS_UploadSize, DS_DownloadSize, DS_Freeze, DS_Recycle", "DS_Username Asc, DS_Id Desc", Query, 50, Page, ref Conn);

                return AppCommon.ResponseMessage(Json);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
