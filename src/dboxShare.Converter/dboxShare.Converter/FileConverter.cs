using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using dboxShare.Base;


namespace dboxShare
{


sealed class FileConverter
{


    private static string AppPath = AppDomain.CurrentDomain.BaseDirectory;
    private static XmlDocument XDocument = new XmlDocument();


    public static void Main()
    {
        string[] Args = System.Environment.GetCommandLineArgs();
        string ConverterName = "";
        string FilePath = "";

        try
        {
            ConverterName = Args[1].TypeString();

            if (string.IsNullOrEmpty(ConverterName) == true)
            {
                return;
            }

            FilePath = Args[2].TypeString();

            if (string.IsNullOrEmpty(FilePath) == true)
            {
                return;
            }

            if (File.Exists(FilePath) == false)
            {
                return;
            }

            XDocument.Load(Base.Common.PathCombine(AppPath, "converter.xml"));

                ConverterProcess(ConverterName, FilePath);
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
    /// 转换器处理
    /// </summary>
    private static void ConverterProcess(string ConverterName, string FilePath)
    {
        string Exec = "";
        string Cmd = "";
        string Extension = "";
        int Wait = 0;

        try
        {
            Exec = ConverterParam(ConverterName, "exec").TypeString();

            if (string.IsNullOrEmpty(Exec) == true)
            {
                return;
            }

            if (Exec.IndexOf(':') == -1)
            {
                Exec = Base.Common.PathCombine(AppPath, Exec);
            }

            Cmd = ConverterParam(ConverterName, "cmd").TypeString();

            if (string.IsNullOrEmpty(Cmd) == true)
            {
                return;
            }

            Extension = ConverterParam(ConverterName, "extension").TypeString();

            if (string.IsNullOrEmpty(Extension) == true)
            {
                return;
            }

            Wait = ConverterParam(ConverterName, "wait").TypeInt();

            Cmd = Cmd.Replace("{source}", "\"" + FilePath + "\"");
            Cmd = Cmd.Replace("{target}", "\"" + FilePath + "." + Extension + "\"");

            Base.Common.ProcessExecute(Exec, Cmd, Wait);
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
    /// 获取转换器参数
    /// </summary>
    private static string ConverterParam(string Name, string Param)
    {
        XmlNode XNodes = default(XmlNode);

        try
        {
            XNodes = XDocument.SelectSingleNode("/config");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                if (XNode.Name.ToString() == Name)
                {
                    if (Base.Common.IsNothing(XNode.Attributes[Param]) == true)
                    {
                        return "";
                    }
                    else
                    {
                        return XNode.Attributes[Param].Value;
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