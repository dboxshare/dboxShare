using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using System.Xml;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class SystemConfig : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            ProcessConfig(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 系统配置
        /// </summary>
        private void ProcessConfig(HttpContext Context)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNode = default(XmlNode);
            string XPath = "/configuration/appSettings/add[@key=\"{key}\"]";
            string ConfigPath = Context.Server.MapPath("/web.config");
            string AppName = "";
            string UploadExtension = "";
            string UploadSize = "";
            string VersionCount = "";
            string MailAddress = "";
            string MailServer = "";
            string MailPort = "";
            string MailSsl = "";
            string MailUsername = "";
            string MailPassword = "";

            AppName = Context.Request.Form["AppName"].TypeString();

            if (Base.Common.StringCheck(AppName, @"^[\w]{1,50}$") == false)
            {
                return;
            }

            UploadExtension = Context.Request.Form["UploadExtension"].TypeString();

            if (string.IsNullOrEmpty(UploadExtension) == false)
            {
                if (Base.Common.StringCheck(UploadExtension, @"^([\w]{1,8}[\,]?){1,500}$") == false)
                {
                    return;
                }
            }

            UploadSize = Context.Request.Form["UploadSize"].TypeString();

            if (Base.Common.StringCheck(UploadSize, @"^[\d]{2,4}$") == false)
            {
                return;
            }

            VersionCount = Context.Request.Form["VersionCount"].TypeString();

            if (Base.Common.StringCheck(VersionCount, @"^[\d]{2,4}$") == false)
            {
                return;
            }

            MailAddress = Context.Request.Form["MailAddress"].TypeString();

            if (Base.Common.StringCheck(MailAddress, @"^[\w\-\@\.]{1,50}$") == false)
            {
                return;
            }

            MailServer = Context.Request.Form["MailServer"].TypeString();

            if (Base.Common.StringCheck(MailServer, @"^[\w\-\.\:]{1,50}$") == false)
            {
                return;
            }

            MailPort = Context.Request.Form["MailPort"].TypeString();

            if (Base.Common.StringCheck(MailPort, @"^[\d]+$") == false)
            {
                return;
            }

            MailSsl = Context.Request.Form["MailSsl"].TypeString();

            if (Base.Common.StringCheck(MailSsl, @"^(true|false)$") == false)
            {
                return;
            }

            MailUsername = Context.Request.Form["MailUsername"].TypeString();

            if (Base.Common.StringCheck(MailUsername, @"^[\w\-\@\.]{1,50}$") == false)
            {
                return;
            }

            MailPassword = Context.Request.Form["MailPassword"].TypeString();

            if (Base.Common.StringCheck(MailPassword, @"^[\S]{1,50}$") == false)
            {
                return;
            }

            XDocument.Load(ConfigPath);

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "AppName"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppName;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "UploadExtension"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = UploadExtension;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "UploadSize"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = UploadSize;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "VersionCount"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = VersionCount;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailAddress"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailAddress;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailServer"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailServer;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailPort"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailPort;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailSsl"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailSsl;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailUsername"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailUsername;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailPassword"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailPassword;
            }

            XDocument.Save(ConfigPath);

            Context.Response.Write("complete");
        }


    }


}
