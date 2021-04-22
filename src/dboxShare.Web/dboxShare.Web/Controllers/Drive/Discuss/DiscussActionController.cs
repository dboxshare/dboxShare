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


    public class DiscussActionController : ApiController
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
        /// 消息发表
        /// </summary>
        [Route("api/drive/discuss/post")]
        [HttpPost]
        public HttpResponseMessage Post()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Content = Context.Request.Form["Content"].TypeString();

            if (Base.Common.StringCheck(Content, @"^[\s\S]{1,200}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            Content = Base.Common.HtmlEncode(Content, true);

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Name, DS_Extension From DS_File Where DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int Folder = FileTable["DS_Folder"].TypeInt();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                Base.Data.SqlQuery("Insert Into DS_Discuss(DS_FileId, DS_FileName, DS_FileExtension, DS_IsFolder, DS_UserId, DS_Username, DS_Content, DS_Revoke, DS_Time) Values(" + Id + ", '" + Name + "', '" + Extension + "', " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Content + "', 0, '" + DateTime.Now.ToString() + "')", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 消息撤回
        /// </summary>
        [Route("api/drive/discuss/revoke")]
        [HttpPost]
        public HttpResponseMessage Revoke()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable DiscussTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Time From DS_Discuss Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id, ref Conn, ref DiscussTable);

                if (DiscussTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                if (DateTime.Compare(DiscussTable["DS_Time"].TypeDateTime(), DateTime.Now.AddMinutes(-20)) < 0)
                {
                    return AppCommon.ResponseMessage("operation has expired");
                }

                DiscussTable.Clear();

                Base.Data.SqlQuery("Update DS_Discuss Set DS_Revoke = 1 Where DS_Id = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
