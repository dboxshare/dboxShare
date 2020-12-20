using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using dboxShare.Base;


namespace dboxShare
{


sealed class FileProcesser
{


    private static string AppPath = AppDomain.CurrentDomain.BaseDirectory;
    private static XmlDocument XDocument = new XmlDocument();


    public static void Main()
    {
        if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length - 1 > 0)
        {
            Application.Exit();
        }

        dynamic Conn = default(dynamic);
        string ConnectionString = "";
        string StoragePath = "";
        string FileStoragePath = "";
        string TaskAssign = "";
        int MonitorInterval = 0;
        DateTime PastWriteTime = default(DateTime);
        DateTime LastWriteTime = default(DateTime);

        try
        {
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].TypeString();

            if (string.IsNullOrEmpty(ConnectionString) == true)
            {
                return;
            }

            StoragePath = ConfigurationManager.AppSettings["StoragePath"].TypeString();

            if (string.IsNullOrEmpty(StoragePath) == true)
            {
                return;
            }

            if (Directory.Exists(StoragePath) == false)
            {
                return;
            }

            FileStoragePath = Base.Common.PathCombine(StoragePath, "file");

            TaskAssign = ConfigurationManager.AppSettings["TaskAssign"].TypeString();

            if (Base.Common.StringCheck(TaskAssign, @"^\d{1,10}$") == false)
            {
                TaskAssign = "0123456789";
            }

            MonitorInterval = ConfigurationManager.AppSettings["MonitorInterval"].TypeInt();

            if (MonitorInterval < 5 || MonitorInterval > 60)
            {
                MonitorInterval = 10;
            }

            PastWriteTime = Directory.GetLastWriteTime(FileStoragePath);

            XDocument.Load(Base.Common.PathCombine(AppPath, "converter.xml"));

            do
            {
                Thread.Sleep(MonitorInterval * 1000);

                LastWriteTime = Directory.GetLastWriteTime(FileStoragePath);

                if (DateTime.Compare(PastWriteTime, LastWriteTime) < 0)
                {
                    PastWriteTime = LastWriteTime;

                    Conn = Base.Data.DBConnection(ConnectionString);

                    Conn.Open();

                    TaskQueue(FileStoragePath, TaskAssign, ref Conn);

                    Conn.Close();
                    Conn.Dispose();
                }

                Application.DoEvents();
            } while (true);
        }
        catch (Exception ex)
        {
            File.AppendAllText(Base.Common.PathCombine(AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
        }
        finally
        {
            if (Base.Common.IsNothing(Conn) == false)
            {
                Conn.Close();
                Conn.Dispose();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }


    /// <summary>
    /// 任务队列
    /// </summary>
    private static void TaskQueue(string FileStoragePath, string TaskAssign, ref object Conn)
    {
        ArrayList Queues = new ArrayList();

        try
        {
            Base.Data.SqlListToArray("DBS_FileId", "Select DBS_FileId, DBS_Convert From DBS_File_Process Where DBS_Convert = 1", ref Conn, ref Queues);

            if (Queues.Count == 0)
            {
                return;
            }
            else
            {
                TaskExecute(0, FileStoragePath, TaskAssign, Queues, ref Conn);
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(Base.Common.PathCombine(AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
        }
        finally
        {

        }
    }


    /// <summary>
    /// 任务执行
    /// </summary>
    private static void TaskExecute(int Index, string FileStoragePath, string TaskAssign, ArrayList Queues, ref object Conn)
    {
        Hashtable FileTable = new Hashtable();
        int Id = 0;
        string FolderPath = "";
        string CodeId = "";
        string Extension = "";
        string FilePath = "";
        string FileConverter = "";

        try
        {
            Base.Data.SqlDataToTable("Select DBS_Id, DBS_Folder, DBS_FolderPath, DBS_CodeId, DBS_Extension, DBS_Recycle From DBS_File Where DBS_Id = " + Queues[Index] + " And DBS_Folder = 0 And DBS_Recycle = 0", ref Conn, ref FileTable);

            if (FileTable["Exist"].TypeBool() == true)
            {
                Id = FileTable["DBS_Id"].TypeInt();
                FolderPath = FileTable["DBS_FolderPath"].TypeString();
                CodeId = FileTable["DBS_CodeId"].TypeString();
                Extension = FileTable["DBS_Extension"].TypeString();
            }

            FileTable.Clear();

            if (Id > 0)
            {
                if (IsAssignTask(Id, TaskAssign) == true)
                {
                    FilePath = Base.Common.PathCombine(FileStoragePath, FolderPath.Substring(1), CodeId + Extension);

                    // 根据文件扩展名获取相应转换器名称
                    if (string.IsNullOrEmpty(Extension) == false)
                    {
                        FileConverter = ConverterName(Extension.Substring(1));
                    }

                    // 判断文件格式是否支持转换
                    if (string.IsNullOrEmpty(FileConverter) == false)
                    {
                        Base.Common.ProcessExecute(Base.Common.PathCombine(AppPath, "dboxShare.Converter.exe"), "\"" + FileConverter + "\" \"" + FilePath + "\"", 0);
                    }

                    Base.Crypto.FileEncrypt(FilePath, CodeId, false, true, true);

                    if (File.Exists("" + FilePath + ".pdf") == true)
                    {
                        Base.Crypto.FileEncrypt("" + FilePath + ".pdf", CodeId, false, true, true);
                    }

                    if (File.Exists("" + FilePath + ".flv") == true)
                    {
                        Base.Crypto.FileEncrypt("" + FilePath + ".flv", CodeId, false, true, true);
                    }

                    Base.Data.SqlQuery("Update DBS_File_Process Set DBS_Convert = 0 Where DBS_FileId = " + Id, ref Conn);
                }
            }

            Application.DoEvents();

            if (Index + 1 < Queues.Count)
            {
                TaskExecute(Index + 1, FileStoragePath, TaskAssign, Queues, ref Conn);
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(Base.Common.PathCombine(AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }


    /// <summary>
    /// 是否分配任务
    /// </summary>
    private static bool IsAssignTask(int Id, string TaskAssign)
    {
        int Task = Id % 10;
        int Index = 0;

        try
        {
            for (Index = 0; Index <= TaskAssign.Length; Index++)
            {
                if (TaskAssign.Substring(Index, 1).TypeInt() == Task) {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(Base.Common.PathCombine(AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
        }
        finally
        {

        }

        return false;
    }


    /// <summary>
    /// 获取转换器名称
    /// </summary>
    private static string ConverterName(string Extension)
    {
        XmlNode XNodes = default(XmlNode);
        string Value = "";
        string[] Extensions = {};
        int Node = 0;
        int Item = 0;

        try
        {
            XNodes = XDocument.SelectSingleNode("/config");

            for (Node = 0; Node < XNodes.ChildNodes.Count; Node++)
            {
                Value = XNodes.ChildNodes[Node].InnerText;

                Extensions = Value.Split(',');

                for (Item = 0; Item < Extensions.Length; Item++)
                {
                    if (Extensions[Item] == Extension)
                    {
                        return XNodes.ChildNodes[Node].Name;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            File.AppendAllText(Base.Common.PathCombine(AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
        }
        finally
        {

        }

        return "";
    }


}


}