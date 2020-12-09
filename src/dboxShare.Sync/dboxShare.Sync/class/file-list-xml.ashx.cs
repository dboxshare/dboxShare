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


    public class FileListXml : IHttpHandler, IReadOnlySessionState
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
            List<Hashtable> FileList = new List<Hashtable>();
            ArrayList XmlList = new ArrayList();
            Hashtable FileTable = new Hashtable();
            int FolderId = 0;
            int FolderShare = 0;
            int FolderLock = 0;
            string FolderPurview = "";
            string Query = "";
            string Sql = "";
            int Index = 0;

            if (Base.Common.IsNumeric(Context.Request.QueryString["FolderId"]) == true)
            {
                FolderId = Context.Request.QueryString["FolderId"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Share, DBS_Lock, DBS_Recycle From DBS_File Where DBS_Folder = 1 And DBS_Recycle = 0 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                FolderShare = FileTable["DBS_Share"].TypeInt();
                FolderLock = FileTable["DBS_Lock"].TypeInt();
            }

            FileTable.Clear();

            FolderPurview = AppCommon.PurviewRole(FolderId, true, ref Conn);;

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_Folder = 0 And " + 
                     "A1.DBS_FolderId = " + FolderId + " And " + 
                     "A1.DBS_Recycle = 0 And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_Folder = 0 And " + 
                     "A2.DBS_FolderId = " + FolderId + " And " + 
                     "A2.DBS_Recycle = 0 And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Recycle = 0 And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_Id, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_CodeId, DBS_Hash, DBS_Name, DBS_Extension, DBS_Size, DBS_Share, DBS_Lock, DBS_UpdateTime From DBS_File Where " + Query + " Order By DBS_Id Desc";

            Base.Data.SqlListToTable(Sql, ref Conn, ref FileList);

            for (Index = 0; Index < FileList.Count; Index++)
            {
                XmlList.Add("<item>");
                XmlList.Add("<id>" + FileList[Index]["DBS_Id"].TypeString() + "</id>");
                XmlList.Add("<versionId>" + FileList[Index]["DBS_VersionId"].TypeString() + "</versionId>");
                XmlList.Add("<codeId>" + FileList[Index]["DBS_CodeId"].TypeString() + "</codeId>");
                XmlList.Add("<hash>" + FileList[Index]["DBS_Hash"].TypeString() + "</hash>");
                XmlList.Add("<name>" + FileList[Index]["DBS_Name"].TypeString() + "" + FileList[Index]["DBS_Extension"].TypeString() + "</name>");
                XmlList.Add("<size>" + FileList[Index]["DBS_Size"].TypeString() + "</size>");
                XmlList.Add("<lock>" + FileList[Index]["DBS_Lock"].TypeString() + "</lock>");
                XmlList.Add("<updateTime>" + FileList[Index]["DBS_UpdateTime"].TypeString() + "</updateTime>");
                XmlList.Add("</item>");
            }

            Context.Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><folder><share>" + FolderShare + "</share><lock>" + FolderLock + "</lock><purview>" + FolderPurview + "</purview></folder><files>" + string.Join("", XmlList.ToArray()) + "</files></root>");
        }


    }


}
