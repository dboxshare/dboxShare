using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public partial class AppGlobal : HttpApplication
    {


        private void Application_Error(object sender, EventArgs e)
        {
            AppCommon.Error(Server.GetLastError());
        }


        private void Session_Start(object sender, EventArgs e)
        {

        }


        private void Session_End(object sender, EventArgs e)
        {

        }


    }


}