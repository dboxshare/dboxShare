using dboxSyncer;
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


namespace dboxSyncer
{


    sealed class SyncProcess
    {


        public static void Main()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length - 1 > 0)
            {
                Application.Exit();
            }

            try
            {
                TaskQueue();
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
        /// 任务队列
        /// </summary>
        private static void TaskQueue()
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNodeList XNodes = default(XmlNodeList);
            string LocalFolderPath = "";
            int NetFolderId = 0;
            string NetFolderPath = "";
            string Type = "";

            XDocument.Load(AppConfig.DataPath);

            XNodes = XDocument.SelectNodes("/rules/item");

            foreach (XmlNode XNode in XNodes)
            {
                List<Hashtable> LocalDataList = new List<Hashtable>();
                List<Hashtable> NetDataList = new List<Hashtable>();
                Hashtable NetFolderTable = new Hashtable();

                LocalFolderPath = XNode.SelectSingleNode("localFolderPath").InnerText.TypeString();
                NetFolderId = XNode.SelectSingleNode("netFolderId").InnerText.TypeInt();
                NetFolderPath = XNode.SelectSingleNode("netFolderPath").InnerText.TypeString();
                Type = XNode.SelectSingleNode("type").InnerText.TypeString();

                LocalDataToList(LocalFolderPath, ref LocalDataList);

                NetDataToList(NetFolderId, ref NetDataList, ref NetFolderTable);
                
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


        /// <summary>
        /// 本地文件夹扫描(上传文件)
        /// </summary>
        private static void UploadScan(string LocalFolderPath, ref List<Hashtable> LocalDataList, int NetFolderId, string NetFolderPath, string NetFolderPurview, ref List<Hashtable> NetDataList)
        {
            int Index = 0;
            
            for (Index = 0; Index < LocalDataList.Count; Index++)
            {
                Hashtable NetTable = new Hashtable();

                // 找查网络是否存在该文件并载入到NetTable
                NetDataTable(LocalDataList[Index]["Name"].TypeString(), ref NetDataList, ref NetTable);

                // 判断网络是否存在该文件
                if (NetTable["Exist"].TypeBool() == false)
                {
                    FileUpload(LocalDataList[Index]["Path"].TypeString(), LocalDataList[Index]["Size"].TypeString(), LocalDataList[Index]["Hash"].TypeString(), NetFolderId);
                }
                else
                {
                    // 利用哈希码判断本地文件与网络文件是否相同
                    if (LocalDataList[Index]["Hash"].TypeString() == NetTable["Hash"].TypeString())
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
                                if (NetVersionExist(LocalDataList[Index]["Hash"].TypeString(), NetTable["Id"].TypeInt(), NetDataList) == false)
                                {
                                    // 不存在该文件版本(上传新版本)
                                    FileUpversion(LocalDataList[Index]["Path"].TypeString(), LocalDataList[Index]["Size"].TypeString(), LocalDataList[Index]["Hash"].TypeString(), NetTable["Id"].TypeInt());
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
        private static void DownloadScan(string LocalFolderPath, ref List<Hashtable> LocalDataList, int NetFolderId, string NetFolderPath, string NetFolderPurview, ref List<Hashtable> NetDataList)
        {
            int Index = 0;

            for (Index = 0; Index < NetDataList.Count; Index++)
            {
                // 判断是否文件版本(跳过)
                if (NetDataList[Index]["VersionId"].TypeInt() > 0)
                {
                    continue;
                }

                Hashtable LocalTable = new Hashtable();

                // 找查本地是否存在该文件并载入到LocalTable
                LocalDataTable(NetDataList[Index]["Name"].TypeString(), ref LocalDataList, ref LocalTable);

                // 判断本地是否存在该文件
                if (LocalTable["Exist"].TypeBool() == false)
                {
                    FileDownload(NetDataList[Index]["Id"].TypeInt(), NetDataList[Index]["CodeId"].TypeString(), NetDataList[Index]["Name"].TypeString(), LocalFolderPath);
                }
                else
                {
                    // 利用哈希码判断网络文件与本地文件是否相同
                    if (NetDataList[Index]["Hash"].TypeString() == LocalTable["Hash"].TypeString())
                    {
                        // 文件相同无需处理
                    }
                    else
                    {
                        // 判断网络文件夹是否具有版本下载权限
                        if (AppCommon.StringCheck(NetFolderPurview, @"^(editor|manager|creator)$") == true)
                        {
                            // 利用更新时间判断文件版本新旧
                            if (DateTime.Compare(NetDataList[Index]["UpdateTime"].TypeDateTime(), LocalTable["UpdateTime"].TypeDateTime()) > 0)
                            {
                                // 网络文件版本比本地文件版本新(下载新版本)
                                FileDownload(NetDataList[Index]["Id"].TypeInt(), NetDataList[Index]["CodeId"].TypeString(), NetDataList[Index]["Name"].TypeString(), LocalFolderPath);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 遍历本地文件夹获取文件集合返回list<hashtable>
        /// </summary>
        private static void LocalDataToList(string DirectoryPath, ref List<Hashtable> LocalDataList)
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                return;
            }

            DirectoryInfo DI = new DirectoryInfo(DirectoryPath);

            foreach (FileInfo FileItem in DI.GetFiles())
            {
                Hashtable Table = new Hashtable();

                Table.Add("Path", FileItem.FullName);
                Table.Add("Name", FileItem.Name);
                Table.Add("Size", FileItem.Length);
                Table.Add("Hash", FileHash(FileItem.FullName));
                Table.Add("UpdateTime", File.GetLastWriteTime(FileItem.FullName));

                LocalDataList.Add(Table);
            }
        }


        /// <summary>
        /// 读取本地文件数据返回hashtable
        /// </summary>
        private static void LocalDataTable(string NetFileName, ref List<Hashtable> LocalDataList, ref Hashtable Table)
        {
            int Position = -1;
            int Index = 0;

            for (Index = 0; Index < LocalDataList.Count; Index++)
            {
                if (LocalDataList[Index]["Name"].TypeString() == NetFileName)
                {
                    Position = Index;
                    break;
                }
            }

            if (Position == -1)
            {
                Table.Add("Exist", false);
            }
            else
            {
                Table.Add("Exist", true);
                Table.Add("Hash", LocalDataList[Position]["Hash"]);
                Table.Add("UpdateTime", LocalDataList[Position]["UpdateTime"]);
            }
        }


        /// <summary>
        /// 读取网络文件夹获取文件集合返回list<hashtable>
        /// </summary>
        private static void NetDataToList(int NetFolderId, ref List<Hashtable> NetDataList, ref Hashtable NetFolderTable)
        {
            WebClient WebClient = new WebClient();
            Uri HttpUri = default(Uri);
            byte[] ResponseData = {};
            string XData = "";

            try
            {
                HttpUri = new Uri("" + AppConfig.ServerHost + "/sync/file-list-xml.ashx?folderid=" + NetFolderId + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "");

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                ResponseData = WebClient.DownloadData(HttpUri);

                WebClient.Dispose();

                XData = Encoding.UTF8.GetString(ResponseData);

                if (XData.IndexOf("<item>") == -1 || XData.IndexOf("</item>") == -1)
                {
                    return;
                }

                XmlDocument XDocument = new XmlDocument();
                XmlNodeList XNodes = default(XmlNodeList);

                XDocument.LoadXml(XData);

                XNodes = XDocument.SelectNodes("/root/folder");

                foreach (XmlNode XNode in XNodes)
                {
                    NetFolderTable.Add("Share", XNode.SelectSingleNode("share").InnerText);
                    NetFolderTable.Add("Lock", XNode.SelectSingleNode("lock").InnerText);
                    NetFolderTable.Add("Purview", XNode.SelectSingleNode("purview").InnerText);
                }

                XNodes = XDocument.SelectNodes("/root/files/item");

                foreach (XmlNode XNode in XNodes)
                {
                    Hashtable Table = new Hashtable();

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
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
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
        /// 读取网络文件数据返回hashtable
        /// </summary>
        private static void NetDataTable(string LocalFileName, ref List<Hashtable> NetDataList, ref Hashtable Table)
        {
            int Position = -1;
            int Index = 0;

            for (Index = 0; Index < NetDataList.Count; Index++)
            {
                if (NetDataList[Index]["VersionId"].TypeInt() == 0)
                {
                    if (NetDataList[Index]["Name"].TypeString() == LocalFileName)
                    {
                        Position = Index;
                        break;
                    }
                }
            }

            if (Position == -1)
            {
                Table.Add("Exist", false);
            }
            else
            {
                Table.Add("Exist", true);
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
            int Index = 0;

            for (Index = 0; Index < NetDataList.Count; Index++)
            {
                if (NetDataList[Index]["VersionId"].TypeInt() == NetFileId)
                {
                    if (NetDataList[Index]["Hash"].TypeString() == LocalFileHash)
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
            byte[] Bytes = File.ReadAllBytes(FilePath);
            byte[] MD5 = new MD5CryptoServiceProvider().ComputeHash(Bytes);
            string Hash = "";
            int Index = 0;

            for (Index = 0; Index < MD5.Length; Index++)
            {
                Hash += MD5[Index].ToString("X2");
            }

            return Hash;
        }


        /// <summary>
        /// 新文件上传
        /// </summary>
        private static void FileUpload(string FilePath, string FileSize, string FileHash, int FolderId)
        {
            WebClient WebClient = new WebClient();
            FileStream FileStream = default(FileStream);
            Stream WebStream = default(Stream);
            string UploadUrl = "";
            byte[] ByteBuffer = {};
            int ByteRead = 0;
            int IntervalTime = 0;

            try
            {
                UploadUrl = "" + AppConfig.ServerHost + "/sync/file-upload.ashx?guid=" + System.Guid.NewGuid().ToString("N") + "&folderid=" + FolderId + "&filepath=" + HttpUtility.UrlEncode(FilePath) + "&filesize=" + FileSize + "&filehash=" + FileHash + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                if (AppConfig.UploadSpeed.TypeInt() == 0)
                {
                    IntervalTime = 0;
                }
                else
                {
                    IntervalTime = (int)1000 / (AppConfig.UploadSpeed.TypeInt());
                }

                FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                WebStream = WebClient.OpenWrite(UploadUrl);

                ByteBuffer = new byte[1024];

                ByteRead = FileStream.Read(ByteBuffer, 0, 1024);

                while (ByteRead > 0)
                {
                    WebStream.Write(ByteBuffer, 0, ByteRead);

                    ByteRead = FileStream.Read(ByteBuffer, 0, 1024);

                    Application.DoEvents();

                    if (IntervalTime > 0)
                    {
                        Thread.Sleep(IntervalTime);
                    }
                }

                WebClient.Dispose();

                WebStream.Close();
                WebStream.Dispose();

                FileStream.Close();
                FileStream.Dispose();

                SyncLog("upload", FilePath);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }

                if (AppCommon.IsNothing(WebStream) == false)
                {
                    WebStream.Close();
                    WebStream.Dispose();
                }

                if (AppCommon.IsNothing(FileStream) == false)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                }
            }
        }


        /// <summary>
        /// 新版本上传
        /// </summary>
        private static void FileUpversion(string FilePath, string FileSize, string FileHash, int FileId)
        {
            WebClient WebClient = new WebClient();
            FileStream FileStream = default(FileStream);
            Stream WebStream = default(Stream);
            string UpversionUrl = "";
            byte[] ByteBuffer = {};
            int ByteRead = 0;
            int IntervalTime = 0;
            
            try
            {
                UpversionUrl = "" + AppConfig.ServerHost + "/sync/file-upversion.ashx?guid=" + System.Guid.NewGuid().ToString("N") + "&fileid=" + FileId + "&filepath=" + HttpUtility.UrlEncode(FilePath) + "&filesize=" + FileSize + "&filehash=" + FileHash + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                if (AppConfig.UploadSpeed.TypeInt() == 0)
                {
                    IntervalTime = 0;
                }
                else
                {
                    IntervalTime = (int)1000 / (AppConfig.UploadSpeed.TypeInt());
                }
                
                FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                WebStream = WebClient.OpenWrite(UpversionUrl);

                ByteBuffer = new byte[1024];

                ByteRead = FileStream.Read(ByteBuffer, 0, 1024);

                while (ByteRead > 0)
                {
                    WebStream.Write(ByteBuffer, 0, ByteRead);

                    ByteRead = FileStream.Read(ByteBuffer, 0, 1024);

                    Application.DoEvents();

                    if (IntervalTime > 0)
                    {
                        Thread.Sleep(IntervalTime);
                    }
                }

                WebClient.Dispose();

                WebStream.Close();
                WebStream.Dispose();

                FileStream.Close();
                FileStream.Dispose();

                SyncLog("upversion", FilePath);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }

                if (AppCommon.IsNothing(WebStream) == false)
                {
                    WebStream.Close();
                    WebStream.Dispose();
                }

                if (AppCommon.IsNothing(FileStream) == false)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                }
            }
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        private static void FileDownload(int FileId, string FileCodeId, string FileName, string LocalFolderPath)
        {
            WebClient WebClient = new WebClient();
            FileStream FileStream = default(FileStream);
            Stream WebStream = default(Stream);
            string DownloadUrl = "";
            string TempFilePath = "";
            string SaveFilePath = "";
            byte[] ByteBuffer = {};
            int ByteRead = 0;
            int IntervalTime = 0;

            try
            {
                DownloadUrl = "" + AppConfig.ServerHost + "/sync/file-download.ashx?id=" + FileId + "&codeid=" + FileCodeId + "&timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                if (Directory.Exists(AppCommon.PathCombine(AppConfig.AppPath, "temp")) == false)
                {
                    Directory.CreateDirectory(AppCommon.PathCombine(AppConfig.AppPath, "temp"));
                }

                TempFilePath = AppCommon.PathCombine(AppConfig.AppPath, "temp", FileName);

                SaveFilePath = AppCommon.PathCombine(LocalFolderPath, FileName);

                if (File.Exists(TempFilePath) == true)
                {
                    File.Delete(TempFilePath);
                }

                if (AppConfig.DownloadSpeed.TypeInt() == 0)
                {
                    IntervalTime = 0;
                }
                else
                {
                    IntervalTime = (int)1000 / (AppConfig.DownloadSpeed.TypeInt());
                }

                FileStream = new FileStream(TempFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, true);

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                WebStream = WebClient.OpenRead(DownloadUrl);

                ByteBuffer = new byte[1024];

                ByteRead = WebStream.Read(ByteBuffer, 0, 1024);

                while (ByteRead > 0)
                {
                    FileStream.Write(ByteBuffer, 0, ByteRead);

                    ByteRead = WebStream.Read(ByteBuffer, 0, 1024);

                    Application.DoEvents();

                    if (IntervalTime > 0)
                    {
                        Thread.Sleep(IntervalTime);
                    }
                }

                WebClient.Dispose();

                WebStream.Close();
                WebStream.Dispose();

                FileStream.Close();
                FileStream.Dispose();

                File.Copy(TempFilePath, SaveFilePath, true);

                SyncLog("download", SaveFilePath);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "error.log"), "" + DateTime.Now.ToString() + "\n" + ex.ToString() + "\n\n");
            }
            finally
            {
                if (AppCommon.IsNothing(WebClient) == false)
                {
                    WebClient.Dispose();
                }

                if (AppCommon.IsNothing(WebStream) == false)
                {
                    WebStream.Close();
                    WebStream.Dispose();
                }

                if (AppCommon.IsNothing(FileStream) == false)
                {
                    FileStream.Close();
                    FileStream.Dispose();
                }

                File.Delete(TempFilePath);
            }
        }


        /// <summary>
        /// 同步日志
        /// </summary>
        private static void SyncLog(string Action, string FilePath)
        {
            string[] Files = {};

            if (Directory.Exists(AppCommon.PathCombine(AppConfig.AppPath, "log")) == false)
            {
                Directory.CreateDirectory(AppCommon.PathCombine(AppConfig.AppPath, "log"));
            }

            Files = Directory.GetFiles(AppCommon.PathCombine(AppConfig.AppPath,  "log"));

            foreach (string SubFile in Files)
            {
                if (DateTime.Compare(File.GetLastWriteTime(SubFile), DateTime.Now.AddDays(-14)) < 0)
                {
                    File.Delete(SubFile);
                }
            }

            File.AppendAllText(AppCommon.PathCombine(AppConfig.AppPath, "log", "" + DateTime.Now.ToString("yyyy-MM-dd") + ".log"), "" + DateTime.Now.ToString() + " " + Action + " " + FilePath + "\n");
        }


    }


}