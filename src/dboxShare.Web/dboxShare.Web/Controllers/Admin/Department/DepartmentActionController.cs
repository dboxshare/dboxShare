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


    public class DepartmentActionController : ApiController
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
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            string Name = Context.Request.Form["Name"].TypeString();

            if (string.IsNullOrEmpty(Name) == true)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                string[] Names = Name.Split('\n');

                for (int i = 0; i < Names.Length; i++)
                {
                    Name = Names[i];

                    if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{2,24}$") == false)
                    {
                        continue;
                    }

                    Hashtable DepartmentTable = new Hashtable();

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
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            int DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            string Name = Context.Request.Form["Name"].TypeString();

            if (Base.Common.StringCheck(Name, @"^[^\\\/\:\*\?\""\<\>\|]{2,24}$") == false)
            {
                return AppCommon.ResponseMessage("parameter item check failed");
            }
            else
            {
                Name = Base.Common.InputFilter(Name);
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable DepartmentTable = new Hashtable();

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
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                Hashtable DepartmentTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id From DS_Department Where DS_Id = " + Id, ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
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
            ArrayList Departments = new ArrayList();

            Base.Data.SqlListToArray("DS_Id", "Select DS_Id, DS_DepartmentId From DS_Department Where DS_DepartmentId = " + Id, ref Conn, ref Departments);

            for (int i = 0; i < Departments.Count; i++)
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
                return AppCommon.ResponseMessage("login authentication failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no operation data");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (int i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    int Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    Hashtable DepartmentTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id From DS_Department Where DS_Id = " + Id, ref Conn, ref DepartmentTable);

                    if (DepartmentTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    DepartmentTable.Clear();

                    int Sequence = Context.Request.Form.GetValues("Id").Length - i;

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
            string Json = Base.Data.SqlListToJson("Select DS_Id, DS_DepartmentId, DS_Name, DS_Sequence From DS_Department Order By DS_DepartmentId Asc, DS_Sequence Desc, DS_Id Asc", ref Conn);

            string ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "department-data-json.js");

            string ScriptVariable = Base.Common.StringGet(File.ReadAllText(ScriptPath), @"^var\s+(\w+)\s+=");

            string ScriptContent = "var " + ScriptVariable + " = " + Json + ";";

            byte[] Bytes = new UTF8Encoding(true).GetBytes(ScriptContent);

            using (FileStream FStream = File.Create(ScriptPath))
            {
                FStream.Write(Bytes, 0, Bytes.Length);
            }
        }


    }


}
