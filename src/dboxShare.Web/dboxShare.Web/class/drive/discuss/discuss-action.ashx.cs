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


    public class DiscussAction : IHttpHandler, IReadOnlySessionState
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

            switch (Context.Request.QueryString["Action"].TypeString())
            {
                case "post":
                    Post(Context);
                    break;

                case "revoke":
                    Revoke(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 消息发表
        /// </summary>
        private void Post(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int Folder = 0;
            string Name = "";
            string Extension = "";
            string Content = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Content = Context.Request.Form["Content"].TypeString();

            if (Base.Common.StringCheck(Content, @"^[\s\S]{1,200}$") == false)
            {
                return;
            }

            Content = Base.Common.HtmlEncode(Content, true);

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Name, DBS_Extension From DBS_File Where DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Folder = FileTable["DBS_Folder"].TypeInt();
                Name = FileTable["DBS_Name"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Insert Into DBS_Discuss(DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Content, DBS_Revoke, DBS_Time) Values(" + Id + ", '" + Name + "', '" + Extension + "', " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Content + "', 0, '" + DateTime.Now.ToString() + "')", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 消息撤回
        /// </summary>
        private void Revoke(HttpContext Context)
        {
            Hashtable DiscussTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Time From DBS_Discuss Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Id = " + Id, ref Conn, ref DiscussTable);

            if (DiscussTable["Exist"].TypeBool() == false)
            {
                return;
            }

            if (DateTime.Compare(DiscussTable["DBS_Time"].TypeDateTime(), DateTime.Now.AddMinutes(-20)) < 0)
            {
                return;
            }

            DiscussTable.Clear();

            Base.Data.SqlQuery("Update DBS_Discuss Set DBS_Revoke = 1 Where DBS_Id = " + Id, ref Conn);

            Context.Response.Write("complete");
        }


    }


}
