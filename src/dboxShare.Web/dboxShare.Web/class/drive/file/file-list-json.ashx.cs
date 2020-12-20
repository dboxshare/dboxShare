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


    public class FileListJson : IHttpHandler, IReadOnlySessionState
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
        /// 读取文件数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            bool DBFilter = true;
            int FolderId = 0;
            string FolderPath = "";
            string Type = "";
            string Size = "";
            string Time = "";
            int Day = 0;
            string Keyword = "";
            int Page = 0;
            string Query = "";
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["FolderId"]) == true)
            {
                FolderId = Context.Request.QueryString["FolderId"].TypeInt();
            }

            if (FolderId > 0)
            {
                FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);
            }

            Type = Context.Request.QueryString["Type"].TypeString();

            if (string.IsNullOrEmpty(Type) == false)
            {
                if (Base.Common.StringCheck(Type, @"^\w+$") == false)
                {
                    return;
                }
            }

            Size = Context.Request.QueryString["Size"].TypeString();

            if (string.IsNullOrEmpty(Size) == false)
            {
                if (Base.Common.StringCheck(Size, @"^[\w\-]+$") == false)
                {
                    return;
                }
            }

            Time = Context.Request.QueryString["Time"].TypeString();

            if (string.IsNullOrEmpty(Time) == false)
            {
                if (Base.Common.StringCheck(Time, @"^[\w\-]+$") == false)
                {
                    return;
                }
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

            if (string.IsNullOrEmpty(Keyword) == true)
            {
                Query += "Exists (";

                // 所有者查询
                Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                         "A1.DBS_Id = DBS_File.DBS_Id And " + 
                         "A1.DBS_VersionId = 0 And " + 
                         "A1.DBS_FolderId = " + FolderId + " And " + 
                         "A1.DBS_Recycle = 0 And " + 
                         "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                // 创建者查询
                Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                         "A2.DBS_Id = DBS_File.DBS_Id And " + 
                         "A2.DBS_VersionId = 0 And " + 
                         "A2.DBS_FolderId = " + FolderId + " And " + 
                         "A2.DBS_Recycle = 0 And " + 
                         "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                if (FolderId == 0)
                {
                    // 共享部门查询(根目录)
                    Query += "Select B1.DBS_Id From DBS_File As B1 Inner Join DBS_File_Purview On " + 
                             "DBS_File_Purview.DBS_FileId = B1.DBS_Id Where " + 
                             "DBS_File.DBS_Id = B1.DBS_Id And " + 
                             "DBS_File.DBS_VersionId = 0 And " + 
                             "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                             "DBS_File.DBS_Share = 1 And " + 
                             "DBS_File.DBS_Recycle = 0 And " + 
                             "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";
                }

                // 共享部门查询(下级目录)
                Query += "Select B2.DBS_Id From DBS_File As B2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = B2.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = B2.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                if (FolderId == 0)
                {
                    // 共享角色查询(根目录)
                    Query += "Select C1.DBS_Id From DBS_File As C1 Inner Join DBS_File_Purview On " + 
                             "DBS_File_Purview.DBS_FileId = C1.DBS_Id Where " + 
                             "DBS_File.DBS_Id = C1.DBS_Id And " + 
                             "DBS_File.DBS_VersionId = 0 And " + 
                             "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                             "DBS_File.DBS_Share = 1 And " + 
                             "DBS_File.DBS_Recycle = 0 And " + 
                             "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";
                }

                // 共享角色查询(下级目录)
                Query += "Select C2.DBS_Id From DBS_File As C2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = C2.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = C2.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                if (FolderId == 0)
                {
                    // 共享用户查询(根目录)
                    Query += "Select D1.DBS_Id From DBS_File As D1 Inner Join DBS_File_Purview On " + 
                             "DBS_File_Purview.DBS_FileId = D1.DBS_Id Where " + 
                             "DBS_File.DBS_Id = D1.DBS_Id And " + 
                             "DBS_File.DBS_VersionId = 0 And " + 
                             "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                             "DBS_File.DBS_Share = 1 And " + 
                             "DBS_File.DBS_Recycle = 0 And " + 
                             "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";
                }

                // 共享用户查询(下级目录)
                Query += "Select D2.DBS_Id From DBS_File As D2 Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = D2.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = D2.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

                Query += ") And ";
            }
            else
            {
                if (FolderId > 0)
                {
                    Query += "DBS_FolderPath Like '" + FolderPath + "%' And ";
                }

                Query += "DBS_Name Like '%" + Keyword + "%' And ";

                Query += "Exists (";

                // 所有者查询
                Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                         "A1.DBS_Id = DBS_File.DBS_Id And " + 
                         "A1.DBS_VersionId = 0 And " + 
                         "A1.DBS_Folder = 0 And " + 
                         "A1.DBS_Recycle = 0 And " + 
                         "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                // 创建者查询
                Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                         "A2.DBS_Id = DBS_File.DBS_Id And " + 
                         "A2.DBS_VersionId = 0 And " + 
                         "A2.DBS_Folder = 0 And " + 
                         "A2.DBS_Recycle = 0 And " + 
                         "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                // 共享部门查询
                Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = B.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = B.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_Folder = 0 And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                // 共享角色查询
                Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = C.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = C.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_Folder = 0 And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                // 共享用户查询
                Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                         "DBS_File_Purview.DBS_FileId = D.DBS_FolderId Where " + 
                         "DBS_File.DBS_Id = D.DBS_Id And " + 
                         "DBS_File.DBS_VersionId = 0 And " + 
                         "DBS_File.DBS_Folder = 0 And " + 
                         "DBS_File.DBS_Share = 1 And " + 
                         "DBS_File.DBS_Recycle = 0 And " + 
                         "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

                Query += ") And ";
            }

            // 判断是否使用数据库查询过滤
            if (DBFilter == true)
            {
                if (string.IsNullOrEmpty(Type) == false)
                {
                    Query += "DBS_Type = '" + Type + "' And ";
                }

                if (string.IsNullOrEmpty(Size) == false)
                {
                    switch (Size)
                    {
                        case "0kb-100kb":
                            Query += "DBS_Size < " + (100 * 1024) + " And ";
                            break;

                        case "100kb-500kb":
                            Query += "DBS_Size > " + (100 * 1024) + " And ";
                            Query += "DBS_Size < " + (500 * 1024) + " And ";
                            break;

                        case "500kb-1mb":
                            Query += "DBS_Size > " + (500 * 1024) + " And ";
                            Query += "DBS_Size < " + (1024 * 1024) + " And ";
                            break;

                        case "1mb-5mb":
                            Query += "DBS_Size > " + (1024 * 1024) + " And ";
                            Query += "DBS_Size < " + (5 * 1024 * 1024) + " And ";
                            break;

                        case "5mb-10mb":
                            Query += "DBS_Size > " + (5 * 1024 * 1024) + " And ";
                            Query += "DBS_Size < " + (10 * 1024 * 1024) + " And ";
                            break;

                        case "10mb-50mb":
                            Query += "DBS_Size > " + (10 * 1024 * 1024) + " And ";
                            Query += "DBS_Size < " + (50 * 1024 * 1024) + " And ";
                            break;

                        case "50mb-100mb":
                            Query += "DBS_Size > " + (50 * 1024 * 1024) + " And ";
                            Query += "DBS_Size < " + (100 * 1024 * 1024) + " And ";
                            break;

                        case "100mb-more":
                            Query += "DBS_Size > " + (100 * 1024 * 1024) + " And ";
                            break;
                    }
                }

                if (string.IsNullOrEmpty(Time) == false)
                {
                    Day = (int)DateTime.Now.DayOfWeek;

                    switch (Time)
                    {
                        case "this-day":
                            Query += "DBS_CreateTime > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-day-ago":
                            Query += "DBS_CreateTime > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DBS_CreateTime < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-day-ago":
                            Query += "DBS_CreateTime > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DBS_CreateTime < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-week":
                            Query += "DBS_CreateTime > '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-week-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.AddDays(((7 - Day) - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-week-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.AddDays(((7 - Day) - 7 - 7) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-month":
                            Query += "DBS_CreateTime > '" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/1 00:00:00' And ";
                            break;

                        case "1-month-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/1 00:00:00' And ";
                            break;

                        case "2-month-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.AddMonths(-1).Month.ToString() + "/1 00:00:00' And ";
                            break;

                        case "this-year":
                            Query += "DBS_CreateTime > '" + DateTime.Now.Year.ToString() + "/1/1 00:00:00' And ";
                            break;

                        case "1-year-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.Year.ToString() + "/1/1 00:00:00' And ";
                            break;

                        case "2-year-ago":
                            Query += "DBS_CreateTime < '" + DateTime.Now.AddYears(-1).Year.ToString() + "/1/1 00:00:00' And ";
                            break;
                    }
                }
            }

            Query = Query.Substring(0, Query.Length - 5);

            Json = Base.Data.SqlPageToJson("DBS_File", "DBS_Id, DBS_UserId, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Size, DBS_Type, DBS_Remark, DBS_Share, DBS_Lock, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime", "DBS_Folder Desc, DBS_Name Asc, DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
