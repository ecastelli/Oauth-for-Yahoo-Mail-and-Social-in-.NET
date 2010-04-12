<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PostAuthSamples.aspx.vb" Inherits="PostAuthSamples" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <p>
        Congrats!&nbsp; The fact that you made it to this page means you now have Oauth 
        credentials for making additional requests.&nbsp; This project contains two 
        samples:&nbsp; One for querying Yahoo! mail via SOAP calls, and another for 
        querying Yahoo! Social APIs. Click the link below to try each:</p>
    <form id="form1" runat="server">
    <div>
    
        <asp:HyperLink ID="hlnkMail" runat="server" 
            NavigateUrl="~/GetFoldersAndMailHeaders.aspx">Yahoo! Mail Sample - Get messages from the user&#39;s folders</asp:HyperLink>
    
    </div>
    <p>
        <asp:HyperLink ID="hlnkSocialContacts" runat="server" 
            NavigateUrl="~/AddContactToAddressBookSocialAPI.aspx">Yahoo! Social Sample - Add Contact to Address Book</asp:HyperLink>
    </p>
    </form>
</body>
</html>
