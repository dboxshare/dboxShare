using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Share.Controllers
{


    public class Share_ExtractLoginController : ApiController
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
        /// 链接分享提取登录
        /// </summary>
        [Route("api/link/extract-login")]
        [HttpPost]
        public HttpResponseMessage ExtractLogin()
        {
            var UserId = Context.Request.Form["UserId"].TypeInt();

            var CodeId = Context.Request.Form["CodeId"].TypeString();

            if (Base.Common.StringCheck(CodeId, @"^[\w]{16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\w]{6}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var LockId = "Share-Lock-IP-" + Base.Common.ClientIP() + "";

            if (Context.Cache[LockId].TypeInt() == 5)
            {
                return AppCommon.ResponseMessage("link-lock-ip");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Query = "";

                Query += "DS_UserId = " + UserId + " And ";
                Query += "DS_CodeId = '" + CodeId + "' And ";
                Query += "DS_Password = '" + Password + "' And ";
                Query += "DS_Deadline > '" + DateTime.Now.ToString() + "' And ";
                Query += "DS_Revoke = 0";

                var LinkTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_CodeId, DS_UserId, DS_Deadline, DS_Password, DS_Revoke From DS_Link Where " + Query + "", ref Conn, ref LinkTable);

                if (LinkTable["Exists"].TypeBool() == false)
                {
                    if (Base.Common.IsNothing(Context.Cache[LockId]) == true)
                    {
                        Context.Cache.Insert(LockId, 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }
                    else
                    {
                        Context.Cache.Insert(LockId, Context.Cache[LockId].TypeInt() + 1, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
                    }

                    return AppCommon.ResponseMessage("link-login-failed");
                }

                var Id = LinkTable["DS_Id"].TypeInt();

                LinkTable.Clear();

                Base.Data.SqlQuery("Update DS_Link Set DS_Count = DS_Count + 1 Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Insert Into DS_Link_Log(DS_LinkId, DS_IP, DS_Time) Values(" + Id + ", '" + Base.Common.ClientIP() + "', '" + DateTime.Now.ToString() + "')", ref Conn);

                Context.Session["LinkId"] = Id;

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


    }


}
