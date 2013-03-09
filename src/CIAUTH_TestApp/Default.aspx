<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CIAUTH_TestApp.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CIAUTH web flow example</title>
</head>
<body>
      <form id="form1" runat="server">
    <div>
        <asp:Panel ID="AuthPanel" runat="server" GroupingText="Authorize App">
            Click to be redirected to a secure server to authorize this app to access CIAPI on your behalf<br/>
            <asp:Button ID="AuthButton" runat="server" Text="Authorize" 
                onclick="AuthButton_Click" />
        </asp:Panel>
    </div>
    <asp:Panel ID="CIAPIPanel" runat="server" GroupingText="App accessing CIAPI">
        CIAPI says your LogonUserName is 
        <asp:Label ID="LogonUserNameLabel" runat="server" Text=""></asp:Label>
    </asp:Panel>
    </form>
</body>
</html>
