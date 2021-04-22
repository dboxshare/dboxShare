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


    public class SyncFileListXmlController : ApiController
    {


        private dynamic Conn;


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        [Route("api/sync/file-list-xml")]
        [HttpGet]
        public HttpResponseMessage FileListXml()
        {
            if (AppCommon.LoginAuth("PC") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int FolderId = Context.Request.QueryString["FolderId"].TypeInt();

            string Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Folder = 0 And " +
                     "A1.DS_FolderId = " + FolderId + " And " +
                     "A1.DS_Recycle = 0 And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_Folder = 0 And " +
                     "A2.DS_FolderId = " + FolderId + " And " +
                     "A2.DS_Recycle = 0 And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = B.DS_FolderId Where " +
                     "DS_File.DS_Id = B.DS_Id And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_FolderId Where " +
                     "DS_File.DS_Id = C.DS_Id And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_FolderId Where " +
                     "DS_File.DS_Id = D.DS_Id And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_FolderId = " + FolderId + " And " +
                     "DS_File.DS_Share = 1 And " +
                     "DS_File.DS_Recycle = 0 And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            string Sql = "Select DS_Id, DS_VersionId, DS_Folder, DS_FolderId, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Share, DS_Lock, DS_UpdateTime From DS_File Where " + Query + " Order By DS_Id Desc";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock, DS_Recycle From DS_File Where DS_Folder = 1 And DS_Recycle = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int FolderShare = FileTable["DS_Share"].TypeInt();
                int FolderLock = FileTable["DS_Lock"].TypeInt();

                FileTable.Clear();

                string FolderPurview = AppCommon.PurviewRole(FolderId, true, ref Conn); ;

                List<Hashtable> FileList = new List<Hashtable>();

                Base.Data.SqlListToTable(Sql, ref Conn, ref FileList);

                ArrayList XmlList = new ArrayList();

                for (int i = 0; i < FileList.Count; i++)
                {
                    XmlList.Add("<item>");
                    XmlList.Add("<id>" + FileList[i]["DS_Id"].TypeString() + "</id>");
                    XmlList.Add("<versionId>" + FileList[i]["DS_VersionId"].TypeString() + "</versionId>");
                    XmlList.Add("<codeId>" + FileList[i]["DS_CodeId"].TypeString() + "</codeId>");
                    XmlList.Add("<hash>" + FileList[i]["DS_Hash"].TypeString() + "</hash>");
                    XmlList.Add("<name>" + FileList[i]["DS_Name"].TypeString() + "" + FileList[i]["DS_Extension"].TypeString() + "</name>");
                    XmlList.Add("<size>" + FileList[i]["DS_Size"].TypeString() + "</size>");
                    XmlList.Add("<lock>" + FileList[i]["DS_Lock"].TypeString() + "</lock>");
                    XmlList.Add("<updateTime>" + FileList[i]["DS_UpdateTime"].TypeString() + "</updateTime>");
                    XmlList.Add("</item>");
                }

                return AppCommon.ResponseMessage("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><folder><share>" + FolderShare + "</share><lock>" + FolderLock + "</lock><purview>" + FolderPurview + "</purview></folder><files>" + string.Join("", XmlList.ToArray()) + "</files></root>");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
