using System;
using System.Configuration;
using System.Windows.Forms;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.UI;

namespace DesktopAppTest
{
    public partial class Form1 : Form
    {
        private Client _client;
        private string _session;
        private AccessToken _token;
        private string _userName;

        public Form1()
        {
            InitializeComponent();
        }

        public AccessToken Token
        {
            get { return _token; }
            set
            {
                _token = value;
                ConfigureUi();
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                lbl_username.Text = value;
            }
        }

        public string Session
        {
            get { return _session; }
            set
            {
                _session = value;
                lbl_session.Text = value;
            }
        }

        private void ConfigureUi()
        {
            if (_token == null)
            {
                btn_login.Enabled = true;
                btn_logout.Enabled = false;
                btn_account_info.Enabled = false;
            }
            else
            {
                btn_login.Enabled = false;
                btn_logout.Enabled = true;
                btn_account_info.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.TokenEvent += loginForm_TokenEvent;


            DialogResult result = loginForm.ShowDialog(this);
            loginForm.TokenEvent -= loginForm_TokenEvent;


            if (result != DialogResult.OK)
            {
                MessageBox.Show(@"Login failed");
            }

            // do not dispose the login form or default browser will pop up
            // just let it go out of scope.
            //loginForm.Dispose();
        }

        private void loginForm_TokenEvent(object sender, AccessTokenEventArgs e)
        {
            lbl_message.Text = e.Message;

            if (e.AccessToken != null)
            {
                Token = e.AccessToken;
                UserName = Token.UserName;
                Session = Token.SessionId;
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
                _client = BuildRpcClient();
            }
            else
            {
                Token = null;
                UserName = null;
                Session = null;
            }
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            bool result = _client.LogOut();
            if (result)
            {
                Token = null;
                UserName = null;
                Session = null;
            }
        }

        private Client BuildRpcClient()
        {
            var client = new Client(new Uri(ConfigurationManager.AppSettings["api_rpc_server"]),
                                    new Uri("http://foo.com"), "CIAUTH_DESKTOP_DEMO")
                {
                    Session = Session,
                    UserName = UserName
                };
            return client;
        }

        private void btn_account_info_Click(object sender, EventArgs e)
        {
            AccountInformationResponseDTO result = _client.AccountInformation.GetClientAndTradingAccount();
            tb__account_info.Text = _client.Serializer.SerializeObject(result);
        }
    }
}