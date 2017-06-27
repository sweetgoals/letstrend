<%@ Page Title="Trending - Lets Trend" 
         Language="VB" 
         MasterPageFile="~/Site.master" 
         AutoEventWireup="false" 
         CodeFile="trending.aspx.vb" 
         Inherits="trending" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Trending - Lets Trend</title>
    <meta name="Title" content="Trending - Lets Trend"/>
    <meta name="description" content="Provides Effective ways to track stocks using Technical Analysis" />
    <meta name="keywords" content="stock,technical, analysis, technical analysis,Stock Investing, Tracking Portfolio, researching stocks, Wallstreet, Stock Charts, Stock Market, Stock Trends, Market Trends, Total Investment, Total Gain, Percent Gain" />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" />
    <meta name="google-site-verification" content="uJHSCMiIpCQIQp5glKqHuoCG-Htsr_ZjbgFtW84fGmI" />
    <meta name="author" content="Jakkjakk Par" />
    <meta name="RATING" content="General"/>
    <meta name="ROBOTS" content="index,follow"/>
    <meta name="REVISIT-AFTER" content="4 weeks"/>
    <meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>
    <meta name="alexaVerifyID" content="9rFPkPNrqYLjGah_of_fZf9W9hk" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="margin-left:auto; margin-right:auto; width:100%; font-size: large; text-align: center;">
        <asp:Table ID="trendTable" runat="server" CellSpacing="5" 
            HorizontalAlign="Center" BorderStyle="Solid" BorderWidth="0px" 
            Caption="&lt;h1&gt;Trending&lt;/h1&gt;" CaptionAlign="Top">
            <asp:TableRow runat="server" ID="row_0"> 
                <asp:TableCell ID="TableCell1" runat="server">Delete</asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server">Ticker</asp:TableCell>
                <asp:TableCell ID="TableCell3" runat="server">Watch Start</asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server">Buy Option</asp:TableCell>
                <asp:TableCell ID="TableCell5" runat="server">Buy Signal</asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server">Sell Option</asp:TableCell>
                <asp:TableCell ID="TableCell7" runat="server">Sell Signal</asp:TableCell>
                <asp:TableCell ID="TableCell9" runat="server">Trade Cost</asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server">Capital</asp:TableCell>
                <asp:TableCell ID="TableCell14" runat="server">Value</asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server">$ Gain</asp:TableCell>
                <asp:TableCell ID="TableCell11" runat="server">% Gain</asp:TableCell>
                <asp:TableCell ID="TableCell12" runat="server">Status</asp:TableCell>
                <asp:TableCell ID="TableCell13" runat="server">Last Trade Date</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br /> 
    </div>
    <div style="font-size: medium; text-align: center;">
        <asp:Button ID="refreshButton" runat="server" Text="Refresh" 
            HorizontalAlign="Center" Height="30px" Width="100px"/><br />
        <asp:CheckBox ID="mailCheck" runat="server" Text="Receive Email Updates" />
    </div>
</asp:Content>

