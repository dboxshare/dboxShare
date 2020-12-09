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


    public class RoleAction : IHttpHandler, IReadOnlySessionState
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
        /// 角色添加
        /// </summary>
        private void Add(HttpContext Context)
        {
            Hashtable RoleTable = new Hashtable();
            int Id = 0;
            string Name = "";

            Name = Base.Common.InputFilter(Context.Request.Form["Name"].TypeString());

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{1,24}$") == false)
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Name From DBS_Role Where DBS_Name = '" + Name + "'", ref Conn, ref RoleTable);

            if (RoleTable["Exist"].TypeBool() == true)
            {
                Context.Response.Write("existed");
                return;
            }

            RoleTable.Clear();

            Id = Base.Data.SqlInsert("Insert Into DBS_Role(DBS_Name, DBS_Sequence) Values('" + Name + "', 0)", ref Conn);

            if (Id == 0)
            {
                return;
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 角色修改
        /// </summary>
        private void Modify(HttpContext Context)
        {
            Hashtable RoleTable = new Hashtable();
            int Id = 0;
            string Name = "";

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
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

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_Role Where DBS_Id = " + Id, ref Conn, ref RoleTable);

            if (RoleTable["Exist"].TypeBool() == false)
            {
                return;
            }

            RoleTable.Clear();

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Name From DBS_Role Where DBS_Name = '" + Name + "'", ref Conn, ref RoleTable);

            if (RoleTable["Exist"].TypeBool() == true)
            {
                if (RoleTable["DBS_Id"].TypeInt() != Id)
                {
                    Context.Response.Write("existed");
                    return;
                }
            }

            RoleTable.Clear();

            Base.Data.SqlQuery("Update DBS_Role Set DBS_Name = '" + Name + "' Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 角色删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            Hashtable RoleTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_Role Where DBS_Id = " + Id, ref Conn, ref RoleTable);

            if (RoleTable["Exist"].TypeBool() == false)
            {
                return;
            }

            RoleTable.Clear();

            Base.Data.SqlQuery("Delete From DBS_Role Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 角色排序
        /// </summary>
        private void Sort(HttpContext Context)
        {
            Hashtable RoleTable = new Hashtable();
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

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_Role Where DBS_Id = " + Id, ref Conn, ref RoleTable);

                if (RoleTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                RoleTable.Clear();

                Base.Data.SqlQuery("Update DBS_Role Set DBS_Sequence = " + (Context.Request.Form.GetValues("Id").Length - Index) + " Where DBS_Id = " + Id, ref Conn);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 读取角色数据导出json格式文件
        /// </summary>
        private void DataToJson(HttpContext Context)
        {
            FileStream FileStream = default(FileStream);
            string ScriptPath = "";
            string ScriptVariable = "";
            string ScriptContent = "";
            byte[] Bytes = {};
            string Json = "";

            Json = Base.Data.SqlListToJson("Select DBS_Id, DBS_Name, DBS_Sequence From DBS_Role Order By DBS_Sequence Desc, DBS_Id Asc", ref Conn);

            ScriptPath = Context.Server.MapPath("/storage/data/role-data-json.js");

            ScriptVariable = Base.Common.StringGet(File.ReadAllText(ScriptPath), @"^var\s+(\w+)\s+=");

            ScriptContent = "var " + ScriptVariable + " = " + Json + ";";

            Bytes = new UTF8Encoding(true).GetBytes(ScriptContent);

            FileStream = File.Create(Context.Server.MapPath("/storage/data/role-data-json.js"));

            FileStream.Write(Bytes, 0, Bytes.Length);

            FileStream.Close();
            FileStream.Dispose();
        }


    }


}
