<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientSideAuth.aspx.cs"
    Inherits="CIAUTH_TestApp.ClientSideAuth" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="scripts/jquery-1.7.1.js" type="text/javascript"></script>
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
                            this.container.getElementsByTagName("iframe")[0].style.height = height + "px";

                        },
                        alertMessage: function (msg) {
                            alert(msg);
                        },
                        receiveToken: function (msg) {

                            $("#div_login").hide();
                            $("#div_logout").show();
                            token = msg;
                            $("#div_action").text(JSON.stringify(token, null, '  '));

                        }
                    }
                });

                $(document).ready(function () {
                    $("#btn_logout").live("click", function () {
                        token = null;
                        $("#div_logout").hide();
                        $("#div_login").show();
                        $("#div_action").text("logged out");
                    });

                });
    </script>
    <style type="text/css">
        #embedded iframe
        {
            height: 100%;
            width: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="div_logout" style="display: none;">
        <input type="button" id="btn_logout" value="Log Out" />
    </div>
    <div id="div_login">
    </div>
    <pre><code>
        <div id="div_action">
        </div>
    </code></pre>
    </form>
</body>
</html>
