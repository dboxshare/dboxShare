using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public partial class AppGlobal : HttpApplication
    {


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.Add(new AppException());
        }


        protected void Application_PostAuthorizeRequest()
        {
            if (Context.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api") == true)
            {
                Context.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            }
        }


        protected void Session_Start()
        {
            if (Base.Common.IsNothing(Context.Request.Cookies["App-Language"]) == true)
            {
                Context.Response.Cookies["App-Language"].Value = AppConfig.AppLanguage;
                Context.Response.Cookies["App-Language"].Expires = DateTime.MaxValue;
            }
        }


    }


}