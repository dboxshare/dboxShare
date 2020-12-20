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


    public class ListAttributeJson : IHttpHandler, IReadOnlySessionState
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

            ListAttributeToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 获取列表属性数据返回json格式字符串
        /// </summary>
        private void ListAttributeToJson(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            bool Folder = false;
            string Purview = "";
            string Lock = "";
            string Recycle = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Folder = Context.Request.QueryString["Folder"].TypeString() == "true" ? true : false;

            Purview = AppCommon.PurviewRole(Id, Folder, ref Conn);

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Lock, DBS_Recycle From DBS_File Where DBS_Folder = " + (Folder == true ? 1 : 0) + " And DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Lock = FileTable["DBS_Lock"].TypeString();
                Recycle = FileTable["DBS_Recycle"].TypeString();
            }

            FileTable.Clear();

            Context.Response.Write("{'purview':'" + Purview + "','lock':'" + Lock + "','recycle':'" + Recycle + "'}");
        }


    }


}
