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


    public class RoleActionController : ApiController
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
        /// 角色添加
        /// </summary>
        [Route("api/admin/role/add")]
        [HttpPost]
        public HttpResponseMessage Add()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

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

                    Hashtable RoleTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Name From DS_Role Where DS_Name = '" + Name + "'", ref Conn, ref RoleTable);

                    if (RoleTable["Exists"].TypeBool() == true)
                    {
                        continue;
                    }

                    RoleTable.Clear();

                    Base.Data.SqlQuery("Insert Into DS_Role(DS_Name, DS_Sequence) Values('" + Name + "', 0)", ref Conn);
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
        /// 角色修改
        /// </summary>
        [Route("api/admin/role/modify")]
        [HttpPost]
        public HttpResponseMessage Modify()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login authentication failed");
            }

            int Id = Context.Request.Form["Id"].TypeInt();

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

                Hashtable RoleTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Name From DS_Role Where DS_Name = '" + Name + "'", ref Conn, ref RoleTable);

                if (RoleTable["Exists"].TypeBool() == true)
                {
                    if (RoleTable["DS_Id"].TypeInt() != Id)
                    {
                        return AppCommon.ResponseMessage("existed");
                    }
                }

                RoleTable.Clear();

                Base.Data.SqlQuery("Update DS_Role Set DS_Name = '" + Name + "' Where DS_Id = " + Id, ref Conn);

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
        /// 角色删除
        /// </summary>
        [Route("api/admin/role/delete")]
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

                Hashtable RoleTable = new Hashtable();

                Base.Data.SqlDataToTable("Select DS_Id From DS_Role Where DS_Id = " + Id, ref Conn, ref RoleTable);

                if (RoleTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data does not exist or is invalid");
                }

                RoleTable.Clear();

                Base.Data.SqlQuery("Delete From DS_Role Where DS_Id = " + Id, ref Conn);

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
        /// 角色排序
        /// </summary>
        [Route("api/admin/role/sort")]
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

                    Hashtable RoleTable = new Hashtable();

                    Base.Data.SqlDataToTable("Select DS_Id From DS_Role Where DS_Id = " + Id, ref Conn, ref RoleTable);

                    if (RoleTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    RoleTable.Clear();

                    int Sequence = Context.Request.Form.GetValues("Id").Length - i;

                    Base.Data.SqlQuery("Update DS_Role Set DS_Sequence = " + Sequence + " Where DS_Id = " + Id, ref Conn);
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
        /// 读取角色数据导出 JSON 格式文件
        /// </summary>
        private void DataToJson()
        {
            string Json = Base.Data.SqlListToJson("Select DS_Id, DS_Name, DS_Sequence From DS_Role Order By DS_Sequence Desc, DS_Id Asc", ref Conn);

            string ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "role-data-json.js");

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
