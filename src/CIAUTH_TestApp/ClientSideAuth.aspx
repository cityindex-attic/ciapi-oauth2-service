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

        var token = null;
        var REMOTE = "<%= AuthServer %>";
        var REMOTE_AUTH = "/Authorize/AjaxLogin?response_type=code&client_id=123&redirect_uri=<%= HttpUtility.UrlEncode(AuthServer) %>/callback&state=statevalue";


        var remote = new easyXDM.Rpc(/** The channel configuration */{

        local: "scripts/easyXDM/name.html",
        swf: REMOTE + "/scripts/easyXDM/easyxdm.swf",

        remote: REMOTE + REMOTE_AUTH,
        remoteHelper: REMOTE + "/scripts/easyXDM/name.html",

        container: "div_login",
        props: {
            style: {
                border: "2px dotted red",
                height: "200px"
            }
        },
        onReady: function () {
            // maybe verify connection
        }
    },
                {
                    remote: {
                        noOp: {}
                    },
                    local: {
                        resize: function (height) {
                   
                            var iframes = $("#div_login iframe")[0];
                          
                            iframes.style.height = height + "px";

                        },
                        alertMessage: function (msg) {
                            alert(msg);
                        },
                        receiveToken: function (msg) {


                            token = msg;
                            $("#div_action").text(JSON.stringify(token, null, '  '));

                        }
                    }
                });

    $(document).ready(function () {
        $("#btn_logout").live("click", function () {
            token = null;

            $("#div_action").text("logged out");
        });

    });
    </script>
    <style type="text/css">
 
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Ajax Authentication Demo</h2>
        <p> The dotted red line borders an IFRAME running in the context of the auth server.</p>
        <p> Upon successful login and/or password change, the IFRAME posts a message to the 
            parent application with the decoded access token.</p>
        <p> Using the client_id, the login frame may&nbsp; be rendered in such a way that it 
            will only communicate with a parent with a specific URL, in much the same 
            fashion as the full workflow allows only client specific callback URLs.</p>
        <p> Using the client_id, the login frame may be styled to conform to the client 
            application.</p>
        <p> Cross domain communication is accomplished using 
            <a href="http://easyxdm.net/wp/">easyXDMMM</a>.</p>
        <p> What the user-agent (javascript) application may do with the access token is 
            rather limited at this time (javascript XD limitations).&nbsp; </p>
        <p> I suspect it will fall to us to provide guidance for cross-domain communication 
            with a generated api proxy to be hosted on the client application&#39;s server 
            and/or CORS/WebSockets implementations </p>
    <div id="div_login">
    </div>
    <p>The dotted green line borders content in this document reflecting the access token posted from the auth server IFRAME</p>
    <pre><code>
        <div id="div_action" style="border:2px dotted green">
        </div>
    </code></pre>
    </form>
</body>
</html>
