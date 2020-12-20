using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class FolderPathJson : IHttpHandler, IReadOnlySessionState
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

            FolderPathToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 获取文件夹路径数据返回json格式字符串
        /// </summary>
        private void FolderPathToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int FolderId = 0;
            string Name = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["FolderId"]) == true)
            {
                FolderId = Context.Request.QueryString["FolderId"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DBS_Id"].TypeInt();
                FolderId = FileTable["DBS_FolderId"].TypeInt();
                Name = FileTable["DBS_Name"].TypeString();
            }

            FileTable.Clear();

            if (Id == 0)
            {
                return;
            }

            JsonList.Add("{'dbs_id':'" + Id + "','dbs_folderid':'" + FolderId + "','dbs_name':'" + Base.Common.JsonEscape(Name) + "'}");

            while (Id > 0)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Lock From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = FileTable["DBS_Id"].TypeInt();
                    FolderId = FileTable["DBS_FolderId"].TypeInt();
                    Name = FileTable["DBS_Name"].TypeString();
                }

                FileTable.Clear();

                if (Id > 0)
                {
                    JsonList.Add("{'dbs_id':'" + Id + "','dbs_folderid':'" + FolderId + "','dbs_name':'" + Base.Common.JsonEscape(Name) + "'}");
                }
            }

            Context.Response.Write("[" + string.Join(",", JsonList.ToArray()) + "]");
        }


    }


}
