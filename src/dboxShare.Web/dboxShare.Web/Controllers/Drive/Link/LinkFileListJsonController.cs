using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_LinkFileListJsonController : ApiController
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
        /// 读取分享文件数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/link/file-list-json")]
        [HttpGet]
        public HttpResponseMessage FileListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeString();

            if (Base.Common.StringCheck(Id, @"^([\d]+[\,]?)+$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Query = "DS_Id In (" + Id + ") And ";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Lock = 0 And " +
                     "A1.DS_Recycle = 0 And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_Lock = 0 And " +
                     "A2.DS_Recycle = 0 And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B1.DS_Id From DS_File As B1 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B1.DS_Id And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' And " +
                     "DS_File_Purview.DS_Purview = 'editor' Where " +
                     "B1.DS_Id = DS_File.DS_Id And " +
                     "B1.DS_Share = 1 And " +
                     "B1.DS_Lock = 0 And " +
                     "B1.DS_Recycle = 0 Union All ";

            Query += "Select B2.DS_Id From DS_File As B2 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B2.DS_Id And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' And " +
                     "DS_File_Purview.DS_Purview = 'manager' Where " +
                     "B2.DS_Id = DS_File.DS_Id And " +
                     "B2.DS_Share = 1 And " +
                     "B2.DS_Lock = 0 And " +
                     "B2.DS_Recycle = 0 Union All ";

            // 共享角色查询
            Query += "Select C1.DS_Id From DS_File As C1 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C1.DS_Id And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " And " +
                     "DS_File_Purview.DS_Purview = 'editor' Where " +
                     "C1.DS_Id = DS_File.DS_Id And " +
                     "C1.DS_Share = 1 And " +
                     "C1.DS_Lock = 0 And " +
                     "C1.DS_Recycle = 0 Union All ";

            Query += "Select C2.DS_Id From DS_File As C2 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C2.DS_Id And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " And " +
                     "DS_File_Purview.DS_Purview = 'manager' Where " +
                     "C2.DS_Id = DS_File.DS_Id And " +
                     "C2.DS_Share = 1 And " +
                     "C2.DS_Lock = 0 And " +
                     "C2.DS_Recycle = 0 Union All ";

            // 共享用户查询
            Query += "Select D1.DS_Id From DS_File As D1 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D1.DS_Id And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " And " +
                     "DS_File_Purview.DS_Purview = 'editor' Where " +
                     "D1.DS_Id = DS_File.DS_Id And " +
                     "D1.DS_Share = 1 And " +
                     "D1.DS_Lock = 0 And " +
                     "D1.DS_Recycle = 0 Union All ";

            Query += "Select D2.DS_Id From DS_File As D2 Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D2.DS_Id And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " And " +
                     "DS_File_Purview.DS_Purview = 'manager' Where " +
                     "D2.DS_Id = DS_File.DS_Id And " +
                     "D2.DS_Share = 1 And " +
                     "D2.DS_Lock = 0 And " +
                     "D2.DS_Recycle = 0";

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Sql = "Select DS_Id, DS_UserId, DS_Version, DS_Folder, DS_Name, DS_Extension From DS_File Where " + Query + " Order By DS_Folder Desc, DS_Name Asc, DS_Id Desc";

                var Json = Base.Data.SqlListToJson(Sql, ref Conn);

                return AppCommon.ResponseMessage(Json);
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 读取分享文件数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/link/share-file-list-json")]
        [HttpGet]
        public HttpResponseMessage ShareFileListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            var Query = "";

            Query += "Exists (";

            // 链接分享文件查询
            Query += "Select A.DS_Id From DS_File As A Inner Join DS_Link_File On " +
                     "DS_Link_File.DS_FileId = A.DS_Id And " +
                     "DS_Link_File.DS_LinkId = " + Id + " Where " +
                     "A.DS_Id = DS_File.DS_Id";

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Sql = "Select DS_Id, DS_UserId, DS_Version, DS_Folder, DS_Name, DS_Extension From DS_File Where " + Query + " Order By DS_Folder Desc, DS_Name Asc, DS_Id Desc";

                var Json = Base.Data.SqlListToJson(Sql, ref Conn);

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
