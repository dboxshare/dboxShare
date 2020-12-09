using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class ActivityFlowListJson : IHttpHandler, IReadOnlySessionState
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
        /// 读取日志数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            string Type = "";
            int Page = 0;
            string Query = "";
            string Json = "";

            Type = Context.Request.QueryString["Type"].TypeString();

            if (string.IsNullOrEmpty(Type) == false)
            {
                if (Base.Common.StringCheck(Type, @"^(created|shared)$") == false)
                {
                    return;
                }
            }

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            if (Page > 50)
            {
                return;
            }

            Query += "Exists (";

            if (string.IsNullOrEmpty(Type) == true || Type == "created")
            {
                Query += "Select A.DBS_Id From DBS_Log As A Inner Join DBS_File On " + 
                         "DBS_File.DBS_Id = A.DBS_FileId Where " + 
                         "A.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "'";
            }

            if (string.IsNullOrEmpty(Type) == true)
            {
                Query += " Union All ";
            }

            if (string.IsNullOrEmpty(Type) == true || Type == "shared")
            {
                Query += "Select B1.DBS_Id From DBS_Log As B1 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = B1.DBS_FileId Where " + 
                         "B1.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                Query += "Select B2.DBS_Id From DBS_Log As B2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = (Select B3.DBS_FolderId From DBS_File As B3 Where B3.DBS_Id = B2.DBS_FileId) Where " + 
                         "B2.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                Query += "Select C1.DBS_Id From DBS_Log As C1 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = C1.DBS_FileId Where " + 
                         "C1.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                Query += "Select C2.DBS_Id From DBS_Log As C2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = (Select C3.DBS_FolderId From DBS_File As C3 Where C3.DBS_Id = C2.DBS_FileId) Where " + 
                         "C2.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                Query += "Select D1.DBS_Id From DBS_Log As D1 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = D1.DBS_FileId Where " + 
                         "D1.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                Query += "Select D2.DBS_Id From DBS_Log As D2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = (Select D3.DBS_FolderId From DBS_File As D3 Where D3.DBS_Id = D2.DBS_FileId) Where " + 
                         "D2.DBS_Id = DBS_Log.DBS_Id And " + 
                         "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";
            }

            Query += ")";

            Json = Base.Data.SqlPageToJson("DBS_Log", "DBS_Id, DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_FileVersion, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Action, DBS_IP, DBS_Time", "DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
