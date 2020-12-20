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


namespace dboxShare.Web.Drive
{


    public class FolderTotalJson : IHttpHandler, IReadOnlySessionState
    {


        public bool IsReusable
        {
            get
            {
                return true;
            }
        }


        private dynamic Conn;


        public void ProcessRequest(HttpContext Context)
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            DataToJson(Context);

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 获取文件夹合计数据返回json格式字符串
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            ArrayList JsonList = new ArrayList();
            int Id = 0;
            string FolderPath = "";
            string StoragePath = "";

            if (Base.Common.IsNumeric(Context.Request.QueryString["Id"]) == true)
            {
                Id = Context.Request.QueryString["Id"].TypeInt();
            }
            else
            {
                return;
            }

            FolderPath = AppCommon.FolderIdPath(Id, ref Conn);

            if (string.IsNullOrEmpty(FolderPath) == true)
            {
                return;
            }

            StoragePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1));

            JsonList.Add("'occupy_space':'" + UsedSpace(StoragePath) + "'");
            JsonList.Add("'folder_count':'" + FolderCount(FolderPath) + "'");
            JsonList.Add("'file_count':'" + FileCount(FolderPath) + "'");

            Context.Response.Write("{" + string.Join(",", JsonList.ToArray()) + "}");
        }


        /// <summary>
        /// 文件夹统计
        /// </summary>
        private string FolderCount(string Path)
        {
            return Base.Data.SqlScalar("Select Count(*) From DBS_File Where DBS_Folder = 1 And DBS_FolderPath Like '" + Path + "%'", ref Conn).TypeString();
        }


        /// <summary>
        /// 文件统计
        /// </summary>
        private string FileCount(string Path)
        {
            return Base.Data.SqlScalar("Select Count(*) From DBS_File Where DBS_VersionId = 0 And DBS_Folder = 0 And DBS_FolderPath Like '" + Path + "%'", ref Conn).TypeString();
        }


        /// <summary>
        /// 占用空间统计
        /// </summary>
        private long UsedSpace(string Path)
        {
            if (Directory.Exists(Path) == false)
            {
                return 0;
            }

            DirectoryInfo DI = new DirectoryInfo(Path);
            long Bytes = 0;

            foreach (DirectoryInfo DirectoryItem in DI.GetDirectories())
            {
                Bytes += UsedSpace(DirectoryItem.FullName);
            }

            foreach (FileInfo FileItem in DI.GetFiles())
            {
                Bytes += FileItem.Length;
            }

            return Bytes;
        }


    }


}
