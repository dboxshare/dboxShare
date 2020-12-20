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


        private Hashtable LangTable = new Hashtable();


        public FormFolderAdd()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;

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
        }


        private void FormFolderAdd_Load(object sender, EventArgs e)
        {
            labelLoadingTips.Text = "";
        }


        private void FormFolderAdd_Shown(object sender, EventArgs e)
        {
            NetFolderLoad();
        }


        private void FormFolderAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormMain FormMain = new FormMain();

            FormMain.Show();
        }


        /// <summary>
        /// 网络文件夹数据加载
        /// </summary>
        private void NetFolderLoad()
        {
            WebClient WebClient = new WebClient();
            Uri HttpUri = default(Uri);

            HttpUri = new Uri("" + AppConfig.ServerHost + "/sync/folder-list-xml.ashx?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "");

            labelLoadingTips.Text = LangTable["tipsLoading"].TypeString();

            WebClient.Proxy = null;

            WebClient.Headers.Add("Cookie", AppConfig.UserSession);

            WebClient.DownloadDataCompleted += (object sender, DownloadDataCompletedEventArgs e) =>
            {
                XmlDocument XDocument = new XmlDocument();
                XmlNodeList XNodes = default(XmlNodeList);
                string XData = "";

                try
                {
                    if (e.Cancelled == false)
                    {
                        if (AppCommon.IsNothing(e.Error) == true)
                        {
                            XData = Encoding.UTF8.GetString(e.Result);

                            WebClient.Dispose();

                            XDocument.LoadXml(XData);

                            XNodes = XDocument.SelectNodes("/folders/item");

                            if (AppCommon.IsNothing(XNodes) == false)
                            {
                                if (XNodes.Count == 0)
                                {
                                    labelLoadingTips.Text = LangTable["tipsNoData"].TypeString();
                                }
                                else
                                {
                                    labelLoadingTips.Visible = false;

                                    NetFolderLoad_NodeAdd(treeViewNetFolder.Nodes, ref XNodes, 0);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    labelLoadingTips.Text = LangTable["tipsLoadFailed"].TypeString();
                }
                finally
                {
                    if (AppCommon.IsNothing(WebClient) == false)
                    {
                        WebClient.Dispose();
                    }
                }
            };

            WebClient.DownloadDataAsync(HttpUri);
        }


        /// <summary>
        /// 网络文件夹树形目录节点添加
        /// </summary>
        private void NetFolderLoad_NodeAdd(TreeNodeCollection TreeNodes, ref XmlNodeList XNodes, int ParentId)
        {
            TreeNode TreeNode = default(TreeNode);
            string Id = "";
            string FolderId = "";
            string Name = "";

            foreach (XmlNode XNode in XNodes)
            {
                Id = XNode.SelectSingleNode("id").InnerText;
                FolderId = XNode.SelectSingleNode("folderId").InnerText;
                Name = XNode.SelectSingleNode("name").InnerText;

                if (FolderId.TypeInt() == ParentId)
                {
                    TreeNode = TreeNodes.Add(Name);

                    TreeNode.Name = Id;

                    NetFolderLoad_NodeAdd(TreeNode.Nodes, ref XNodes, Id.TypeInt());
                }
            }
        }


        /// <summary>
        /// 网络文件夹树形目录节点选择
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
            DialogResult Result;
            string Path = "";

            Result = folderBrowserDialog.ShowDialog();

            if (Result == DialogResult.OK)
            {
                Path = folderBrowserDialog.SelectedPath;

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
            XmlDocument XDocument = new XmlDocument();
            XmlNodeList XNodes = default(XmlNodeList);
            XmlElement XElement = default(XmlElement);
            XmlNode XRoot = default(XmlNode);

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

            XDocument.Load(AppConfig.DataPath);

            XNodes = XDocument.SelectSingleNode("rules").ChildNodes;

            if (AppCommon.IsNothing(XNodes) == false)
            {
                if (XNodes.Count >= 10)
                {
                    MessageBox.Show(LangTable["tipsFolderCountLimit"].TypeString());
                    return;
                }

                foreach (XmlNode XNode in XNodes)
                {
                    XElement = (XmlElement)XNode;

                    if (XElement.ChildNodes[0].InnerText == textBoxLocalFolderPath.Text || XElement.ChildNodes[1].InnerText == textBoxNetFolderPath.Text || XElement.ChildNodes[2].InnerText == textBoxNetFolderId.Text)
                    {
                        MessageBox.Show(LangTable["tipsFolderExisted"].TypeString());
                        return;
                    }
                }
            }

            XRoot = XDocument.SelectSingleNode("rules");

            XmlElement XItem = XDocument.CreateElement("item");

            XRoot.AppendChild(XItem);

            XmlElement LocalFolderPath = XDocument.CreateElement("localFolderPath");
            LocalFolderPath.InnerText = textBoxLocalFolderPath.Text;
            XItem.AppendChild(LocalFolderPath);

            XmlElement NetFolderPath = XDocument.CreateElement("netFolderPath");
            NetFolderPath.InnerText = textBoxNetFolderPath.Text;
            XItem.AppendChild(NetFolderPath);

            XmlElement NetFolderId = XDocument.CreateElement("netFolderId");
            NetFolderId.InnerText = textBoxNetFolderId.Text;
            XItem.AppendChild(NetFolderId);

            XmlElement Type = XDocument.CreateElement("type");

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

            XDocument.Save(AppConfig.DataPath);

            this.Close();
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

            XNodes = XDocument.SelectSingleNode("/language/formFolderAdd");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
