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


    public class UserAction : IHttpHandler, IReadOnlySessionState
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

                case "classify":
                    Classify(Context);
                    break;

                case "transfer":
                    Transfer(Context);
                    break;

                case "remove":
                    Remove(Context);
                    break;

                case "removeall":
                    RemoveAll(Context);
                    break;

                case "restore":
                    Restore(Context);
                    break;

                case "restoreall":
                    RestoreAll(Context);
                    break;

                case "delete":
                    Delete(Context);
                    break;

                case "deleteall":
                    DeleteAll(Context);
                    break;
            }

            Conn.Close();
            Conn.Dispose();
        }


        /// <summary>
        /// 用户添加
        /// </summary>
        private void Add(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;
            int DepartmentId = 0;
            string DepartmentIdPath = "";
            int RoleId = 0;
            string Username = "";
            string Password = "";
            string Code = "";
            string Position = "";
            string Email = "";
            string Phone = "";
            string Tel = "";
            string Admin = "";
            string Send = "";
            string Sql = "";

            if (Base.Common.IsNumeric(Context.Request.Form["DepartmentId"]) == true)
            {
                DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["RoleId"]) == true)
            {
                RoleId = Context.Request.Form["RoleId"].TypeInt();
            }
            else
            {
                return;
            }

            Username = Context.Request.Form["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return;
            }

            Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\S]{6,16}$") == false)
            {
                return;
            }

            Password = Base.Common.StringCrypto(Password, "MD5");

            Code = Context.Request.Form["Code"].TypeString();

            if (string.IsNullOrEmpty(Code) == false)
            {
                if (Base.Common.StringCheck(Code, @"^[\w\-]{2,16}$") == false)
                {
                    return;
                }
            }

            Position = Base.Common.InputFilter(Context.Request.Form["Position"].TypeString());

            if (string.IsNullOrEmpty(Position) == false)
            {
                if (Base.Common.StringCheck(Position, @"^[\s\S]{2,32}$") == false)
                {
                    return;
                }
            }

            Email = Context.Request.Form["Email"].TypeString();

            if (string.IsNullOrEmpty(Email) == false)
            {
                if (Base.Common.StringCheck(Email, @"^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
                {
                    return;
                }
            }

            Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return;
                }
            }

            Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return;
                }
            }

            Admin = Context.Request.Form["Admin"].TypeString();

            if (Base.Common.StringCheck(Admin, @"^(true|false)$") == false)
            {
                return;
            }
            else
            {
                Admin = Admin == "true" ? "1" : "0";
            }

            Send = Context.Request.Form["Send"].TypeString();

            if (Base.Common.StringCheck(Send, @"^(true|false)$") == false)
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Username From DBS_User Where DBS_Username = '" + Username + "'", ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == true)
            {
                Context.Response.Write("username-existed");
                return;
            }

            UserTable.Clear();

            if (string.IsNullOrEmpty(Email) == false)
            {
                Base.Data.SqlDataToTable("Select DBS_Email From DBS_User Where DBS_Email = '" + Email + "'", ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == true)
                {
                    Context.Response.Write("email-existed");
                    return;
                }

                UserTable.Clear();
            }

            if (string.IsNullOrEmpty(Phone) == false)
            {
                Base.Data.SqlDataToTable("Select DBS_Phone From DBS_User Where DBS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == true)
                {
                    Context.Response.Write("phone-existed");
                    return;
                }

                UserTable.Clear();
            }

            if (DepartmentId == 0)
            {
                DepartmentIdPath = "/0/";
            }
            else
            {
                DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);
            }

            Sql = "Insert Into DBS_User(DBS_DepartmentId, DBS_RoleId, DBS_Username, DBS_Password, DBS_Code, DBS_Position, DBS_Email, DBS_Phone, DBS_Tel, DBS_Admin, DBS_Status, DBS_Recycle, DBS_Time, DBS_LoginIP, DBS_LoginTime) ";
            Sql += "Values('" + DepartmentIdPath + "', " + RoleId + ", '" + Username + "', '" + Password + "', '" + Code + "', '" + Position + "', '" + Email + "', '" + Phone + "', '" + Tel + "', " + Admin + ", 1, 0, '" + DateTime.Now.ToString() + "', '0.0.0.0', '1970/1/1 00:00:00')";

            Id = Base.Data.SqlInsert(Sql, ref Conn);

            if (Id == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(Email) == false && Send == "true")
            {
                Password = Context.Request.Form["Password"].TypeString();

                CreateMail(Username, Password, Email, Context);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户修改
        /// </summary>
        private void Modify(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;
            int DepartmentId = 0;
            string DepartmentIdPath = "";
            int RoleId = 0;
            string Password = "";
            string Code = "";
            string Position = "";
            string Email = "";
            string Phone = "";
            string Tel = "";
            string Admin = "";
            string Status = "";
            string Leave = "";
            string Sql = "";

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

            if (Base.Common.IsNumeric(Context.Request.Form["RoleId"]) == true)
            {
                RoleId = Context.Request.Form["RoleId"].TypeInt();
            }
            else
            {
                return;
            }

            Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[\S]{6,16}$") == true)
            {
                Password = Base.Common.StringCrypto(Password, "MD5");
            }

            Code = Context.Request.Form["Code"].TypeString();

            if (string.IsNullOrEmpty(Code) == false)
            {
                if (Base.Common.StringCheck(Code, @"^[\w\-]{2,16}$") == false)
                {
                    return;
                }
            }

            Position = Base.Common.InputFilter(Context.Request.Form["Position"].TypeString());

            if (string.IsNullOrEmpty(Position) == false)
            {
                if (Base.Common.StringCheck(Position, @"^[\s\S]{2,32}$") == false)
                {
                    return;
                }
            }

            Email = Context.Request.Form["Email"].TypeString();

            if (string.IsNullOrEmpty(Email) == false)
            {
                if (Base.Common.StringCheck(Email, @"^[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
                {
                    return;
                }
            }

            Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return;
                }
            }

            Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return;
                }
            }

            Admin = Context.Request.Form["Admin"].TypeString();

            if (Base.Common.StringCheck(Admin, @"^(true|false)$") == false)
            {
                return;
            }
            else
            {
                Admin = Admin == "true" ? "1" : "0";
            }

            Leave = Context.Request.Form["Leave"].TypeString();

            if (Base.Common.StringCheck(Leave, @"^(true|false)$") == false)
            {
                return;
            }
            else
            {
                Status = Leave == "true" ? "0" : "1";
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            if (string.IsNullOrEmpty(Email) == false)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Email From DBS_User Where DBS_Email = '" + Email + "'", ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == true)
                {
                    if (UserTable["DBS_Id"].TypeInt() != Id)
                    {
                        Context.Response.Write("email-existed");
                        return;
                    }
                }

                UserTable.Clear();
            }

            if (string.IsNullOrEmpty(Phone) == false)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Phone From DBS_User Where DBS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == true)
                {
                    if (UserTable["DBS_Id"].TypeInt() != Id)
                    {
                        Context.Response.Write("phone-existed");
                        return;
                    }
                }

                UserTable.Clear();
            }

            if (DepartmentId == 0)
            {
                DepartmentIdPath = "/0/";
            }
            else
            {
                DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);
            }

            if (string.IsNullOrEmpty(Password) == true)
            {
                Sql = "Update DBS_User Set DBS_DepartmentId = '" + DepartmentIdPath + "', DBS_RoleId = " + RoleId + ", DBS_Code = '" + Code + "', DBS_Position = '" + Position + "', DBS_Email = '" + Email + "', DBS_Phone = '" + Phone + "', DBS_Tel = '" + Tel + "', DBS_Admin = " + Admin + ", DBS_Status = " + Status + " Where DBS_Id = " + Id;
            }
            else
            {
                Sql = "Update DBS_User Set DBS_DepartmentId = '" + DepartmentIdPath + "', DBS_RoleId = " + RoleId + ", DBS_Password = '" + Password + "', DBS_Code = '" + Code + "', DBS_Position = '" + Position + "', DBS_Email = '" + Email + "', DBS_Phone = '" + Phone + "', DBS_Tel = '" + Tel + "', DBS_Admin = " + Admin + ", DBS_Status = " + Status + " Where DBS_Id = " + Id;
            }

            Base.Data.SqlQuery(Sql, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户归类
        /// </summary>
        private void Classify(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;
            int ClassifyId = 0;
            string ClassifyType = "";
            string DepartmentIdPath = "";
            int Index = 0;

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["ClassifyId"]) == true)
            {
                ClassifyId = Context.Request.Form["ClassifyId"].TypeInt();
            }
            else
            {
                return;
            }

            ClassifyType = Context.Request.Form["ClassifyType"].TypeString();

            if (Base.Common.StringCheck(ClassifyType, @"^(department|role)$") == false)
            {
                return;
            }

            if (ClassifyType == "department")
            {
                DepartmentIdPath = AppCommon.DepartmentIdPath(ClassifyId, ref Conn);
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

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                UserTable.Clear();

                if (ClassifyType == "department")
                {
                    Base.Data.SqlQuery("Update DBS_User Set DBS_DepartmentId = '" + DepartmentIdPath + "' Where DBS_Id = " + Id, ref Conn);
                }
                else if (ClassifyType == "role")
                {
                    Base.Data.SqlQuery("Update DBS_User Set DBS_RoleId = " + ClassifyId + " Where DBS_Id = " + Id, ref Conn);
                }
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户移交
        /// </summary>
        private void Transfer(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int FromUserId = 0;
            int ToUserId = 0;
            string ToUsername = "";

            if (Base.Common.IsNumeric(Context.Request.Form["FromUserId"]) == true)
            {
                FromUserId = Context.Request.Form["FromUserId"].TypeInt();
            }
            else
            {
                return;
            }

            if (Base.Common.IsNumeric(Context.Request.Form["ToUserId"]) == true)
            {
                ToUserId = Context.Request.Form["ToUserId"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + FromUserId, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Username From DBS_User Where DBS_Id = " + ToUserId, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                ToUsername = UserTable["DBS_Username"].TypeString();
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Update DBS_File Set DBS_UserId = " + ToUserId + ", DBS_Username = '" + ToUsername + "' Where DBS_UserId = " + FromUserId, ref Conn);
            Base.Data.SqlQuery("Update DBS_File_Purview Set DBS_UserId = " + ToUserId + " Where DBS_UserId = " + FromUserId, ref Conn);

            Base.Data.SqlQuery("Delete Top (1) From DBS_File_Purview Where Exists (Select * From (Select * From DBS_File_Purview Where DBS_UserId = " + ToUserId + ") As Temp Where DBS_File_Purview.DBS_FileId = Temp.DBS_FileId And DBS_File_Purview.DBS_UserId = Temp.DBS_UserId Group By Temp.DBS_FileId, Temp.DBS_UserId Having Count(*) > 1)", ref Conn);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户移除
        /// </summary>
        private void Remove(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Update DBS_User Set DBS_Recycle = 1 Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户批量移除
        /// </summary>
        private void RemoveAll(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
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

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Update DBS_User Set DBS_Recycle = 1 Where DBS_Id = " + Id, ref Conn);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户还原
        /// </summary>
        private void Restore(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Update DBS_User Set DBS_Recycle = 0 Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户批量还原
        /// </summary>
        private void RestoreAll(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
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

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Update DBS_User Set DBS_Recycle = 0 Where DBS_Id = " + Id, ref Conn);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户删除
        /// </summary>
        private void Delete(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
            int Id = 0;

            if (Base.Common.IsNumeric(Context.Request.Form["Id"]) == true)
            {
                Id = Context.Request.Form["Id"].TypeInt();
            }
            else
            {
                return;
            }

            Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

            if (UserTable["Exist"].TypeBool() == false)
            {
                return;
            }

            UserTable.Clear();

            Base.Data.SqlQuery("Delete From DBS_User Where DBS_Id = " + Id, ref Conn);

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 用户批量删除
        /// </summary>
        private void DeleteAll(HttpContext Context)
        {
            Hashtable UserTable = new Hashtable();
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

                Base.Data.SqlDataToTable("Select DBS_Id From DBS_User Where DBS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exist"].TypeBool() == false)
                {
                    continue;
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Delete From DBS_User Where DBS_Id = " + Id, ref Conn);
            }

            DataToJson(Context);

            Context.Response.Write("complete");
        }


        /// <summary>
        /// 发送账号创建邮件
        /// </summary>
        private void CreateMail(string Username, string Password, string Email, HttpContext Context)
        {
            Hashtable MailTable = new Hashtable();
            string AppName = "";
            string Subject = "";
            string Content = "";

            AppCommon.XmlDataReader(Context.Server.MapPath("/storage/config/mail-template.xml"), "/template/user-create", MailTable);

            if (MailTable.Count == 0)
            {
                return;
            }

            AppName = ConfigurationManager.AppSettings["AppName"].TypeString();

            Subject = MailTable["subject"].TypeString();
            Subject = Subject.Replace("{appname}", AppName);
            Subject = Subject.Replace("{username}", Username);

            Content = MailTable["content"].TypeString();
            Content = Content.Replace("{appname}", AppName);
            Content = Content.Replace("{username}", Username);
            Content = Content.Replace("{password}", Password);
            Content = Content.Replace("{url}", "" + Context.Request.Url.Scheme + "://" + Context.Request.Url.Host + "/");

            AppCommon.SendMail(Email, Subject, Content);
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

            Json = Base.Data.SqlListToJson("Select DBS_Id, DBS_DepartmentId, DBS_RoleId, DBS_Username, DBS_Position, DBS_Status, DBS_Recycle From DBS_User Where DBS_Status = 1 And DBS_Recycle = 0 Order By DBS_Username Asc, DBS_Id Asc", ref Conn);

            ScriptPath = Context.Server.MapPath("/storage/data/user-data-json.js");

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
