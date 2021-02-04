using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


namespace dboxSyncer
{


    public static class AppConfig
    {


        public static string AppPath = _AppPath();
        public static string DataFilePath = _DataFilePath();
        public static string ConfigFilePath = _ConfigFilePath();
        public static string ServerHost = _ServerHost();
        public static int IntervalTime = _IntervalTime();
        public static int UploadSpeed = _UploadSpeed();
        public static int DownloadSpeed = _DownloadSpeed();
        public static string LoginId = _LoginId();
        public static string Password = _Password();
        public static string UserSession = _UserSession();
        public static bool AutoLogin = _AutoLogin();


        private static string _AppPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


        private static string _DataFilePath()
        {
            return AppCommon.PathCombine(AppConfig.AppPath, "data.xml");
        }


        private static string _ConfigFilePath()
        {
            return AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.exe.config");
        }


        private static string _ServerHost()
        {
            string ServerHost = AppSettings("ServerHost").TypeString();

            if (AppCommon.StringCheck(ServerHost, @"^(http|https)\:\/\/[\w\-\.\:]+\/?$") == false)
            {
                return "";
            }

            if (ServerHost.Substring(ServerHost.Length - 1, 1) == "/")
            {
                ServerHost = ServerHost.Substring(0, ServerHost.Length - 1);
            }

            return ServerHost;
        }


        private static int _IntervalTime()
        {
            int IntervalTime = AppSettings("IntervalTime").TypeInt();

            if (IntervalTime < 20)
            {
                IntervalTime = 20;
            }

            if (IntervalTime > 360)
            {
                IntervalTime = 300;
            }

            return IntervalTime;
        }


        private static int _UploadSpeed()
        {
            int UploadSpeed = AppSettings("UploadSpeed").TypeInt();

            if (UploadSpeed < 0)
            {
                UploadSpeed = 0;
            }

            if (UploadSpeed > 1000)
            {
                UploadSpeed = 1000;
            }

            return UploadSpeed;
        }


        private static int _DownloadSpeed()
        {
            int DownloadSpeed = AppSettings("DownloadSpeed").TypeInt();

            if (DownloadSpeed < 0)
            {
                DownloadSpeed = 0;
            }

            if (DownloadSpeed > 1000)
            {
                DownloadSpeed = 1000;
            }

            return DownloadSpeed;
        }


        private static string _LoginId()
        {
            return LoginId = AppSettings("LoginId").TypeString();
        }


        private static string _Password()
        {
            return AppSettings("Password").TypeString();
        }


        private static string _UserSession()
        {
            return AppSettings("UserSession").TypeString();
        }


        private static bool _AutoLogin()
        {
            return AppSettings("AutoLogin") == "true" ? true : false;
        }


        private static string AppSettings(string Name)
        {
            Configuration Config = ConfigurationManager.OpenExeConfiguration(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.exe"));

            AppSettingsSection AppSettings = (AppSettingsSection)Config.GetSection("appSettings");

            if (AppCommon.IsNothing(AppSettings.Settings[Name]) == true)
            {
                return "";
            }
            else
            {
                return AppSettings.Settings[Name].Value;
            }
        }


    }


}
