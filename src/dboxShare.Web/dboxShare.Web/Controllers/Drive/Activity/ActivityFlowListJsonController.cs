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


    public class ActivityFlowListJsonController : ApiController
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
        /// 读取日志数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/activity/flow-list-json")]
        [HttpGet]
        public HttpResponseMessage FlowListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Type = Context.Request.QueryString["Type"].TypeString();

            if (string.IsNullOrEmpty(Type) == false)
            {
                if (Base.Common.StringCheck(Type, @"^(created|shared)$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            if (Page > 50)
            {
                return AppCommon.ResponseMessage("limit 50 pages");
            }

            string Query = "";

            Query += "Exists (";

            if (string.IsNullOrEmpty(Type) == true || Type == "created")
            {
                Query += "Select A.DS_Id From DS_Log As A Inner Join DS_File On " +
                         "DS_File.DS_Id = A.DS_FileId Where " +
                         "A.DS_Id = DS_Log.DS_Id And " +
                         "DS_File.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "'";
            }

            if (string.IsNullOrEmpty(Type) == true)
            {
                Query += " Union All ";
            }

            if (string.IsNullOrEmpty(Type) == true || Type == "shared")
            {
                Query += "Select B1.DS_Id From DS_Log As B1 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = B1.DS_FileId Where " +
                         "B1.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                Query += "Select B2.DS_Id From DS_Log As B2 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = (Select B3.DS_FolderId From DS_File As B3 Where B3.DS_Id = B2.DS_FileId) Where " +
                         "B2.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                Query += "Select C1.DS_Id From DS_Log As C1 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = C1.DS_FileId Where " +
                         "C1.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                Query += "Select C2.DS_Id From DS_Log As C2 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = (Select C3.DS_FolderId From DS_File As C3 Where C3.DS_Id = C2.DS_FileId) Where " +
                         "C2.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                Query += "Select D1.DS_Id From DS_Log As D1 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = D1.DS_FileId Where " +
                         "D1.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                Query += "Select D2.DS_Id From DS_Log As D2 Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = (Select D3.DS_FolderId From DS_File As D3 Where D3.DS_Id = D2.DS_FileId) Where " +
                         "D2.DS_Id = DS_Log.DS_Id And " +
                         "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";
            }

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Json = Base.Data.SqlPageToJson("DS_Log", "DS_Id, DS_FileId, DS_FileName, DS_FileExtension, DS_FileVersion, DS_IsFolder, DS_UserId, DS_Username, DS_Action, DS_IP, DS_Time", "DS_Id Desc", Query, 50, Page, ref Conn);

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
