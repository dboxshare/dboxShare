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


    public class Sync_FileListXmlController : ApiController
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
        [Route("api/sync/file-list-xml")]
        [HttpGet]
        public HttpResponseMessage FileListXml()
        {
            if (AppCommon.LoginAuth("PC") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var FolderId = Context.Request.QueryString["FolderId"].TypeInt();

            var Query = "";

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
                     "DS_File_Purview.DS_FileId = B.DS_FolderId And " +
                     "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Where " +
                     "B.DS_Id = DS_File.DS_Id And " +
                     "B.DS_Folder = 0 And " +
                     "B.DS_FolderId = " + FolderId + " And " +
                     "B.DS_Share = 1 And " +
                     "B.DS_Recycle = 0 Union All ";

            // 共享角色查询
            Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = C.DS_FolderId And " +
                     "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Where " +
                     "C.DS_Id = DS_File.DS_Id And " +
                     "C.DS_Folder = 0 And " +
                     "C.DS_FolderId = " + FolderId + " And " +
                     "C.DS_Share = 1 And " +
                     "C.DS_Recycle = 0 Union All ";

            // 共享用户查询
            Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                     "DS_File_Purview.DS_FileId = D.DS_FolderId And " +
                     "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Where " +
                     "D.DS_Id = DS_File.DS_Id And " +
                     "D.DS_Folder = 0 And " +
                     "D.DS_FolderId = " + FolderId + " And " +
                     "D.DS_Share = 1 And " +
                     "D.DS_Recycle = 0";

            Query += ")";

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Share, DS_Lock, DS_Recycle From DS_File Where DS_Folder = 1 And DS_Recycle = 0 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var FolderShare = FileTable["DS_Share"].TypeInt();
                var FolderLock = FileTable["DS_Lock"].TypeInt();

                FileTable.Clear();

                var FolderPurview = AppCommon.PurviewRole(FolderId, true, ref Conn); ;

                var FileList = new List<Hashtable>();

                var Sql = "Select DS_Id, DS_VersionId, DS_Folder, DS_FolderId, DS_CodeId, DS_Hash, DS_Name, DS_Extension, DS_Size, DS_Share, DS_Lock, DS_UpdateTime From DS_File Where " + Query + " Order By DS_Id Desc";

                Base.Data.SqlListToTable(Sql, ref Conn, ref FileList);

                var Xml = new ArrayList();

                for (var i = 0; i < FileList.Count; i++)
                {
                    Xml.Add("<item>");
                    Xml.Add("<id>" + FileList[i]["DS_Id"].TypeString() + "</id>");
                    Xml.Add("<versionId>" + FileList[i]["DS_VersionId"].TypeString() + "</versionId>");
                    Xml.Add("<codeId>" + FileList[i]["DS_CodeId"].TypeString() + "</codeId>");
                    Xml.Add("<hash>" + FileList[i]["DS_Hash"].TypeString() + "</hash>");
                    Xml.Add("<name>" + FileList[i]["DS_Name"].TypeString() + "" + FileList[i]["DS_Extension"].TypeString() + "</name>");
                    Xml.Add("<size>" + FileList[i]["DS_Size"].TypeString() + "</size>");
                    Xml.Add("<lock>" + FileList[i]["DS_Lock"].TypeString() + "</lock>");
                    Xml.Add("<updateTime>" + FileList[i]["DS_UpdateTime"].TypeString() + "</updateTime>");
                    Xml.Add("</item>");
                }

                return AppCommon.ResponseMessage("<?xml version=\"1.0\" encoding=\"utf-8\"?><root><folder><share>" + FolderShare + "</share><lock>" + FolderLock + "</lock><purview>" + FolderPurview + "</purview></folder><files>" + string.Join("", Xml.ToArray()) + "</files></root>");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
