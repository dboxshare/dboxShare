using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class SystemConfigDataJson : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        public void ProcessRequest(HttpContext Context)
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                Context.Session.Abandon();
                return;
            }


            DataToJson(Context);
        }


        /// <summary>
        /// 读取web.config配置信息返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();

            JsonList.Add("'appname':'" + ConfigurationManager.AppSettings["AppName"].TypeString() + "'");
            JsonList.Add("'uploadextension':'" + ConfigurationManager.AppSettings["UploadExtension"].TypeString() + "'");
            JsonList.Add("'uploadsize':'" + ConfigurationManager.AppSettings["UploadSize"].TypeString() + "'");
            JsonList.Add("'versioncount':'" + ConfigurationManager.AppSettings["VersionCount"].TypeString() + "'");
            JsonList.Add("'mailaddress':'" + ConfigurationManager.AppSettings["MailAddress"].TypeString() + "'");
            JsonList.Add("'mailserver':'" + ConfigurationManager.AppSettings["MailServer"].TypeString() + "'");
            JsonList.Add("'mailport':'" + ConfigurationManager.AppSettings["MailPort"].TypeString() + "'");
            JsonList.Add("'mailssl':'" + ConfigurationManager.AppSettings["MailSsl"].TypeString() + "'");
            JsonList.Add("'mailusername':'" + ConfigurationManager.AppSettings["MailUsername"].TypeString() + "'");
            JsonList.Add("'mailpassword':'" + ConfigurationManager.AppSettings["MailPassword"].TypeString() + "'");

            Context.Response.Write("{" + string.Join(",", JsonList.ToArray()) + "}");
        }


    }


}
