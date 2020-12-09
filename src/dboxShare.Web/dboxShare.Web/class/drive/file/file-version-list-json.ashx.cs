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


    public class FileVersionListJson : IHttpHandler, IReadOnlySessionState
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
        /// 读取文件版本数据列表返回json格式字符串
        /// </summary>
        private void ListDataToJson(HttpContext Context)
        {
            int Id = 0;
            bool Myself = false;
            int Page = 0;
            string Query = "";
            string Json = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Myself = Context.Request.QueryString["Myself"].TypeString() == "true" ? true : false;

            if (Base.Common.IsNumeric(Context.Request.QueryString["Page"]) == true)
            {
                Page = Context.Request.QueryString["Page"].TypeInt();
                Page = Page < 1 ? 1 : Page;
            }
            else
            {
                Page = 1;
            }

            Query += "DBS_VersionId = " + Id + " And ";

            if (Myself == true)
            {
                Query += "Exists (";

                // 创建者查询
                Query += "Select A.DBS_Id From DBS_File As A Where " + 
                         "A.DBS_Id = DBS_File.DBS_Id And " + 
                         "A.DBS_Version = 1 And " + 
                         "A.DBS_VersionId = " + Id + " And " + 
                         "A.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

                // 更新者查询
                Query += "Select B.DBS_Id From DBS_File As B Where " + 
                         "B.DBS_Id = DBS_File.DBS_Id And " + 
                         "B.DBS_VersionId = " + Id + " And " + 
                         "B.DBS_UpdateUsername = '" + Context.Session["Username"].TypeString() + "'";

                Query += ") And ";
            }

            Query += "DBS_Recycle = 0";

            Json = Base.Data.SqlPageToJson("DBS_File", "DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_CodeId, DBS_Name, DBS_Extension, DBS_Remark, DBS_Share, DBS_Lock, DBS_Recycle, DBS_CreateUsername, DBS_CreateTime, DBS_UpdateUsername, DBS_UpdateTime", "DBS_Id Desc", Query, 50, Page, ref Conn);

            Context.Response.Write(Json);
        }


    }


}
