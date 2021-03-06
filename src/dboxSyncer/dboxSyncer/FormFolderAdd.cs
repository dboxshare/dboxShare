using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;


namespace dboxSyncer
{


    public partial class FormFolderAdd : Form
    {


        private Hashtable LangTable = new Hashtable(StringComparer.OrdinalIgnoreCase);


        public FormFolderAdd()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void FormFolderAdd_Load(object sender, EventArgs e)
        {
            LoadLanguage(ref LangTable);

            labelLocalFolder.Text = LangTable["labelLocalFolder"].TypeString();
            labelNetFolder.Text = LangTable["labelNetFolder"].TypeString();
            labelType.Text = LangTable["labelType"].TypeString();
            labelTypeTips.Text = LangTable["labelTypeTips"].TypeString();
            labelSubfolderTips.Text = LangTable["labelSubfolderTips"].TypeString();
            radioButtonTypeSync.Text = LangTable["radioTypeSync"].TypeString();
            radioButtonTypeUpload.Text = LangTable["radioTypeUpload"].TypeString();
            radioButtonTypeDownload.Text = LangTable["radioTypeDownload"].TypeString();
            buttonBrowse.Text = LangTable["buttonBrowse"].TypeString();
            buttonAdd.Text = LangTable["buttonAdd"].TypeString();
            labelLoadingTips.Text = "";
        }


        private void FormFolderAdd_Shown(object sender, EventArgs e)
        {
            NetFolderLoad();
        }


        private void FormFolderAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            var FormMain = new FormMain();

            FormMain.Show();
        }


        /// <summary>
        /// 网络文件夹数据加载
        /// </summary>
        private void NetFolderLoad()
        {
            labelLoadingTips.Text = LangTable["tipsLoading"].TypeString();

            var WebClient = (WebClient)null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Cookie", AppConfig.UserSession);

                WebClient.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
                {
                    try
                    {
                        if (e.Cancelled == false)
                        {
                            if (AppCommon.IsNothing(e.Error) == true)
                            {
                                var XData = Encoding.UTF8.GetString(e.Result);

                                var XDocument = new XmlDocument();

                                XDocument.LoadXml(XData);

                                var XNodes = XDocument.SelectNodes("/folders/item");

                                if (AppCommon.IsNothing(XNodes) == false)
                                {
                                    if (XNodes.Count == 0)
                                    {
                                        labelLoadingTips.Text = LangTable["tipsNoData"].TypeString();
                                    }
                                    else
                                    {
                                        labelLoadingTips.Visible = false;

                                        NetFolderTreeView(treeViewNetFolder.Nodes, ref XNodes, 0);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        labelLoadingTips.Text = LangTable["tipsLoadFailed"].TypeString();
                    }
                };

                var Url = "" + AppConfig.ServerUrl + "/api/sync/folder-list-xml?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";

                WebClient.DownloadDataAsync(new Uri(Url));
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
        /// 网络文件夹树形目录显示
        /// </summary>
        private void NetFolderTreeView(TreeNodeCollection TreeNodes, ref XmlNodeList XNodes, int ParentId)
        {
            foreach (XmlNode XNode in XNodes)
            {
                var Id = XNode.SelectSingleNode("id").InnerText;
                var FolderId = XNode.SelectSingleNode("folderId").InnerText;
                var Name = XNode.SelectSingleNode("name").InnerText;

                if (FolderId.TypeInt() == ParentId)
                {
                    var TreeNode = TreeNodes.Add(Name);

                    TreeNode.Name = Id;

                    NetFolderTreeView(TreeNode.Nodes, ref XNodes, Id.TypeInt());
                }
            }
        }


        /// <summary>
        /// 网络文件夹树形目录选择
        /// </summary>
        private void TreeViewNetFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (AppCommon.IsNothing(e.Node) == false)
            {
                textBoxNetFolderPath.Text = e.Node.FullPath;
                textBoxNetFolderId.Text = e.Node.Name;
            }
        }


        /// <summary>
        /// 本地文件夹浏览按钮点击事件
        /// </summary>
        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            var Result = folderBrowserDialog.ShowDialog();

            if (Result == DialogResult.OK)
            {
                var Path = folderBrowserDialog.SelectedPath;

                if (AppCommon.StringCheck(Path, @"^[A-Z]\:\\[\s\S]+$") == false)
                {
                    MessageBox.Show(LangTable["tipsSelectLocalFolder"].TypeString());
                    return;
                }

                textBoxLocalFolderPath.Text = Path;
            }
        }


        /// <summary>
        /// 同步文件夹添加按钮点击事件
        /// </summary>
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxLocalFolderPath.Text) == true)
            {
                MessageBox.Show(LangTable["tipsSelectLocalFolder"].TypeString());
                return;
            }

            if (string.IsNullOrEmpty(textBoxNetFolderPath.Text) == true)
            {
                MessageBox.Show(LangTable["tipsSelectNetFolder"].TypeString());
                return;
            }

            if (string.IsNullOrEmpty(textBoxNetFolderId.Text) == true)
            {
                return;
            }

            var XDocument = new XmlDocument();

            XDocument.Load(AppConfig.DataFilePath);

            var XNodes = XDocument.SelectSingleNode("rules").ChildNodes;

            if (AppCommon.IsNothing(XNodes) == false)
            {
                if (XNodes.Count >= 10)
                {
                    MessageBox.Show(LangTable["tipsFolderCountLimit"].TypeString());
                    return;
                }

                foreach (XmlNode XNode in XNodes)
                {
                    var XElement = (XmlElement)XNode;

                    if (XElement.ChildNodes[0].InnerText == textBoxLocalFolderPath.Text || XElement.ChildNodes[1].InnerText == textBoxNetFolderPath.Text || XElement.ChildNodes[2].InnerText == textBoxNetFolderId.Text)
                    {
                        MessageBox.Show(LangTable["tipsFolderExisted"].TypeString());
                        return;
                    }
                }
            }

            var XRoot = XDocument.SelectSingleNode("rules");

            var XItem = XDocument.CreateElement("item");

            XRoot.AppendChild(XItem);

            var LocalFolderPath = XDocument.CreateElement("localFolderPath");
            LocalFolderPath.InnerText = textBoxLocalFolderPath.Text;
            XItem.AppendChild(LocalFolderPath);

            var NetFolderPath = XDocument.CreateElement("netFolderPath");
            NetFolderPath.InnerText = textBoxNetFolderPath.Text;
            XItem.AppendChild(NetFolderPath);

            var NetFolderId = XDocument.CreateElement("netFolderId");
            NetFolderId.InnerText = textBoxNetFolderId.Text;
            XItem.AppendChild(NetFolderId);

            var Type = XDocument.CreateElement("type");

            if (radioButtonTypeSync.Checked == true)
            {
                Type.InnerText = "sync";
            }

            if (radioButtonTypeUpload.Checked == true)
            {
                Type.InnerText = "upload";
            }

            if (radioButtonTypeDownload.Checked == true)
            {
                Type.InnerText = "download";
            }

            XItem.AppendChild(Type);

            XDocument.Save(AppConfig.DataFilePath);

            this.Close();
        }


        /// <summary>
        /// 加载语言
        /// </summary>
        private void LoadLanguage(ref Hashtable LangTable)
        {
            var XDocument = new XmlDocument();

            XDocument.Load(AppCommon.PathCombine(AppConfig.AppPath, "languages", "" + AppConfig.AppLanguage + ".xml"));

            var XNodes = XDocument.SelectSingleNode("/language/formFolderAdd");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
