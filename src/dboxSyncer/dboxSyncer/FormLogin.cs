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


        private Hashtable LangTable = new Hashtable();


        public FormLogin()
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void FormLogin_Load(object sender, EventArgs e)
        {
            LoadLanguage(ref LangTable);

            labelServerHost.Text = LangTable["labelServerHost"].TypeString();
            labelLoginId.Text = LangTable["labelLoginId"].TypeString();
            labelPassword.Text = LangTable["labelPassword"].TypeString();
            checkBoxAutoLogin.Text = LangTable["checkBoxAutoLogin"].TypeString();
            buttonLogin.Text = LangTable["buttonLogin"].TypeString();
            labelLoginTips.Text = "";

            if (string.IsNullOrEmpty(AppConfig.LoginId) == false && string.IsNullOrEmpty(AppConfig.Password) == false && AppConfig.AutoLogin == true)
            {
                LoginProcess(AppConfig.ServerHost, AppConfig.LoginId, AppConfig.Password, false);
            }

            if (string.IsNullOrEmpty(AppConfig.ServerHost) == false)
            {
                textBoxServerHost.Text = AppConfig.ServerHost;
            }

            if (string.IsNullOrEmpty(AppConfig.LoginId) == false)
            {
                textBoxLoginId.Text = AppConfig.LoginId;
            }

            if (AppConfig.AutoLogin == true)
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
            string ServerHost = textBoxServerHost.Text.TypeString();

            if (AppCommon.StringCheck(ServerHost, @"^(http|https)\:\/\/[\w\-\.\:]+\/?$") == false)
            {
                labelLoginTips.Text = LangTable["tipsServerHost"].TypeString();
                return;
            }

            AppConfig.ServerHost = ServerHost;

            string LoginId = textBoxLoginId.Text.TypeString();

            if (AppCommon.StringCheck(LoginId, @"^([^\s\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\{\}\;\:\'\""\\\|\,\.\<\>\/\?]{2,16}|[\w\-]+\@[\w\-]+\.[\w]{2,4}(\.[\w]{2,4})?|\+?([\d]{2,4}\-?)?[\d]{6,11})$") == false)
            {
                labelLoginTips.Text = LangTable["tipsLoginId"].TypeString();
                return;
            }

            AppConfig.LoginId = LoginId;

            string Password = textBoxPassword.Text.TypeString();

            if (AppCommon.StringCheck(Password, @"^[^\s\'\""\%\*\<\>\=]{6,16}$") == false)
            {
                labelLoginTips.Text = LangTable["tipsPassword"].TypeString();
                return;
            }

            AppConfig.Password = Password;

            bool AutoLogin = checkBoxAutoLogin.Checked.TypeBool();

            AppConfig.AutoLogin = AutoLogin;

            LoginProcess(ServerHost, LoginId, Password, true);
        }


        /// <summary>
        /// 用户登录处理程序
        /// </summary>
        private void LoginProcess(string ServerHost, string LoginId, string Password, bool Taskbar)
        {
            buttonLogin.Enabled = false;
            buttonLogin.Text = LangTable["buttonLogging"].TypeString();

            WebClient WebClient = null;

            try
            {
                WebClient = new WebClient();

                WebClient.Proxy = null;

                WebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                WebClient.UploadDataCompleted += (object sender, UploadDataCompletedEventArgs e) =>
                {
                    try
                    {
                        if (e.Cancelled == false)
                        {
                            if (AppCommon.IsNothing(e.Error) == true)
                            {
                                string Result = Encoding.UTF8.GetString(e.Result);

                                string Cookie = WebClient.ResponseHeaders.Get("Set-Cookie");

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
                        buttonLogin.Enabled = true;
                        buttonLogin.Text = LangTable["buttonLogin"].TypeString();
                    }
                };

                string PostUrl = "" + ServerHost + "/api/sync/user-login?timestamp=" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + "";
                string PostString = "loginid=" + HttpUtility.UrlEncode(LoginId) + "&password=" + HttpUtility.UrlEncode(Password) + "";
                byte[] PostData = Encoding.UTF8.GetBytes(PostString);

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
        /// 用户登录数据保存
        /// </summary>
        private void LoginDataSave()
        {
            XmlDocument XDocument = new XmlDocument();

            XDocument.Load(AppConfig.ConfigFilePath);

            string XPath = "/configuration/appSettings/add[@key=\"{key}\"]";

            XmlNode XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "ServerHost"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.ServerHost.TypeString();
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "LoginId"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.LoginId.TypeString();
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "Password"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.Password.TypeString();
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "UserSession"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.UserSession.TypeString();
            }

            XNode = (XmlNode)XDocument.SelectSingleNode(XPath.Replace("{key}", "AutoLogin"));

            if (AppCommon.IsNothing(XNode) == false)
            {
                XNode.Attributes["value"].InnerText = AppConfig.AutoLogin.TypeString();
            }

            XDocument.Save(AppConfig.ConfigFilePath);
        }


        /// <summary>
        /// 加载语言
        /// </summary>
        private void LoadLanguage(ref Hashtable LangTable)
        {
            XmlDocument XDocument = new XmlDocument();

            XDocument.Load(AppCommon.PathCombine(AppConfig.AppPath, "language.xml"));

            XmlNode XNodes = XDocument.SelectSingleNode("/language/formLogin");

            foreach (XmlNode XNode in XNodes.ChildNodes)
            {
                LangTable.Add(XNode.Name, XNode.InnerText);
            }
        }


    }


}
