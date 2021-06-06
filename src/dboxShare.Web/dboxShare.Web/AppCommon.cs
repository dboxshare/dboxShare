using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
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
            try
            {
                var UserId = Context.Session["UserId"].TypeString();

                if (string.IsNullOrEmpty(UserId) == true)
                {
                    return false;
                }

                var LoginToken = Context.Session["LoginToken"].TypeString();

                if (string.IsNullOrEmpty(LoginToken) == true)
                {
                    return false;
                }

                // 通过 Session 验证登录 Token
                if (LoginToken != Base.Crypto.TextEncrypt(Context.Session.SessionID, AppConfig.SecurityKey))
                {
                    return false;
                }

                // 通过 Cache 验证登录 Token
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
        /// 生成文件 CodeId
        /// </summary>
        public static string FileCodeId()
        {
            using (var RNG = new RNGCryptoServiceProvider())
            {
                var Code = new ArrayList();

                var Time = DateTime.Now;

                Code.Add(Time.ToString("yyyyMMdd"));
                Code.Add(Time.ToString("HHmmss"));
                Code.Add(Time.ToString("ffffff"));
                
                var Buffer = new byte[4];

                RNG.GetBytes(Buffer);

                Code.Add(new Random(BitConverter.ToInt16(Buffer, 0)).Next(1000, 9999).ToString());
                Code.Add(new Random(BitConverter.ToInt32(Buffer, 0)).Next(10000000, 99999999).ToString());

                return string.Join("-", Code.ToArray());
            }
        }


        /// <summary>
        /// 获取文件哈希码
        /// </summary>
        public static string FileHash(string FilePath)
        {
            var Hash = "";

            using (var FStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var MD5 = new MD5CryptoServiceProvider())
                {
                    var Bytes = MD5.ComputeHash(FStream);

                    for (var i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }

            return Hash;
        }


        /// <summary>
        /// 获取文件类型
        /// </summary>
        public static string FileType(string Extension)
        {
            if (string.IsNullOrEmpty(Extension) == true)
            {
                return "other";
            }

            if (Extension.Substring(0, 1) == ".")
            {
                Extension = Extension.Substring(1);
            }

            var TypeTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            AppCommon.XmlDataReader(Base.Common.PathCombine(AppConfig.DataStoragePath, "file-type.xml"), "/type", TypeTable);

            if (TypeTable.Count == 0)
            {
                return "other";
            }

            // 文件类型列表
            foreach (var Key in TypeTable.Keys)
            {
                if (string.IsNullOrEmpty(TypeTable[Key].TypeString()) == false)
                {
                    var Extensions = TypeTable[Key].TypeString().Split(',');

                    // 查找扩展名列表
                    for (var i = 0; i < Extensions.Length; i++)
                    {
                        if (Extensions[i] == Extension)
                        {
                            return Key.ToString();
                        }
                    }
                }
            }

            return "other";
        }


        /// <summary>
        /// 获取文件夹序列名称
        /// </summary>
        public static string FolderName(int FolderId, int FolderShare, string FolderName, ref object Conn)
        {
            if (Base.Common.StringCheck(FolderName, @"\s\(\d+\)$") == true)
            {
                FolderName = Base.Common.StringGet(FolderName, @"^([\s\S]+)\s\(\d+\)$").TypeString();
            }

            var Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_Folder = 1 And " +
                     "A1.DS_FolderId = " + FolderId + " And " +
                     "A1.DS_Name Like '" + FolderName + "%' And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_Folder = 1 And " +
                     "A2.DS_FolderId = " + FolderId + " And " +
                     "A2.DS_Name Like '" + FolderName + "%' And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "'";

            if (FolderShare == 1)
            {
                Query += " Union All ";

                // 共享部门查询
                Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = B.DS_Id And " +
                         "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Where " + 
                         "B.DS_Id = DS_File.DS_Id And " +
                         "B.DS_Folder = 1 And " +
                         "B.DS_FolderId = " + FolderId + " And " +
                         "B.DS_Share = 1 And " +
                         "B.DS_Name Like '" + FolderName + "%' Union All ";

                // 共享角色查询
                Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = C.DS_Id And " +
                         "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Where " + 
                         "C.DS_Id = DS_File.DS_Id And " +
                         "C.DS_Folder = 1 And " +
                         "C.DS_FolderId = " + FolderId + " And " +
                         "C.DS_Share = 1 And " +
                         "C.DS_Name Like '" + FolderName + "%' Union All ";

                // 共享用户查询
                Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = D.DS_Id And " +
                         "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Where " +
                         "D.DS_Id = DS_File.DS_Id And " +
                         "D.DS_Folder = 1 And " +
                         "D.DS_FolderId = " + FolderId + " And " +
                         "D.DS_Share = 1 And " +
                         "D.DS_Name Like '" + FolderName + "%'";
            }

            Query += ")";

            var Sql = "Select DS_Folder, DS_FolderId, DS_Name From DS_File Where " + Query + " Order By DS_Id Desc";

            var Folders = new ArrayList();

            Base.Data.SqlListToArray("DS_Name", Sql, ref Conn, ref Folders);

            if (Folders.Count == 0)
            {
                return FolderName;
            }

            var Sequence = 0;

            for (var i = 0; i < Folders.Count; i++)
            {
                if (Base.Common.StringCheck(Folders[i].TypeString(), @"\s\(\d+\)$") == true)
                {
                    var Value = Base.Common.StringGet(Folders[i].TypeString(), @"\s\((\d+)\)$").TypeInt();

                    if (Value > Sequence)
                    {
                        Sequence = Value;
                    }
                }
            }

            if (Sequence == 0)
            {
                return "" + FolderName + " (2)";
            }
            else
            {
                return "" + FolderName + " (" + (Sequence + 1) + ")";
            }
        }


        /// <summary>
        /// 获取文件序列名称
        /// </summary>
        public static string FileName(int FolderId, int FolderShare, string FileName, string FileExtension, ref object Conn)
        {
            if (Base.Common.StringCheck(FileName, @"\s\(\d+\)$") == true)
            {
                FileName = Base.Common.StringGet(FileName, @"^([\s\S]+)\s\(\d+\)$").TypeString();
            }

            var Query = "";

            Query += "Exists (";

            // 所有者查询
            Query += "Select A1.DS_Id From DS_File As A1 Where " +
                     "A1.DS_Id = DS_File.DS_Id And " +
                     "A1.DS_VersionId = 0 And " +
                     "A1.DS_Folder = 0 And " +
                     "A1.DS_FolderId = " + FolderId + " And " +
                     "A1.DS_Name Like '" + FileName + "%' And " +
                     "A1.DS_Extension = '" + FileExtension + "' And " +
                     "A1.DS_UserId = " + Context.Session["UserId"].TypeString() + " Union All ";

            // 创建者查询
            Query += "Select A2.DS_Id From DS_File As A2 Where " +
                     "A2.DS_Id = DS_File.DS_Id And " +
                     "A2.DS_VersionId = 0 And " +
                     "A2.DS_Folder = 0 And " +
                     "A2.DS_FolderId = " + FolderId + " And " +
                     "A2.DS_Name Like '" + FileName + "%' And " +
                     "A2.DS_Extension = '" + FileExtension + "' And " +
                     "A2.DS_CreateUsername = '" + Context.Session["Username"].TypeString() + "'";

            if (FolderShare == 1)
            {
                Query += " Union All ";

                // 共享部门查询
                Query += "Select B.DS_Id From DS_File As B Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = B.DS_FolderId And " +
                         "DS_File_Purview.DS_DepartmentId Like '" + Context.Session["DepartmentId"].TypeString() + "%' Where " +
                         "B.DS_Id = DS_File.DS_Id And " +
                         "B.DS_VersionId = 0 And " +
                         "B.DS_Folder = 0 And " +
                         "B.DS_FolderId = " + FolderId + " And " +
                         "B.DS_Share = 1 And " +
                         "B.DS_Name Like '" + FileName + "%' And " +
                         "B.DS_Extension = '" + FileExtension + "' Union All ";

                // 共享角色查询
                Query += "Select C.DS_Id From DS_File As C Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = C.DS_FolderId And " +
                         "DS_File_Purview.DS_RoleId = " + Context.Session["RoleId"].TypeString() + " Where " +
                         "C.DS_Id = DS_File.DS_Id And " +
                         "C.DS_VersionId = 0 And " +
                         "C.DS_Folder = 0 And " +
                         "C.DS_FolderId = " + FolderId + " And " +
                         "C.DS_Share = 1 And " +
                         "C.DS_Name Like '" + FileName + "%' And " +
                         "C.DS_Extension = '" + FileExtension + "' Union All ";

                // 共享用户查询
                Query += "Select D.DS_Id From DS_File As D Inner Join DS_File_Purview On " +
                         "DS_File_Purview.DS_FileId = D.DS_FolderId And " +
                         "DS_File_Purview.DS_UserId = " + Context.Session["UserId"].TypeString() + " Where " +
                         "D.DS_Id = DS_File.DS_Id And " +
                         "D.DS_VersionId = 0 And " +
                         "D.DS_Folder = 0 And " +
                         "D.DS_FolderId = " + FolderId + " And " +
                         "D.DS_Share = 1 And " +
                         "D.DS_Name Like '" + FileName + "%' And " +
                         "D.DS_Extension = '" + FileExtension + "";
            }

            Query += ")";

            var Sql = "Select DS_VersionId, DS_Folder, DS_FolderId, DS_Name, DS_Extension From DS_File Where " + Query + " Order By DS_Id Desc";

            var Files = new ArrayList();

            Base.Data.SqlListToArray("DS_Name", Sql, ref Conn, ref Files);

            if (Files.Count == 0)
            {
                return FileName;
            }

            var Sequence = 0;

            for (var i = 0; i < Files.Count; i++)
            {
                if (Base.Common.StringCheck(Files[i].TypeString(), @"\s\(\d+\)$") == true)
                {
                    var Value = Base.Common.StringGet(Files[i].TypeString(), @"\s\((\d+)\)$").TypeInt();

                    if (Value > Sequence)
                    {
                        Sequence = Value;
                    }
                }
            }

            if (Sequence == 0)
            {
                return "" + FileName + " (2)";
            }
            else
            {
                return "" + FileName + " (" + (Sequence + 1) + ")";
            }
        }


        /// <summary>
        /// 获取文件版本编号
        /// </summary>
        public static int FileVersionNumber(int FileId, ref object Conn)
        {
            var Query = "";

            Query += "Exists (";

            // 文件正本查询
            Query += "Select A.DS_Id From DS_File As A Where " +
                     "A.DS_Id = DS_File.DS_Id And " +
                     "A.DS_Id = " + FileId + " Union All ";

            // 文件版本查询
            Query += "Select B.DS_Id From DS_File As B Where " +
                     "B.DS_Id = DS_File.DS_Id And " +
                     "B.DS_VersionId = " + FileId + "";

            Query += ")";

            var Sql = "Select Top 1 DS_Id, DS_Version, DS_VersionId From DS_File Where " + Query + " Order By DS_Version Desc";

            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Base.Data.SqlDataToTable(Sql, ref Conn, ref FileTable);

            var Version = 0;

            if (FileTable["Exists"].TypeBool() == true)
            {
                Version = FileTable["DS_Version"].TypeInt();
            }

            FileTable.Clear();

            return Version + 1;
        }


        /// <summary>
        /// 文件版本清理
        /// </summary>
        public static void FileVersionCleanup(int FileId, ref object Conn)
        {
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Base.Data.SqlDataToTable("Select Top 1 DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension From DS_File Where DS_Version > 1 And DS_VersionId = " + FileId + " And DS_Folder = 0 Order By DS_Id Asc", ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                return;
            }

            var Id = FileTable["DS_Id"].TypeInt();
            var FolderPath = FileTable["DS_FolderPath"].TypeString();
            var CodeId = FileTable["DS_CodeId"].TypeString();
            var Extension = FileTable["DS_Extension"].TypeString();

            FileTable.Clear();

            var FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

            if (File.Exists(FilePath) == true)
            {
                File.Delete(FilePath);
            }

            var PDFFilePath = "" + FilePath + ".pdf";

            if (File.Exists(PDFFilePath) == true)
            {
                File.Delete(PDFFilePath);
            }

            var FLVFilePath = "" + FilePath + ".flv";

            if (File.Exists(FLVFilePath) == true)
            {
                File.Delete(FLVFilePath);
            }

            Base.Data.SqlQuery("Delete From DS_File Where DS_Id = " + Id, ref Conn);
        }


        /// <summary>
        /// 获取部门编号路径
        /// </summary>
        public static string DepartmentIdPath(int DepartmentId, ref object Conn)
        {
            var DepartmentTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var Id = 0;

            Base.Data.SqlDataToTable("Select DS_Id, DS_DepartmentId From DS_Department Where DS_Id = " + DepartmentId, ref Conn, ref DepartmentTable);

            if (DepartmentTable["Exists"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = DepartmentTable["DS_Id"].TypeInt();
                DepartmentId = DepartmentTable["DS_DepartmentId"].TypeInt();
            }

            DepartmentTable.Clear();

            var Path = "";

            if (Id > 0)
            {
                Path = "/" + Id + "/";
            }

            while (Id > 0)
            {
                Base.Data.SqlDataToTable("Select DS_Id, DS_DepartmentId From DS_Department Where DS_Id = " + DepartmentId, ref Conn, ref DepartmentTable);

                if (DepartmentTable["Exists"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = DepartmentTable["DS_Id"].TypeInt();
                    DepartmentId = DepartmentTable["DS_DepartmentId"].TypeInt();
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
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var Id = 0;

            Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                Id = 0;
            }
            else
            {
                Id = FileTable["DS_Id"].TypeInt();
                FolderId = FileTable["DS_FolderId"].TypeInt();
            }

            FileTable.Clear();

            var Path = "";

            if (Id > 0)
            {
                Path = "/" + Id + "/";
            }

            while (Id > 0)
            {
                Base.Data.SqlDataToTable("Select DS_Id, DS_Folder, DS_FolderId From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    Id = 0;
                }
                else
                {
                    Id = FileTable["DS_Id"].TypeInt();
                    FolderId = FileTable["DS_FolderId"].TypeInt();
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
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var FolderId = 0;

            if (IsFolder == true)
            {
                FolderId = Id;
            }
            else
            {
                // 判断是否拥有文件创建者及相关权限
                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_VersionId, DS_Folder, DS_FolderId, DS_CreateUsername, DS_UpdateUsername From DS_File Where DS_Folder = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return false;
                }
                else
                {
                    FolderId = FileTable["DS_FolderId"].TypeInt();
                }

                // 判断是否文件所胡者及创建者
                if (FileTable["DS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
                {
                    return true;
                }

                // 判断是否文件版本
                if (FileTable["DS_VersionId"].TypeInt() > 0)
                {
                    // 判断是否文件版本更新者
                    if (FileTable["DS_UpdateUsername"].TypeString() == Context.Session["Username"].TypeString())
                    {
                        return true;
                    }
                }

                FileTable.Clear();
            }

            // 判断是否拥有文件夹创建者及相关权限
            Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Folder, DS_CreateUsername From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                return false;
            }

            // 判断是否文件夹所胡者及创建者
            if (FileTable["DS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
            {
                return true;
            }

            FileTable.Clear();

            var PurviewTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            // 判断是否拥有部门权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_DepartmentId, DS_Purview From DS_File_Purview Where DS_DepartmentId = '" + Context.Session["DepartmentId"].TypeString() + "' And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            // 判断是否拥有角色权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_RoleId, DS_Purview From DS_File_Purview Where DS_RoleId = " + Context.Session["RoleId"].TypeString() + " And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            // 判断是否拥有用户权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId, DS_Purview From DS_File_Purview Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                if (PurviewRoleOrder(PurviewTable["DS_Purview"].TypeString()) >= PurviewRoleOrder(Purview))
                {
                    return true;
                }
            }

            PurviewTable.Clear();

            return false;
        }


        /// <summary>
        /// 获取共享权限角色
        /// </summary>
        public static string PurviewRole(int Id, bool IsFolder, ref object Conn)
        {
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            var FolderId = 0;

            if (IsFolder == true)
            {
                FolderId = Id;
            }
            else
            {
                // 判断是否拥有文件创建者及相关权限
                Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_VersionId, DS_Folder, DS_FolderId, DS_CreateUsername, DS_UpdateUsername From DS_File Where DS_Folder = 0 And DS_Id = " + Id, ref Conn, ref FileTable);

                if (FileTable["Exists"].TypeBool() == false)
                {
                    return "viewer";
                }
                else
                {
                    FolderId = FileTable["DS_FolderId"].TypeInt();
                }

                // 判断是否文件所胡者及创建者
                if (FileTable["DS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
                {
                    return "creator";
                }

                // 判断是否文件版本
                if (FileTable["DS_VersionId"].TypeInt() > 0)
                {
                    // 判断是否文件版本更新者
                    if (FileTable["DS_UpdateUsername"].TypeString() == Context.Session["Username"].TypeString())
                    {
                        return "manager";
                    }
                }

                FileTable.Clear();
            }

            // 判断是否拥有文件夹创建者及相关权限
            Base.Data.SqlDataToTable("Select DS_Id, DS_UserId, DS_Folder, DS_CreateUsername From DS_File Where DS_Folder = 1 And DS_Id = " + FolderId, ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                return "viewer";
            }

            // 判断是否文件夹所胡者及创建者
            if (FileTable["DS_UserId"].TypeInt() == Context.Session["UserId"].TypeInt() || FileTable["DS_CreateUsername"].TypeString() == Context.Session["Username"].TypeString())
            {
                return "creator";
            }

            FileTable.Clear();

            var PurviewTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            // 获取部门权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_DepartmentId, DS_Purview From DS_File_Purview Where DS_DepartmentId = '" + Context.Session["DepartmentId"].TypeString() + "' And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            var Department = "";

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                Department = PurviewTable["DS_Purview"].TypeString();
            }

            PurviewTable.Clear();

            // 获取角色权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_RoleId, DS_Purview From DS_File_Purview Where DS_RoleId = " + Context.Session["RoleId"].TypeString() + " And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            var Role = "";

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                Role = PurviewTable["DS_Purview"].TypeString();
            }

            PurviewTable.Clear();

            // 获取用户权限
            Base.Data.SqlDataToTable("Select DS_FileId, DS_UserId, DS_Purview From DS_File_Purview Where DS_UserId = " + Context.Session["UserId"].TypeString() + " And DS_FileId = " + FolderId, ref Conn, ref PurviewTable);

            var User = "";

            if (PurviewTable["Exists"].TypeBool() == true)
            {
                User = PurviewTable["DS_Purview"].TypeString();
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
        /// 文件转换校验
        /// </summary>
        public static bool IsConvert(string Extension)
        {
            if (string.IsNullOrEmpty(Extension) == true)
            {
                return false;
            }
            else
            {
                Extension = Extension.Substring(1);
            }

            var XDocument = new XmlDocument();

            XDocument.Load(Context.Server.MapPath("/bin/converter.xml"));

            var XNodes = XDocument.SelectSingleNode("/config");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                var Extensions = XNode.InnerText.Split(',');

                for (var i = 0; i < Extensions.Length; i++)
                {
                    if (Extensions[i] == Extension)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 日志记录
        /// </summary>
        public static void Log(int Id, string Action, ref object Conn)
        {
            var FileTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

            Base.Data.SqlDataToTable("Select DS_Id, DS_Version, DS_VersionId, DS_Folder, DS_Name, DS_Extension From DS_File Where DS_Id = " + Id, ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                return;
            }

            var Version = FileTable["DS_Version"].TypeInt();
            var VersionId = FileTable["DS_VersionId"].TypeInt();
            var Folder = FileTable["DS_Folder"].TypeInt();
            var Name = FileTable["DS_Name"].TypeString();
            var Extension = FileTable["DS_Extension"].TypeString();

            FileTable.Clear();

            Base.Data.SqlQuery("Insert Into DS_Log(DS_FileId, DS_FileName, DS_FileExtension, DS_FileVersion, DS_IsFolder, DS_UserId, DS_Username, DS_Action, DS_IP, DS_Time) Values(" + (VersionId == 0 ? Id : VersionId) + ", '" + Name + "', '" + Extension + "', " + Version + ", " + Folder + ", " + Context.Session["UserId"].TypeString() + ", '" + Context.Session["Username"].TypeString() + "', '" + Action + "', '" + Base.Common.ClientIP() + "', '" + DateTime.Now.ToString() + "')", ref Conn);
        }


        /// <summary>
        /// 文件处理器触发
        /// </summary>
        public static void FileProcessorTrigger()
        {
            if (Directory.Exists(AppConfig.FileStoragePath) == false)
            {
                return;
            }

            var FilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, ".processing");

            var Bytes = new UTF8Encoding(true).GetBytes(DateTime.Now.ToString());

            using (var FStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                FStream.Write(Bytes, 0, Bytes.Length);
            }
        }


        /// <summary>
        /// 返回 ResponseMessage(HTTP 状态代码)
        /// </summary>
        public static HttpResponseMessage ResponseMessage(HttpStatusCode StatusCode)
        {
            var Response = new HttpResponseMessage(StatusCode);

            Response.Content = new StringContent(StatusCode.ToString());

            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


        /// <summary>
        /// 返回 ResponseMessage(消息)
        /// </summary>
        public static HttpResponseMessage ResponseMessage(string Message)
        {
            var Response = new HttpResponseMessage(HttpStatusCode.OK);

            Response.Content = new StringContent(Message);

            Response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;

            return Response;
        }


        /// <summary>
        /// 读取 Xml 文件数据返回 Hashtable
        /// </summary>
        public static void XmlDataReader(string FilePath, string Node, Hashtable Hashtable)
        {
            var XDocument = new XmlDocument();

            XDocument.Load(FilePath);

            var XNodes = XDocument.SelectSingleNode(Node);

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                Hashtable.Add(XNode.Name, XNode.InnerText);
            }
        }


        /// <summary>
        /// 读取 Xml 文件参数值
        /// </summary>
        public static string XmlParamReader(string FilePath, string Node, string Param)
        {
            var XDocument = new XmlDocument();

            XDocument.Load(FilePath);

            var XNodes = XDocument.SelectSingleNode(Node);

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
        /// 应用错误日志
        /// </summary>
        public static void Error(Exception ex)
        {
            var FilePath = Base.Common.PathCombine(AppConfig.LogStoragePath, "error.log");

            if (Directory.Exists(AppConfig.LogStoragePath) == false)
            {
                Directory.CreateDirectory(AppConfig.LogStoragePath);
            }

            var Message = "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n";

            var Bytes = new UTF8Encoding(true).GetBytes(Message);

            using (var FStream = new FileStream(FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                FStream.Write(Bytes, 0, Bytes.Length);
            }
        }


    }


}
