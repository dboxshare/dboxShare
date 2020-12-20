using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace dboxSyncer
{


    public static class AppCommon
    {


        /// <summary>
        /// 判断是否数字类型
        /// </summary>
        public static bool IsNumeric(object Object)
        {
            try
            {
                long Numeric = System.Convert.ToInt64(Object);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 判断对象是否nothing
        /// </summary>
        public static bool IsNothing(object Object)
        {
            try
            {
                return ReferenceEquals(Object, null);
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 字符串获取
        /// </summary>
        public static string StringGet(string String, string Pattern)
        {
            if (string.IsNullOrEmpty(String) == true)
            {
                return "";
            }

            return Regex.Match(String, Pattern, RegexOptions.IgnoreCase).Groups[1].Value;
        }


        /// <summary>
        /// 字符串替换
        /// </summary>
        public static string StringReplace(string String, string Pattern, string Value)
        {
            if (string.IsNullOrEmpty(String) == true)
            {
                return "";
            }

            return Regex.Replace(String, Pattern, Value, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 字符串校验
        /// </summary>
        public static bool StringCheck(string String, string Pattern)
        {
            if (string.IsNullOrEmpty(String) == true)
            {
                return false;
            }

            if (Regex.IsMatch(String, Pattern, RegexOptions.IgnoreCase) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 字符串加密
        /// </summary>
        public static string StringCrypto(string String, string Method)
        {
            byte[] Hash = {};
            string Code = "";
            int Index = 0;

            if (string.IsNullOrEmpty(String) == true)
            {
                return "";
            }

            if (Method.ToUpper() == "MD5")
            {
                Hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(String));
            }
            else if (Method.ToUpper() == "SHA1")
            {
                Hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(String));
            }
            else
            {
                return "";
            }

            for (Index = 0; Index < Hash.Length; Index++)
            {
                Code += Hash[Index].ToString("X2");
            }

            return Code;
        }


        /// <summary>
        /// 路径组合(两个参数)
        /// </summary>
        public static string PathCombine(string StringA, string StringB)
        {
            if (string.IsNullOrEmpty(StringA) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringB) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (StringA.IndexOf(SeparatorChar) == -1)
            {
                StringA = StringA.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringB.IndexOf(SeparatorChar) == -1)
            {
                StringB = StringB.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            return Path.Combine(StringA, StringB);
        }


        /// <summary>
        /// 路径组合(三个参数)
        /// </summary>
        public static string PathCombine(string StringA, string StringB, string StringC)
        {
            if (string.IsNullOrEmpty(StringA) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringB) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringC) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (StringA.IndexOf(SeparatorChar) == -1)
            {
                StringA = StringA.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringB.IndexOf(SeparatorChar) == -1)
            {
                StringB = StringB.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringC.IndexOf(SeparatorChar) == -1)
            {
                StringC = StringC.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            return Path.Combine(StringA, StringB, StringC);
        }


        /// <summary>
        /// 路径组合(四个参数)
        /// </summary>
        public static string PathCombine(string StringA, string StringB, string StringC, string StringD)
        {
            if (string.IsNullOrEmpty(StringA) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringB) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringC) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(StringD) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (StringA.IndexOf(SeparatorChar) == -1)
            {
                StringA = StringA.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringB.IndexOf(SeparatorChar) == -1)
            {
                StringB = StringB.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringC.IndexOf(SeparatorChar) == -1)
            {
                StringC = StringC.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            if (StringD.IndexOf(SeparatorChar) == -1)
            {
                StringD = StringD.Replace(ReplaceChar, Path.DirectorySeparatorChar);
            }

            return Path.Combine(StringA, StringB, StringC, StringD);
        }


        /// <summary>
        /// 进程执行(通用)
        /// </summary>
        public static void ProcessExecute(string FileName, string Arguments, int Wait)
        {
            Process Process = new Process();

            try
            {
                Process.StartInfo.FileName = FileName;
                Process.StartInfo.Arguments = Arguments;
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.RedirectStandardInput = false;
                Process.StartInfo.RedirectStandardOutput = false;
                Process.StartInfo.RedirectStandardError = false;
                Process.StartInfo.CreateNoWindow = true;
                Process.StartInfo.Verb = "runas";
                Process.Start();

                if (Wait == 0)
                {
                    Process.WaitForExit();
                }
                else if (Wait > 0)
                {
                    Process.WaitForExit(Wait);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (AppCommon.IsNothing(Process) == false)
                {
                    Process.Close();
                    Process.Dispose();
                }
            }
        }


        /// <summary>
        /// 进程执行(带工作目录参数)
        /// </summary>
        public static void ProcessExecute(string FileName, string WorkingDirectory, string Arguments, int Wait)
        {
            Process Process = new Process();

            try
            {
                Process.StartInfo.FileName = FileName;
                Process.StartInfo.WorkingDirectory = WorkingDirectory;
                Process.StartInfo.Arguments = Arguments;
                Process.StartInfo.UseShellExecute = false;
                Process.StartInfo.RedirectStandardInput = false;
                Process.StartInfo.RedirectStandardOutput = false;
                Process.StartInfo.RedirectStandardError = false;
                Process.StartInfo.CreateNoWindow = true;
                Process.StartInfo.Verb = "runas";
                Process.Start();

                if (Wait == 0)
                {
                    Process.WaitForExit();
                }
                else if (Wait > 0)
                {
                    Process.WaitForExit(Wait);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (AppCommon.IsNothing(Process) == false)
                {
                    Process.Close();
                    Process.Dispose();
                }
            }
        }


        /// <summary>
        /// 判断进程是否存在
        /// </summary>
        public static bool ProcessExist(string Keys)
        {
            string[] Items = {};
            int Index = 0;

            Items = Keys.Split(',');

            foreach (Process ThisProcess in Process.GetProcesses())
            {
                for (Index = 0; Index < Items.Length; Index++)
                {
                    if (ThisProcess.ToString().ToLower().Contains(Items[Index].ToLower()) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 结束进程
        /// </summary>
        public static void ProcessExit(string Key)
        {
            foreach (Process ThisProcess in Process.GetProcesses())
            {
                if (ThisProcess.ToString().ToLower().Contains(Key.ToLower()) == true)
                {
                    ThisProcess.Kill();
                    return;
                }
            }
        }


    }


}
