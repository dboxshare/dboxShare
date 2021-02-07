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
        /// 判断对象是否 Nothing
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
        public static string StringGet(string Content, string Pattern)
        {
            if (string.IsNullOrEmpty(Content) == true)
            {
                return "";
            }

            return Regex.Match(Content, Pattern, RegexOptions.IgnoreCase).Groups[1].Value;
        }


        /// <summary>
        /// 字符串替换
        /// </summary>
        public static string StringReplace(string Content, string Pattern, string Value)
        {
            if (string.IsNullOrEmpty(Content) == true)
            {
                return "";
            }

            return Regex.Replace(Content, Pattern, Value, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 字符串校验
        /// </summary>
        public static bool StringCheck(string Content, string Pattern)
        {
            if (string.IsNullOrEmpty(Content) == true)
            {
                return false;
            }

            if (Regex.IsMatch(Content, Pattern, RegexOptions.IgnoreCase) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 字符串哈希码
        /// </summary>
        public static string StringHash(string Content, string Method)
        {
            string Hash = "";

            if (string.IsNullOrEmpty(Content) == true)
            {
                return "";
            }

            if (Method.ToUpper() == "MD5")
            {
                using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
                {
                    byte[] Bytes = MD5.ComputeHash(Encoding.UTF8.GetBytes(Content));

                    for (int i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }
            else if (Method.ToUpper() == "SHA1")
            {
                using (SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider())
                {
                    byte[] Bytes = SHA1.ComputeHash(Encoding.UTF8.GetBytes(Content));

                    for (int i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }
            else if (Method.ToUpper() == "SHA256")
            {
                using (SHA256CryptoServiceProvider SHA256 = new SHA256CryptoServiceProvider())
                {
                    byte[] Bytes = SHA256.ComputeHash(Encoding.UTF8.GetBytes(Content));

                    for (int i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }
            else
            {
                return "";
            }

            return Hash;
        }


        /// <summary>
        /// 路径组合(两个参数)
        /// </summary>
        public static string PathCombine(string Path1, string Path2)
        {
            if (string.IsNullOrEmpty(Path1) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path2) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (Path1.IndexOf(SeparatorChar) == -1)
            {
                Path1 = Path1.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path2.IndexOf(SeparatorChar) == -1)
            {
                Path2 = Path2.Replace(ReplaceChar, SeparatorChar);
            }

            return Path.Combine(Path1, Path2);
        }


        /// <summary>
        /// 路径组合(三个参数)
        /// </summary>
        public static string PathCombine(string Path1, string Path2, string Path3)
        {
            if (string.IsNullOrEmpty(Path1) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path2) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path3) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (Path1.IndexOf(SeparatorChar) == -1)
            {
                Path1 = Path1.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path2.IndexOf(SeparatorChar) == -1)
            {
                Path2 = Path2.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path3.IndexOf(SeparatorChar) == -1)
            {
                Path3 = Path3.Replace(ReplaceChar, SeparatorChar);
            }

            return Path.Combine(Path1, Path2, Path3);
        }


        /// <summary>
        /// 路径组合(四个参数)
        /// </summary>
        public static string PathCombine(string Path1, string Path2, string Path3, string Path4)
        {
            if (string.IsNullOrEmpty(Path1) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path2) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path3) == true)
            {
                return "";
            }

            if (string.IsNullOrEmpty(Path4) == true)
            {
                return "";
            }

            char SeparatorChar = Path.DirectorySeparatorChar;
            char ReplaceChar = SeparatorChar == '/' ? '\\' : '/';

            if (Path1.IndexOf(SeparatorChar) == -1)
            {
                Path1 = Path1.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path2.IndexOf(SeparatorChar) == -1)
            {
                Path2 = Path2.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path3.IndexOf(SeparatorChar) == -1)
            {
                Path3 = Path3.Replace(ReplaceChar, SeparatorChar);
            }

            if (Path4.IndexOf(SeparatorChar) == -1)
            {
                Path4 = Path4.Replace(ReplaceChar, SeparatorChar);
            }

            return Path.Combine(Path1, Path2, Path3, Path4);
        }


        /// <summary>
        /// 进程执行(通用)
        /// </summary>
        public static void ProcessExecute(string FileName, string Arguments, int Wait)
        {
            using (Process Process = new Process())
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
        }


        /// <summary>
        /// 进程执行(带工作目录参数)
        /// </summary>
        public static void ProcessExecute(string FileName, string WorkingDirectory, string Arguments, int Wait)
        {
            using (Process Process = new Process())
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
        }


        /// <summary>
        /// 判断进程是否存在
        /// </summary>
        public static bool ProcessExists(string Keys)
        {
            string[] Items = Keys.Split(',');

            foreach (Process ProcessItem in Process.GetProcesses())
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (ProcessItem.ToString().ToLower().Contains(Items[i].ToLower()) == true)
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
            foreach (Process ProcessItem in Process.GetProcesses())
            {
                if (ProcessItem.ToString().ToLower().Contains(Key.ToLower()) == true)
                {
                    ProcessItem.Kill();
                }
            }
        }


    }


}
