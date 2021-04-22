using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using dboxShare.Base;


namespace dboxShare
{


    sealed class FileProcessor
    {


        private static dynamic Conn;


        public static string AppPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }


        public static string ConnectionString
        {
            get
            {
                return AppConfig("ConnectionString").TypeString();
            }
        }


        public static string StoragePath
        {
            get
            {
                string StoragePath = AppConfig("StoragePath").TypeString();

                if (string.IsNullOrEmpty(StoragePath) == true)
                {
                    StoragePath = Base.Common.PathCombine(AppPath, @"..\storage");

                    StoragePath = new DirectoryInfo(StoragePath).FullName;
                }

                return StoragePath;
            }
        }


        public static string FileStoragePath
        {
            get
            {
                return Base.Common.PathCombine(StoragePath, "file");
            }
        }


        public static string TempStoragePath
        {
            get
            {
                return Base.Common.PathCombine(StoragePath, "temp");
            }
        }


        public static string LogStoragePath
        {
            get
            {
                return Base.Common.PathCombine(StoragePath, "log");
            }
        }


        public static void Main()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Environment.Exit(0);
            }

            bool IsProcessing = true;

            DateTime PastWriteTime = Directory.GetLastWriteTime(FileStoragePath);

            do
            {
                Thread.Sleep(10000);

                DateTime LastWriteTime = Directory.GetLastWriteTime(FileStoragePath);

                if (DateTime.Compare(PastWriteTime, LastWriteTime) < 0)
                {
                    PastWriteTime = LastWriteTime;

                    IsProcessing = true;
                }

                if (IsProcessing == true)
                {
                    try
                    {
                        Conn = Base.Data.DBConnection(ConnectionString);

                        Conn.Open();

                        FileProcess();
                    }
                    catch (Exception ex)
                    {
                        if (Directory.Exists(LogStoragePath) == false)
                        {
                            Directory.CreateDirectory(LogStoragePath);
                        }

                        File.AppendAllText(Base.Common.PathCombine(LogStoragePath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
                    }
                    finally
                    {
                        Conn.Close();
                        Conn.Dispose();
                    }

                    IsProcessing = false;
                }
            } while (true);
        }


        /// <summary>
        /// 文件处理
        /// </summary>
        private static void FileProcess()
        {
            Hashtable FileTable = new Hashtable();
            string Query = "";

            Query += "Exists (";
            Query += "Select DS_File_Task.DS_FileId From DS_File_Task Where " +
                     "DS_File_Task.DS_FileId = DS_File.DS_Id And " +
                     "DS_File_Task.DS_Process = 1 And " +
                     "DS_File.DS_Folder = 0 And " +
                     "DS_File.DS_Recycle = 0";
            Query += ")";

            Base.Data.SqlDataToTable("Select Top 1 DS_Id, DS_Folder, DS_FolderPath, DS_CodeId, DS_Extension, DS_Recycle From DS_File Where " + Query + "", ref Conn, ref FileTable);

            if (FileTable["Exists"].TypeBool() == false)
            {
                return;
            }

            int Id = FileTable["DS_Id"].TypeInt();
            string FolderPath = FileTable["DS_FolderPath"].TypeString();
            string CodeId = FileTable["DS_CodeId"].TypeString();
            string Extension = FileTable["DS_Extension"].TypeString();

            FileTable.Clear();

            string FilePath = Base.Common.PathCombine(FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

            // 设置任务为锁定状态
            Base.Data.SqlQuery("Update DS_File_Task Set DS_Process = -1 Where DS_FileId = " + Id, ref Conn);

            // 文件转换程序
            if (string.IsNullOrEmpty(Extension) == false)
            {
                string FileConverter = ConverterName(Extension.Substring(1));

                if (string.IsNullOrEmpty(FileConverter) == false)
                {
                    FileConversion(FileConverter, FilePath);
                }
            }

            // 文件加密程序
            FileEncryption(FilePath, CodeId);

            // 设置任务为完成状态
            Base.Data.SqlQuery("Update DS_File_Task Set DS_Process = 0 Where DS_FileId = " + Id, ref Conn);

            Thread.Sleep(100);

            // 循环处理
            FileProcess();
        }


        /// <summary>
        /// 文件转换
        /// </summary>
        private static void FileConversion(string ConverterId, string FilePath)
        {
            string Exec = ConverterParam(ConverterId, "exec").TypeString();

            if (string.IsNullOrEmpty(Exec) == true)
            {
                return;
            }

            if (Exec.IndexOf(':') == -1)
            {
                Exec = Base.Common.PathCombine(AppPath, Exec);
            }

            string Cmd = ConverterParam(ConverterId, "cmd").TypeString();

            if (string.IsNullOrEmpty(Cmd) == true)
            {
                return;
            }

            string Extension = ConverterParam(ConverterId, "extension").TypeString();

            if (string.IsNullOrEmpty(Extension) == true)
            {
                return;
            }

            int Wait = ConverterParam(ConverterId, "wait").TypeInt();

            Cmd = Cmd.Replace("{source}", "\"" + FilePath + "\"");
            Cmd = Cmd.Replace("{target}", "\"" + FilePath + "." + Extension + "\"");

            Base.Common.ProcessExecute(Exec, Cmd, Wait);
        }


        /// <summary>
        /// 文件加密
        /// </summary>
        private static void FileEncryption(string SourceFilePath, string CodeId)
        {
            string TempFilePath = Base.Common.PathCombine(TempStoragePath, System.Guid.NewGuid().ToString());
            string PDFFilePath = Base.Common.PathCombine(SourceFilePath, ".pdf");
            string FLVFilePath = Base.Common.PathCombine(SourceFilePath, ".flv");

            try
            {
                Base.Crypto.FileEncrypt(SourceFilePath, TempFilePath, CodeId);
            }
            finally
            {
                if (File.Exists(SourceFilePath) == true)
                {
                    File.Delete(SourceFilePath);
                }

                if (File.Exists(TempFilePath) == true)
                {
                    File.Move(TempFilePath, SourceFilePath);
                }
            }

            if (File.Exists(PDFFilePath) == true)
            {
                try
                {
                    Base.Crypto.FileEncrypt(PDFFilePath, TempFilePath, CodeId);
                }
                finally
                {
                    if (File.Exists(PDFFilePath) == true)
                    {
                        File.Delete(PDFFilePath);
                    }

                    if (File.Exists(TempFilePath) == true)
                    {
                        File.Move(TempFilePath, PDFFilePath);
                    }
                }
            }

            if (File.Exists(FLVFilePath) == true)
            {
                try
                {
                    Base.Crypto.FileEncrypt(FLVFilePath, TempFilePath, CodeId);
                }
                finally
                {
                    if (File.Exists(FLVFilePath) == true)
                    {
                        File.Delete(FLVFilePath);
                    }

                    if (File.Exists(TempFilePath) == true)
                    {
                        File.Move(TempFilePath, FLVFilePath);
                    }
                }
            }
        }


        /// <summary>
        /// 获取配置文件参数
        /// </summary>
        private static string AppConfig(string Name)
        {
            string ConfigFilePath = Base.Common.PathCombine(AppPath, @"..\web.config");

            ExeConfigurationFileMap ConfigFileMap = new ExeConfigurationFileMap();

            ConfigFileMap.ExeConfigFilename = new FileInfo(ConfigFilePath).FullName;

            Configuration Config = ConfigurationManager.OpenMappedExeConfiguration(ConfigFileMap, ConfigurationUserLevel.None);

            AppSettingsSection AppSettings = (AppSettingsSection)Config.GetSection("appSettings");

            if (Base.Common.IsNothing(AppSettings.Settings[Name]) == true)
            {
                return "";
            }
            else
            {
                return AppSettings.Settings[Name].Value;
            }
        }


        /// <summary>
        /// 获取转换器名称
        /// </summary>
        private static string ConverterName(string Extension)
        {
            XmlDocument XDocument = new XmlDocument();

            XDocument.Load(Base.Common.PathCombine(AppPath, "converter.xml"));

            XmlNode XNodes = XDocument.SelectSingleNode("/config");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                string[] Extensions = XNode.InnerText.Split(',');

                for (int i = 0; i < Extensions.Length; i++)
                {
                    if (Extensions[i] == Extension)
                    {
                        return XNode.Name;
                    }
                }
            }

            return "";
        }


        /// <summary>
        /// 获取转换器参数
        /// </summary>
        private static string ConverterParam(string Converter, string Param)
        {
            XmlDocument XDocument = new XmlDocument();

            XDocument.Load(Base.Common.PathCombine(AppPath, "converter.xml"));

            XmlNode XNodes = XDocument.SelectSingleNode("/config");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                if (XNode.Name.ToString() == Converter)
                {
                    if (Base.Common.IsNothing(XNode.Attributes[Param]) == false)
                    {
                        return XNode.Attributes[Param].Value;
                    }
                }
            }

            return "";
        }


    }


}