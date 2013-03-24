using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using CIAUTH.UI;
using Newtonsoft.Json;
namespace PhoneAppTest
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();

        }
        private string _returnPage;
        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            var authServer = NavigationContext.QueryString["auth"];
            _returnPage = NavigationContext.QueryString["returnPage"];
            authControl.AuthServer = authServer;
            
            // need a tick for emulator to connect. no sleep = 404
            Thread.Sleep(1000);

            authControl.ShowLogin();

        }

        private void authControl_TokenEvent(object sender, AccessTokenEventArgs e)
        {
            // send the message and token back to the main page.
            var url = string.Format(_returnPage + "?msg={0}&token={1}", e.Message, e.AccessToken.access_token);
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

    }
}