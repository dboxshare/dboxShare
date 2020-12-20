using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;


namespace dboxSyncer
{


    public partial class FormLogin : Form
    {


        private Hashtable  LangTable = new Hashtable();


        public FormLogin()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadLanguage(ref LangTable);

            labelServerHost.Text = LangTable["labelServerHost"].TypeString();
            labelLoginId.Text = LangTable["labelLoginId"].TypeString();
            labelPassword.Text = LangTable["labelPassword"].TypeString();
            checkBoxAutoLogin.Text = LangTable["checkBoxAutoLogin"].TypeString();
            buttonLogin.Text = LangTable["buttonLogin"].TypeString();
            labelLoginTips.Text = "";
        }


        private void FormLogin_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AppConfig.LoginId) == false && string.IsNullOrEmpty(AppConfig.Password) == false && AppConfig.AutoLogin == "true")
            {
                LoginProcess(AppConfig.ServerHost, AppConfig.LoginId, AppConfig.Password, AppConfig.AutoLogin, false);
            }

            if (string.IsNullOrEmpty(AppConfig.ServerHost) == false)
            {
                textBoxServerHost.Text = AppConfig.ServerHost;
            }

            if (string.IsNullOrEmpty(AppConfig.LoginId) == false)
            {
                textBoxLoginId.Text = AppConfig.LoginId;
            }

            if (AppConfig.AutoLogin == "true")
            {
                checkBoxAutoLogin.Checked = true;
            }
        }


        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }


        /// <summary>
        /// 用户登录按钮点击事件
        /// </summary>
        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            string ServerHost = textBoxServerHost.Text;
            string LoginId = textBoxLoginId.Text;
            string Password = textBoxPassword.Text;
            string AutoLogin = checkBoxAutoLogin.Checked.ToString().ToLower();

            if (AppCommon.StringCheck(ServerHost, @"^(http|https)\:\/\/[\w\-\.\:]+\/?$") == false)
            {
                labelLoginTips.Text = LangTable["tipsServerHost"].TypeString();
                return;
            }

            AppConfig.ServerHost = ServerHost;

            if (string.IsNullOrEmpty(LoginId) == true)
            {
                labelLoginTips.Text = LangTable["tipsLoginId"].TypeString();
                return;
            }

            AppConfig.LoginId = LoginId;

            if (string.IsNullOrEmpty(Password) == true)
            {
                labelLoginTips.Text = LangTable["tipsPassword"].TypeString();
                return;
            }

            AppConfig.Password = Password;

            AppConfig.AutoLogin = AutoLogin;

            LoginProcess(ServerHost, LoginId, Password, AutoLogin, true);
        }


        /// <summary>
        /// 用户登录处理程序
        /// </summary>
        private void LoginProcess(string ServerHost, string LoginId, string Password, string AutoLogin, bool Taskbar)
        {
            WebClient WebClient = new WebClient();
            Uri PostUri = default(Uri);
            string PostString = "";
            byte[] PostData = {};

            PostUri = new Uri("" + ServerHost + "/sync/login.ashx?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "");
            PostString = "loginid=" + HttpUtility.UrlEncode(LoginId) + "&password=" + HttpUtility.UrlEncode(Password) + "";
            PostData = Encoding.UTF8.GetBytes(PostString);

            buttonLogin.Enabled = false;
            buttonLogin.Text = LangTable["buttonLogging"].TypeString();

            WebClient.Proxy = null;

            WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            WebClient.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) =>
            {
                string Result = "";
                string Cookie = "";

                try
                {
                    if (e.Cancelled == false)
                    {
                        if (AppCommon.IsNothing(e.Error) == true)
                        {
                            Result = Encoding.UTF8.GetString(e.Result);

                            Cookie = WebClient.ResponseHeaders.Get("Set-Cookie");

                            WebClient.Dispose();

                            if (Result == "complete")
                            {
                                AppConfig.UserSession = Cookie;

                                LoginDataSave();

                                FormMain FormMain = new FormMain();

                                if (Taskbar == true)
                                {
                                    FormMain.Show();
                                }
                                else
                                {
                                    FormMain.notifyIcon.Visible = true;
                                }

                                this.Hide();
                            }
                            else if (Result == "lock-user-id")
                            {
                                labelLoginTips.Text = LangTable["tipsLockUserId"].TypeString();
                            }
                            else
                            {
                                labelLoginTips.Text = LangTable["tipsLoginFailed"].TypeString();
                            }
                        }
                        else
                        {
                            labelLoginTips.Text = LangTable["tipsLoginFailed"].TypeString();
                        }
                    }
                    else
                    {
                        labelLoginTips.Text = LangTable["tipsLoginFailed"].TypeString();
                    }
                }
                catch (Exception)
                {
                    labelLoginTips.Text = LangTable["tipsLoginFailed"].TypeString();
                }
                finally
                {
                    if (AppCommon.IsNothing(WebClient) == false)
                    {
                        WebClient.Dispose();
                    }

                    buttonLogin.Enabled = true;
                    buttonLogin.Text = LangTable["buttonLogin"].TypeString();
                }
            };

            WebClient.UploadDataAsync(PostUri, PostData);
        }


        /// <summary>
        /// 用户登录数据保存
        /// </summary>
        private void LoginDataSave()
        {
            XmlDocument XDocument = new XmlDocument();
            XmlNode XNode = default(XmlNode);
            string XPath = "/configuration/appSettings/add[@key=\"{key}\"]";
            string ConfigPath = AppCommon.PathCombine(AppConfig.AppPath, "dboxSyncer.exe.config");

            XDocument.Load(ConfigPath);

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "ServerHost"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.ServerHost;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "LoginId"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.LoginId;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "Password"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.Password;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "UserSession"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.UserSession;
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "AutoLogin"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.AutoLogin;
            }

            XDocument.Save(ConfigPath);
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

            XNodes = XDocument.SelectSingleNode("/language/formLogin");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
