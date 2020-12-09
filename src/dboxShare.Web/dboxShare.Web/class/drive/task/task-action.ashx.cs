using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Drive
{


    public class TaskAction : IHttpHandler, IReadOnlySessionState
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

            switch (Context.Request.QueryString["Action"].TypeString())
            {
                case "assign":
                    Assign(Context);
                    break;

                case "revoke":
                    Revoke(Context);
                    break;

                case "accept":
                    Accept(Context);
                    break;

                case "reject":
                    Reject(Context);
                    break;

                case "completed":
                    Completed(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 任务分派
        /// </summary>
        private void Assign(HttpContext Context)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            int Folder = 0;
            string Name = "";
            string Extension = "";
            int TaskId = 0;
            int UserId = 0;
            int Level = 0;
            string Deadline = "";
            string Content = "";
            int Index = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (Context.Request.Form.GetValues("User").Length == 0)
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["Level"]) == true)
            {
                Level = Context.Request.Form["Level"].TypeInt();
            }

            Deadline = Context.Request.Form["Deadline"].TypeString();

            if (Base.Common.StringCheck(Deadline, @"^20[\d]{2}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}:[\d]{2}$") == false)
            {
                return;
            }

            Content = Context.Request.Form["Content"].TypeString();

            if (Base.Common.StringCheck(Content, @"^[\s\S]{1,500}$") == false)
            {
                return;
            }

            Content = Base.Common.HtmlEncode(Content, true);

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_Name, DBS_Extension From DBS_File Where DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Folder = FileTable["DBS_Folder"].TypeInt();
                Name = FileTable["DBS_Name"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            TaskId = Base.Data.SqlInsert("Insert Into DBS_Task(DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Content, DBS_Level, DBS_Deadline, DBS_Revoke, DBS_Cause, DBS_Time) Values(" + Id + ", '" + Name + "', '" + Extension + "', " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Content + "', " + Level + ", '" + Deadline + "', 0, 'null', '" + DateTime.Now.ToString() + "')", ref Conn);

            if (TaskId == 0)
            {
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("User").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("User")[Index]) == true)
                {
                    UserId = Context.Request.Form.GetValues("User")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                Base.Data.SqlQuery("Insert Into DBS_Task_Member(DBS_TaskId, DBS_UserId, DBS_Username, DBS_Reason, DBS_Postscript, DBS_Status, DBS_AcceptedTime, DBS_RejectedTime, DBS_CompletedTime) Select " + TaskId + ", DBS_Id, DBS_Username, 'null', 'null', 0, '1970/1/1 00:00:00', '1970/1/1 00:00:00', '1970/1/1 00:00:00' From DBS_User Where DBS_Id = " + UserId, ref Conn);
            }

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 任务撤消
        /// </summary>
        private void Revoke(HttpContext Context)
        {
            Hashtable TaskTable = new Hashtable();
            int Id = 0;
            string Cause = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Cause = Context.Request.Form["Cause"].TypeString();

            if (Base.Common.StringCheck(Cause, @"^[\s\S]{1,200}$") == false)
            {
                return;
            }
            
            Cause = Base.Common.HtmlEncode(Cause, false);

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Time From DBS_Task Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_Id = " + Id, ref Conn, ref TaskTable);

            if (TaskTable["Exist"].TypeBool() == false)
            {
                return;
            }

            if (DateTime.Compare(TaskTable["DBS_Time"].TypeDateTime(), DateTime.Now.AddHours(-12)) < 0)
            {
                return;
            }

            TaskTable.Clear();

            Base.Data.SqlQuery("Update DBS_Task Set DBS_Revoke = 1, DBS_Cause = '" + Cause + "' Where DBS_Id = " + Id, ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 任务接受
        /// </summary>
        private void Accept(HttpContext Context)
        {
            Hashtable TaskTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_TaskId, DBS_UserId From DBS_Task_Member Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn, ref TaskTable);

            if (TaskTable["Exist"].TypeBool() == false)
            {
                return;
            }

            TaskTable.Clear();

            Base.Data.SqlQuery("Update DBS_Task_Member Set DBS_Status = 1, DBS_AcceptedTime = '" + DateTime.Now.ToString() + "' Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 任务拒绝
        /// </summary>
        private void Reject(HttpContext Context)
        {
            Hashtable TaskTable = new Hashtable();
            int Id = 0;
            string Reason = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Reason = Context.Request.Form["Reason"].TypeString();

            if (Base.Common.StringCheck(Reason, @"^[\s\S]{1,200}$") == false)
            {
                return;
            }

            Reason = Base.Common.HtmlEncode(Reason, false);

            Base.Data.SqlDataToTable("Select DBS_TaskId, DBS_UserId From DBS_Task_Member Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn, ref TaskTable);

            if (TaskTable["Exist"].TypeBool() == false)
            {
                return;
            }

            TaskTable.Clear();

            Base.Data.SqlQuery("Update DBS_Task_Member Set DBS_Reason = '" + Reason + "', DBS_Status = -1, DBS_RejectedTime = '" + DateTime.Now.ToString() + "' Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 任务完成
        /// </summary>
        private void Completed(HttpContext Context)
        {
            Hashtable TaskTable = new Hashtable();
            int Id = 0;
            string Postscript = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Postscript = Context.Request.Form["Postscript"].TypeString();

            if (string.IsNullOrEmpty(Postscript) == false)
            {
                if (Base.Common.StringCheck(Postscript, @"^[\s\S]{1,200}$") == false)
                {
                    return;
                }

                Postscript = Base.Common.HtmlEncode(Postscript, false);
            }

            Base.Data.SqlDataToTable("Select DBS_TaskId, DBS_UserId From DBS_Task_Member Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn, ref TaskTable);

            if (TaskTable["Exist"].TypeBool() == false)
            {
                return;
            }

            TaskTable.Clear();

            Base.Data.SqlQuery("Update DBS_Task_Member Set DBS_Postscript = '" + Postscript + "', DBS_Status = 2, DBS_CompletedTime = '" + DateTime.Now.ToString() + "' Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_TaskId = " + Id, ref Conn);

            Context.Response.Write("complete");
        }


    }


}
