using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;
using dboxShare.Sync;


namespace dboxShare.Sync
{


    public class FolderListXml : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Sync") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            ListDataToXml(Context);

            Conn.Close();
            Conn.Dispose();
        }


        private void ListDataToXml(HttpContext Context)
        {
            List<Hashtable> FolderList = new List<Hashtable>();
            ArrayList XmlList = new ArrayList();
            string Query = "";
            string Sql = "";
            int Index = 0;

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_Folder = 1 And " + 
                     "A1.DBS_Recycle = 0 And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_Folder = 1 And " + 
                     "A2.DBS_Recycle = 0 And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";
            
            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_Id Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_Id Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_Id Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Share, DBS_Recycle From DBS_File Where " + Query + " Order By DBS_Name Asc, DBS_Id Asc";

            Base.Data.SqlListToTable(Sql, ref Conn, ref FolderList);

            for (Index = 0; Index < FolderList.Count; Index++)
            {
                XmlList.Add("<item>");
                XmlList.Add("<id>" + FolderList[Index]["DBS_Id"].TypeString() + "</id>");
                XmlList.Add("<folderId>" + FolderList[Index]["DBS_FolderId"].TypeString() + "</folderId>");
                XmlList.Add("<name>" + FolderList[Index]["DBS_Name"].TypeString() + "</name>");
                XmlList.Add("</item>");
            }

            Context.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><folders>" + string.Join("", XmlList.ToArray()) + "</folders>");
        }


    }


}
