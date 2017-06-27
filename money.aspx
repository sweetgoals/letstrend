<%@ Page Title="Money Maker - Lets Trend" 
    Language="VB" 
    MasterPageFile="~/Site.master" 
    AutoEventWireup="false" 
    CodeFile="money.aspx.vb" 
    Inherits="money" 
    validateRequest="false"
    ResponseEncoding="ISO-8859-9"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Money Maker - Lets Trend</title>
    <%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

    <meta name="Title" content="Money Maker - Lets Trend"/>
    <meta name="description" 
    content="Web Based Stock Investing System Builder. Use differrent stock indicators to build an effective 
            trading system. In the future you will be able to make up your own indicators." />
    <meta name="keywords" 
    content="stock,technical, analysis, technical analysis, Dow Industrial, Dow Transportation, Indices Gains day,
            investing, Investing System Builder, Money Maker, Stock Analysis, Investing System Analysis" />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" />
    <meta name="author" content="Jakkjakk Par" />
    <meta name="RATING" content="General"/>
    <meta name="ROBOTS" content="index,follow"/>
    <meta name="REVISIT-AFTER" content="4 weeks"/>
    <meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.js"></script>
	<script type="text/javascript" src="highcharts/highcharts.js"></script>          
	<script>
		  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
		  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
		  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
		  })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

		  ga('create', 'UA-63033431-2', 'auto');
		  ga('send', 'pageview');

	</script>
    <link rel="stylesheet" type="text/css" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.1/themes/base/jquery-ui.css"/>

    <!--[if IE]>
        <script type="text/javascript" src="../js/excanvas.compiled.js"></script>
    <![endif]-->
    <script language="javascript" type="text/javascript">      
        var stockChart;
        var volumeChart;
        var macdChart;  
        var rsiChart;
        var rocChart;
                function renderStockChart()
                {
                    var dates = [<%=graphDate %>];
                    stockChart = new Highcharts.Chart({
		                chart: {
		                    renderTo: 'stockGraph',
		                    defaultSeriesType: 'line',
                            margin: [50, 150, 60, 80]
		                },
		                title: {
		                    text: '<%=lookup.Text %> Price',
		                    style: {
		                        margin: '0px 100px 0 0' // center it
		                    }
		                },
		                subtitle: {
                            text: <%=graphRange %>,
		                    style: {
		                        margin: '0 100px 0 0' // center it
		                    }
		                },
		                xAxis: 
			            {
		                    categories: dates,
   		                    title: 
	        			    {
		                        text: 'Date'
		                    },
                        	    labels: 
	        		    	    {
			        	    	    step: Math.ceil(dates.length/11.0)
				                }
    			        },
		                yAxis: {
		                    title: {
		                        text: 'Price'
		                    },
		                    plotLines: [{
		                        value: 0,
		                        width: 1,
		                        color: '#808080'
		                    }]
		                },
		                tooltip: {
		                    formatter: function () {
		                        return '<b>' + this.series.name + '</b><br/>' +
							    this.x + ': ' + this.y;
		                    }
		                },
		                legend: {
		                    layout: 'vertical',
		                    style: {
		                        left: 'auto',
		                        bottom: 'auto',
		                        right: '10px',
		                        top: '100px'
		                    }
		                },
                        <%=graphSeries %>
		            });
                }
                             
                function renderMacdChart()
                {
                    var dates = [<%=graphDate %>]; 
		            macdChart = new Highcharts.Chart   ({
		                chart: 
                        {
		                    renderTo: 'macdGraph',
		                    defaultSeriesType: 'line',
                            margin: [50, 150, 60, 80]
		                },
		                title: 
                        {
		                    text: '<%=lookup.Text %> MACD',
		                    style: 
                            {
		                        margin: '0px 100px 0 0' // center it
		                    }
		                },
		                subtitle: 
                        {
                            text: <%=graphRange %>,
		                    style: 
                            {
		                        margin: '0 100px 0 0' // center it
		                    }
		                },
    		            xAxis: 
                        {
	    	                categories: dates,
                            // maxZoom: 10,
		                    title: 
                            {
		                        text: 'Date'
                            },
                       	    labels: 
        		    	    {
		        	    	    step: Math.ceil(dates.length/11.0)
			                }
		                },

		                yAxis: 
                        {
		                    title: 
                            {
		                        text: ''
		                    },
		                    plotLines: 
                            [{
		                        value: 0,
		                        width: 1,
		                        color: '#808080'
		                    }]
		                },
		                tooltip: 
                        {
		                    formatter: function () 
                            {
		                        return '<b>' + this.series.name + '</b><br/>' +
							    this.x + ': ' + this.y;
		                    }
		                },
		                legend: 
                        {
		                    layout: 'vertical',
		                    style: 
                            {
		                        left: 'auto',
		                        bottom: 'auto',
		                        right: '10px',
		                        top: '100px'
		                    }
		                },
                        <%=macdSeries %>
		            });
                }                     
            
                function renderRsiChart()
                {               
                    var dates = [<%=graphDate %>];
                    rsiChart = new Highcharts.Chart({
		                chart: 
                        {
		                    renderTo: 'rsiGraph',
		                    defaultSeriesType: 'line',
                            margin: [50, 150, 60, 80]
		                },
		                title: 
                        {
		                    text: '<%=lookup.Text %> Relative Strength Index',
		                    style: 
                            {
		                        margin: '0px 100px 0 0' // center it
		                    }
		                },
		                subtitle: 
                        {
                            text: <%=graphRange %>,
		                    style: 
                            {
		                        margin: '0 100px 0 0' // center it
		                    }
		                },
		                xAxis: 
                        {
		                    categories: dates,
                            // maxZoom: 10,
		                    title: 
                            {
		                        text: 'Date'
		                    },
                       	    labels: 
        		    	    {
		        	    	    step: Math.ceil(dates.length/11.0)
			                }
		                },
		                yAxis: 
                        {
		                    title: 
                            {
		                        text: ''
		                    },
		                    plotLines: 
                            [{
		                        value: 0,
		                        width: 1,
		                        color: '#808080'
		                    }]
		                },
		                tooltip: 
                        {
		                    formatter: function() 
                            {
		                        return '<b>' + this.series.name + '</b><br/>' +
							        this.x + ': ' + this.y;
		                    }
		                },
		                legend: 
                        {
		                    layout: 'vertical',
		                    style: 
                            {
		                        left: 'auto',
		                        bottom: 'auto',
		                        right: '10px',
		                        top: '100px'
		                    }
		                },
                        <%=rsiSeries %>
		            });                            
                }
            
                function renderRocChart()
                {              
                    var dates = [<%=graphDate %>];

		            rocChart = new Highcharts.Chart({
		                chart: {
		                    renderTo: 'rocGraph',
		                    defaultSeriesType: 'line',
                            margin: [50, 150, 60, 80]
		                },
		                title: {
		                    text: '<%=lookup.Text %> Rate of Change',
		                    style: {
		                        margin: '0px 100px 0 0' // center it
		                    }
		                },
		                subtitle: {
                            text: <%=graphRange %>,
		                    style: {
		                        margin: '0 100px 0 0' // center it
		                    }
		                },
		                xAxis: {
		                    categories: [<%=graphDate %>],
                           // maxZoom: 10,
		                    title: {
		                        text: 'Date'
		                    },
                       	    labels: 
        		    	    {
		        	    	    step: Math.ceil(dates.length/11.0)
			                }
		                },
		                yAxis: {
		                    title: {
		                        text: ''
		                    },
		                    plotLines: [{
		                        value: 0,
		                        width: 1,
		                        color: '#808080'
		                    }]
		                },
		                tooltip: {
		                    formatter: function () {
		                        return '<b>' + this.series.name + '</b><br/>' +
							    this.x + ': ' + this.y;
		                    }
		                },
		                legend: {
		                    layout: 'vertical',
		                    style: {
		                        left: 'auto',
		                        bottom: 'auto',
		                        right: '10px',
		                        top: '100px'
		                    }
		                },
                        <%=rocSeries %>
		            });                     
                }

            $(document).ready(function () {   
               var dateCheck = [<%=graphDate %>]  	    
               if (dateCheck.length == 1)
               {
                    offHide('stockHide', 'stockGraph', 'graphButton');
                    offHide('macdHide', 'macdGraph', 'graphMacdButton');
                    offHide('rocHide', 'rocGraph', 'graphRocButton');
                    offHide('rsiHide', 'rsiGraph', 'graphRsiButton');
               }
               iniGraphVis('stockHide', 'stockGraph', 'graphButton');
               iniGraphVis('macdHide', 'macdGraph', 'graphMacdButton');
               iniGraphVis('rocHide', 'rocGraph', 'graphRocButton');
               iniGraphVis('rsiHide', 'rsiGraph', 'graphRsiButton');
    //           iniGraphVis('volHide', 'volumeGraph');

               $('.greenButtonClass').bind('click', function() 
               {

                   var syncState = $("#syncHide"); 

                   if ( $(this).css('background-color') == 'green' ) 
                   {
                        $(this).css('background-color', 'white');
                        $(this).css('color', 'black');
                        syncState.val = "off";
                   } 
                   else 
                   {
                       $(this).css('background-color', 'green');
                       $(this).css('color', 'white');
                       syncState.val = "on";
                   }
                });
            });

            // signalSelect is the dropdown menu buyOption or sellOption
            // opt is either sellPanelExtender or buyPanelExtender       
            function openPanel(opt)
            {
                var cpe;
                cpe = $find(opt);
                cpe.set_Collapsed(false);
            }

            function closePanel(opt)
            {
                cpe = $find(opt); 
                cpe.set_Collapsed(true);
            }

            function openPanelSelect(signalSelect, opt) 
            {
                var DropdownList = document.getElementById('MainContent_'+signalSelect);
                var SelectedIndex = DropdownList.selectedIndex;
                var SelectedValue = DropdownList.value;

                closeAllExtendedPanels(opt);
                if (SelectedValue == "Price") 
                    openPanel('MainContent_'+opt);
                else if(SelectedValue=="maAvg") 
                    openPanel('MainContent_maAvg'+opt);
                else if(SelectedValue=="macd") 
                    openPanel('MainContent_macd'+opt);
                else if(SelectedValue=="rsi") 
                    openPanel('MainContent_rsi'+opt);
                else if(SelectedValue=="roc") 
                    openPanel('MainContent_roc'+opt);
            }
           
            function closeAllExtendedPanels(opt)
            {
                closePanel('MainContent_'+opt);
                closePanel('MainContent_maAvg'+opt);
                closePanel('MainContent_macd'+opt);
                closePanel('MainContent_rsi'+opt);
                closePanel('MainContent_roc'+opt);
            }
            function turnGreen()
            {
                var customButton = document.getElementById('<%=customEnable.ClientID %>');
                if (customButton.style.backgroundColor == "") 
                    customButton.style.backgroundColor = "lime";
                else customButton.style.backgroundColor = "";
            }

            function setHideSignalSync(hidefield, hideButton)
            {
                if (hidefield.value == "on")
                    hidefield.value = "off";
                else hidefield.value = "on";   
                turnDarkGreen('signalSync');
            }

            function setSync()
            {
                var hid = document.getElementById('MainContent_syncHide');
                var customButton = document.getElementById('<%=signalSync.ClientID %>'); 
                setHideSignalSync(hid,customButton);
            }
    // ]]>
    </script>

