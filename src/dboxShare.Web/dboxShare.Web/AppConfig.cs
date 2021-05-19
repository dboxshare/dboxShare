using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using dboxShare.Base;
using dboxShare.Web;


namespace dboxShare.Web
{


    public static class AppConfig
    {


        public static HttpContext Context
        {
            get
            {
                return HttpContext.Current;
            }
        }


        public static string ConfigFilePath
        {
            get
            {
                return Context.Server.MapPath("/web.config");
            }
        }


        public static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings["AppName"].TypeString();
            }
        }


        public static string SecurityKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SecurityKey"].TypeString();
            }
        }


        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["ConnectionString"].TypeString();
            }
        }


        public static string UploadExtension
        {
            get
            {
                return ConfigurationManager.AppSettings["UploadExtension"].TypeString();
            }
        }


        public static int UploadSize
        {
            get
            {
                int UploadSize = ConfigurationManager.AppSettings["UploadSize"].TypeInt();

                if (UploadSize < 1)
                {
                    UploadSize = 1;
                }

                if (UploadSize > 2048)
                {
                    UploadSize = 2048;
                }

                return UploadSize;
            }
        }


        public static string StoragePath
        {
            get
            {
                string StoragePath = ConfigurationManager.AppSettings["StoragePath"].TypeString();

                if (string.IsNullOrEmpty(StoragePath) == true)
                {
                    StoragePath = Context.Server.MapPath("/storage/");
                }

                return StoragePath;
            }
        }


        public static string DataStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "data");
            }
        }


        public static string FileStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "file");
            }
        }


        public static string TempStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "temp");
            }
        }


        public static string LogStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "log");
            }
        }


        public static int VersionCount
        {
            get
            {
                int VersionCount = ConfigurationManager.AppSettings["VersionCount"].TypeInt();

                if (VersionCount < 50)
                {
                    VersionCount = 50;
                }

                if (VersionCount > 500)
                {
                    VersionCount = 500;
                }

                return VersionCount;
            }
        }


        public static string MailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["MailAddress"].TypeString();
            }
        }


        public static string MailUsername
        {
            get
            {
                return ConfigurationManager.AppSettings["MailUsername"].TypeString();
            }
        }


        public static string MailPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["MailPassword"].TypeString();
            }
        }


        public static string MailServer
        {
            get
            {
                return ConfigurationManager.AppSettings["MailServer"].TypeString();
            }
        }


        public static int MailServerPort
        {
            get
            {
                return ConfigurationManager.AppSettings["MailServerPort"].TypeInt();
            }
        }


        public static bool MailServerSSL
        {
            get
            {
                return ConfigurationManager.AppSettings["MailServerSSL"].TypeString() == "true" ? true : false;
            }
        }


    }


}