using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin
{


    public class DepartmentAction : IHttpHandler, IReadOnlySessionState
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
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                Context.Session.Abandon();
                return;
            }

            Conn = Base.Data.DBConnection(ConfigurationManager.AppSettings["ConnectionString"].TypeString());

            Conn.Open();

            switch (Context.Request.QueryString["Action"].TypeString())
            {
                case "add":
                    Add(Context);
                    break;

                case "modify":
                    Modify(Context);
                    break;

                case "delete":
                    Delete(Context);
                    break;

                case "sort":
                    Sort(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 部门添加
        /// </summary>
        private void Add(HttpContext Context)
        {
            Hashtable DepartmentTable = new Hashtable();
            int Id = 0;
            int DepartmentId = 0;
            string Name = "";

            if (Base.Common.IsNumeric(Context.Request.Form["DepartmentId"]) == true)
            {
                DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();
            }

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,24}$") == false)
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_DepartmentId, DBS_Name From DBS_Department Where DBS_DepartmentId = " + DepartmentId + " And DBS_Name = '" + Name + "'", ref Conn, ref DepartmentTable);

            if (DepartmentTable["Exist"].TypeBool() == true)
            {
                Context.Response.Write("existed");
                return;
            }

            DepartmentTable.Clear();

            Id = Base.Data.SqlInsert("Insert Into DBS_Department(DBS_DepartmentId, DBS_Name, DBS_Sequence) Values(" + DepartmentId + ", '" + Name + "', 0)", ref Conn);

            if (Id == 0)
            {
                return;
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 部门修改
        /// </summary>
        private void Modify(HttpContext Context)
        {
            Hashtable DepartmentTable = new Hashtable();
            int Id = 0;
            int DepartmentId = 0;
            string Name = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["DepartmentId"]) == true)
            {
                DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();
            }
            else
            {
                return;
            }

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,24}$") == false)
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_DepartmentId, DBS_Name From DBS_Department Where DBS_DepartmentId = " + DepartmentId + " And DBS_Name = '" + Name + "'", ref Conn, ref DepartmentTable);

            if (DepartmentTable["Exist"].TypeBool() == true)
            {
                if (DepartmentTable["DBS_Id"].TypeInt() != Id)
                {
                    Context.Response.Write("existed");
                    return;
                }
            }

            DepartmentTable.Clear();

            Base.Data.SqlQuery("Update DBS_Department Set DBS_Name = '" + Name + "' Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 部门删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            Hashtable DepartmentTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_Department Where DBS_Id = " + Id, ref Conn, ref DepartmentTable);

            if (DepartmentTable["Exist"].TypeBool() == false)
            {
                return;
            }

            DepartmentTable.Clear();

            Base.Data.SqlQuery("Delete From DBS_Department Where DBS_Id = " + Id, ref Conn);

            Delete_Subitem(Id);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 部门删除(子项目)
        /// </summary>
        private void Delete_Subitem(int Id)
        {
            ArrayList Departments = new ArrayList();
            int Index = 0;

            Base.Data.SqlListToArray("DBS_Id", "Select DBS_Id, DBS_DepartmentId From DBS_Department Where DBS_DepartmentId = " + Id, ref Conn, ref Departments);

            for (Index = 0; Index < Departments.Count; Index++)
            {
                Delete_Subitem(Departments[Index].TypeInt());
            }

            Base.Data.SqlQuery("Delete From DBS_Department Where DBS_DepartmentId = " + Id, ref Conn);
        }


        /// <summary>
        /// 部门排序
        /// </summary>
        private void Sort(HttpContext Context)
        {
            Hashtable DepartmentTable = new Hashtable();
            int Id = 0;
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return;
            }

            for (Index = 0; Index < Context.Request.Form.GetValues("Id").Length; Index++)
            {
                if (Base.Common.IsNumeric(Context.Request.Form.GetValues("Id")[Index]) == true)
                {
                    Id = Context.Request.Form.GetValues("Id")[Index].TypeInt();
                }
                else
                {
                    continue;
                }

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_Department Where DBS_Id = " + Id, ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                DepartmentTable.Clear();

                Base.Data.SqlQuery("Update DBS_Department Set DBS_Sequence = " + (Context.Request.Form.GetValues("Id").Length - Index) + " Where DBS_Id = " + Id, ref Conn);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 读取部门数据导出json格式文件
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            FileStream FileStream = default(FileStream);
            string ScriptPath = "";
            string ScriptVariable = "";
            string ScriptContent = "";
            byte[] Bytes = {};
            string Json = "";

            Json = Base.Data.SqlListToJson("Select DBS_Id, DBS_DepartmentId, DBS_Name, DBS_Sequence From DBS_Department Order By DBS_DepartmentId Asc, DBS_Sequence Desc, DBS_Id Asc", ref Conn);

            ScriptPath = Context.Server.MapPath("/storage/data/department-data-json.js");

            ScriptVariable = Base.Common.StringGet(File.ReadAllText(ScriptPath), @"^var\s+(\w+)\s+=");

            ScriptContent = "var " + ScriptVariable + " = " + Json + ";";

            Bytes = new UTF8Encoding(true).GetBytes(ScriptContent);

            FileStream = File.Create(ScriptPath);

            FileStream.Write(Bytes, 0, Bytes.Length);

            FileStream.Close();
            FileStream.Dispose();
        }


    }


}
