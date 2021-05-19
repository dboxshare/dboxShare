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


        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.Add(new AppException());
        }


        protected void Application_PostAuthorizeRequest()
        {
            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api") == true)
            {
                HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            }
        }


    }


}