using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Web;
using System.Windows.Forms;
using System.Xml;


namespace dboxSyncer
{


    public partial class FormMain : Form
    {


        private System.Timers.Timer LoginKeepTimer = new System.Timers.Timer();
        private System.Timers.Timer PrecessTaskTimer = new System.Timers.Timer();
        private Hashtable LangTable = new Hashtable(StringComparer.OrdinalIgnoreCase);


        public FormMain()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Environment.Exit(0);
            }

            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); ;
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadLanguage(ref LangTable);

            groupBoxSync.Text = LangTable["groupBoxSync"].TypeString();
            labelIntervalTime.Text = LangTable["labelIntervalTime"].TypeString();
            labelIntervalMinute.Text = LangTable["labelIntervalMinute"].TypeString();
            labelUploadSpeed.Text = LangTable["labelUploadSpeed"].TypeString();
            labelDownloadSpeed.Text = LangTable["labelDownloadSpeed"].TypeString();
            buttonAdd.Text = LangTable["buttonAdd"].TypeString();
            buttonDelete.Text = LangTable["buttonDelete"].TypeString();
            buttonSetting.Text = LangTable["buttonSetting"].TypeString();
            buttonSync.Text = LangTable["buttonSync"].TypeString();
            buttonWeb.Text = LangTable["buttonWeb"].TypeString();
            buttonHide.Text = LangTable["buttonHide"].TypeString();
            toolStripMenuItemMain.Text = LangTable["contextMain"].TypeString();
            toolStripMenuItemSync.Text = LangTable["contextSync"].TypeString();
            toolStripMenuItemWeb.Text = LangTable["contextWeb"].TypeString();
            toolStripMenuItemQuit.Text = LangTable["contextQuit"].TypeString();
            listBoxIntervalTime.Text = AppConfig.IntervalTime.TypeString();
            textBoxUploadSpeed.Text = AppConfig.UploadSpeed.TypeString();
            textBoxDownloadSpeed.Text = AppConfig.DownloadSpeed.TypeString();
        }


        private void FormMain_Shown(object sender, EventArgs e)
        {
            LoginKeepTimer.Interval = 1000 * 60 * 5;
            LoginKeepTimer.Elapsed += new System.Timers.ElapsedEventHandler(LoginKeep);
            LoginKeepTimer.AutoReset = true;
            LoginKeepTimer.Start();

            PrecessTaskTimer.Interval = 1000 * 60 * AppConfig.IntervalTime;
            PrecessTaskTimer.Elapsed += new System.Timers.ElapsedEventHandler(PrecessTask);
            PrecessTaskTimer.AutoReset = true;
            PrecessTaskTimer.Start();

            ListViewDataLoad();
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {
                if (MessageBox.Show(LangTable["tipsQuit"].TypeString(), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    AppCommon.ProcessExit("dboxSyncer.Process");

                    Environment.Exit(0);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                AppCommon.ProcessExit("dboxSyncer.Process");

                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 定时执行登录状态保持
        /// </summary>
        private void LoginKeep(object sender, EventArgs e)
        {
            LoginKeeping();
        }


        private void LoginKeeping()
        {
            var WebClient = (WebClient)null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                WebClient.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) =>
                {
                    try
                    {
                        if (e.Cancelled == false)
                        {
                            // 完成
                        }
                        else
                        {
                            // 失败
                        }
                    }
                    catch (Exception)
                    {
                        // 失败
                    }
                };

                var PostUrl = "" + AppConfig.ServerUrl + "/api/sync/user-login?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";
                var PostString = "loginid=" + HttpUtility.UrlEncode(AppConfig.LoginId) + "&password=" + HttpUtility.UrlEncode(AppConfig.Password) + "";
                var PostData = Encoding.UTF8.GetBytes(PostString);

                WebClient.UploadDataAsync(new Uri(PostUrl), PostData);
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
        /// 定时执行处理任务
        /// </summary>
        private void PrecessTask(object sender, EventArgs e)
        {
            AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "upload", -1);
            AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "download", -1);
        }


        /// <summary>
        /// 同步文件夹数据加载
        /// </summary>
        private void ListViewDataLoad()
        {
            listViewData.View = View.Details;
            listViewData.Columns.Clear();
            listViewData.Items.Clear();

            listViewData.Columns.Add(LangTable["listViewLocalPath"].TypeString(), 350, HorizontalAlignment.Left);
            listViewData.Columns.Add(LangTable["listViewNetPath"].TypeString(), 325, HorizontalAlignment.Left);
            listViewData.Columns.Add(LangTable["listViewType"].TypeString(), 75, HorizontalAlignment.Left);

            if (File.Exists(AppConfig.DataFilePath) == false)
            {
                File.AppendAllText(AppConfig.DataFilePath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><rules></rules>", Encoding.UTF8);
                return;
            }

            var XDocument = new XmlDocument();

            XDocument.Load(AppConfig.DataFilePath);

            var XNodes = XDocument.SelectNodes("/rules/item");

            foreach (XmlNode XNode in XNodes)
            {
                var ViewItem = new ListViewItem(XNode.SelectSingleNode("localFolderPath").InnerText);

                ViewItem.SubItems.Add(XNode.SelectSingleNode("netFolderPath").InnerText);

                var Type = "";

                switch (XNode.SelectSingleNode("type").InnerText)
                {
                    case "sync":
                        Type = LangTable["typeDataSync"].TypeString();
                        break;

                    case "upload":
                        Type = LangTable["typeDataUpload"].TypeString();
                        break;

                    case "download":
                        Type = LangTable["typeDownload"].TypeString();
                        break;
                }

                ViewItem.SubItems.Add(Type);

                listViewData.Items.Add(ViewItem);
            }
        }


        /// <summary>
        /// 同步文件夹数据重新加载
        /// </summary>
        public void ListViewDataReload()
        {
            ListViewDataLoad();
        }


        /// <summary>
        /// 同步文件夹添加按钮点击事件
        /// </summary>
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            var FormFolderAdd = new FormFolderAdd();

            FormFolderAdd.Show();

            this.Hide();
        }


        /// <summary>
        /// 同步文件夹删除按钮点击事件
        /// </summary>
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (listViewData.SelectedItems.Count == 0)
            {
                MessageBox.Show(LangTable["tipsSelectItem"].TypeString());
                return;
            }

            var XDocument = new XmlDocument();

            XDocument.Load(AppConfig.DataFilePath);

            var XNodes = XDocument.SelectSingleNode("rules");

            if (AppCommon.IsNothing(XNodes) == false)
            {
                if (XNodes.ChildNodes.Count == 0)
                {
                    return;
                }

                var Item = listViewData.SelectedItems[listViewData.SelectedItems.Count - 1].Index;
                var Index = 0;

                foreach (XmlNode XNode in XNodes.ChildNodes)
                {
                    if (Item == Index)
                    {
                        XNode.ParentNode.RemoveChild(XNode);
                        break;
                    }

                    Index++;
                }

                XDocument.Save(AppConfig.DataFilePath);

                ListViewDataReload();
            }
        }


        /// <summary>
        /// 同步设置按钮点击事件
        /// </summary>
        private void ButtonSetting_Click(object sender, EventArgs e)
        {
            var IntervalTime = listBoxIntervalTime.Text;

            var UploadSpeed = textBoxUploadSpeed.Text;

            if (UploadSpeed.TypeInt() < 0 || UploadSpeed.TypeInt() > 1000)
            {
                MessageBox.Show(LangTable["tipsUploadSpeedError"].TypeString());
                return;
            }

            var DownloadSpeed = textBoxDownloadSpeed.Text;

            if (DownloadSpeed.TypeInt() < 0 || DownloadSpeed.TypeInt() > 1000)
            {
                MessageBox.Show(LangTable["tipsDownloadSpeedError"].TypeString());
                return;
            }

            var XDocument = new XmlDocument();

            XDocument.Load(AppConfig.ConfigFilePath);

            var XPath = "/configuration/appSettings/add[@key=\"{key}\"]";

            var XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "IntervalTime"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = IntervalTime;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "UploadSpeed"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = UploadSpeed;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "DownloadSpeed"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = DownloadSpeed;
            }

            XDocument.Save(AppConfig.ConfigFilePath);

            AppCommon.ProcessExit("dboxSyncer.Process");

            PrecessTaskTimer.Interval = 1000 * 60 * AppConfig.IntervalTime;
        }


        /// <summary>
        /// 手动同步按钮点击事件
        /// </summary>
        private void ButtonSync_Click(object sender, EventArgs e)
        {
            if (AppCommon.ProcessExists("dboxSyncer.Process") == true)
            {
                MessageBox.Show(LangTable["tipsProcessing"].TypeString());
            }
            else
            {
                buttonSync.Enabled = false;
                buttonSync.Text = LangTable["buttonProcessing"].TypeString();

                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "upload", -1);
                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "download", -1);

                buttonSync.Enabled = true;
                buttonSync.Text = LangTable["buttonSync"].TypeString();
            }
        }


        /// <summary>
        /// 打开网站按钮点击事件
        /// </summary>
        private void ButtonWeb_Click(object sender, EventArgs e)
        {
            Process.Start(AppConfig.ServerUrl);
        }


        /// <summary>
        /// 隐藏按钮点击事件
        /// </summary>
        private void ButtonHide_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;

            notifyIcon.Visible = true;
        }


        /// <summary>
        /// 打开面板菜单点击事件
        /// </summary>
        private void ToolStripMenuItemMain_Click(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;

            notifyIcon.Visible = false;
        }


        /// <summary>
        /// 手工同步菜单点击事件
        /// </summary>
        private void ToolStripMenuItemSync_Click(object sender, EventArgs e)
        {
            if (AppCommon.ProcessExists("dboxSyncer.Process") == true)
            {
                MessageBox.Show(LangTable["tipsProcessing"].TypeString());
            }
            else
            {
                buttonSync.Enabled = false;
                buttonSync.Text = LangTable["buttonProcessing"].TypeString();

                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "upload", -1);
                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "download", -1);

                buttonSync.Enabled = true;
                buttonSync.Text = LangTable["buttonSync"].TypeString();
            }
        }


        /// <summary>
        /// 进入网站菜单点击事件
        /// </summary>
        private void ToolStripMenuItemWeb_Click(object sender, EventArgs e)
        {
            Process.Start(AppConfig.ServerUrl);
        }


        /// <summary>
        /// 退出菜单点击事件
        /// </summary>
        private void ToolStripMenuItemQuit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LangTable["tipsQuit"].TypeString(), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AppCommon.ProcessExit("dboxSyncer.Process");

                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 加载语言
        /// </summary>
        private void LoadLanguage(ref Hashtable LangTable)
        {
            var XDocument = new XmlDocument();

            XDocument.Load(AppCommon.PathCombine(AppConfig.AppPath, "languages", "" + AppConfig.AppLanguage + ".xml"));

            var XNodes = XDocument.SelectSingleNode("/language/formMain");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
