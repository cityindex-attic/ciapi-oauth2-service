using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CIAUTH_TestApp
{
    public partial class ClientSideAuth : System.Web.UI.Page
    {
        public string AuthServer;
        protected void Page_Load(object sender, EventArgs e)
        {
            AuthServer = WebConfigurationManager.AppSettings["authServer"];
        }
    }
}