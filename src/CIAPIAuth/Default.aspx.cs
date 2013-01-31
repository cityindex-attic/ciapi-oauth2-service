using System;
using System.Text;
using System.Web;
using System.Web.UI;
using CIAPI.DTO;
using CIAPI.Rpc;

namespace CIAPIAuthWeb
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ErrorLabel.Text = "";
        }

        protected void AuthorizeButton_Click(object sender, EventArgs e)
        {
            string returnUrl = Request["returnUrl"];

            using (
                var client = new Client(new Uri("https://ciapi.cityindex.com/tradingapi"), new Uri("http://foo.com"),
                                        "CIAPIAuth"))
            {
                try
                {
                    ApiLogOnResponseDTO result = client.LogIn(UsernameTextBox.Text, PasswordTextBox.Text);
                    if (result.PasswordChangeRequired)
                    {
                        Response.Redirect("default.aspx?changePassword=true&returnUrl=" +
                                          HttpUtility.UrlEncode(returnUrl));
                    }
                    string package = client.UserName + ":" + client.Session;
                    var encrypted = new SimplerAes().EncryptToUrl(package);



                    string url = returnUrl + "?auth=" + encrypted;

                    Response.Redirect(url);
                }
                catch (InvalidCredentialsException ice)
                {
                    ErrorLabel.Text = "Invalid Username or Password";
                }
                catch (Exception ex)
                {
                    ErrorLabel.Text = ex.Message;
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            string returnUrl = Request["returnUrl"];
            Response.Redirect(returnUrl);
        }
    }
}