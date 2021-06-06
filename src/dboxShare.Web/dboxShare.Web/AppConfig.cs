using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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


        public static string AppLanguage
        {
            get
            {
                return ConfigurationManager.AppSettings["AppLanguage"].TypeString();
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
                var UploadSize = ConfigurationManager.AppSettings["UploadSize"].TypeInt();

                if (UploadSize < 1)
                {
                    UploadSize = 1;
                }

                if (UploadSize > 10240)
                {
                    UploadSize = 10240;
                }

                return UploadSize;
            }
        }


        public static int UserUploadSize
        {
            get
            {
                var UploadSize = Context.Session["UploadSize"].TypeInt();

                if (UploadSize == 0)
                {
                    UploadSize = AppConfig.UploadSize;
                }
                else
                {
                    if (UploadSize > AppConfig.UploadSize)
                    {
                        UploadSize = AppConfig.UploadSize;
                    }
                }

                return UploadSize;
            }
        }


        public static int DownloadSize
        {
            get
            {
                var DownloadSize = ConfigurationManager.AppSettings["DownloadSize"].TypeInt();

                if (DownloadSize < 1)
                {
                    DownloadSize = 1;
                }

                if (DownloadSize > 10240)
                {
                    DownloadSize = 10240;
                }

                return DownloadSize;
            }
        }


        public static int UserDownloadSize
        {
            get
            {
                var DownloadSize = Context.Session["DownloadSize"].TypeInt();

                if (DownloadSize == 0)
                {
                    DownloadSize = AppConfig.DownloadSize;
                }
                else
                {
                    if (DownloadSize > AppConfig.DownloadSize)
                    {
                        DownloadSize = AppConfig.DownloadSize;
                    }
                }

                return DownloadSize;
            }
        }


        public static string StoragePath
        {
            get
            {
                var StoragePath = ConfigurationManager.AppSettings["StoragePath"].TypeString();

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


        public static string IndexStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "index");
            }
        }


        public static string LogStoragePath
        {
            get
            {
                return Base.Common.PathCombine(AppConfig.StoragePath, "log");
            }
        }


        public static string TempStoragePath
        {
            get
            {
                var TempStoragePath = ConfigurationManager.AppSettings["TempStoragePath"].TypeString();

                if (string.IsNullOrEmpty(TempStoragePath) == true)
                {
                    TempStoragePath = Base.Common.PathCombine(AppConfig.StoragePath, "temp");
                }

                return TempStoragePath;
            }
        }


        public static bool StorageEncryption
        {
            get
            {
                var StorageEncryption = ConfigurationManager.AppSettings["StorageEncryption"].TypeString() == "true" ? true : false;

                var EncryptionLockFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, ".encryption");

                if (File.Exists(EncryptionLockFilePath) == true)
                {
                    return true;
                }

                var NoEncryptionLockFilePath = Base.Common.PathCombine(AppConfig.FileStoragePath, ".no-encryption");

                if (File.Exists(NoEncryptionLockFilePath) == true)
                {
                    return false;
                }

                if (StorageEncryption == true)
                {
                    File.Create(EncryptionLockFilePath);
                }
                else
                {
                    File.Create(NoEncryptionLockFilePath);
                }

                return StorageEncryption;
            }
        }


        public static int VersionCount
        {
            get
            {
                var VersionCount = ConfigurationManager.AppSettings["VersionCount"].TypeInt();

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


        public static int RetentionDays
        {
            get
            {
                var RetentionDays = ConfigurationManager.AppSettings["RetentionDays"].TypeInt();

                if (RetentionDays < 1)
                {
                    RetentionDays = 1;
                }

                if (RetentionDays > 30)
                {
                    RetentionDays = 30;
                }

                return RetentionDays;
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