<%@ Page Language="C#" 
    Title="Portfolio - Lets Trend" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true"
    CodeFile="Portfolio.aspx.cs" 
    Inherits="tableb" 
    EnableViewState="true"
    ResponseEncoding="ISO-8859-9"
%>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <title>Portfolio - Lets Trend</title>
    <meta name="Title" content="Portfolio - Lets Trend"/>
    <meta name="description" content="Provides Effective ways to invest in stocks. Portfolio provides simple free position tracking." />
    <meta name="keywords" content="stock,technical, analysis, technical analysis,Stock Investing, Tracking Portfolio, researching stocks, Wallstreet, Stock Charts, Stock Market, Stock Trends, Market Trends, Total Investment, Total Gain, Percent Gain" />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" />
    <meta name="google-site-verification" content="uJHSCMiIpCQIQp5glKqHuoCG-Htsr_ZjbgFtW84fGmI" />
    <meta name="author" content="Jakkjakk Par" />
    <meta name="RATING" content="General"/>
    <meta name="ROBOTS" content="index,follow"/>
    <meta name="REVISIT-AFTER" content="4 weeks"/>
    <meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>
    <meta name="alexaVerifyID" content="9rFPkPNrqYLjGah_of_fZf9W9hk" />
	<script>
	  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
	  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
	  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
	  })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

	  ga('create', 'UA-63033431-2', 'auto');
	  ga('send', 'pageview');

	</script>	
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" ViewStateMode="Disabled">
    <div style="position :relative; width:100%; top: 0px; font-size: medium; text-align: center;">   
        <h1>PORTFOLIO</h1>
        <div style="border-style:solid; boder-width:2px; width:99%; margin-left:auto; margin-right:auto; text-align:center">
            <asp:Table ID="portfolioTable" runat="server" HorizontalAlign="Center" ForeColor="Black" width="100%"
                style="Font-Size:Medium;">
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell8" runat="server">Remove</asp:TableCell>
                    <asp:TableCell ID="TableCell1" runat="server">Ticker</asp:TableCell>
                    <asp:TableCell ID="TableCell2" runat="server"># Shares</asp:TableCell>
                    <asp:TableCell ID="TableCell3" runat="server">Share Price</asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server">Investment</asp:TableCell>
                    <asp:TableCell ID="TableCell11" runat="server">Cost</asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server">Buy Date</asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server">Sell Date</asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server">Close</asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server">Cash Gain</asp:TableCell>
                    <asp:TableCell ID="TableCell7" runat="server">Percent Gain</asp:TableCell>
                    <asp:TableCell runat="server">Value</asp:TableCell>
                    <asp:TableCell runat="server">Notes</asp:TableCell>
                    <asp:TableCell runat="server"></asp:TableCell>
                </asp:TableRow>     
            </asp:Table>
            <asp:Button ID="addRow" runat="server" Text="Refresh" onclick="addRowClick" 
                Font-Size="Medium" />
            <asp:Button ID="ResetPortfolio" runat="server" Text="Reset" onclick="resetPortfolioClick" 
                Font-Size="Medium" />
            <br />
            <asp:Label ID="lerr" runat="server" ForeColor="Red" Text="" style="font-size:medium;"></asp:Label>
            <br />
            <asp:Panel ID="summaryPanel" runat="server">
                <asp:Table ID="summaryTable" runat="server" Width="100%" HorizontalAlign="Center" ForeColor="Black" 
                    style="font-size:medium;">
                    <asp:TableRow ID="TableRow2" runat="server">
                            <asp:TableCell ID="cell1" runat="server" HorizontalAlign="Center">Total Investment</asp:TableCell>
                            <asp:TableCell runat="server">Total Value</asp:TableCell>
                            <asp:TableCell ID="cell2" runat="server" HorizontalAlign="Center">Total Cash Gain</asp:TableCell>
                            <asp:TableCell ID="cell3" runat="server" HorizontalAlign="Center">Total Percent Gain</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="TableRow3" runat="server">
                            <asp:TableCell ID="totalInvestment" runat="server" HorizontalAlign="Center"></asp:TableCell>
                            <asp:TableCell ID="totalCashGain" runat="server" HorizontalAlign="Center"></asp:TableCell>
                            <asp:TableCell ID="totalPercentGain" runat="server" HorizontalAlign="Center"></asp:TableCell>
                            <asp:TableCell runat="server"></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:Panel>
        </div>
        <br />
        <asp:Panel ID="savePanel" runat="server">
            <br />
            <asp:Table ID="savedPortfolioTable" runat="server" Caption="Saved Portfolio's " CaptionAlign="Top" Width="100%" 
                HorizontalAlign="Center" ForeColor="Black" style="font-size:medium; border-style:solid; boder-width:2px;
                 width:99%; margin-left:auto; margin-right:auto; text-align:center" ViewStateMode="Disabled">
            </asp:Table>   
            <asp:Label ID="portfolioNameLabel" runat="server" Text="Portfolio Name" ForeColor="Black" Font-Size="Medium"></asp:Label>
            <asp:TextBox ID="portfolioNameTextBox" runat="server" style="margin-left: 1px" Width="87px" Font-Size="Medium"></asp:TextBox>
            <asp:Button ID="portfolioLoad" runat="server" onclick="portfolioLoadClick" Text="Load" Font-Size="Medium" />
            <asp:Button ID="portfolioSave" runat="server" Text="Save" onclick="portfolioSaveClick" Font-Size="Medium" />
            <asp:Button ID="portfolioDelete" runat="server" onclick="portfolioDeleteClick" Text="Delete" Width="63px" Font-Size="Medium" />
            <br />
            <asp:Label ID="dataStatus" runat="server" Height="28px" Width="290px" ForeColor="Black" Font-Size="Medium"></asp:Label>
            <br />
        </asp:Panel>
        <asp:Panel ID="publicPanel" runat="server">
            <asp:Table ID="publicTable" runat="server" Caption="Public Portfolio's " CaptionAlign="Top" Width="100%" 
                HorizontalAlign="Center" ForeColor="Black" style="font-size:medium; border-style:solid; boder-width:2px;
                 width:99%; margin-left:auto; margin-right:auto; text-align:center" ViewStateMode="Disabled">
            </asp:Table>   
        </asp:Panel>
    </div>
 </asp:Content>