using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using dboxSyncer;


namespace dboxSyncer
{


    sealed class SyncProcess
    {


        public static void Main()
        {
            var Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                return;
            }

            var Action = Args[1].TypeString();

            if (Action != "upload" && Action != "download")
            {
                return;
            }

            try
            {
                TaskSchedule(Action);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        /// <summary>
        /// 任务计划
        /// </summary>
        private static void TaskSchedule(string Action)
        {
            var XDocument = new XmlDocument();

            XDocument.Load(AppConfig.DataFilePath);

            var XNodes = XDocument.SelectNodes("/rules/item");

            foreach (XmlNode XNode in XNodes)
            {
                var LocalFolderPath = XNode.SelectSingleNode("localFolderPath").InnerText;
                var NetFolderId = XNode.SelectSingleNode("netFolderId").InnerText;
                var NetFolderPath = XNode.SelectSingleNode("netFolderPath").InnerText;
                var Type = XNode.SelectSingleNode("type").InnerText;

                var LocalDataList = new List<Hashtable>();

                LocalDataToList(LocalFolderPath, ref LocalDataList);

                var NetDataList = new List<Hashtable>();
                var NetFolderTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                NetDataToList(NetFolderId, ref NetDataList, ref NetFolderTable);

                // 执行上传处理
                if (Action == "upload")
                {
                    // 判断网络文件夹是否具有上传权限
                    if (AppCommon.StringCheck(NetFolderTable["Purview"].TypeString(), @"^(uploader|editor|manager|creator)$") == true)
                    {
                        if (Type == "sync" || Type == "upload")
                        {
                            // 判断网络文件夹是否锁定状态
                            if (NetFolderTable["Lock"].TypeInt() == 0)
                            {
                                UploadScan(LocalFolderPath, ref LocalDataList, NetFolderId, NetFolderPath, NetFolderTable["Purview"].TypeString(), ref NetDataList);
                            }
                        }
                    }
                }

                // 执行下载处理
                if (Action == "download")
                {
                    // 判断网络文件夹是否具有下载权限
                    if (AppCommon.StringCheck(NetFolderTable["Purview"].TypeString(), @"^(downloader|uploader|editor|manager|creator)$") == true)
                    {
                        if (Type == "sync" || Type == "download")
                        {
                            DownloadScan(LocalFolderPath, ref LocalDataList, NetFolderId, NetFolderPath, NetFolderTable["Purview"].TypeString(), ref NetDataList);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 本地文件夹扫描(上传文件)
        /// </summary>
        private static void UploadScan(string LocalFolderPath, ref List<Hashtable> LocalDataList, string NetFolderId, string NetFolderPath, string NetFolderPurview, ref List<Hashtable> NetDataList)
        {
            for (var i = 0; i < LocalDataList.Count; i++)
            {
                var NetTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 找查网络是否存在该文件并载入到 NetTable
                NetDataTable(LocalDataList[i]["Name"].TypeString(), ref NetDataList, ref NetTable);

                // 判断网络是否存在该文件
                if (NetTable["Exists"].TypeBool() == false)
                {
                    FileUpload(LocalDataList[i]["Path"].TypeString(), LocalDataList[i]["Size"].TypeString(), LocalDataList[i]["Hash"].TypeString(), NetFolderId);
                }
                else
                {
                    // 利用哈希码判断本地文件与网络文件是否相同
                    if (LocalDataList[i]["Hash"].TypeString() == NetTable["Hash"].TypeString())
                    {
                        // 文件相同无需处理
                    }
                    else
                    {
                        // 判断文件是否锁定状态
                        if (NetTable["Lock"].TypeInt() == 0)
                        {
                            // 判断网络文件夹是否具有版本上传权限
                            if (AppCommon.StringCheck(NetFolderPurview, @"^(editor|manager|creator)$") == true)
                            {
                                // 利用哈希码判断网络是否存在该文件版本
                                if (NetVersionExist(LocalDataList[i]["Hash"].TypeString(), NetTable["Id"].TypeInt(), NetDataList) == false)
                                {
                                    // 不存在该文件版本(上传新版本)
                                    FileUpversion(LocalDataList[i]["Path"].TypeString(), LocalDataList[i]["Size"].TypeString(), LocalDataList[i]["Hash"].TypeString(), NetTable["Id"].TypeInt());
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 网络文件夹扫描(下载文件)
        /// </summary>
        private static void DownloadScan(string LocalFolderPath, ref List<Hashtable> LocalDataList, string NetFolderId, string NetFolderPath, string NetFolderPurview, ref List<Hashtable> NetDataList)
        {
            for (var i = 0; i < NetDataList.Count; i++)
            {
                // 判断是否文件版本(跳过)
                if (NetDataList[i]["VersionId"].TypeInt() > 0)
                {
                    continue;
                }

                var LocalTable = new Hashtable(StringComparer.OrdinalIgnoreCase);

                // 找查本地是否存在该文件并载入到 LocalTable
                LocalDataTable(NetDataList[i]["Name"].TypeString(), ref LocalDataList, ref LocalTable);

                // 判断本地是否存在该文件
                if (LocalTable["Exists"].TypeBool() == false)
                {
                    FileDownload(NetDataList[i]["Id"].TypeInt(), NetDataList[i]["CodeId"].TypeString(), NetDataList[i]["Name"].TypeString(), LocalFolderPath);
                }
                else
                {
                    // 利用哈希码判断网络文件与本地文件是否相同
                    if (NetDataList[i]["Hash"].TypeString() == LocalTable["Hash"].TypeString())
                    {
                        // 文件相同无需处理
                    }
                    else
                    {
                        // 判断网络文件夹是否具有版本下载权限
                        if (AppCommon.StringCheck(NetFolderPurview, @"^(editor|manager|creator)$") == true)
                        {
                            // 利用更新时间判断文件版本新旧
                            if (DateTime.Compare(NetDataList[i]["UpdateTime"].TypeDateTime(), LocalTable["UpdateTime"].TypeDateTime()) > 0)
                            {
                                // 网络文件版本比本地文件版本新(下载新版本)
                                FileDownload(NetDataList[i]["Id"].TypeInt(), NetDataList[i]["CodeId"].TypeString(), NetDataList[i]["Name"].TypeString(), LocalFolderPath);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 遍历本地文件夹获取文件集合返回 List<Hashtable>
        /// </summary>
        private static void LocalDataToList(string DirectoryPath, ref List<Hashtable> LocalDataList)
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                return;
            }

            var DI = new DirectoryInfo(DirectoryPath);

            foreach (var FileItem in DI.GetFiles())
            {
                var Table = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Table.Add("Path", FileItem.FullName);
                Table.Add("Name", FileItem.Name);
                Table.Add("Size", FileItem.Length);
                Table.Add("Hash", FileHash(FileItem.FullName));
                Table.Add("UpdateTime", File.GetLastWriteTime(FileItem.FullName));

                LocalDataList.Add(Table);
            }
        }


        /// <summary>
        /// 读取本地文件数据返回 Hashtable
        /// </summary>
        private static void LocalDataTable(string NetFileName, ref List<Hashtable> LocalDataList, ref Hashtable Table)
        {
            var Position = -1;

            for (var i = 0; i < LocalDataList.Count; i++)
            {
                if (LocalDataList[i]["Name"].TypeString() == NetFileName)
                {
                    Position = i;
                    break;
                }
            }

            if (Position == -1)
            {
                Table.Add("Exists", false);
            }
            else
            {
                Table.Add("Exists", true);
                Table.Add("Hash", LocalDataList[Position]["Hash"]);
                Table.Add("UpdateTime", LocalDataList[Position]["UpdateTime"]);
            }
        }


        /// <summary>
        /// 读取网络文件夹获取文件集合返回 List<Hashtable>
        /// </summary>
        private static void NetDataToList(string NetFolderId, ref List<Hashtable> NetDataList, ref Hashtable NetFolderTable)
        {
            var WebClient = (WebClient)null;
            var XData = "";

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                var Url = "" + AppConfig.ServerUrl + "/api/sync/file-list-xml?folderid=" + NetFolderId + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                var ResponseData = WebClient.DownloadData(new Uri(Url));

                XData = Encoding.UTF8.GetString(ResponseData);
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }
            }

            if (XData.IndexOf("<item>") == -1 || XData.IndexOf("</item>") == -1)
            {
                return;
            }

            var XDocument = new XmlDocument();

            XDocument.LoadXml(XData);

            var XNodes = XDocument.SelectNodes("/root/folder");

            foreach (XmlNode XNode in XNodes)
            {
                NetFolderTable.Add("Share", XNode.SelectSingleNode("share").InnerText);
                NetFolderTable.Add("Lock", XNode.SelectSingleNode("lock").InnerText);
                NetFolderTable.Add("Purview", XNode.SelectSingleNode("purview").InnerText);
            }

            XNodes = XDocument.SelectNodes("/root/files/item");

            foreach (XmlNode XNode in XNodes)
            {
                var Table = new Hashtable(StringComparer.OrdinalIgnoreCase);

                Table.Add("Id", XNode.SelectSingleNode("id").InnerText);
                Table.Add("VersionId", XNode.SelectSingleNode("versionId").InnerText);
                Table.Add("CodeId", XNode.SelectSingleNode("codeId").InnerText);
                Table.Add("Hash", XNode.SelectSingleNode("hash").InnerText);
                Table.Add("Name", XNode.SelectSingleNode("name").InnerText);
                Table.Add("Size", XNode.SelectSingleNode("size").InnerText);
                Table.Add("Lock", XNode.SelectSingleNode("lock").InnerText);
                Table.Add("UpdateTime", XNode.SelectSingleNode("updateTime").InnerText);

                NetDataList.Add(Table);
            }
        }


        /// <summary>
        /// 读取网络文件数据返回 Hashtable
        /// </summary>
        private static void NetDataTable(string LocalFileName, ref List<Hashtable> NetDataList, ref Hashtable Table)
        {
            var Position = -1;

            for (var i = 0; i < NetDataList.Count; i++)
            {
                if (NetDataList[i]["VersionId"].TypeInt() == 0)
                {
                    if (NetDataList[i]["Name"].TypeString() == LocalFileName)
                    {
                        Position = i;
                        break;
                    }
                }
            }

            if (Position == -1)
            {
                Table.Add("Exists", false);
            }
            else
            {
                Table.Add("Exists", true);
                Table.Add("Id", NetDataList[Position]["Id"]);
                Table.Add("Hash", NetDataList[Position]["Hash"]);
                Table.Add("Lock", NetDataList[Position]["Lock"]);
                Table.Add("UpdateTime", NetDataList[Position]["UpdateTime"]);
            }
        }


        /// <summary>
        /// 找查网络文件夹是否存在该文件版本
        /// </summary>
        private static bool NetVersionExist(string LocalFileHash, int NetFileId, List<Hashtable> NetDataList)
        {
            for (var i = 0; i < NetDataList.Count; i++)
            {
                if (NetDataList[i]["VersionId"].TypeInt() == NetFileId)
                {
                    if (NetDataList[i]["Hash"].TypeString() == LocalFileHash)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 获取文件哈希码
        /// </summary>
        private static string FileHash(string FilePath)
        {
            var Hash = "";

            using (var FStream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var MD5 = new MD5CryptoServiceProvider())
                {
                    var Bytes = MD5.ComputeHash(FStream);

                    for (var i = 0; i < Bytes.Length; i++)
                    {
                        Hash += Bytes[i].ToString("X2");
                    }
                }
            }

            return Hash;
        }


        /// <summary>
        /// 新文件上传
        /// </summary>
        private static void FileUpload(string FilePath, string FileSize, string FileHash, string FolderId)
        {
            var IntervalTime = 0;

            if (AppConfig.UploadSpeed == 0)
            {
                IntervalTime = 0;
            }
            else
            {
                IntervalTime = (int)1000 / (AppConfig.UploadSpeed);
            }

            var WebClient = (WebClient)null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                var UploadUrl = "" + AppConfig.ServerUrl + "/api/sync/file-upload?guid=" + System.Guid.NewGuid().ToString() + "&folderid=" + FolderId + "&filepath=" + HttpUtility.UrlEncode(FilePath) + "&filesize=" + FileSize + "&filehash=" + FileHash + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                using (var WebStream = WebClient.OpenWrite(UploadUrl))
                {
                    using (var FStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    {
                        var Buffer = new byte[65536];
                        var Read = 0;

                        while ((Read = FStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                        {
                            WebStream.Write(Buffer, 0, Read);

                            Application.DoEvents();

                            if (IntervalTime > 0)
                            {
                                Thread.Sleep(IntervalTime);
                            }
                        }
                    }
                }

                SyncLog("upload", FilePath);
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }
            }
        }


        /// <summary>
        /// 新版本上传
        /// </summary>
        private static void FileUpversion(string FilePath, string FileSize, string FileHash, int FileId)
        {
            var IntervalTime = 0;

            if (AppConfig.UploadSpeed == 0)
            {
                IntervalTime = 0;
            }
            else
            {
                IntervalTime = (int)1000 / (AppConfig.UploadSpeed);
            }

            var WebClient = (WebClient)null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                var UpversionUrl = "" + AppConfig.ServerUrl + "/api/sync/file-upversion?guid=" + System.Guid.NewGuid().ToString() + "&fileid=" + FileId + "&filepath=" + HttpUtility.UrlEncode(FilePath) + "&filesize=" + FileSize + "&filehash=" + FileHash + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                using (var WebStream = WebClient.OpenWrite(UpversionUrl))
                {
                    using (var FStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true))
                    {
                        var Buffer = new byte[65536];
                        var Read = 0;

                        while ((Read = FStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                        {
                            WebStream.Write(Buffer, 0, Read);

                            Application.DoEvents();

                            if (IntervalTime > 0)
                            {
                                Thread.Sleep(IntervalTime);
                            }
                        }
                    }
                }

                SyncLog("upversion", FilePath);
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }
            }
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        private static void FileDownload(int FileId, string FileCodeId, string FileName, string LocalFolderPath)
        {
            if (Directory.Exists(AppCommon.PathCombine(AppConfig.AppPath, "temp")) == false)
            {
                Directory.CreateDirectory(AppCommon.PathCombine(AppConfig.AppPath, "temp"));
            }

            var TempFilePath = AppCommon.PathCombine(AppConfig.AppPath, "temp", FileName);

            var SaveFilePath = AppCommon.PathCombine(LocalFolderPath, FileName);

            if (File.Exists(TempFilePath) == true)
            {
                File.Delete(TempFilePath);
            }

            var IntervalTime = 0;

            if (AppConfig.DownloadSpeed == 0)
            {
                IntervalTime = 0;
            }
            else
            {
                IntervalTime = (int)1000 / (AppConfig.DownloadSpeed);
            }

            var WebClient = (WebClient)null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                var DownloadUrl = "" + AppConfig.ServerUrl + "/api/sync/file-download?id=" + FileId + "&codeid=" + FileCodeId + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                using (var WebStream = WebClient.OpenRead(DownloadUrl))
                {
                    using (var FStream = new FileStream(TempFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                    {
                        var Buffer = new byte[65536];
                        var Read = 0;

                        while ((Read = WebStream.Read(Buffer, 0, (int)Buffer.Length)) > 0)
                        {
                            FStream.Write(Buffer, 0, Read);

                            Application.DoEvents();

                            if (IntervalTime > 0)
                            {
                                Thread.Sleep(IntervalTime);
                            }
                        }
                    }
                }

                File.Copy(TempFilePath, SaveFilePath, true);

                if (File.Exists(TempFilePath) == true)
                {
                    File.Delete(TempFilePath);
                }

                SyncLog("download", SaveFilePath);
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }
            }
        }


        /// <summary>
        /// 同步日志
        /// </summary>
        private static void SyncLog(string Action, string FilePath)
        {
            if (Directory.Exists(AppCommon.PathCombine(AppConfig.AppPath, "log")) == false)
            {
                Directory.CreateDirectory(AppCommon.PathCombine(AppConfig.AppPath, "log"));
            }

            var Files = Directory.GetFiles(AppCommon.PathCombine(AppConfig.AppPath, "log"));

            foreach (var Path in Files)
            {
                if (DateTime.Compare(File.GetLastWriteTime(Path), DateTime.Now.AddDays(-14)) < 0)
                {
                    File.Delete(Path);
                }
            }

            File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "log", "" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"), "" + DateTime.Now.ToString() + " " + Action + " " + FilePath + "\n");
        }


    }


}