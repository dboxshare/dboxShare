using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public class AppException : ExceptionFilterAttribute
    {


        public override void OnException(HttpActionExecutedContext Context)
        {
            if (Context.Exception.GetType().ToString() != "System.Threading.Tasks.TaskCanceledException")
            {
                AppCommon.Error(Context.Exception);
            }
        }


    }


}