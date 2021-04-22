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


    public class TaskActionController : ApiController
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
        /// 任务分派
        /// </summary>
        [Route("api/drive/task/assign")]
        [HttpPost]
        public HttpResponseMessage Assign()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            if (Context.Request.Form.GetValues("User").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            int Level = Context.Request.Form["Level"].TypeInt();

            string Deadline = Context.Request.Form["Deadline"].TypeString();

            if (Base.Common.StringCheck(Deadline, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}:[\d]{2}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            string Content = Context.Request.Form["Content"].TypeString();

            if (Base.Common.StringCheck(Content, @"^[\s\S]{1,500}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            Content = Base.Common.HtmlEncode(Content, true);

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable FileTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_Name, DS_Extension From DS_File Where DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                int Folder = FileTable["DS_Folder"].TypeInt();
                string Name = FileTable["DS_Name"].TypeString();
                string Extension = FileTable["DS_Extension"].TypeString();

                FileTable.Clear();

                if (Folder == 1)
                {
                    if (AppCommon.PurviewCheck(Id, true, "manager", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no operation permission");
                    }
                }
                else
                {
                    if (AppCommon.PurviewCheck(Id, false, "viewer", ref Conn) == false)
                    {
                        return AppCommon.ResponseMessage("no operation permission");
                    }
                }

                int TaskId = Base.Data.SqlInsert("Insert Into DS_Task(DS_FileId, DS_FileName, DS_FileExtension, DS_IsFolder, DS_UserId, DS_Username, DS_Content, DS_Level, DS_Deadline, DS_Revoke, DS_Cause, DS_Time) Values(" + Id + ", '" + Name + "', '" + Extension + "', " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Content + "', " + Level + ", '" + Deadline + "', 0, 'null', '" + DateTime.Now.ToString() + "')", ref Conn);

                if (TaskId == 0)
                {
                    return AppCommon.ResponseMessage("data insertion failed");
                }

                for (int i = 0; i < Context.Request.Form.GetValues("User").Length; i++)
                {
                    int UserId = Context.Request.Form.GetValues("User")[i].TypeInt();

                    Base.Data.SqlQuery("Insert Into DS_Task_Member(DS_TaskId, DS_UserId, DS_Username, DS_Reason, DS_Remark, DS_Status, DS_AcceptedTime, DS_RejectedTime, DS_CompletedTime) Select " + TaskId + ", DS_Id, DS_Username, 'null', 'null', 0, '1970/1/1 00:00:00', '1970/1/1 00:00:00', '1970/1/1 00:00:00' From DS_User Where DS_Id = " + UserId, ref Conn);
                }

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 任务撤消
        /// </summary>
        [Route("api/drive/task/revoke")]
        [HttpPost]
        public HttpResponseMessage Revoke()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Cause = Context.Request.Form["Cause"].TypeString();

            if (Base.Common.StringCheck(Cause, @"^[\s\S]{1,200}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            Cause = Base.Common.HtmlEncode(Cause, false);

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Time From DS_Task Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_Id = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                if (DateTime.Compare(TaskTable["DS_Time"].TypeDateTime(), DateTime.Now.AddHours(-12)) < 0)
                {
                    return AppCommon.ResponseMessage("operation has expired");
                }

                TaskTable.Clear();

                Base.Data.SqlQuery("Update DS_Task Set DS_Revoke = 1, DS_Cause = '" + Cause + "' Where DS_Id = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 任务接受
        /// </summary>
        [Route("api/drive/task/accept")]
        [HttpPost]
        public HttpResponseMessage Accept()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_TaskId, DS_UserId From DS_Task_Member Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                TaskTable.Clear();

                Base.Data.SqlQuery("Update DS_Task_Member Set DS_Status = 1, DS_AcceptedTime = '" + DateTime.Now.ToString() + "' Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 任务拒绝
        /// </summary>
        [Route("api/drive/task/reject")]
        [HttpPost]
        public HttpResponseMessage Reject()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Reason = Context.Request.Form["Reason"].TypeString();

            if (Base.Common.StringCheck(Reason, @"^[\s\S]{1,200}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }

            Reason = Base.Common.HtmlEncode(Reason, false);

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_TaskId, DS_UserId From DS_Task_Member Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                TaskTable.Clear();

                Base.Data.SqlQuery("Update DS_Task_Member Set DS_Reason = '" + Reason + "', DS_Status = -1, DS_RejectedTime = '" + DateTime.Now.ToString() + "' Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 任务完成
        /// </summary>
        [Route("api/drive/task/completed")]
        [HttpPost]
        public HttpResponseMessage Completed()
        {
            if (AppCommon.LoginAuth("Web") == false)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            string Remark = Context.Request.Form["Remark"].TypeString();

            if (string.IsNullOrEmpty(Remark) == false)
            {
                if (Base.Common.StringCheck(Remark, @"^[\s\S]{1,200}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter item check failed");
                }

                Remark = Base.Common.HtmlEncode(Remark, false);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable TaskTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_TaskId, DS_UserId From DS_Task_Member Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn, ref TaskTable);

                if (TaskTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                TaskTable.Clear();

                Base.Data.SqlQuery("Update DS_Task_Member Set DS_Remark = '" + Remark + "', DS_Status = 2, DS_CompletedTime = '" + DateTime.Now.ToString() + "' Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_TaskId = " + Id, ref Conn);

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
