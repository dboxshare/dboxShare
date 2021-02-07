using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace dboxShare.Base
{


    public static class Common
    {


        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


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
        /// 输入过滤
        /// </summary>
        public static string InputFilter(string Content)
        {
            if (string.IsNullOrEmpty(Content) == true)
            {
                return "";
            }

            if (Regex.IsMatch(Content, @"(and|or)?\s+[\w\@\.\']+\s*[\<\>\=]+\s*[\w\@\.\']+", RegexOptions.IgnoreCase) == true)
            {
                return "";
            }

            if (Regex.IsMatch(Content, @"(and|or)?\s+(alter|backup|create|declare|delete|drop|exec|insert|restore|select|truncate|update)\s+[\w\s\(\)\[\]\<\>\=\+\-\*\$\#\@\%\;\.\,\'\""]+", RegexOptions.IgnoreCase) == true)
            {
                return "";
            }

            string List = "'|\"|%|*|<|>|=";

            string[] Items = List.Split('|');

            for (int i = 0; i < Items.Length; i++)
            {
                Content = Content.Replace(Items[i], "");
            }

            return Content.Trim();
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
        /// 安全标识代码
        /// </summary>
        public static string SecurityCode(string Variable)
        {
            string ClientIP = Common.ClientIP();
            string ClientPC = Environment.MachineName;
            string ClientUser = Environment.UserName;
            string ClientDomain = Environment.UserDomainName;
            string ServerDate = DateTime.Now.ToShortDateString();

            return Common.StringHash(ClientIP + ClientPC + ClientUser + ClientDomain + ServerDate + Variable, "MD5");
        }


        /// <summary>
        /// 获取客户端 IP 地址
        /// </summary>
        public static string ClientIP()
        {
            if (string.IsNullOrEmpty(Context.Request.ServerVariables["REMOTE_ADDR"].TypeString()) == false)
            {
                return Context.Request.ServerVariables["REMOTE_ADDR"].TypeString();
            }
            else if (string.IsNullOrEmpty(Context.Request.UserHostAddress.ToString()) == false)
            {
                return Context.Request.UserHostAddress.ToString();
            }
            else
            {
                return "0.0.0.0";
            }
        }


        /// <summary>
        /// JSON 特殊字符转义
        /// </summary>
        public static string JsonEscape(string Content)
        {
            ArrayList List = new ArrayList();

            for (int i = 0; i < Content.Length; i++)
            {
                char Character = Content.ToCharArray()[i].TypeChar();

                if (Character == '\'')
                {
                    List.Add("\\'");
                }
                else if (Character == '\"')
                {
                    List.Add("\\\"");
                }
                else if (Character == '\\')
                {
                    List.Add("\\\\");
                }
                else if (Character == '/')
                {
                    List.Add("\\/");
                }
                else if (Character == '\0')
                {
                    List.Add("\\0");
                }
                else if (Character == '\a')
                {
                    List.Add("\\a");
                }
                else if (Character == '\b')
                {
                    List.Add("\\b");
                }
                else if (Character == '\f')
                {
                    List.Add("\\f");
                }
                else if (Character == '\n')
                {
                    List.Add("\\n");
                }
                else if (Character == '\r')
                {
                    List.Add("\\r");
                }
                else if (Character == '\t')
                {
                    List.Add("\\t");
                }
                else if (Character == '\v')
                {
                    List.Add("\\v");
                }
                else
                {
                    List.Add(Character);
                }
            }

            return string.Join("", List.ToArray());
        }


        /// <summary>
        /// HTML 编码
        /// </summary>
        public static string HtmlEncode(string Html, bool Wrap)
        {
            if (string.IsNullOrEmpty(Html) == true)
            {
                return "";
            }

            Html = Html.Replace("\'", "&apos;");
            Html = Html.Replace("\"", "&quot;");
            Html = Html.Replace("<", "&lt;");
            Html = Html.Replace(">", "&gt;");
            Html = Html.Replace(" ", "&nbsp;");

            if (Wrap == true)
            {
                Html = Html.Replace("\n", "<br />");
            }

            return Html;
        }


        /// <summary>
        /// HTML 解码
        /// </summary>
        public static string HtmlDecode(string Html, bool Wrap)
        {
            if (string.IsNullOrEmpty(Html) == true)
            {
                return "";
            }

            Html = Html.Replace("&apos;", "\'");
            Html = Html.Replace("&quot;", "\"");
            Html = Html.Replace("&lt;", "<");
            Html = Html.Replace("&gt;", ">");
            Html = Html.Replace("&nbsp;", " ");

            if (Wrap == true)
            {
                Html = Html.Replace("<br />", "\n");
            }

            return Html;
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        public static void SendMail(string AppName, string MailAddress, string MailUsername, string MailPassword, string MailServer, int MailServerPort, bool MailServerSSL, string Email, string Subject, string Body, bool Async)
        {
            MailMessage Mail = null;
            SmtpClient Smtp = null;

            try
            {
                Mail = new MailMessage();

                Mail.Subject = Subject;
                Mail.SubjectEncoding = Encoding.UTF8;
                Mail.Body = Body;
                Mail.BodyEncoding = Encoding.UTF8;
                Mail.IsBodyHtml = true;
                Mail.Priority = MailPriority.High;
                Mail.From = new MailAddress(MailAddress, AppName, Encoding.UTF8);
                Mail.To.Add(new MailAddress(Email));

                Smtp = new SmtpClient();

                Smtp.Host = MailServer;
                Smtp.Port = MailServerPort;
                Smtp.EnableSsl = MailServerSSL;
                Smtp.Timeout = 20000;
                Smtp.Credentials = new NetworkCredential(MailUsername, MailPassword);

                if (Async == true)
                {
                    Smtp.SendCompleted += (object sender, AsyncCompletedEventArgs e) =>
                    {
                        Smtp.Dispose();
                        Mail.Dispose();
                    };

                    Smtp.SendAsync(Mail, null);
                }
                else
                {
                    Smtp.Send(Mail);
                    Smtp.Dispose();
                    Mail.Dispose();
                }
            }
            catch (Exception)
            {
                if (Common.IsNothing(Smtp) == false)
                {
                    Smtp.Dispose();
                }

                if (Common.IsNothing(Mail) == false)
                {
                    Mail.Dispose();
                }
            }
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
