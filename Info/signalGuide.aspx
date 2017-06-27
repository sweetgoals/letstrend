<%@ Page Language="VB" AutoEventWireup="false" CodeFile="signalGuide.aspx.vb" Inherits="Info_signalconfig" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="position :relative; margin-left:auto; top: 0px;
            font-size: medium; text-align: center;">
        <h1>Signal Guide Setup</h1>
        <asp:Table ID="signalTable" runat="server" HorizontalAlign="Center">

        </asp:Table>
        <br />
        Examples <br />
        SMA50xSMA200 10 -&gt; % Gain after 10 days when the SMA 50 crosses SMA 200 <br />
        RSI20&gt;40 15 -&gt; % Gain after 15 Days when RSI goes above 40 <br />
        EMA15&lt;SMA25 50 -&gt; % Gain after 50 days when EMA15 goes below SMA25<br />
        <asp:Label ID="errorLabel" runat="server" Text="" Font-Bold="True" ForeColor="Red"></asp:Label><br />
        <asp:Label ID="updated" runat="server" Text="" Font-Bold="True" ForeColor="Green"></asp:Label><br />
        <asp:Button ID="SubmitButton" runat="server" Text="Submit" /><br /><br /><br />
    </div>
    </form>
</body>
</html>
