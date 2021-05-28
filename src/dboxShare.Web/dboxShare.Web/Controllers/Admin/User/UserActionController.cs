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


    public class Admin_UserActionController : ApiController
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
        /// 用户添加
        /// </summary>
        [Route("api/admin/user/add")]
        [HttpPost]
        public HttpResponseMessage Add()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            var RoleId = Context.Request.Form["RoleId"].TypeInt();

            var Username = Context.Request.Form["Username"].TypeString();

            if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Password = Context.Request.Form["Password"].TypeString();

            if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Name = Context.Request.Form["Name"].TypeString();

            if (string.IsNullOrEmpty(Name) == false)
            {
                Name = Base.Common.InputFilter(Name);

                if (Base.Common.StringCheck(Name, @"^[\s\S]{2,24}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Code = Context.Request.Form["Code"].TypeString();

            if (string.IsNullOrEmpty(Code) == false)
            {
                if (Base.Common.StringCheck(Code, @"^[\w\-]{2,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Title = Context.Request.Form["Title"].TypeString();

            if (string.IsNullOrEmpty(Title) == false)
            {
                Title = Base.Common.InputFilter(Title);

                if (Base.Common.StringCheck(Title, @"^[\s\S]{2,32}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Email = Context.Request.Form["Email"].TypeString();

            if (string.IsNullOrEmpty(Email) == false)
            {
                if (Base.Common.StringCheck(Email, @"^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var UploadSize = Context.Request.Form["UploadSize"].TypeInt();

            if (UploadSize < 0 || UploadSize > 10240)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var DownloadSize = Context.Request.Form["DownloadSize"].TypeInt();

            if (DownloadSize < 0 || DownloadSize > 10240)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Admin = Context.Request.Form["Admin"].TypeString() == "true" ? 1 : 0;

            var Send = Context.Request.Form["Send"].TypeString() == "true" ? true : false;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 判断用户账号是否存在
                Base.Data.SqlDataToTable("Select DS_Username From DS_User Where DS_Username = '" + Username + "'", ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == true)
                {
                    return AppCommon.ResponseMessage("username-existed");
                }

                UserTable.Clear();

                // 判断电子邮箱是否存在
                if (string.IsNullOrEmpty(Email) == false)
                {
                    Base.Data.SqlDataToTable("Select DS_Email From DS_User Where DS_Email = '" + Email + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        return AppCommon.ResponseMessage("email-existed");
                    }

                    UserTable.Clear();
                }

                // 判断手机号码是否存在
                if (string.IsNullOrEmpty(Phone) == false)
                {
                    Base.Data.SqlDataToTable("Select DS_Phone From DS_User Where DS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        return AppCommon.ResponseMessage("phone-existed");
                    }

                    UserTable.Clear();
                }

                var DepartmentIdPath = "";

                if (DepartmentId == 0)
                {
                    DepartmentIdPath = "/0/";
                }
                else
                {
                    DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);
                }

                var PasswordMD5 = Base.Common.StringHash(Password, "MD5");

                var Sql = "Insert Into DS_User(DS_DepartmentId, DS_RoleId, DS_DomainId, DS_Username, DS_Password, DS_Name, DS_Code, DS_Title, DS_Email, DS_Phone, DS_Tel, DS_UploadSize, DS_DownloadSize, DS_Admin, DS_Freeze, DS_Recycle, DS_Time, DS_LoginIP, DS_LoginTime) " +
                          "Values('" + DepartmentIdPath + "', " + RoleId + ", 'null', '" + Username + "', '" + PasswordMD5 + "', '" + Name + "', '" + Code + "', '" + Title + "', '" + Email + "', '" + Phone + "', '" + Tel + "', " + UploadSize + ", " + DownloadSize + ", " + Admin + ", 0, 0, '" + DateTime.Now.ToString() + "', '0.0.0.0', '1970/1/1 00:00:00')";

                var Id = Base.Data.SqlInsert(Sql, ref Conn);

                if (Id == 0)
                {
                    return AppCommon.ResponseMessage("data-insertion-failed");
                }

                if (string.IsNullOrEmpty(Email) == false && Send == true)
                {
                    CreateMail(Username, Password, Email);
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
        /// 用户批量创建
        /// </summary>
        [Route("api/admin/user/create")]
        [HttpPost]
        public HttpResponseMessage Create()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Username").Length == 0)
            {
                return AppCommon.ResponseMessage("no-valid-data");
            }

            var DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var DepartmentIdPath = "";

                if (DepartmentId == 0)
                {
                    DepartmentIdPath = "/0/";
                }
                else
                {
                    DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);
                }

                for (var i = 0; i < Context.Request.Form.GetValues("Username").Length; i++)
                {
                    var Username = Context.Request.Form.GetValues("Username")[i].TypeString();

                    if (Base.Common.StringCheck(Username, @"^[^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\|\,\.\<\>\/\?]{2,16}$") == false)
                    {
                        continue;
                    }

                    var Password = Context.Request.Form.GetValues("Password")[i].TypeString();

                    if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
                    {
                        continue;
                    }

                    var Name = Context.Request.Form.GetValues("Name")[i].TypeString();

                    if (string.IsNullOrEmpty(Name) == false)
                    {
                        Name = Base.Common.InputFilter(Name);

                        if (Base.Common.StringCheck(Name, @"^[\s\S]{2,24}$") == false)
                        {
                            continue;
                        }
                    }

                    var Code = Context.Request.Form.GetValues("Code")[i].TypeString();

                    if (string.IsNullOrEmpty(Code) == false)
                    {
                        if (Base.Common.StringCheck(Code, @"^[\w\-]{2,16}$") == false)
                        {
                            continue;
                        }
                    }

                    var Title = Context.Request.Form.GetValues("Title")[i].TypeString();

                    if (string.IsNullOrEmpty(Title) == false)
                    {
                        Title = Base.Common.InputFilter(Title);

                        if (Base.Common.StringCheck(Title, @"^[\s\S]{2,32}$") == false)
                        {
                            continue;
                        }
                    }

                    var Email = Context.Request.Form.GetValues("Email")[i].TypeString();

                    if (string.IsNullOrEmpty(Email) == false)
                    {
                        if (Base.Common.StringCheck(Email, @"^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
                        {
                            continue;
                        }
                    }

                    var Phone = Context.Request.Form.GetValues("Phone")[i].TypeString();

                    if (string.IsNullOrEmpty(Phone) == false)
                    {
                        if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                        {
                            continue;
                        }
                    }

                    var Tel = Context.Request.Form.GetValues("Tel")[i].TypeString();

                    if (string.IsNullOrEmpty(Tel) == false)
                    {
                        if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                        {
                            continue;
                        }
                    }

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    // 判断用户账号是否存在
                    Base.Data.SqlDataToTable("Select DS_Username From DS_User Where DS_Username = '" + Username + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    // 判断电子邮箱是否存在
                    if (string.IsNullOrEmpty(Email) == false)
                    {
                        Base.Data.SqlDataToTable("Select DS_Email From DS_User Where DS_Email = '" + Email + "'", ref Conn, ref UserTable);


                        if (UserTable["Exists"].TypeBool() == true)
                        {
                            continue;
                        }

                        UserTable.Clear();
                    }

                    // 判断手机号码是否存在
                    if (string.IsNullOrEmpty(Phone) == false)
                    {
                        Base.Data.SqlDataToTable("Select DS_Phone From DS_User Where DS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                        if (UserTable["Exists"].TypeBool() == true)
                        {
                            continue;
                        }

                        UserTable.Clear();
                    }

                    var PasswordMD5 = Base.Common.StringHash(Password, "MD5");

                    var Sql = "Insert Into DS_User(DS_DepartmentId, DS_RoleId, DS_DomainId, DS_Username, DS_Password, DS_Name, DS_Code, DS_Title, DS_Email, DS_Phone, DS_Tel, DS_UploadSize, DS_DownloadSize, DS_Admin, DS_Freeze, DS_Recycle, DS_Time, DS_LoginIP, DS_LoginTime) " +
                              "Values('" + DepartmentIdPath + "', 0, 'null', '" + Username + "', '" + PasswordMD5 + "', '" + Name + "', '" + Code + "', '" + Title + "', '" + Email + "', '" + Phone + "', '" + Tel + "', 0, 0, 0, 0, 0, '" + DateTime.Now.ToString() + "', '0.0.0.0', '1970/1/1 00:00:00')";

                    var Id = Base.Data.SqlInsert(Sql, ref Conn);

                    if (Id == 0)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(Email) == false)
                    {
                        CreateMail(Username, Password, Email);
                    }
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
        /// 用户修改
        /// </summary>
        [Route("api/admin/user/modify")]
        [HttpPost]
        public HttpResponseMessage Modify()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var Id = Context.Request.Form["Id"].TypeInt();

            var DepartmentId = Context.Request.Form["DepartmentId"].TypeInt();

            var RoleId = Context.Request.Form["RoleId"].TypeInt();

            var Password = Context.Request.Form["Password"].TypeString();

            if (string.IsNullOrEmpty(Password) == false)
            {
                if (Base.Common.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Name = Context.Request.Form["Name"].TypeString();

            if (string.IsNullOrEmpty(Name) == false)
            {
                Name = Base.Common.InputFilter(Name);

                if (Base.Common.StringCheck(Name, @"^[\s\S]{2,24}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Code = Context.Request.Form["Code"].TypeString();

            if (string.IsNullOrEmpty(Code) == false)
            {
                if (Base.Common.StringCheck(Code, @"^[\w\-]{2,16}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Title = Context.Request.Form["Title"].TypeString();

            if (string.IsNullOrEmpty(Title) == false)
            {
                Title = Base.Common.InputFilter(Title);

                if (Base.Common.StringCheck(Title, @"^[\s\S]{2,32}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Email = Context.Request.Form["Email"].TypeString();

            if (string.IsNullOrEmpty(Email) == false)
            {
                if (Base.Common.StringCheck(Email, @"^[\w\-\.]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Phone = Context.Request.Form["Phone"].TypeString();

            if (string.IsNullOrEmpty(Phone) == false)
            {
                if (Base.Common.StringCheck(Phone, @"^\+?([\d]{2,4}\-?)?[\d]{6,11}$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var Tel = Context.Request.Form["Tel"].TypeString();

            if (string.IsNullOrEmpty(Tel) == false)
            {
                if (Base.Common.StringCheck(Tel, @"^\+?([\d]{2,4}\-?){0,2}[\d]{6,8}(\-?[\d]{2,8})?$") == false)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var UploadSize = Context.Request.Form["UploadSize"].TypeInt();

            if (UploadSize < 0 || UploadSize > 10240)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var DownloadSize = Context.Request.Form["DownloadSize"].TypeInt();

            if (DownloadSize < 0 || DownloadSize > 10240)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            var Admin = Context.Request.Form["Admin"].TypeString() == "true" ? 1 : 0;

            var Freeze = Context.Request.Form["Freeze"].TypeString() == "true" ? 1 : 0;

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                UserTable.Clear();

                // 判断电子邮箱是否存在
                if (string.IsNullOrEmpty(Email) == false)
                {
                    Base.Data.SqlDataToTable("Select DS_Id, DS_Email From DS_User Where DS_Email = '" + Email + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        if (UserTable["DS_Id"].TypeInt() != Id)
                        {
                            return AppCommon.ResponseMessage("email-existed");
                        }
                    }

                    UserTable.Clear();
                }

                // 判断手机号码是否存在
                if (string.IsNullOrEmpty(Phone) == false)
                {
                    Base.Data.SqlDataToTable("Select DS_Id, DS_Phone From DS_User Where DS_Phone = '" + Phone + "'", ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == true)
                    {
                        if (UserTable["DS_Id"].TypeInt() != Id)
                        {
                            return AppCommon.ResponseMessage("phone-existed");
                        }
                    }

                    UserTable.Clear();
                }

                var DepartmentIdPath = "";

                if (DepartmentId == 0)
                {
                    DepartmentIdPath = "/0/";
                }
                else
                {
                    DepartmentIdPath = AppCommon.DepartmentIdPath(DepartmentId, ref Conn);
                }

                var Sql = "";

                if (string.IsNullOrEmpty(Password) == true)
                {
                    Sql = "Update DS_User Set DS_DepartmentId = '" + DepartmentIdPath + "', DS_RoleId = " + RoleId + ", DS_Name = '" + Name + "', DS_Code = '" + Code + "', DS_Title = '" + Title + "', DS_Email = '" + Email + "', DS_Phone = '" + Phone + "', DS_Tel = '" + Tel + "', DS_UploadSize = " + UploadSize + ", DS_DownloadSize = " + DownloadSize + ", DS_Admin = " + Admin + ", DS_Freeze = " + Freeze + " Where DS_Id = " + Id;
                }
                else
                {
                    var PasswordMD5 = Base.Common.StringHash(Password, "MD5");

                    Sql = "Update DS_User Set DS_DepartmentId = '" + DepartmentIdPath + "', DS_RoleId = " + RoleId + ", DS_Password = '" + PasswordMD5 + "', DS_Name = '" + Name + "', DS_Code = '" + Code + "', DS_Title = '" + Title + "', DS_Email = '" + Email + "', DS_Phone = '" + Phone + "', DS_Tel = '" + Tel + "', DS_UploadSize = " + UploadSize + ", DS_DownloadSize = " + DownloadSize + ", DS_Admin = " + Admin + ", DS_Freeze = " + Freeze + " Where DS_Id = " + Id;
                }

                Base.Data.SqlQuery(Sql, ref Conn);

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
        /// 用户更改
        /// </summary>
        [Route("api/admin/user/change")]
        [HttpPost]
        public HttpResponseMessage Change()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            var UploadSize = Context.Request.Form["UploadSize"].TypeString();

            if (string.IsNullOrEmpty(UploadSize) == false)
            {
                if (UploadSize.TypeInt() < 1 || UploadSize.TypeInt() > 10240)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            var DownloadSize = Context.Request.Form["DownloadSize"].TypeString();

            if (string.IsNullOrEmpty(DownloadSize) == false)
            {
                if (DownloadSize.TypeInt() < 1 || DownloadSize.TypeInt() > 10240)
                {
                    return AppCommon.ResponseMessage("parameter-check-failed");
                }
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    if (string.IsNullOrEmpty(UploadSize) == false)
                    {
                        Base.Data.SqlQuery("Update DS_User Set DS_UploadSize = " + UploadSize + " Where DS_Id = " + Id, ref Conn);
                    }
                    else if (string.IsNullOrEmpty(DownloadSize) == false)
                    {
                        Base.Data.SqlQuery("Update DS_User Set DS_DownloadSize = " + DownloadSize + " Where DS_Id = " + Id, ref Conn);
                    }
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
        /// 用户归类
        /// </summary>
        [Route("api/admin/user/classify")]
        [HttpPost]
        public HttpResponseMessage Classify()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            if (Context.Request.Form.GetValues("Id").Length == 0)
            {
                return AppCommon.ResponseMessage("no-data");
            }

            var ClassifyId = Context.Request.Form["ClassifyId"].TypeInt();

            var ClassifyType = Context.Request.Form["ClassifyType"].TypeString();

            if (Base.Common.StringCheck(ClassifyType, @"^(department|role)$") == false)
            {
                return AppCommon.ResponseMessage("parameter-check-failed");
            }

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var DepartmentIdPath = "";

                if (ClassifyType == "department")
                {
                    DepartmentIdPath = AppCommon.DepartmentIdPath(ClassifyId, ref Conn);
                }

                for (var i = 0; i < Context.Request.Form.GetValues("Id").Length; i++)
                {
                    var Id = Context.Request.Form.GetValues("Id")[i].TypeInt();

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    if (ClassifyType == "department")
                    {
                        Base.Data.SqlQuery("Update DS_User Set DS_DepartmentId = '" + DepartmentIdPath + "' Where DS_Id = " + Id, ref Conn);
                    }
                    else if (ClassifyType == "role")
                    {
                        Base.Data.SqlQuery("Update DS_User Set DS_RoleId = " + ClassifyId + " Where DS_Id = " + Id, ref Conn);
                    }
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
        /// 用户移交
        /// </summary>
        [Route("api/admin/user/transfer")]
        [HttpPost]
        public HttpResponseMessage Transfer()
        {
            if (AppCommon.LoginAuth("Web") == false || Context.Session["Admin"].TypeInt() == 0)
            {
                return AppCommon.ResponseMessage("login-authentication-failed");
            }

            var FromUserId = Context.Request.Form["FromUserId"].TypeInt();

            var ToUserId = Context.Request.Form["ToUserId"].TypeInt();

            try
            {
                Conn = Base.Data.DBConnection(AppConfig.ConnectionString);

                Conn.Open();

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + FromUserId, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                UserTable.Clear();

                Base.Data.SqlDataToTable("Select DS_Id, DS_Username From DS_User Where DS_Id = " + ToUserId, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                var ToUsername = UserTable["DS_Username"].TypeString();

                UserTable.Clear();

                Base.Data.SqlQuery("Update DS_File Set DS_UserId = " + ToUserId + ", DS_Username = '" + ToUsername + "' Where DS_UserId = " + FromUserId, ref Conn);
                Base.Data.SqlQuery("Update DS_File_Purview Set DS_UserId = " + ToUserId + " Where DS_UserId = " + FromUserId, ref Conn);

                // 删除多余共享权限记录
                Base.Data.SqlQuery("Delete Top (1) From DS_File_Purview Where Exists (Select * From (Select * From DS_File_Purview Where DS_UserId = " + ToUserId + ") As Temp Where DS_File_Purview.DS_FileId = Temp.DS_FileId And DS_File_Purview.DS_UserId = Temp.DS_UserId Group By Temp.DS_FileId, Temp.DS_UserId Having Count(*) > 1)", ref Conn);

                return AppCommon.ResponseMessage("complete");
            }
            finally
            {
                Conn.Close();
                Conn.Dispose();
            }
        }


        /// <summary>
        /// 用户移除
        /// </summary>
        [Route("api/admin/user/remove")]
        [HttpPost]
        public HttpResponseMessage Remove()
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

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Update DS_User Set DS_Recycle = 1 Where DS_Id = " + Id, ref Conn);

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
        /// 用户批量移除
        /// </summary>
        [Route("api/admin/user/remove-all")]
        [HttpPost]
        public HttpResponseMessage RemoveAll()
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

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    Base.Data.SqlQuery("Update DS_User Set DS_Recycle = 1 Where DS_Id = " + Id, ref Conn);
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
        /// 用户还原
        /// </summary>
        [Route("api/admin/user/restore")]
        [HttpPost]
        public HttpResponseMessage Restore()
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

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Update DS_User Set DS_Recycle = 0 Where DS_Id = " + Id, ref Conn);

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
        /// 用户批量还原
        /// </summary>
        [Route("api/admin/user/restore-all")]
        [HttpPost]
        public HttpResponseMessage RestoreAll()
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

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    Base.Data.SqlQuery("Update DS_User Set DS_Recycle = 0 Where DS_Id = " + Id, ref Conn);
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
        /// 用户删除
        /// </summary>
        [Route("api/admin/user/delete")]
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

                var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                if (UserTable["Exists"].TypeBool() == false)
                {
                    return AppCommon.ResponseMessage("data-not-exist");
                }

                UserTable.Clear();

                Base.Data.SqlQuery("Delete From DS_User Where DS_Id = " + Id, ref Conn);
                Base.Data.SqlQuery("Delete From DS_User_Log Where DS_UserId = " + Id, ref Conn);

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
        /// 用户批量删除
        /// </summary>
        [Route("api/admin/user/delete-all")]
        [HttpPost]
        public HttpResponseMessage DeleteAll()
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

                    var UserTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                    Base.Data.SqlDataToTable("Select DS_Id From DS_User Where DS_Id = " + Id, ref Conn, ref UserTable);

                    if (UserTable["Exists"].TypeBool() == false)
                    {
                        continue;
                    }

                    UserTable.Clear();

                    Base.Data.SqlQuery("Delete From DS_User Where DS_Id = " + Id, ref Conn);
                    Base.Data.SqlQuery("Delete From DS_User_Log Where DS_UserId = " + Id, ref Conn);
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
        /// 发送账号创建邮件
        /// </summary>
        private void CreateMail(string Username, string Password, string Email)
        {
            var MailTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            AppCommon.XmlDataReader(Base.Common.PathCombine(AppConfig.DataStoragePath, "mail-template.xml"), "/template/user-create", MailTable);

            if (MailTable.Count == 0)
            {
                return;
            }

            var Subject = MailTable["Subject"].TypeString();

            Subject = Subject.Replace("{@appname}", AppConfig.AppName);
            Subject = Subject.Replace("{@username}", Username);

            var Content = MailTable["Content"].TypeString();

            Content = Content.Replace("{@appname}", AppConfig.AppName);
            Content = Content.Replace("{@username}", Username);
            Content = Content.Replace("{@password}", Password);
            Content = Content.Replace("{@url}", "" + Context.Request.Url.Scheme + "://" + Context.Request.Url.Host + "/");

            Base.Common.SendMail(AppConfig.AppName, AppConfig.MailAddress, AppConfig.MailUsername, AppConfig.MailPassword, AppConfig.MailServer, AppConfig.MailServerPort, AppConfig.MailServerSSL, Email, Subject, Content, true);
        }


        /// <summary>
        /// 读取用户数据导出 JSON 格式文件
        /// </summary>
        private void DataToJson()
        {
            var Json = Base.Data.SqlListToJson("Select DS_Id, DS_DepartmentId, DS_RoleId, DS_Username, DS_Title, DS_Freeze, DS_Recycle From DS_User Where DS_Freeze = 0 And DS_Recycle = 0 Order By DS_Username Asc, DS_Id Asc", ref Conn);

            var ScriptPath = Base.Common.PathCombine(AppConfig.DataStoragePath, "user-data-json.js");

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
