using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive.Controllers
{


    public class Drive_FolderTotalJsonController : ApiController
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
        /// 获取文件夹统计数据返回 JSON 格式字符串
        /// </summary>
        [Route("api/drive/folder/total-json")]
        [HttpGet]
        public HttpResponseMessage TotalJson()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.QueryString["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

                var Json = new ArrayList();

                Json.Add("'occupy_total':'" + OccupyTotal(FolderPath) + "'");
                Json.Add("'folder_total':'" + FolderTotal(FolderPath) + "'");
                Json.Add("'file_total':'" + FileTotal(FolderPath) + "'");

                return AppCommon.ResponseMessage("{" + string.Join(",", Json.ToArray()) + "}");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 占用空间统计
        /// </summary>
        private long OccupyTotal(string FolderPath)
        {
            return Base.Data.SqlScalar("Select Sum(DS_Size) As Total From DS_File Where DS_Folder = 0 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn);
        }


        /// <summary>
        /// 文件夹数量统计
        /// </summary>
        private int FolderTotal(string FolderPath)
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_Folder = 1 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn);
        }


        /// <summary>
        /// 文件数量统计
        /// </summary>
        private int FileTotal(string FolderPath)
        {
            return Base.Data.SqlScalar("Select Count(*) From DS_File Where DS_VersionId = 0 And DS_Folder = 0 And DS_FolderPath Like '" + FolderPath + "%'", ref Conn);
        }


    }


}
