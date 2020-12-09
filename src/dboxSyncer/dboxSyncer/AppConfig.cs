using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;


namespace dboxSyncer
{


    public static class AppConfig
    {


        public static string AppPath = GetAppPath();
        public static string DataPath = GetDataPath();
        public static string ServerHost = GetServerHost();
        public static string IntervalTime = GetIntervalTime();
        public static string UploadSpeed = GetUploadSpeed();
        public static string DownloadSpeed = GetDownloadSpeed();
        public static string LoginId = GetLoginId();
        public static string Password = GetPassword();
        public static string UserSession = GetUserSession();
        public static string AutoLogin = GetAutoLogin();


        /// <summary>
        /// 获取配置文件参数值
        /// </summary>
        private static string ConfigValue(string Name)
        {
            System.Configuration.Configuration Config = default(System.Configuration.Configuration);
            AppSettingsSection AppSetting = default(AppSettingsSection);

            Config = ConfigurationManager.OpenExeConfiguration(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.exe"));

            AppSetting = (System.Configuration.AppSettingsSection)Config.GetSection("appSettings");

            if (AppCommon.IsNothing(AppSetting.Settings[Name]) == true)
            {
                return string.Empty;
            }
            else
            {
                return AppSetting.Settings[Name].Value;
            }
        }


        /// <summary>
        /// 获取当前程序目录路径
        /// </summary>
        private static string GetAppPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


        /// <summary>
        /// 获取数据文件路径
        /// </summary>
        private static string GetDataPath()
        {
            return AppCommon.PathCombine(AppConfig.AppPath, "data.xml");
        }


        /// <summary>
        /// 获取服务器主机网址
        /// </summary>
        private static string GetServerHost()
        {
            string ServerHost = ConfigValue("ServerHost").TypeString();

            if (AppCommon.StringCheck(ServerHost, @"^(http|https)\:\/\/[\w\-\.\:]+\/?$") == false)
            {
                return string.Empty;
            }

            if (ServerHost.Substring(ServerHost.Length - 1, 1) == "/")
            {
                ServerHost = ServerHost.Substring(0, ServerHost.Length - 1);
            }

            return ServerHost;
        }


        /// <summary>
        /// 获取同步间隔时间
        /// </summary>
        private static string GetIntervalTime()
        {
            string IntervalTime = ConfigValue("IntervalTime").TypeString();

            if (AppCommon.IsNumeric(IntervalTime) == false)
            {
                IntervalTime = "20";
            }

            if (IntervalTime.TypeInt() < 20)
            {
                IntervalTime = "20";
            }

            if (IntervalTime.TypeInt() > 360)
            {
                IntervalTime = "360";
            }

            return IntervalTime;
        }


        /// <summary>
        /// 获取上传文件速度限制
        /// </summary>
        private static string GetUploadSpeed()
        {
            string UploadSpeed = ConfigValue("UploadSpeed").TypeString();

            if (AppCommon.IsNumeric(UploadSpeed) == false)
            {
                UploadSpeed = "0";
            }

            if (UploadSpeed.TypeInt() < 0)
            {
                UploadSpeed = "0";
            }

            if (UploadSpeed.TypeInt() > 1000)
            {
                UploadSpeed = "1000";
            }

            return UploadSpeed;
        }


        /// <summary>
        /// 获取下载文件速度限制
        /// </summary>
        private static string GetDownloadSpeed()
        {
            string DownloadSpeed = ConfigValue("DownloadSpeed").TypeString();

            if (AppCommon.IsNumeric(DownloadSpeed) == false)
            {
                DownloadSpeed = "0";
            }

            if (DownloadSpeed.TypeInt() < 0)
            {
                DownloadSpeed = "0";
            }

            if (DownloadSpeed.TypeInt() > 1000)
            {
                DownloadSpeed = "1000";
            }

            return DownloadSpeed;
        }


        /// <summary>
        /// 获取登录id
        /// </summary>
        private static string GetLoginId()
        {
            return ConfigValue("LoginId").TypeString();
        }


        /// <summary>
        /// 获取登录密码
        /// </summary>
        private static string GetPassword()
        {
            return ConfigValue("Password").TypeString();
        }


        /// <summary>
        /// 获取服务器用户session
        /// </summary>
        private static string GetUserSession()
        {
            return ConfigValue("UserSession").TypeString();
        }


        /// <summary>
        /// 获取自动登录状态
        /// </summary>
        private static string GetAutoLogin()
        {
            string AutoLogin  = ConfigValue("AutoLogin").TypeString();
            
            if (AppCommon.StringCheck(AutoLogin, @"^(true|false)$") == false)
            {
                AutoLogin = "false";
            }

            return AutoLogin;
        }


    }
    
    
}
