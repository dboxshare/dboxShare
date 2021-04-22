using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Sync.Controllers
{


    public class SyncFolderListXmlController : ApiController
    {


        private dynamic Conn;


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        [Route("api/sync/folder-list-xml")]
        [HttpGet]
        public HttpResponseMessage FolderListXml()
        {
            if (AppCommon.LoginAuth("PC") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Folder = 1 And " +
                     "A1.DS_Recycle = 0 And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_Folder = 1 And " +
                     "A2.DS_Recycle = 0 And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B.DS_Id Where " +
                     "DS_File.DS_Id = B.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'editor' And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";
            Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B.DS_Id Where " +
                     "DS_File.DS_Id = B.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'manager' And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_Id Where " +
                     "DS_File.DS_Id = C.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'editor' And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_Id Where " +
                     "DS_File.DS_Id = C.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'manager' And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_Id Where " +
                     "DS_File.DS_Id = D.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'editor' And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_Id Where " +
                     "DS_File.DS_Id = D.DS_Id And " +
                     "DS_File.DS_Folder = 1 And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_Purview = 'manager' And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            string Sql = "Select DS_Id, DS_Folder, DS_FolderId, DS_Name, DS_Share, DS_Recycle From DS_File Where " + Query + " Order By DS_Name Asc, DS_Id Asc";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                List<Hashtable> FolderList = new List<Hashtable>();

                Base.Data.SqlListToTable(Sql, ref Conn, ref FolderList);

                ArrayList XmlList = new ArrayList();

                for (int i = 0; i < FolderList.Count; i++)
                {
                    XmlList.Add("<item>");
                    XmlList.Add("<id>" + FolderList[i]["DS_Id"].TypeString() + "</id>");
                    XmlList.Add("<folderId>" + FolderList[i]["DS_FolderId"].TypeString() + "</folderId>");
                    XmlList.Add("<name>" + FolderList[i]["DS_Name"].TypeString() + "</name>");
                    XmlList.Add("</item>");
                }

                return AppCommon.ResponseMessage("<?xml version=\"1.0\" encoding=\"utf-8\"?><folders>" + string.Join("", XmlList.ToArray()) + "</folders>");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
