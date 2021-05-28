using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_FolderPathJsonController : ApiController
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
        /// 获取文件夹路径数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/folder/path-json")]
        [HttpGet]
        public HttpResponseMessage PathJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var FolderId = Context.Request.QueryString["FolderId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
                var Id = 0;
                var Name = "";

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_Name, DS_Lock From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = FileTable["DS_Id"].TypeInt();
                    FolderId = FileTable["DS_FolderId"].TypeInt();
                    Name = FileTable["DS_Name"].TypeString();
                }

                FileTable.Clear();

                if (Id == 0)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var Json = new ArrayList();

                Json.Add("{'ds_id':'" + Id + "','ds_folderid':'" + FolderId + "','ds_name':'" + Base.Common.JsonEscape(Name) + "'}");

                while (Id > 0)
                {
                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId, DS_Name, DS_Lock From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        Id = 0;
                    }
                    else
                    {
                        Id = FileTable["DS_Id"].TypeInt();
                        FolderId = FileTable["DS_FolderId"].TypeInt();
                        Name = FileTable["DS_Name"].TypeString();
                    }

                    FileTable.Clear();

                    if (Id > 0)
                    {
                        Json.Add("{'ds_id':'" + Id + "','ds_folderid':'" + FolderId + "','ds_name':'" + Base.Common.JsonEscape(Name) + "'}");
                    }
                }

                return AppCommon.ResponseMessage("[" + string.Join(",", Json.ToArray()) + "]");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
