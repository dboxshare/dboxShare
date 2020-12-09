using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public static class AppCommon
    {


        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        /// <summary>
        /// 登录验证
        /// </summary>
        public static bool LoginAuth(string Sign)
        {
            string SecurityKey = "";
            string UserId = "";
            string LoginToken = "";

            try
            {
                SecurityKey = ConfigurationManager.AppSettings["SecurityKey"].TypeString();

                UserId = Context.Session["UserId"].TypeString();

                if (string.IsNullOrEmpty(UserId) == true)
                {
                    return false;
                }

                LoginToken = Context.Session["LoginToken"].TypeString();

                if (string.IsNullOrEmpty(LoginToken) == true)
                {
                    return false;
                }

                // 通过session验证登录token
                if (LoginToken != Base.Crypto.TextEncrypt(Context.Session.SessionID, SecurityKey))
                {
                    return false;
                }

                // 通过cache验证登录token
                if (LoginToken != Context.Cache["Login-Token-" + Sign + "-" + UserId + ""].TypeString())
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 生成时间代码id
        /// </summary>
        public static string CodeId()
        {
            string TimeCode = DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff");
            string RandomCode = new Random().Next(100, 999).ToString();

            return "" + TimeCode + "-" + RandomCode + "";
        }


        /// <summary>
        /// 获取文件哈希码
        /// </summary>
        public static string FileHash(string FilePath)
        {
            byte[] Bytes = File.ReadAllBytes(FilePath);
            byte[] MD5 = new MD5CryptoServiceProvider().ComputeHash(Bytes);
            string Hash = "";
            int Index = 0;

            for (Index = 0; Index < MD5.Length; Index++)
            {
                Hash += MD5[Index].ToString("X2");
            }

            return Hash;
        }


        /// <summary>
        /// 获取文件类型
        /// </summary>
        public static string FileType(string Extension)
        {
            Hashtable TypeTable = new Hashtable();
            string[] Extensions = {};
            int Index = 0;

            if (string.IsNullOrEmpty(Extension) == true)
            {
                return "other";
            }

            if (Extension.Substring(0, 1) == ".")
            {
                Extension = Extension.Substring(1);
            }

            AppCommon.XmlDataReader(Context.Server.MapPath("/storage/config/file-type.xml"), "/type", TypeTable);

            if (TypeTable.Count == 0)
            {
                return "other";
            }

            // 文件类型列表
            foreach (string Key in TypeTable.Keys)
            {
                if (string.IsNullOrEmpty(TypeTable[Key].TypeString()) == false)
                {
                    Extensions = TypeTable[Key].TypeString().Split(',');

                    // 查找扩展名列表
                    for (Index = 0; Index < Extensions.Length; Index++)
                    {
                        if (Extensions[Index] == Extension)
                        {
                            return Key;
                        }
                    }
                }
            }

            return "other";
        }


        /// <summary>
        /// 获取文件夹序列名称
        /// </summary>
        public static string FolderName(int FolderId, string FolderName, ref object Conn)
        {
            ArrayList Folders = new ArrayList();
            string Query = "";
            string Sql = "";
            string Order = "";
            int Value = 0;
            int Count = 0;
            int Index = 0;

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_Folder = 1 And " + 
                     "A1.DBS_FolderId = " + FolderId + " And " + 
                     "A1.DBS_Name = '" + FolderName + "' And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_Folder = 1 And " + 
                     "A2.DBS_FolderId = " + FolderId + " And " + 
                     "A2.DBS_Name = '" + FolderName + "' And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_Id Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FolderName + "' And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_Id Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FolderName + "' And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_Id Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_Folder = 1 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FolderName + "' And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_Folder, DBS_FolderId, DBS_Name From DBS_File Where " + Query + " Order By DBS_Id Desc";

            Base.Data.SqlListToArray("DBS_Name", Sql, ref Conn, ref Folders);

            if (Folders.Count == 0)
            {
                return FolderName;
            }

            for (Index = 0; Index < Folders.Count; Index++)
            {
                if (Folders[Index].TypeString().Length > FolderName.Length)
                {
                    Order = Folders[Index].TypeString().Substring(FolderName.Length + 1);
                }
                else
                {
                    continue;
                }

                if (Base.Common.StringCheck(Order, @"^\s\(\d+\)$") == true)
                {
                    Value = Base.Common.StringGet(Order, @"^\s\((\d+)\)$").TypeInt();

                    if (Value > Count)
                    {
                        Count = Value;
                    }
                }
            }

            if (Count == 0)
            {
                return "" + FolderName + " (2)";
            }
            else
            {
                return "" + FolderName + " (" + (Count + 1) + ")";
            }
        }


        /// <summary>
        /// 获取文件序列名称
        /// </summary>
        public static string FileName(int FolderId, string FileName, string FileExtension, ref object Conn)
        {
            ArrayList Files = new ArrayList();
            string Query = "";
            string Sql = "";
            string Order = "";
            int Value = 0;
            int Count = 0;
            int Index = 0;

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DBS_Id From DBS_File As A1 Where " + 
                     "A1.DBS_Id = DBS_File.DBS_Id And " + 
                     "A1.DBS_VersionId = 0 And " + 
                     "A1.DBS_Folder = 0 And " + 
                     "A1.DBS_FolderId = " + FolderId + " And " + 
                     "A1.DBS_Name = '" + FileName + "' And " + 
                     "A1.DBS_Extension = '" + FileExtension + "' And " + 
                     "A1.DBS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DBS_Id From DBS_File As A2 Where " + 
                     "A2.DBS_Id = DBS_File.DBS_Id And " + 
                     "A2.DBS_VersionId = 0 And " + 
                     "A2.DBS_Folder = 0 And " + 
                     "A2.DBS_FolderId = " + FolderId + " And " + 
                     "A2.DBS_Name = '" + FileName + "' And " + 
                     "A2.DBS_Extension = '" + FileExtension + "' And " + 
                     "A2.DBS_CreateUsername = '" + Context.Session["Username"].TypeString() + "' Union All ";

            // 共享部门查询
            Query += "Select B.DBS_Id From DBS_File As B Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = B.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = B.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FileName + "' And " + 
                     "DBS_File.DBS_Extension = '" + FileExtension + "' And " + 
                     "DBS_File_Purview.DBS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Union All ";

            // 共享角色查询
            Query += "Select C.DBS_Id From DBS_File As C Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = C.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = C.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FileName + "' And " + 
                     "DBS_File.DBS_Extension = '" + FileExtension + "' And " + 
                     "DBS_File_Purview.DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " Union All ";

            // 共享用户查询
            Query += "Select D.DBS_Id From DBS_File As D Inner Join DBS_File_Purview On " + 
                     "DBS_File_Purview.DBS_FileId = D.DBS_FolderId Where " + 
                     "DBS_File.DBS_Id = D.DBS_Id And " + 
                     "DBS_File.DBS_VersionId = 0 And " + 
                     "DBS_File.DBS_Folder = 0 And " + 
                     "DBS_File.DBS_FolderId = " + FolderId + " And " + 
                     "DBS_File.DBS_Share = 1 And " + 
                     "DBS_File.DBS_Name = '" + FileName + "' And " + 
                     "DBS_File.DBS_Extension = '" + FileExtension + "' And " + 
                     "DBS_File_Purview.DBS_UserId = " + Context.Session["UserId"].TypeString() + "";

            Query += ")";

            Sql = "Select DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_Name, DBS_Extension From DBS_File Where " + Query + " Order By DBS_Id Desc";

            Base.Data.SqlListToArray("DBS_Name", Sql, ref Conn, ref Files);

            if (Files.Count == 0)
            {
                return FileName;
            }

            for (Index = 0; Index < Files.Count; Index++)
            {
                if (Files[Index].TypeString().Length > FileName.Length)
                {
                    Order = Files[Index].TypeString().Substring(FileName.Length + 1);
                }
                else
                {
                    continue;
                }

                if (Base.Common.StringCheck(Order, @"^\s\(\d+\)$") == true)
                {
                    Value = Base.Common.StringGet(Order, @"^\s\((\d+)\)$").TypeInt();

                    if (Value > Count)
                    {
                        Count = Value;
                    }
                }
            }

            if (Count == 0)
            {
                return "" + FileName + " (2)";
            }
            else
            {
                return "" + FileName + " (" + (Count + 1) + ")";
            }
        }


        /// <summary>
        /// 获取文件版本编号
        /// </summary>
        public static int FileVersionNumber(int FileId, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            int Version = 0;
            string Query = "";
            string Sql = "";

            Query += "Exists (";

            // 文件正本查询
            Query += "Select A.DBS_Id From DBS_File As A Where " + 
                     "A.DBS_Id = DBS_File.DBS_Id And " + 
                     "A.DBS_Id = " + FileId + " Union All ";
            
            // 文件版本查询
            Query += "Select B.DBS_Id From DBS_File As B Where " + 
                     "B.DBS_Id = DBS_File.DBS_Id And " + 
                     "B.DBS_VersionId = " + FileId + "";

            Query += ")";

            Sql = "Select Top 1 DBS_Id, DBS_Version, DBS_VersionId From DBS_File Where " + Query + " Order By DBS_Version Desc";

            Base.Data.SqlDataToTable(Sql, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == true)
            {
                Version = FileTable["DBS_Version"].TypeInt();
            }

            FileTable.Clear();

            return Version + 1;
        }


        /// <summary>
        /// 文件旧版本清理
        /// </summary>
        public static void FileVersionCleanup(int FileId, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string FolderPath = "";
            string CodeId = "";
            string Extension = "";
            string FilePath = "";

            Base.Data.SqlDataToTable("Select Top 1 DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Extension From DBS_File Where DBS_Version > 1 And DBS_VersionId = " + FileId + " And DBS_Folder = 0 Order By DBS_Id Asc", ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Id = FileTable["DBS_Id"].TypeInt();
                FolderPath = FileTable["DBS_FolderPath"].TypeString();
                CodeId = FileTable["DBS_CodeId"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            FilePath = Base.Common.PathCombine(Context.Server.MapPath("/storage/file/"), FolderPath.Substring(1), CodeId + Extension);

            if (File.Exists(FilePath) == true)
            {
                File.Delete(FilePath);
                File.Delete("" + FilePath + ".pdf");
                File.Delete("" + FilePath + ".flv");
            }

            Base.Data.SqlQuery("Delete From DBS_File Where DBS_Id = " + Id, ref Conn);
        }


        /// <summary>
        /// 获取部门编号路径
        /// </summary>
        public static string DepartmentIdPath(int DepartmentId, ref object Conn)
        {
            Hashtable DepartmentTable = new Hashtable();
            int Id = 0;
            string Path = "";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_DepartmentId From DBS_Department Where DBS_Id = " + DepartmentId, ref Conn, ref DepartmentTable);

            if (DepartmentTable["Exist"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = DepartmentTable["DBS_Id"].TypeInt();
                DepartmentId = DepartmentTable["DBS_DepartmentId"].TypeInt();
            }

            DepartmentTable.Clear();

            if (Id > 0)
            {
                Path = "/" + Id + "/";
            }

            while (Id > 0)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_DepartmentId From DBS_Department Where DBS_Id = " + DepartmentId, ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exist"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = DepartmentTable["DBS_Id"].TypeInt();
                    DepartmentId = DepartmentTable["DBS_DepartmentId"].TypeInt();
                }

                DepartmentTable.Clear();

                if (Id > 0)
                {
                    Path = "/" + Id + "" + Path + "";
                }
            }

            return Path;
        }


        /// <summary>
        /// 获取文件夹编号路径
        /// </summary>
        public static string FolderIdPath(int FolderId, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            int Id = 0;
            string Path = "";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DBS_Id"].TypeInt();
                FolderId = FileTable["DBS_FolderId"].TypeInt();
            }

            FileTable.Clear();

            if (Id > 0)
            {
                Path = "/" + Id + "/";
            }

            while (Id > 0)
            {
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderId From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = FileTable["DBS_Id"].TypeInt();
                    FolderId = FileTable["DBS_FolderId"].TypeInt();
                }

                FileTable.Clear();

                if (Id > 0)
                {
                    Path = "/" + Id + "" + Path + "";
                }
            }

            return Path;
        }


        /// <summary>
        /// 共享权限角色校验(文件夹、文件)
        /// </summary>
        public static bool PurviewCheck(int Id, bool IsFolder, string Purview, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            Hashtable PurviewTable = new Hashtable();
            int FolderId = 0;

            if (IsFolder == true)
            {
                FolderId = Id;
            }
            else
            {
                // 判断是否拥有文件创建者及相关权限
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_CreateUsername, DBS_UpdateUsername From DBS_File Where DBS_Folder = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    return false;
                }
                else
                {
                    FolderId = FileTable["DBS_FolderId"].TypeInt();
                }

                // 判断是否文件所胡者及创建者
                if (FileTable["DBS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DBS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
                {
                    return true;
                }

                // 判断是否文件版本
                if (FileTable["DBS_VersionId"].TypeInt() > 0)
                {
                    // 判断是否文件版本更新者
                    if (FileTable["DBS_UpdateUsername"].TypeString() == Context.Session["Username"].TypeString())
                    {
                        return true;
                    }
                }

                FileTable.Clear();
            }

            // 判断是否拥有文件夹创建者及相关权限
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Folder, DBS_CreateUsername From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return false;
            }

            // 判断是否文件夹所胡者及创建者
            if (FileTable["DBS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DBS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
            {
                return true;
            }

            FileTable.Clear();

            // 判断是否拥有部门权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_DepartmentId, DBS_Purview From DBS_File_Purview Where DBS_DepartmentId = '" + Context.Session["DepartmentId"].TypeString() + "' And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DBS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            // 判断是否拥有角色权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_RoleId, DBS_Purview From DBS_File_Purview Where DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DBS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            // 判断是否拥有用户权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DBS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            return false;
        }


        /// <summary>
        /// 获取共享权限角色(文件夹、文件)
        /// </summary>
        public static string PurviewRole(int Id, bool IsFolder, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            Hashtable PurviewTable = new Hashtable();
            int FolderId = 0;
            string Department = "";
            string Role = "";
            string User = "";

           if (IsFolder == true)
            {
                FolderId = Id;
            }
            else
            {
                // 判断是否拥有文件创建者及相关权限
                Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_VersionId, DBS_Folder, DBS_FolderId, DBS_CreateUsername, DBS_UpdateUsername From DBS_File Where DBS_Folder = 0 And DBS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exist"].TypeBool() == false)
                {
                    return "viewer";
                }
                else
                {
                    FolderId = FileTable["DBS_FolderId"].TypeInt();
                }

                // 判断是否文件所胡者及创建者
                if (FileTable["DBS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DBS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
                {
                    return "creator";
                }

                // 判断是否文件版本
                if (FileTable["DBS_VersionId"].TypeInt() > 0)
                {
                    // 判断是否文件版本更新者
                    if (FileTable["DBS_UpdateUsername"].TypeString() == Context.Session["Username"].TypeString())
                    {
                        return "manager";
                    }
                }

                FileTable.Clear();
            }

            // 判断是否拥有文件夹创建者及相关权限
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_UserId, DBS_Folder, DBS_CreateUsername From DBS_File Where DBS_Folder = 1 And DBS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return "viewer";
            }

            // 判断是否文件夹所胡者及创建者
            if (FileTable["DBS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DBS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
            {
                return "creator";
            }

            FileTable.Clear();

            // 获取部门权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_DepartmentId, DBS_Purview From DBS_File_Purview Where DBS_DepartmentId = '" + Context.Session["DepartmentId"].TypeString() + "' And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                Department = PurviewTable["DBS_Purview"].TypeString();
            }

            PurviewTable.Clear();

            // 获取角色权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_RoleId, DBS_Purview From DBS_File_Purview Where DBS_RoleId = " + Context.Session["RoleId"].TypeString() + " And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                Role = PurviewTable["DBS_Purview"].TypeString();
            }

            PurviewTable.Clear();

            // 获取用户权限
            Base.Data.SqlDataToTable("Select DBS_FileId, DBS_UserId, DBS_Purview From DBS_File_Purview Where DBS_UserId = " + Context.Session["UserId"].TypeString() + " And DBS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exist"].TypeBool() == true)
            {
                User = PurviewTable["DBS_Purview"].TypeString();
            }

            PurviewTable.Clear();

            // 比较角色选取最大权限
            if (PurviewRoleOrder(Department) > PurviewRoleOrder(Role))
            {
                if (PurviewRoleOrder(Department) > PurviewRoleOrder(User))
                {
                    return Department;
                }
                else
                {
                    return User;
                }
            }
            else
            {
                if (PurviewRoleOrder(Role) > PurviewRoleOrder(User))
                {
                    return Role;
                }
                else
                {
                    return User;
                }
            }
        }


        /// <summary>
        /// 获取共享权限角色序列
        /// </summary>
        public static int PurviewRoleOrder(string Purview)
        {
            switch (Purview)
            {
                case "viewer":
                    return 1;

                case "downloader":
                    return 2;

                case "uploader":
                    return 3;

                case "editor":
                    return 4;

                case "manager":
                    return 5;

                case "creator":
                    return 6;
            }

            return 0;
        }


        /// <summary>
        /// 文件处理器触发
        /// </summary>
        public static void FileProcessTrigger()
        {
            string FolderPath = Context.Server.MapPath("/storage/file/");

            if (Directory.Exists(FolderPath) == false)
            {
                return;
            }

            try
            {
                Directory.SetLastWriteTime(FolderPath, DateTime.Now);
            }
            catch (Exception)
            {

            }
        }


        /// <summary>
        /// 日志记录
        /// </summary>
        public static void Log(int Id, string Action, ref object Conn)
        {
            Hashtable FileTable = new Hashtable();
            int Version = 0;
            int VersionId = 0;
            int Folder = 0;
            string Name = "";
            string Extension = "";

            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Version, DBS_VersionId, DBS_Folder, DBS_Name, DBS_Extension From DBS_File Where DBS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == false)
            {
                return;
            }
            else
            {
                Version = FileTable["DBS_Version"].TypeInt();
                VersionId = FileTable["DBS_VersionId"].TypeInt();
                Folder = FileTable["DBS_Folder"].TypeInt();
                Name = FileTable["DBS_Name"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            Base.Data.SqlQuery("Insert Into DBS_Log(DBS_FileId, DBS_FileName, DBS_FileExtension, DBS_FileVersion, DBS_IsFolder, DBS_UserId, DBS_Username, DBS_Action, DBS_IP, DBS_Time) Values(" + (VersionId == 0 ? Id : VersionId) + ", '" + Name + "', '" + Extension + "', " + Version + ", " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Action + "', '" + Base.Common.ClientIP() + "', '" + DateTime.Now.ToString() + "')", ref Conn);
        }


        /// <summary>
        /// 读取xml文件数据返回hashtable
        /// </summary>
        public static void XmlDataReader(string FilePath, string Node, Hashtable Hashtable)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNodes = default(XmlNode);

            XDocument.Load(FilePath);

            XNodes = XDocument.SelectSingleNode(Node);

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                Hashtable.Add(XNode.Name, XNode.InnerText);
            }
        }


        /// <summary>
        /// 读取xml文件参数值
        /// </summary>
        public static string XmlParamReader(string FilePath, string Node, string Param)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNodes = default(XmlNode);

            XDocument.Load(FilePath);

            XNodes = XDocument.SelectSingleNode(Node);

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                if (Base.Common.IsNothing(XNode.Attributes[Param]) == false)
                {
                    return XNodes.Attributes[Param].Value;
                }
            }

            return "";
        }


        /// <summary>
        /// 发送电子邮件
        /// </summary>
        public static void SendMail(string Email, string Subject, string Content)
        {
            string AppName = ConfigurationManager.AppSettings["AppName"].TypeString();
            string MailAddress = ConfigurationManager.AppSettings["MailAddress"].TypeString();
            string MailServer = ConfigurationManager.AppSettings["MailServer"].TypeString();
            string MailPort = ConfigurationManager.AppSettings["MailPort"].TypeString();
            string MailUsername = ConfigurationManager.AppSettings["MailUsername"].TypeString();
            string MailPassword = ConfigurationManager.AppSettings["MailPassword"].TypeString();
            bool MailSsl = ConfigurationManager.AppSettings["MailSsl"].TypeBool();

            Base.Common.SendMail(AppName, MailAddress, MailServer, MailPort, MailUsername, MailPassword, Email, Subject, Content, MailSsl, false);
        }


        /// <summary>
        /// 错误报告
        /// </summary>
        public static void Error(Exception ex)
        {
            FileStream FileStream = default(FileStream);
            byte[] Bytes = {};
            string FolderPath = Context.Server.MapPath("/storage/log/");
            string Log = "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n";

            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            Bytes = new UTF8Encoding(true).GetBytes(Log);

            FileStream = new FileStream(Base.Common.PathCombine(FolderPath, "error.log"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            FileStream.Write(Bytes, 0, Bytes.Length);

            FileStream.Close();
            FileStream.Dispose();
        }


    }


}
