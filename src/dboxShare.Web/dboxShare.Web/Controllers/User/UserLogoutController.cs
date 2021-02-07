using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.User.Controllers
{


    public class UserLogoutController : ApiController
    {


        protected HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 用户注销登录
        /// </summary>
        [Route("api/user/logout")]
        [HttpGet]
        public HttpResponseMessage Logout()
        {
            Context.Session.Abandon();

            HttpResponseMessage Response = new HttpResponseMessage(HttpStatusCode.Moved);

            Response.Headers.Location = new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority));

            return Response;
        }


    }


}
