<%@ Page Title="Market Summary - Lets Trend"
    Language="VB" 
    AutoEventWireup="false" 
    MasterPageFile="~/Site.Master" 
    CodeFile="wallstate.aspx.vb" 
    Inherits="_Default"
    ResponseEncoding="ISO-8859-9" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <title>Market Summary - Lets Trend</title>
<meta name="Title" content="Market Summary - Lets Trend"/>
<meta name="description" content="Gives a status on how the Stock Indices are doing. Lists Gains for all Dow Industrial and Dow Transportation Stocks." />
<meta name="keywords" content="stock,technical, analysis, technical analysis, Dow Industrial, Dow Transportation, Indices Gains day, week, 3 month, 6 months and one year." />
<meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" />
<meta name="author" content="Jakkjakk Par" />
<meta name="RATING" content="General"/>
<meta name="ROBOTS" content="index,follow"/>
<meta name="REVISIT-AFTER" content="4 weeks"/>
<meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>

<script>
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-63033431-2', 'auto');
  ga('send', 'pageview');

</script>    
    <style type="text/css">
        #Text1
        {
            width: 93px;
        }
        #Text2
        {
            width: 95px;
        }
    </style>

    <script type="text/javascript" src="http://code.jquery.com/jquery-1.4.4.js"></script>
	<script language="javascript" type="text/javascript">
	 
        function turnDarkGreenLocal(customButton) {
            var prog;
            if (customButton.style.backgroundColor == "") {
	            customButton.style.backgroundColor = "green";
	            customButton.style.color = "white";
	            openPanel('MainContent_ProgressPanelExtender');
	            /*closePanel('MainContent_cpe');
	            alert('closepanel');*/
	        }
	        else {
	            customButton.style.backgroundColor = "";
	            customButton.style.color = "black";
	            closePanel('MainContent_ProgressPanelExtender');
	            /*prog = $find('MainContent_UpdateProgress1');
	            prog._element.hideFocus = "True";
	            closePanel('MainContent_ProgressPanelExtender');
	            alert('openpanel');*/
            }
	    }

	    function dowDarkGreen() {
	        var customButton = document.getElementById('MainContent_dowButton');
	        turnDarkGreenLocal(customButton);
	    }

	    function dowTranDarkGreen() {
	        var customButton = document.getElementById('MainContent_dowTransButton');
	        turnDarkGreenLocal(customButton);
	    }

	    function openPanel(opt) {
	        var cpe;
	        cpe = $find(opt);
	        cpe.set_Collapsed(false);
	    }

	    function closePanel(opt) {
	        cpe = $find(opt);
	        cpe.set_Collapsed(true);
	    }
     </script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePartialRendering="true"></asp:ToolkitScriptManager>

    <div style="top:0px; margin-left:auto; margin-right:auto;
           width:13%; float: left; font-size: medium; text-align: center; background-color: #CCFF99;">
        <b>Market
        <br />Features</b>
        <br />
        <asp:Button ID="dowButton" runat="server" Text="Dow" Width="100px" BorderColor="Black" BorderStyle="Solid" 
            BorderWidth="2px" OnClientClick="dowDarkGreen();" Font-Size="Medium"/>
        <br />
        <asp:Button ID="dowTransButton" runat="server" Text="Dow Trans" Width="100px" BorderColor="Black" BorderStyle="Solid" 
            BorderWidth="2px" OnClientClick="dowTranDarkGreen();" Font-Size="Medium"/>
        <br />    
        <input id="helpButton" type="button" value="Help" 
            onclick="window.open('http://www.letstrend.com/theblog/category/Market-Summary.aspx');" style="border: 2px solid #000000; width: 100px; font-size: medium;" /><br />
        <input id="Button1" style="border: 2px solid #000000; width: 100px; font-size: medium;" type="button" value="Contact" 
            onclick="window.open('http://www.letstrend.com/theblog/contact.aspx');" />
        <br />
        <br />
    </div>    
    <div style="position :relative; margin-left:auto; width:85%; top: 0px;
        font-size: medium; text-align: center;">   
        <br />

        <asp:CollapsiblePanelExtender ID="ProgressPanelExtender" runat="Server" 
            TargetControlID="progressPanel"
            Collapsed="True"
            SuppressPostBack="False">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="progressPanel" runat="server">       
            <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                <ProgressTemplate>
                    Getting List Info.  Please Wait...
                    <br />
                    <br />
                    <asp:Image ID="ballRotate" runat="server" ImageUrl="~/images/rotateBall.gif" />
                    <br />
                    <br />
                </ProgressTemplate>
            </asp:UpdateProgress>
        </asp:Panel>

        <asp:Table ID="indexTable" runat="server" Width="100%" Caption="<b>Indices</b>" 
            CaptionAlign="Top" forecolor="black" HorizontalAlign="Center" ViewStateMode="Enabled" >
            <asp:TableRow ID="TableRow3" runat="server">
                <asp:TableCell ID="TableCell21" runat="server" HorizontalAlign="Center">Stock Information</asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell23" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell24" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell25" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell26" runat="server" HorizontalAlign="Center">Gains(%)</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server">
                <asp:TableCell runat="server">Name</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Ticker</asp:TableCell>
                <asp:TableCell ID="TableCell27" runat="server" HorizontalAlign="Center">Today's Value</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Today</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Week</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Month</asp:TableCell>
                <asp:TableCell ID="TableCell28" runat="server" HorizontalAlign="Center">3 Months</asp:TableCell>
                <asp:TableCell ID="TableCell29" runat="server" HorizontalAlign="Center">6 Months</asp:TableCell>
                <asp:TableCell ID="TableCell30" runat="server" HorizontalAlign="Center">Year Ago</asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:UpdatePanel ID="dowUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
        <asp:CollapsiblePanelExtender ID="dowPanelExtender" runat="Server" 
            TargetControlID="dowPanel"
            ExpandControlID="dowButton" 
            CollapseControlID="dowButton" 
            Collapsed="True"
            SuppressPostBack="False">
        </asp:CollapsiblePanelExtender>
          
        <asp:Panel ID="dowPanel" runat="server" style="position :relative; margin-left:auto; margin-right:auto; width:100%; top: 0px;
         font-size: medium; text-align: center;">
            <asp:Table ID="dowIndTable" runat="server" Width="100%" Caption="<b>Dow</b>" 
                    CaptionAlign="Top" ViewStateMode="Enabled" 
                    HorizontalAlign="Center" ForeColor="Black">
                <asp:TableRow ID="TableRow1" runat="server">
                    <asp:TableCell ID="TableCell2" runat="server" HorizontalAlign="Center">Stock Information</asp:TableCell>
                    <asp:TableCell ID="TableCell4" runat="server" ></asp:TableCell>
                    <asp:TableCell ID="TableCell5" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell6" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell7" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell10" runat="server" HorizontalAlign="Center">Gains(%)</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Name</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Ticker</asp:TableCell>
                    <asp:TableCell ID="TableCell3" HorizontalAlign="Center" runat="server">Today's Price</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Today</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Week</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Month</asp:TableCell>
                    <asp:TableCell ID="TableCell8" runat="server" HorizontalAlign="Center">3 Months</asp:TableCell>
                    <asp:TableCell ID="TableCell9" runat="server" HorizontalAlign="Center">6 Months</asp:TableCell>
                    <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Center">Year Ago</asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>
            </ContentTemplate>

            <Triggers>
            <asp:AsyncPostBackTrigger ControlID="dowButton" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

        <br />

        <asp:UpdatePanel ID="dowTransUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>

        <asp:CollapsiblePanelExtender ID="dowTransPanelExtender" runat="Server" 
            TargetControlID="dowTransPanel"
            ExpandControlID="dowTransButton" 
            CollapseControlID="dowTransButton" 
            Collapsed="True"
            SuppressPostBack="False">
        </asp:CollapsiblePanelExtender>
                   
        <asp:Panel ID="dowTransPanel" runat="server" style="position :relative; margin-left:auto; margin-right:auto; width:100%; top: 0px;
         font-size: medium; text-align: center;">
            <asp:Table ID="dowTransTable" runat="server" Width="100%" Caption="<b>Dow Transportation</b>" 
                CaptionAlign="Top" ViewStateMode="Enabled" HorizontalAlign="Center" ForeColor="Black">
                <asp:TableRow ID="TableRow2" runat="server">
                    <asp:TableCell ID="TableCell11" runat="server" HorizontalAlign="Center">Stock Information</asp:TableCell>
                    <asp:TableCell ID="TableCell12" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell13" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell14" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell15" runat="server"></asp:TableCell>
                    <asp:TableCell ID="TableCell16" runat="server" HorizontalAlign="Center">Gains(%)</asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Name</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Ticker</asp:TableCell>
                    <asp:TableCell ID="TableCell17" HorizontalAlign="Center" runat="server">Today's Price</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Today</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Week</asp:TableCell>
                    <asp:TableCell runat="server" HorizontalAlign="Center">Month</asp:TableCell>
                    <asp:TableCell ID="TableCell18" runat="server" HorizontalAlign="Center">3 Months</asp:TableCell>
                    <asp:TableCell ID="TableCell19" runat="server" HorizontalAlign="Center">6 Months</asp:TableCell>
                    <asp:TableCell ID="TableCell20" runat="server" HorizontalAlign="Center">Year Ago</asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </asp:Panel>

        </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="dowTransButton" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

        <br />

        </div>
  </asp:Content>