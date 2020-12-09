using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public class BaseDataJS : IHttpHandler, IReadOnlySessionState
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
            Context.Response.ContentType = "text/javascript";

            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            DataToJS(Context);
        }


        /// <summary>
        /// 基本数据返回js脚本
        /// </summary>
        private void DataToJS(HttpContext Context)
        {
            string ScriptPath = Context.Server.MapPath("/web/js/base-data.js");
            string ScriptContent = File.ReadAllText(ScriptPath);

            ScriptContent = ScriptContent.Replace("{userid}", Context.Session["UserId"].TypeString());
            ScriptContent = ScriptContent.Replace("{username}", Context.Session["Username"].TypeString());
            ScriptContent = ScriptContent.Replace("{useradmin}", Context.Session["Admin"].TypeString());
            ScriptContent = ScriptContent.Replace("{systemdate}", DateTime.Now.ToString("yyyy-MM-dd"));
            ScriptContent = ScriptContent.Replace("{uploadextension}", ConfigurationManager.AppSettings["UploadExtension"].TypeString());
            ScriptContent = ScriptContent.Replace("{uploadsize}", ConfigurationManager.AppSettings["UploadSize"].TypeString());

            Context.Response.Write(ScriptContent);
        }


    }


}