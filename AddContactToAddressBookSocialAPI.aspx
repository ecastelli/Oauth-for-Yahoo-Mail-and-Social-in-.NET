<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddContactToAddressBookSocialAPI.aspx.vb" Inherits="AddContactToAddressBookSocialAPI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add to Contacts</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Enter and Name and Email Address Below.&nbsp; Then click Add Contact to add 
        contact to address book:<br />
    
    </div>
    Contact Name: <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
   
    <br />
    
    Contact Email: <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
    <br />
    <asp:Button ID="cmdAddToContacts" runat="server" Text="Add to Contacts" />
    </form>
</body>
</html>
