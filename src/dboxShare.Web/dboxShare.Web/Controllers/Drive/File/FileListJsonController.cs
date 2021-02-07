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


    public class FileListJsonController : ApiController
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
        /// 读取文件数据列表返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/file/list-json")]
        [HttpGet]
        public HttpResponseMessage ListJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int FolderId = Context.Request.QueryString["FolderId"].TypeInt();

            string Type = Context.Request.QueryString["Type"].TypeString();

            if (string.IsNullOrEmpty(Type) == false)
            {
                if (Base.Common.StringCheck(Type, @"^\w+$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string Size = Context.Request.QueryString["Size"].TypeString();

            if (string.IsNullOrEmpty(Size) == false)
            {
                if (Base.Common.StringCheck(Size, @"^[\w\-]+$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string Time = Context.Request.QueryString["Time"].TypeString();

            if (string.IsNullOrEmpty(Time) == false)
            {
                if (Base.Common.StringCheck(Time, @"^[\w\-]+$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string Keyword = Context.Request.QueryString["Keyword"].TypeString();

            if (string.IsNullOrEmpty(Keyword) == false)
            {
                Keyword = Base.Common.InputFilter(Keyword);
            }

            int Page = Context.Request.QueryString["Page"].TypeInt();

            Page = Page < 1 ? 1 : Page;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string Query = "";

                if (string.IsNullOrEmpty(Keyword) == true)
                {
                    Query += "Exists (";

                    // 所有者查询
                    Query += "Select A1.DS_Id From DS_File As A1 Where " +
                             "A1.DS_Id = DS_File.DS_Id And " +
                             "A1.DS_VersionId = 0 And " +
                             "A1.DS_FolderId = " + FolderId + " And " +
                             "A1.DS_Recycle = 0 And " +
                             "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                    // 创建者查询
                    Query += "Select A2.DS_Id From DS_File As A2 Where " +
                             "A2.DS_Id = DS_File.DS_Id And " +
                             "A2.DS_VersionId = 0 And " +
                             "A2.DS_FolderId = " + FolderId + " And " +
                             "A2.DS_Recycle = 0 And " +
                             "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                    if (FolderId == 0)
                    {
                        // 共享部门查询(根目录)
                        Query += "Select B1.DS_Id From DS_File As B1 Inner Join DS_File_Purview On " +
                                 "DS_File_Purview.DS_FileId = B1.DS_Id Where " +
                                 "DS_File.DS_Id = B1.DS_Id And " +
                                 "DS_File.DS_VersionId = 0 And " +
                                 "DS_File.DS_FolderId = " + FolderId + " And " +
                                 "DS_File.DS_Share = 1 And " +
                                 "DS_File.DS_Recycle = 0 And " +
                                 "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";
                    }

                    // 共享部门查询(下级目录)
                    Query += "Select B2.DS_Id From DS_File As B2 Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = B2.DS_FolderId Where " +
                             "DS_File.DS_Id = B2.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_FolderId = " + FolderId + " And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                    if (FolderId == 0)
                    {
                        // 共享角色查询(根目录)
                        Query += "Select C1.DS_Id From DS_File As C1 Inner Join DS_File_Purview On " +
                                 "DS_File_Purview.DS_FileId = C1.DS_Id Where " +
                                 "DS_File.DS_Id = C1.DS_Id And " +
                                 "DS_File.DS_VersionId = 0 And " +
                                 "DS_File.DS_FolderId = " + FolderId + " And " +
                                 "DS_File.DS_Share = 1 And " +
                                 "DS_File.DS_Recycle = 0 And " +
                                 "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";
                    }

                    // 共享角色查询(下级目录)
                    Query += "Select C2.DS_Id From DS_File As C2 Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = C2.DS_FolderId Where " +
                             "DS_File.DS_Id = C2.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_FolderId = " + FolderId + " And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                    if (FolderId == 0)
                    {
                        // 共享用户查询(根目录)
                        Query += "Select D1.DS_Id From DS_File As D1 Inner Join DS_File_Purview On " +
                                 "DS_File_Purview.DS_FileId = D1.DS_Id Where " +
                                 "DS_File.DS_Id = D1.DS_Id And " +
                                 "DS_File.DS_VersionId = 0 And " +
                                 "DS_File.DS_FolderId = " + FolderId + " And " +
                                 "DS_File.DS_Share = 1 And " +
                                 "DS_File.DS_Recycle = 0 And " +
                                 "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";
                    }

                    // 共享用户查询(下级目录)
                    Query += "Select D2.DS_Id From DS_File As D2 Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = D2.DS_FolderId Where " +
                             "DS_File.DS_Id = D2.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_FolderId = " + FolderId + " And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

                    Query += ") And ";
                }
                else
                {
                    string FolderPath = "";

                    if (FolderId > 0)
                    {
                        FolderPath = AppCommon.FolderIdPath(FolderId, ref Conn);

                        Query += "DS_FolderPath Like '" + FolderPath + "%' And ";
                    }

                    Query += "DS_Name Like '%" + Keyword + "%' And ";

                    Query += "Exists (";

                    // 所有者查询
                    Query += "Select A1.DS_Id From DS_File As A1 Where " +
                             "A1.DS_Id = DS_File.DS_Id And " +
                             "A1.DS_VersionId = 0 And " +
                             "A1.DS_Folder = 0 And " +
                             "A1.DS_Recycle = 0 And " +
                             "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

                    // 创建者查询
                    Query += "Select A2.DS_Id From DS_File As A2 Where " +
                             "A2.DS_Id = DS_File.DS_Id And " +
                             "A2.DS_VersionId = 0 And " +
                             "A2.DS_Folder = 0 And " +
                             "A2.DS_Recycle = 0 And " +
                             "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                    // 共享部门查询
                    Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = B.DS_FolderId Where " +
                             "DS_File.DS_Id = B.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_Folder = 0 And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

                    // 共享角色查询
                    Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = C.DS_FolderId Where " +
                             "DS_File.DS_Id = C.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_Folder = 0 And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

                    // 共享用户查询
                    Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                             "DS_File_Purview.DS_FileId = D.DS_FolderId Where " +
                             "DS_File.DS_Id = D.DS_Id And " +
                             "DS_File.DS_VersionId = 0 And " +
                             "DS_File.DS_Folder = 0 And " +
                             "DS_File.DS_Share = 1 And " +
                             "DS_File.DS_Recycle = 0 And " +
                             "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

                    Query += ") And ";
                }

                if (string.IsNullOrEmpty(Type) == false)
                {
                    Query += "DS_Type = '" + Type + "' And ";
                }

                if (string.IsNullOrEmpty(Size) == false)
                {
                    switch (Size)
                    {
                        case "0kb-100kb":
                            Query += "DS_Size > 0 And ";
                            Query += "DS_Size < " + (100 * 1024) + " And ";
                            break;

                        case "100kb-500kb":
                            Query += "DS_Size > " + (100 * 1024) + " And ";
                            Query += "DS_Size < " + (500 * 1024) + " And ";
                            break;

                        case "500kb-1mb":
                            Query += "DS_Size > " + (500 * 1024) + " And ";
                            Query += "DS_Size < " + (1024 * 1024) + " And ";
                            break;

                        case "1mb-5mb":
                            Query += "DS_Size > " + (1024 * 1024) + " And ";
                            Query += "DS_Size < " + (5 * 1024 * 1024) + " And ";
                            break;

                        case "5mb-10mb":
                            Query += "DS_Size > " + (5 * 1024 * 1024) + " And ";
                            Query += "DS_Size < " + (10 * 1024 * 1024) + " And ";
                            break;

                        case "10mb-50mb":
                            Query += "DS_Size > " + (10 * 1024 * 1024) + " And ";
                            Query += "DS_Size < " + (50 * 1024 * 1024) + " And ";
                            break;

                        case "50mb-100mb":
                            Query += "DS_Size > " + (50 * 1024 * 1024) + " And ";
                            Query += "DS_Size < " + (100 * 1024 * 1024) + " And ";
                            break;

                        case "100mb-more":
                            Query += "DS_Size > " + (100 * 1024 * 1024) + " And ";
                            break;
                    }
                }

                if (string.IsNullOrEmpty(Time) == false)
                {
                    int Day = (int)DateTime.Now.DayOfWeek;

                    switch (Time)
                    {
                        case "this-day":
                            Query += "DS_CreateTime > '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-day-ago":
                            Query += "DS_CreateTime > '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And DS_CreateTime < '" + DateTime.Now.ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-day-ago":
                            Query += "DS_CreateTime > '" + DateTime.Now.AddDays(-2).ToShortDateString() + " 00:00:00' And DS_CreateTime < '" + DateTime.Now.AddDays(-1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-week":
                            Query += "DS_CreateTime > '" + DateTime.Now.AddDays((7 - Day) - 14).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-week-ago":
                            Query += "DS_CreateTime < '" + DateTime.Now.AddDays(((7 - Day) - 14) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-week-ago":
                            Query += "DS_CreateTime < '" + DateTime.Now.AddDays(((7 - Day) - 21) + 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-month":
                            Query += "DS_CreateTime > '" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-month-ago":
                            Query += "DS_CreateTime < '" + new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-month-ago":
                            Query += "DS_CreateTime < '" + new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "this-year":
                            Query += "DS_CreateTime > '" + new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "1-year-ago":
                            Query += "DS_CreateTime < '" + new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString() + " 00:00:00' And ";
                            break;

                        case "2-year-ago":
                            Query += "DS_CreateTime < '" + new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1).ToShortDateString() + " 00:00:00' And ";
                            break;
                    }
                }

                Query = Query.Substring(0, Query.Length - 5);

                string Json = Base.Data.SqlPageToJson("DS_File", "DS_Id, DS_UserId, DS_Version, DS_VersionId, DS_Folder, DS_FolderId, DS_CodeId, DS_Name, DS_Extension, DS_Size, DS_Type, DS_Remark, DS_Share, DS_Lock, DS_Recycle, DS_CreateUsername, DS_CreateTime, DS_UpdateUsername, DS_UpdateTime", "DS_Folder Desc, DS_Name Asc, DS_Id Desc", Query, 50, Page, ref Conn);

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
