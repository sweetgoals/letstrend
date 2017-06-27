<%@ Page Language="VB" AutoEventWireup="false" CodeFile="guidelines.aspx.vb" Inherits="Info_guidelines" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="position :relative; margin-left:auto; top: 0px;
        font-size: medium; text-align: center;">
        <h1>Fundamental Guidelines</h1>
        <asp:Table ID="guideTable" runat="server" HorizontalAlign="Center">       
        </asp:Table>
        <asp:Button ID="SubmitButton" runat="server" Text="Submit" /><br /><br /><br />
        <asp:Label ID="updated" runat="server" Text="Updated" Font-Bold="True" 
            Font-Size="Larger" ForeColor="#006600" />
    </div>
    </form>
</body>
</html>
