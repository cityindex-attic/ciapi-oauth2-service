<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientSideAuth.aspx.cs"
         Inherits="CIAUTH_TestApp.ClientSideAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <script src="scripts/jquery-1.7.1.js" type="text/javascript"> </script>
        <script type="text/javascript" src="scripts/easyXDM/easyXDM.js"> </script>
        <script type="text/javascript" src="scripts/json2.js"> </script>
        <script type="text/javascript">
            $(document).ready(function () {
                var token = null;
                var username = null;
                var session = null;

                var authServerUrl = "<%= AuthServer %>";

                var authServerAjax = "/Authorize/UserAgentLogin?response_type=code&client_id=123&redirect_uri=<%= HttpUtility.UrlEncode(AuthServer) %>/callback&state=statevalue";


                var remote = new easyXDM.Rpc(/** The channel configuration */{
                local: "scripts/easyXDM/name.html",
                swf: authServerUrl + "/scripts/easyXDM/easyxdm.swf",

                remote: authServerUrl + authServerAjax,
                remoteHelper: authServerUrl + "/scripts/easyXDM/name.html",

                container: "div_login",

                onReady: function () {

                }
            },
                    {
                        remote: {
                            logIn: {}
                        },
                        local: {
                            alertMessage: function (msg) {
                                alert(msg);
                            },
                            receiveToken: function (msg) {
                                // #TODO: redesign this interface
                                // have separate token, error, data channels
                                debugger;
                                if (!msg) {
                                    username = null;
                                    session = null;
                                    token = null;
                                } else {
                                    if (msg.access_token) {


                                        token = msg;
                                        username = token.access_token.split(":")[0];
                                        session = token.access_token.split(":")[1];
                                    } else {

                                    }

                                }
                                $("#div_action").text(JSON.stringify(msg, null, '  '));

                            }
                        }
                    });


            //
            $("#btn_login").live("click", function () { remote.logIn(); });
    


            $("#btn_account_info").live("click", function () {


                $.getJSON("https://ciapi.cityindex.com/tradingapi/useraccount/ClientAndTradingAccount?userName="
                        + username + "&session=" + session + "&only200=true",
                        function (data, a, b, c) {
                            $("#div_action").text(JSON.stringify(data, null, "  "));
                        }, function (data, a, b, c) {
                            $("#div_action").text(JSON.stringify(data, null, "  "));

                        });

            });


        });

       
    
    </script>
        <style type="text/css">
        
            
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <h2>
                Ajax Authentication Demo</h2>
            <p>
                The dotted red line borders an IFRAME running in the context of the auth server.</p>
            <p>
                Upon successful login and/or password change, the IFRAME posts a message to the
                parent application with the decoded access token.</p>
            <p>
                Using the client_id, the login frame may&nbsp; be rendered in such a way that it
                will only communicate with a parent with a specific URL, in much the same fashion
                as the full workflow allows only client specific callback URLs.</p>
            <p>
                Using the client_id, the login frame may be styled to conform to the client application.</p>
            <p>
                Cross domain communication is accomplished using <a href="http://easyxdm.net/wp/">easyXDMMM</a>.</p>
            <p>
                What the user-agent (javascript) application may do with the access token is rather
                limited at this time (javascript XD limitations).&nbsp;
            </p>
            <p>
                I suspect it will fall to us to provide guidance for cross-domain communication
                with a generated api proxy to be hosted on the client application&#39;s server and/or
                CORS/WebSockets implementations
            </p>
            <div id="div_login">
            </div>
            <p>
                These buttons trigger authentication actions tunnelled to the auth server via IFRAM
                communications</p>
            <div id="div_action_buttons">
                <input type="button" id="btn_login" value="log in" />
                 
            </div>
            <p>
                These buttons use CORS transport to talk directly to the API using the information
                extracted from the access_token</p>
            <p>
                <b>NOTE: these buttons will only work in browsers that fully support the CORS specification</b></p>
            <div id="div_api_buttons">
                <input type="button" id="btn_account_info" value="CORS account info" />
            </div>
            
            <div  style="border: 2px dotted green">
                <pre>
<code id="div_action"></code>
    </pre>
            </div>
        </form>
    </body>
</html>