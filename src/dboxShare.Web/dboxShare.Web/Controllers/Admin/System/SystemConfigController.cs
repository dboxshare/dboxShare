using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class SystemConfigController : ApiController
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
        /// 系统配置
        /// </summary>
        [Route("api/admin/system/config")]
        [HttpPost]
        public HttpResponseMessage Config()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            string AppName = Context.Request.Form["AppName"].TypeString();

            if (Base.Common.StringCheck(AppName, @"^[\w]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string UploadExtension = Context.Request.Form["UploadExtension"].TypeString();

            if (string.IsNullOrEmpty(UploadExtension) == false)
            {
                if (Base.Common.StringCheck(UploadExtension, @"^([\w]{1,8}[\,]?){1,500}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }
            }

            string UploadSize = Context.Request.Form["UploadSize"].TypeString();

            if (Base.Common.StringCheck(UploadSize, @"^[\d]{2,4}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string VersionCount = Context.Request.Form["VersionCount"].TypeString();

            if (Base.Common.StringCheck(VersionCount, @"^[\d]{2,3}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailAddress = Context.Request.Form["MailAddress"].TypeString();

            if (Base.Common.StringCheck(MailAddress, @"^[\w\-\@\.]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailUsername = Context.Request.Form["MailUsername"].TypeString();

            if (Base.Common.StringCheck(MailUsername, @"^[\w\-\@\.]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailPassword = Context.Request.Form["MailPassword"].TypeString();

            if (Base.Common.StringCheck(MailPassword, @"^[\S]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailServer = Context.Request.Form["MailServer"].TypeString();

            if (Base.Common.StringCheck(MailServer, @"^[\w\-\.\:]{1,50}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailServerPort = Context.Request.Form["MailServerPort"].TypeString();

            if (Base.Common.StringCheck(MailServerPort, @"^[\d]+$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string MailServerSSL = Context.Request.Form["MailServerSSL"].TypeString();

            if (Base.Common.StringCheck(MailServerSSL, @"^(true|false)$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            XmlDocument XDocument = new XmlDocument();

            XDocument.Load(AppConfig.ConfigFilePath);

            XmlNode XNode = null;
            string XPath = "/configuration/appSettings/add[@key=\"{key}\"]";

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

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailServer"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailServer;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailServerPort"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailServerPort;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "MailServerSSL"));

            if (Base.Common.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = MailServerSSL;
            }

            XDocument.Save(AppConfig.ConfigFilePath);

            return AppCommon.ResponseMessage("complete");
        }


    }


}
