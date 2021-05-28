using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web.Admin.Controllers
{


    public class Admin_DepartmentActionController : ApiController
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
        /// 部门添加
        /// </summary>
        [Route("api/admin/department/add")]
        [HttpPost]
        public HttpResponseMessage Add()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            var Name = Context.Request.Form["Name"].TypeString();

            if (string.IsNullOrEmpty(Name) == true)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var Names = Name.Split('\n');

                for (var i = 0; i < Names.Length; i++)
                {
                    Name = Names[i];

                    if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{2,24}$") == false)
                    {
                        continue;
                    }

                    var DepartmentTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_DepartmentId, DS_Name From DS_Department Where DS_DepartmentId = " + DepartmentId + " And DS_Name = '" + Name + "'", ref Conn, ref DepartmentTable);

                    if (DepartmentTable["Exists"].TypeBool() == true)
                    {
                        continue;
                    }

                    DepartmentTable.Clear();

                    Base.Data.SqlQuery("Insert Into DS_Department(DS_DepartmentId, DS_Name, DS_Sequence) Values(" + DepartmentId + ", '" + Name + "', 0)", ref Conn);
                }

                DataToJson();

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 部门修改
        /// </summary>
        [Route("api/admin/department/modify")]
        [HttpPost]
        public HttpResponseMessage Modify()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            var Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{2,24}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var DepartmentTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id, DS_DepartmentId, DS_Name From DS_Department Where DS_DepartmentId = " + DepartmentId + " And DS_Name = '" + Name + "'", ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exists"].TypeBool() == true)
                {
                    if (DepartmentTable["DS_Id"].TypeInt() != Id)
                    {
                        return AppCommon.ResponseMessage("existed");
                    }
                }

                DepartmentTable.Clear();

                Base.Data.SqlQuery("Update DS_Department Set DS_Name = '" + Name + "' Where DS_Id = " + Id, ref Conn);

                DataToJson();

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 部门删除
        /// </summary>
        [Route("api/admin/department/delete")]
        [HttpPost]
        public HttpResponseMessage Delete()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var DepartmentTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_Department Where DS_Id = " + Id, ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                DepartmentTable.Clear();

                Base.Data.SqlQuery("Delete From DS_Department Where DS_Id = " + Id, ref Conn);

                Delete_Subitem(Id);

                DataToJson();

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 部门子项目删除
        /// </summary>
        private void Delete_Subitem(int Id)
        {
            var Departments = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_DepartmentId From DS_Department Where DS_DepartmentId = " + Id, ref Conn, ref Departments);

            for (var i = 0; i < Departments.Count; i++)
            {
                Delete_Subitem(Departments[i].TypeInt());
            }

            Base.Data.SqlQuery("Delete From DS_Department Where DS_DepartmentId = " + Id, ref Conn);
        }


        /// <summary>
        /// 部门排序
        /// </summary>
        [Route("api/admin/department/sort")]
        [HttpPost]
        public HttpResponseMessage Sort()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var DepartmentTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_Department Where DS_Id = " + Id, ref Conn, ref DepartmentTable);

                    if (DepartmentTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    DepartmentTable.Clear();

                    var Sequence = Context.Request.Form.GetValues("Id").Length - i;

                    Base.Data.SqlQuery("Update DS_Department Set DS_Sequence = " + Sequence + " Where DS_Id = " + Id, ref Conn);
                }

                DataToJson();

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 读取部门数据导出 JSON 格式文件
        /// </summary>
        private void DataToJson()
        {
            var Json = Base.Data.SqlListToJson("Select DS_Id, DS_DepartmentId, DS_Name, DS_Sequence From DS_Department Order By DS_DepartmentId Asc, DS_Sequence Desc, DS_Id Asc", ref Conn);

            var ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "department-data-json.js");

            var ScriptVariable = Base.Common.StringGet(File.ReadAllText(ScriptPath), @"^var\s+(\w+)\s+=");

            var ScriptContent = "var " + ScriptVariable + " = " + Json + ";";

            var Bytes = new UTF8Encoding(true).GetBytes(ScriptContent);

            using (var FStream = File.Create(ScriptPath))
            {
                FStream.Write(Bytes, 0, Bytes.Length);
            }
        }


    }


}
