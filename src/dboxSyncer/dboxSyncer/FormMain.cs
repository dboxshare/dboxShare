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


        private Hashtable LangTable = new Hashtable();


        public FormMain()
        {
            InitializeComponent();

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length - 1 > 0)
            {
                Environment.Exit(0);
            }
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;

            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); ;

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
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            listBoxIntervalTime.Text = AppConfig.IntervalTime;
            textBoxUploadSpeed.Text = AppConfig.UploadSpeed;
            textBoxDownloadSpeed.Text = AppConfig.DownloadSpeed;
        }


        private void FormMain_Shown(object sender, EventArgs e)
        {

            System.Timers.Timer Timer = new System.Timers.Timer(300000);

            Timer.Elapsed += new System.Timers.ElapsedEventHandler(LoginKeep);
            Timer.AutoReset = true;
            Timer.Start();

            ListViewDataLoad();

            AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Task.exe"), "", -1);
        }


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.ToString() == "UserClosing")
            {
                if (MessageBox.Show(LangTable["tipsQuit"].TypeString(), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    AppCommon.ProcessExit("dboxSyncer.Task");
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
                AppCommon.ProcessExit("dboxSyncer.Task");
                AppCommon.ProcessExit("dboxSyncer.Process");

                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 登录状态保持
        /// </summary>
        private void LoginKeep(object sender, EventArgs e)
        {
            LoginKeeping();
        }


        private void LoginKeeping()
        {
            WebClient WebClient = new WebClient();
            Uri PostUri = default(Uri);
            string PostString = "";
            byte[] PostData = { };

            PostUri = new Uri("" + AppConfig.ServerHost + "/sync/login.ashx?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "");
            PostString = "loginid=" + HttpUtility.UrlEncode(AppConfig.LoginId) + "&password=" + HttpUtility.UrlEncode(AppConfig.Password) + "";
            PostData = Encoding.UTF8.GetBytes(PostString);

            WebClient.Proxy = null;

            WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            WebClient.Headers.Add("Cookie", AppConfig.UserSession);

            WebClient.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) =>
            {
                try
                {
                    if (e.Cancelled == false)
                    {
                        if (AppCommon.IsNothing(e.Error) == true)
                        {
                            WebClient.Dispose();
                        }
                        else
                        {
                            // 失败
                        }
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
                finally
                {
                    if (AppCommon.IsNothing(WebClient) == false)
                    {
                        WebClient.Dispose();
                    }
                }
            };

            WebClient.UploadDataAsync(PostUri, PostData);
        }


        /// <summary>
        /// 同步文件夹数据加载
        /// </summary>
        private void ListViewDataLoad()
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNodeList XNodes = default(XmlNodeList);
            ListViewItem ViewItem = default(ListViewItem);
            string Type = "";

            listViewData.View = View.Details;
            listViewData.Columns.Clear();
            listViewData.Items.Clear();

            listViewData.Columns.Add(LangTable["listViewLocalPath"].TypeString(), 350, HorizontalAlignment.Left);
            listViewData.Columns.Add(LangTable["listViewNetPath"].TypeString(), 325, HorizontalAlignment.Left);
            listViewData.Columns.Add(LangTable["listViewType"].TypeString(), 75, HorizontalAlignment.Left);

            if (File.Exists(AppConfig.DataPath) == false)
            {
                File.AppendAllText(AppConfig.DataPath, "<?xml version=\"1.0\" encoding=\"utf-8\"?><rules></rules>", Encoding.UTF8);
                return;
            }

            XDocument.Load(AppConfig.DataPath);

            XNodes = XDocument.SelectNodes("/rules/item");

            foreach (XmlNode XNode in XNodes)
            {
                ViewItem = new ListViewItem(XNode.SelectSingleNode("localFolderPath").InnerText);

                ViewItem.SubItems.Add(XNode.SelectSingleNode("netFolderPath").InnerText);

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
            FormFolderAdd FormFolderAdd = new FormFolderAdd();

            FormFolderAdd.Show();

            this.Hide();
        }


        /// <summary>
        /// 同步文件夹删除按钮点击事件
        /// </summary>
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNodes = default(XmlNode);
            int Item = 0;
            int Index = 0;

            if (listViewData.SelectedItems.Count == 0)
            {
                MessageBox.Show(LangTable["tipsSelectItem"].TypeString());
                return;
            }

            XDocument.Load(AppConfig.DataPath);

            XNodes = XDocument.SelectSingleNode("rules");

            if (AppCommon.IsNothing(XNodes) == false)
            {
                if (XNodes.ChildNodes.Count == 0)
                {
                    return;
                }

                Item = listViewData.SelectedItems[listViewData.SelectedItems.Count - 1].Index;

                foreach (XmlNode XNode in XNodes.ChildNodes)
                {
                    if (Item == Index)
                    {
                        XNode.ParentNode.RemoveChild(XNode);
                        break;
                    }

                    Index++;
                }

                XDocument.Save(AppConfig.DataPath);

                ListViewDataReload();
            }
        }


        /// <summary>
        /// 同步设置按钮点击事件
        /// </summary>
        private void ButtonSetting_Click(object sender, EventArgs e)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNode = default(XmlNode);
            string XPath = "/configuration/appSettings/add[@key=\"{key}\"]";
            string ConfigPath = AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.exe.config");
            string IntervalTime = listBoxIntervalTime.Text;
            string UploadSpeed = textBoxUploadSpeed.Text;
            string DownloadSpeed = textBoxDownloadSpeed.Text;

            if (UploadSpeed.TypeInt() < 0 || UploadSpeed.TypeInt() > 1000)
            {
                MessageBox.Show(LangTable["tipsUploadSpeedError"].TypeString());
                return;
            }

            if (DownloadSpeed.TypeInt() < 0 || DownloadSpeed.TypeInt() > 1000)
            {
                MessageBox.Show(LangTable["tipsDownloadSpeedError"].TypeString());
                return;
            }

            XDocument.Load(ConfigPath);

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "IntervalTime"));

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

            XDocument.Save(ConfigPath);

            AppCommon.ProcessExit("dboxSyncer.Task");
            AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Task.exe"), "", -1);
        }


        /// <summary>
        /// 手动同步按钮点击事件
        /// </summary>
        private void ButtonSync_Click(object sender, EventArgs e)
        {
            if (AppCommon.ProcessExist("dboxSyncer.Process") == true)
            {
                MessageBox.Show(LangTable["tipsProcessing"].TypeString());
            }
            else
            {
                buttonSync.Enabled = false;
                buttonSync.Text = LangTable["buttonProcessing"].TypeString();

                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "", -1);

                buttonSync.Enabled = true;
                buttonSync.Text = LangTable["buttonSync"].TypeString();
            }
        }


        /// <summary>
        /// 打开网站按钮点击事件
        /// </summary>
        private void ButtonWeb_Click(object sender, EventArgs e)
        {
            Process.Start("" + AppConfig.ServerHost + "/sync/open-web.ashx?loginid=" + HttpUtility.UrlEncode(AppConfig.LoginId) + "&password=" + HttpUtility.UrlEncode(AppConfig.Password) + "");
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
            if (AppCommon.ProcessExist("dboxSyncer.Process") == true)
            {
                MessageBox.Show(LangTable["tipsProcessing"].TypeString());
            }
            else
            {
                buttonSync.Enabled = false;
                buttonSync.Text = LangTable["buttonProcessing"].TypeString();

                AppCommon.ProcessExecute(AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.Process.exe"), "", -1);

                buttonSync.Enabled = true;
                buttonSync.Text = LangTable["buttonSync"].TypeString();
            }
        }


        /// <summary>
        /// 进入网站菜单点击事件
        /// </summary>
        private void ToolStripMenuItemWeb_Click(object sender, EventArgs e)
        {
            Process.Start("" + AppConfig.ServerHost + "/sync/open-web.ashx?loginid=" + HttpUtility.UrlEncode(AppConfig.LoginId) + "&password=" + HttpUtility.UrlEncode(AppConfig.Password) + "");
        }


        /// <summary>
        /// 退出菜单点击事件
        /// </summary>
        private void ToolStripMenuItemQuit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(LangTable["tipsQuit"].TypeString(), this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AppCommon.ProcessExit("dboxSyncer.Task");
                AppCommon.ProcessExit("dboxSyncer.Process");

                Environment.Exit(0);
            }
        }


        /// <summary>
        /// 加载语言
        /// </summary>
        private void LoadLanguage(ref Hashtable LangTable)
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNodes = default(XmlNode);
            string FilePath = AppCommon.PathCombine(AppConfig.AppPath, "language.xml");

            XDocument.Load(FilePath);

            XNodes = XDocument.SelectSingleNode("/language/formMain");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
