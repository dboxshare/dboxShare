using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_LinkActionController : ApiController
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
        /// 链接分享
        /// </summary>
        [Route("api/drive/link/share")]
        [HttpPost]
        public HttpResponseMessage Share()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("FileId").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            var CodeId = Context.Request.Form["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\w]{16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Title = Context.Request.Form["Title"].TypeString();

            if (Base.Common.StringCheck(Title, @"^[\s\S]{1,100}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Day = Context.Request.Form["Day"].TypeInt();

            var Deadline = DateTime.Now.AddDays(Day).ToString();

            var Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\w]{6}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Email = Context.Request.Form["Email"].TypeString();

            if (string.IsNullOrEmpty(Email) == false)
            {
                if (Base.Common.StringCheck(Email, @"^([\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?\;?)+$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Message = Context.Request.Form["Message"].TypeString();

            if (string.IsNullOrEmpty(Message) == false)
            {
                if (Base.Common.StringCheck(Message, @"^[\s\S]{1,200}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var LinkId = Base.Data.SqlInsert("Insert Into DS_Link(DS_CodeId, DS_UserId, DS_Username, DS_Title, DS_Deadline, DS_Password, DS_Count, DS_Revoke, DS_Time) Values('" + CodeId + "', " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Title + "', '" + Deadline + "', '" + Password + "', 0, 0, '" + DateTime.Now.ToString() + "')", ref Conn);

                if (LinkId == 0)
                {
                    return AppCommon.ResponseMessage("data-insertion-failed");
                }

                for (var i = 0; i < Context.Request.Form.GetValues("FileId").Length; i++)
                {
                    var FileId = Context.Request.Form.GetValues("FileId")[i].TypeInt();

                    var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Lock, DS_Recycle From DS_File Where DS_Lock = 0 And DS_Recycle = 0 And DS_Id = " + FileId, ref Conn, ref FileTable);

                    if (FileTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    var Folder = FileTable["DS_Folder"].TypeInt();

                    FileTable.Clear();

                    if (Folder == 1)
                    {
                        if (AppCommon.PurviewCheck(FileId, true, "manager", ref Conn) == false)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (AppCommon.PurviewCheck(FileId, false, "manager", ref Conn) == false)
                        {
                            continue;
                        }
                    }

                    var Id = Base.Data.SqlInsert("Insert Into DS_Link_File(DS_LinkId, DS_FileId) Values(" + LinkId + ", " + FileId + ")", ref Conn);

                    if (Id == 0)
                    {
                        continue;
                    }

                    AppCommon.Log(Id, (Folder == 1 ? "folder-link" : "file-link"), ref Conn);
                }

                if (string.IsNullOrEmpty(Email) == false)
                {
                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id, DS_Username, DS_Name From DS_User Where DS_Id = " + Context.Session["UserId"].TypeString(), ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        return AppCommon.ResponseMessage("data-not-exist");
                    }

                    var UserId = UserTable["DS_Id"].TypeInt();
                    var Username = UserTable["DS_Username"].TypeString();
                    var Name = UserTable["DS_Name"].TypeString();

                    UserTable.Clear();

                    if (string.IsNullOrEmpty(Name) == false)
                    {
                        Username = Name;
                    }

                    var Link = "" + Context.Request.Url.Scheme + "://" + Context.Request.Url.Host + "/web/link/?" + UserId + "_" + CodeId + "";

                    var Emails = Email.Split(';');

                    for (var i = 0; i < Emails.Length; i++)
                    {
                        ShareMail(Username, Title, Message, Link, Password, Emails[i]);
                    }
                }

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 链接撤消
        /// </summary>
        [Route("api/drive/link/revoke")]
        [HttpPost]
        public HttpResponseMessage Revoke()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var LinkTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId From DS_Link Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id, ref Conn, ref LinkTable);

                if (LinkTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                LinkTable.Clear();

                Base.Data.SqlQuery("Update DS_Link Set DS_Revoke = 1 Where DS_Id = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 发送分享通知邮件
        /// </summary>
        private void ShareMail(string Username, string Title, string Message, string Link, string Password, string Email)
        {
            var MailTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            AppCommon.XmlDataReader(Base.Common.PathCombine(AppConfig.DataStoragePath, "mail-template.xml"), "/template/link-share", MailTable);

            if (MailTable.Count == 0)
            {
                return;
            }

            var Subject = MailTable["Subject"].TypeString();

            Subject = Subject.Replace("{@username}", Username);
            Subject = Subject.Replace("{@title}", Title);

            var Content = MailTable["Content"].TypeString();

            Content = Content.Replace("{@username}", Username);
            Content = Content.Replace("{@message}", Message);
            Content = Content.Replace("{@link}", Link);
            Content = Content.Replace("{@password}", Password);

            Base.Common.SendMail(AppConfig.AppName, AppConfig.MailAddress, AppConfig.MailUsername, AppConfig.MailPassword, AppConfig.MailServer, AppConfig.MailServerPort, AppConfig.MailServerSSL, Email, Subject, Content, true);
        }


    }


}
