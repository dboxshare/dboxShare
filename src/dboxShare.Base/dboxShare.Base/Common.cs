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


        private static HttpContext Context
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
        /// 输入过滤
        /// </summary>
        public static string InputFilter(string String)
        {
            if (string.IsNullOrEmpty(String) == true)
            {
                return "";
            }

            if (Regex.IsMatch(String, @"(and|or)?\s+[\w\@\.\']+\s*[\<\>\=]+\s*[\w\@\.\']+", RegexOptions.IgnoreCase) == true)
            {
                return "";
            }

            if (Regex.IsMatch(String, @"(and|or)?(alter|backup|create|declare|delete|drop|exec|insert|restore|select|truncate|update)\s+[\w\s\(\)\[\]\<\>\=\+\-\*\$\#\@\%\;\.\,\'\""]+", RegexOptions.IgnoreCase) == true)
            {
                return "";
            }

            string Lists = "";
            string[] Items = {};
            int Index = 0;

            Lists = "'|\"|%|*|<|>|=";

            Items = Lists.Split('|');

            for (Index = 0; Index < Items.Length; Index++)
            {
                String = String.Replace(Items[Index], "");
            }

            return String.Trim();
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
        /// 安全标识代码
        /// </summary>
        public static string SecurityCode(string Variable)
        {
            string ClientPC = Environment.MachineName;
            string ClientUser = Environment.UserName;
            string ClientDomain = Environment.UserDomainName;
            string ServerDate = DateTime.Now.ToShortDateString();

            return Common.StringCrypto(Common.ClientIP() + ClientPC + ClientUser + ClientDomain + ServerDate + Variable, "MD5");
        }


        /// <summary>
        /// 获取客户端ip地址
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
        /// json特殊字符转义
        /// </summary>
        public static string JsonEscape(string String)
        {
            ArrayList List = new ArrayList();
            char Character = '\0';
            int Index = 0;

            for (Index = 0; Index < String.Length; Index++)
            {
                Character = String.ToCharArray()[Index].TypeChar();

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
        /// 输出json
        /// </summary>
        public static void OutputJson(string String)
        {
            if (string.IsNullOrEmpty(String) == true)
            {
                return;
            }

            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Charset = "UTF-8";
            Context.Response.BinaryWrite(new byte[] { 0xEF, 0xBB, 0xBF });
            Context.Response.Write(String);
        }


        /// <summary>
        /// html编码
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
        /// html解码
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
        public static void SendMail(string AppName, string MailAddress, string MailServer, string MailPort, string MailUsername, string MailPassword, string Email, string Subject, string Body, bool Ssl, bool Async)
        {
            MailMessage Mail = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            try
            {
                Mail.Subject = Subject;
                Mail.SubjectEncoding = Encoding.UTF8;
                Mail.Body = Body;
                Mail.BodyEncoding = Encoding.UTF8;
                Mail.IsBodyHtml = true;
                Mail.Priority = MailPriority.High;
                Mail.From = new MailAddress(MailAddress, AppName, Encoding.UTF8);
                Mail.To.Add(new MailAddress(Email));

                Smtp.Port = MailPort.TypeInt();
                Smtp.Host = MailServer;
                Smtp.EnableSsl = Ssl;
                Smtp.Timeout = 10 * 1000;
                Smtp.Credentials = new NetworkCredential(MailUsername, MailPassword);

                if (Async == true)
                {
                    Smtp.SendCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            Smtp.Dispose();
                        };

                    Smtp.SendAsync(Mail, null);
                }
                else
                {
                    Smtp.Send(Mail);
                    Smtp.Dispose();
                }

                Mail.Dispose();
            }
            catch (Exception)
            {
                
            }
            finally
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
                if (Common.IsNothing(Process) == false)
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
                if (Common.IsNothing(Process) == false)
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