</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="top:0px; margin-left:auto; margin-right:auto;
           width:13%; float: left; font-size: medium; text-align: center; background-color: #CCFF99;">
        <b>Money Maker
        <br />
        Features</b>
        <br />

        <asp:Button ID="signalSync" runat="server" Text="Signal Sync" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="setSync(); return false;" Font-Size="Medium" />
        <br />
        <asp:Button ID="tradeCostButton" runat="server" Text="Trade Cost" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('tradeCostButton');" Font-Size="Medium" />
        <br />

        <asp:Button ID="resultsButton" runat="server" Text="Results" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('resultsButton');" Font-Size="Medium" BackColor="Green" 
            ForeColor="White" />
        <br />

        <asp:Button ID="tradeButton" runat="server" Text="Trades" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('tradeButton');" Font-Size="Medium" BackColor="Green" ForeColor="White" />
        <br />
        <asp:Button ID="trendButton" runat="server" Text="Lets Trend" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" 
            ForeColor="Black" EnableTheming="True" Visible="False" />
        <br />
        <input id="helpButton" type="button" value="Help" 
            onclick="window.open('http://www.letstrend.com/theblog/category/Money-Maker.aspx');" 
            style="border: 2px solid #000000; width: 100px; font-size: medium;" /><br />
        <input id="Button1" style="border: 2px solid #000000; width: 100px; font-size: medium;" type="button" 
            value="Contact" onclick="window.open('http://www.letstrend.com/theblog/contact.aspx');" />
        <br />
        <br />
        <asp:HiddenField ID="stockHide" runat="server" Value="off" />
        <asp:HiddenField ID="macdHide" runat="server" Value="off" />
        <asp:HiddenField ID="rocHide" runat="server" Value="off" />
        <asp:HiddenField ID="rsiHide" runat="server" Value="off" />
        <asp:HiddenField ID="volHide" runat="server" Value="off" />

        <b>Graphs</b><br />
        <asp:Button ID="graphButton" runat="server" Text="Stock" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('graphButton'); 
                           setHide('stockHide', 'stockGraph', 'graphButton');
                           return false;" 
            Font-Size="Medium" />
        <br />
        <asp:Button ID="graphMacdButton" runat="server" Text="MACD" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            onClientClick="turnDarkGreen('graphMacdButton'); 
                           setHide('macdHide', 'macdGraph', 'graphMacdButton');
                           return false;"
            Font-Size="Medium" />
        <br />
        <asp:Button ID="graphRsiButton" runat="server" Text="RSI" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('graphRsiButton'); 
                           setHide('rsiHide','rsiGraph', 'graphRsiButton');
                           return false;" 
            Font-Size="Medium" />
        <br />
        <asp:Button ID="graphRocButton" runat="server" Text="ROC" 
            Width="100px"  BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" 
            OnClientClick="turnDarkGreen('graphRocButton'); 
                           setHide('rocHide', 'rocGraph', 'graphRocButton');
                           return false;" 
            Font-Size="Medium" />
        <br />
        <br />      
        <asp:HiddenField ID="syncHide" runat="server" Value="off" />
    </div>

    <div style="position :relative; margin-left:auto; width:80%; top: 0px;
        font-size: medium; text-align: center;">   
        <asp:Panel ID="moneySavePanel" runat="server">
            <asp:Table ID="moneySaveTable" runat="server" Height="16px" Width="100%" 
                 HorizontalAlign="Center" Caption="&lt;b&gt;Saved Tests&lt;/b&gt;" CaptionAlign="Top" 
                Font-Size="Medium" Font-Bold="False">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">Name</asp:TableCell>
                    <asp:TableCell runat="server">Ticker</asp:TableCell>
                    <asp:TableCell runat="server">Buy Option</asp:TableCell>
                    <asp:TableCell runat="server">Buy Signal</asp:TableCell>
                    <asp:TableCell runat="server">Sell Option</asp:TableCell>
                    <asp:TableCell runat="server">Sell Signal</asp:TableCell>
                    <asp:TableCell runat="server">Capital</asp:TableCell>
                    <asp:TableCell runat="server">Last Run</asp:TableCell>
                    <asp:TableCell runat="server">Notes</asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br />
            <br />
            Name
            <asp:TextBox ID="moneyName" runat="server" Font-Size="Medium"></asp:TextBox>
            <asp:Button ID="moneySave" runat="server" Text="Save" Font-Size="Medium" Width="60px" />
            <asp:Button ID="moneyLoad" runat="server" Text="Load" Font-Size="Medium" Width="60px" />
            <asp:Button ID="moneyDelete" runat="server" Text="Delete" Font-Size="Medium" Width="60px" />
            <br />
            Notes<asp:TextBox ID="note" runat="server" Width="450px" Font-Size="Medium"></asp:TextBox>
            <br />
        </asp:Panel>
        <h2><b>Ticker</b></h2>
    
        <!--- MAIN SECTION -->
        <asp:TextBox 
            ID="lookup" 
            runat="server" 
            Width="70px" Font-Size="Medium"></asp:TextBox>&nbsp;<br />

        <asp:CollapsiblePanelExtender 
                ID="tradeCostPanelExtender"
                runat="Server" 
                ExpandControlID="tradeCostButton" 
                CollapseControlID="tradeCostButton" 
                TargetControlID="tradeCostPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="tradeCostPanel" runat="server">       
            <h2><b>Trade Cost</b></h2>
            <asp:TextBox ID="tradeCostBox" runat="server" Font-Size="Medium" Width="75px"></asp:TextBox>
            <br />
            Cost Per Trade.
        </asp:Panel>

        <asp:Label 
            ID="lookupError" 
            runat="server" 
            ForeColor="Red" 
            Visible="False"
            Font-Size="Medium">
        </asp:Label>
        <h2><b style="color: #000000; font-style: normal; font-weight: bold">CAPITAL</b></h2>
        <asp:TextBox 
            ID="investValue" 
            runat="server" 
            Width="75px" Font-Size="Medium"></asp:TextBox><br />
        <asp:Label 
            ID="capitalEr" 
            runat="server" 
            ForeColor="Red" 
            Visible="False"
            Font-Size="Medium">
        </asp:Label>
        
        <!--- BUY SECTION -->
        <div>
            <h2><b style="color: #000000; font-style: normal; font-weight: bold">Buy Signal</b></h2>
        </div>
        <asp:DropDownList ID="buyOption" name="buyOption" runat="server" 
                AutoPostBack="false"  onChange="openPanelSelect('buyOption','BuyPanelExtender');" Font-Size="Medium">
            <asp:ListItem Selected="True" Value="Default">Select Signal</asp:ListItem>
            <asp:ListItem Value="Price">Price</asp:ListItem>
            <asp:ListItem Value="maAvg">Moving Average</asp:ListItem>
            <asp:ListItem Value="macd">MACD</asp:ListItem>
            <asp:ListItem Value="rsi">RSI</asp:ListItem>
            <asp:ListItem Value="roc">ROC</asp:ListItem>
        </asp:DropDownList>
        <br />

        <asp:CollapsiblePanelExtender 
                ID="BuyPanelExtender" 
                runat="Server" 
                TargetControlID="buyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>
    
        <asp:Panel 
            ID="buyPanel" 
            runat="server">
            <br />
            <asp:TextBox 
                    ID="priceBuy" 
                    runat="server" 
                    Width="50px"
                    Font-Size="Medium">
            </asp:TextBox><br /><br />          
            Examples: 
            <br />
            Single = 10
            <br />
            Range = 10-15<br />
            Less than = &lt;10<br />
            More Than = &gt;10<br />  
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="maAvgBuyPanelExtender" 
                runat="Server" 
                TargetControlID="maAvgBuyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>
        <%--onChange="buySelectTrigger();--%>
        <asp:Panel ID="maAvgBuyPanel" runat="server">  
            <br />
            <asp:TextBox ID="maBuy" runat="server" validateRequest="false" Font-Size="Medium"></asp:TextBox>
            <br />
            Examples:<br /> PRICE&gt;SMA12 = Price is Greater than SMA 12<br /> &nbsp; SMA12&gt;SMA30 = 
            SMA 12 is greater than SMA 30<br /> &nbsp;SMA12&lt;EMA30 = SMA 12 is less than EMA 30<br />
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="macdBuyPanelExtender"
                runat="Server" 
                TargetControlID="macdBuyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="macdBuyPanel" runat="server">
            <br />
            <asp:TextBox ID="macdBuy" runat="server" Font-Size="Medium"></asp:TextBox>
            <br />
            Examples:<br /> MACD greater than MACD EMA&nbsp; = MACD&gt;EMA <br /> Divergence 
            greater than 0 = Div &gt; 0
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="rsiBuyPanelExtender"
                runat="Server" 
                TargetControlID="rsiBuyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="rsiBuyPanel" runat="server">
            <br />
            <asp:TextBox ID="rsiBuy" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> RSI with 14 period less than 40&nbsp; = RSI14&lt;40 <br /> RSI with 20 
            period greater than 70 = RSI20&gt;70
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="rocBuyPanelExtender"
                runat="Server" 
                TargetControlID="rocBuyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="rocBuyPanel" runat="server">
            <br />
            <asp:TextBox ID="rocBuy" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> ROC with 14 period less than 40&nbsp; = ROC14&lt;40 <br /> ROC with 20 
            period greater than 70 = ROC20&gt;70
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="stochBuyPanelExtender"
                runat="Server" 
                TargetControlID="stochBuyPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="stochBuyPanel" runat="server" EnableTheming="True" 
            Visible="False">
            <br />
            <asp:TextBox ID="stochBuy" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> Fast Stochastic with %K Period 14 Days, 3 Day %D; use Fast %K 
            greater than 70 = FK14D3@K&gt;70<br /> Slow Stochastic with %K Priod 12 Days, 5 
            Day %D ; use %D greater than 80 = SK12D5@D&gt;80
        </asp:Panel>

        <asp:Label 
            ID="buyError" 
            runat="server" 
            ForeColor="Red" 
            Visible="False"
            Font-Size="Medium">
        </asp:Label>
    
        <!--- SELL SECTION -->
        <div>
            <h2><b style="color: #000000; font-style: normal; font-weight: bold;">Sell Signal</b></h2>
        </div>

        <asp:DropDownList 
            ID="sellOption" 
            runat="server"
            onchange="openPanelSelect('sellOption','SellPanelExtender');"
            Font-Size="Medium">
                <asp:ListItem value="Default">Select Signal</asp:ListItem>
                <asp:ListItem value="Price">Price</asp:ListItem>
                <asp:ListItem Value="maAvg">Moving Average</asp:ListItem>
                <asp:ListItem Value="macd">MACD</asp:ListItem>
                <asp:ListItem Value="rsi">RSI</asp:ListItem>
                <asp:ListItem Value="roc">ROC</asp:ListItem>
        </asp:DropDownList>

        <asp:CollapsiblePanelExtender 
                ID="SellPanelExtender" 
                runat="Server" 
                TargetControlID="sellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="sellPanel" runat="server">
            <br />
            <asp:TextBox 
                 ID="priceSell" 
                 runat="server" 
                 Width="50px"
                 Font-Size="Medium">
            </asp:TextBox><br /><br />
            Examples: 
            <br />
            Single = 10
            <br />
            Range = 10-15<br />
            Less than = &lt;10<br />
            More Than = &gt;10<br />  
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="maAvgSellPanelExtender" 
                runat="Server" 
                TargetControlID="maAvgSellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="maAvgSellPanel" runat="server">
            <br />
            <asp:TextBox ID="maSell" runat="server" Font-Size="Medium"></asp:TextBox>
            <br />
            Examples:<br /> EMA50&lt;EMA30 = EMA 50 is less than EMA 30<br /> SMA50&gt;EMA40 = EMA 
            50 is greater than EMA 40
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="macdSellPanelExtender" 
                runat="Server" 
                TargetControlID="macdSellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>  
    
        <asp:Panel ID="macdSellPanel" runat="server">
            <br />
            <asp:TextBox ID="macdSell" runat="server" Font-Size="Medium"></asp:TextBox>
            <br />
            Examples:<br /> MACD greater than MACD EMA&nbsp; = MACD&gt;EMA <br /> Divergence 
            greater than 0 = Div &gt; 0
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="rsiSellPanelExtender"
                runat="Server" 
                TargetControlID="rsiSellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="rsiSellPanel" runat="server">
            <br />
            <asp:TextBox ID="rsiSell" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> RSI with 14 less than 40&nbsp; = RSI14&lt;40 <br /> RSI with 20 greater 
            than 70 = RSI20&gt;70
        </asp:Panel>

        <asp:CollapsiblePanelExtender 
                ID="rocSellPanelExtender"
                runat="Server" 
                TargetControlID="rocSellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="rocSellPanel" runat="server">
            <br />
            <asp:TextBox ID="rocSell" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> ROC with 14 less than 40&nbsp; = ROC14&lt;40 <br /> ROC with 20 greater 
            than 70 = ROC20&gt;70
        </asp:Panel>
        <br />

        <asp:CollapsiblePanelExtender 
                ID="stochPanelExtender"
                runat="Server" 
                TargetControlID="stochSellPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="stochSellPanel" runat="server">
            <br />
            <asp:TextBox ID="TextBox2" runat="server" Font-Size="Medium" Width="90px"></asp:TextBox>
            <br />
            Examples:<br /> ROC with 14 less than 40&nbsp; = ROC14&lt;40 <br /> ROC with 20 greater 
            than 70 = ROC20&gt;70
        </asp:Panel>
        <br />

        <asp:Label 
            ID="sellError" 
            runat="server" 
            ForeColor="Red" 
            Visible="False"
            Font-Size="Medium">
        </asp:Label>

        <asp:CollapsiblePanelExtender 
                ID="priceStopPanelExtender" 
                runat="Server" 
                TargetControlID="priceStopPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:CollapsiblePanelExtender 
                ID="maAvgStopPanelExtender" 
                runat="Server" 
                TargetControlID="maAvgStopPanel"
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:DropDownList 
            ID="stopOption" 
            runat="server"
            onchange="stopSelectTrigger();"
            Font-Size="Medium" Visible="False">
                <asp:ListItem value="Default">Select Signal</asp:ListItem>
                <asp:ListItem value="Price">Price</asp:ListItem>
                <asp:ListItem Value="maAvg">Moving Average</asp:ListItem>
                <asp:ListItem Value="maAvg">MACD</asp:ListItem>
        </asp:DropDownList>

        <asp:Panel ID="priceStopPanel" runat="server">
            <br />
            <asp:TextBox
                ID="stop1" 
                runat="server" 
                Width="50px"
                Font-Size="Medium">
            </asp:TextBox><br />
            Amount of the stock price will not go below Buy Signal
            <br />
            Example = 1          
        </asp:Panel>

        <asp:Panel ID="maAvgStopPanel" runat="server">
            <br />
            <asp:TextBox ID="maStop" runat="server" Font-Size="Medium"></asp:TextBox>
            <br />
            <br />
            Examples:<br /> EMA 50 is less than EMA 30 = EMA50&lt;EMA30<br /> EMA 50 is 
            greater than EMA 40 = SMA50&gt;EMA40 
        </asp:Panel>
      
        <asp:Label 
            ID="stopError" 
            runat="server" 
            ForeColor="Red" 
            Visible="False"
            Font-Size="Medium">
        </asp:Label><br />

        <br /><br />
        <!--- BUTTONS SECTION -->
        <%--                <asp:Button ID="week" runat="server" Text="Week" BorderColor="Black" BorderStyle="Solid"
                    BorderWidth="2px" Font-Size="Medium" Width="70px"/>
    --%>
    </div>
        <div style="position :relative; margin-left:auto; width:80%; font-size: medium; text-align: center;">
            <asp:Button 
                ID="weekButton" 
                runat="server" 
                Text="Week" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button 
                ID="monthButton" 
                runat="server" 
                Text="Month" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button 
                ID="threeMonthButton" 
                runat="server" 
                Text="3 Month" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button 
                ID="sixMonthButton" 
                runat="server"
                Text="6 Month" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button 
                ID="yearButton" 
                runat="server" 
                Text="Year" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px"/>      
            <asp:Button 
                ID="customEnable" 
                runat="server" OnClientClick="turnGreen();"
                Text="Custom"  BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium" Width="70px" /> 

        <asp:CollapsiblePanelExtender 
                ID="customPanelExtender" 
                runat="Server" 
                TargetControlID="Panel2"
                ExpandControlID="customEnable" 
                CollapseControlID="customEnable" 
                Collapsed="True"
                SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel 
                ID="Panel2" 
                runat="server"
                Width="100%" 
                ForeColor="Black" HorizontalAlign="Center">
            <br />
            <asp:TextBox 
                ID="customRange" 
                runat="server" 
                style="margin-left: 2px" 
                Width="150px"
                Font-Size="Medium">
            </asp:TextBox>
            <asp:Button 
                ID="analyze"
                runat="server" 
                Text="Analyze" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium"/>
            <br />
            <asp:Label 
                ID="customEr" 
                runat="server" 
                ForeColor="Red" Font-Size="Medium"></asp:Label><br />
            Example: <br />
            10/10/09-01/06/10
        </asp:Panel> 
        <br />
        <br />


        <%-- TABLES SECTION --> --%>
        <div id="stockGraph" style="height: 400px;"></div>
        <div id="macdGraph" style="height: 400px; margin: 0 auto"></div>
        <div id="rsiGraph" style="height: 400px; margin: 0 auto"></div>
        <div id="rocGraph" style="height: 400px; margin: 0 auto"></div>

        <%-- TABLES SECTION --> --%>

        <asp:CollapsiblePanelExtender 
            ID="resultsPanelExtender" 
            runat="Server" 
            TargetControlID="resultsPanel"
            ExpandControlID="resultsButton" 
            CollapseControlID="resultsButton"
            SuppressPostBack="true"
            Collapsed="false">
        </asp:CollapsiblePanelExtender>

        <asp:CollapsiblePanelExtender 
            ID="tradePanelExtender" 
            runat="Server" 
            TargetControlID="tradePanel"
            ExpandControlID="tradeButton" 
            CollapseControlID="tradeButton" 
            Collapsed="false"
            SuppressPostBack="true">
        </asp:CollapsiblePanelExtender>

        <asp:Panel ID="resultsPanel" runat="server">
            <asp:Table 
                ID="resultsTable" 
                runat="server" 
                Caption="<b>Results</b>"            
                CaptionAlign="Top"
                width="100%" 
                HorizontalAlign="Center">
            </asp:Table>
        </asp:Panel>

        <br />
        <asp:Panel ID="tradePanel" runat="server">
            <asp:Table 
                ID="tradeList" 
                runat="server" 
                Caption="&lt;b&gt;Trade List&lt;/b&gt;" 
                Height="16px" 
                width="100%"
                HorizontalAlign="Center">
            </asp:Table>
        </asp:Panel>
    </div>
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
</asp:Content>

