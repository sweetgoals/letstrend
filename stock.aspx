<%@ Page Title="Stock Analysis - Lets Trend" 
         Language="VB" 
         MasterPageFile="~/Site.Master" 
         AutoEventWireup="false" 
         CodeFile="stock.aspx.vb" 
         Inherits="_Default" 
         ResponseEncoding="ISO-8859-9"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <title>Stock Analysis - Lets Trend</title>

    <meta name="Title" content="Stock Analysis - Lets Trend"/>
    <meta name="description" content="Graph any stock by ticker symbol. Charts for Week, Month, 3 month, 6 month, 
                                year and custom periods. Use cool indicators to see how stock is doing. 
                                Compare multiple years at once" />
    <meta name="keywords" content="stock,technical, analysis, technical analysis, Stock Chart, Simple Moving Averages, 
                        High & Low, DonChain Investing Strategy, 4 week High and low, Calculated Support 
                        and Resistance levels, Stock Trends, Technical Analysis" />
    <meta name="author" content="Jakkjakk Par" />  
    <meta name="RATING" content="General" />
    <meta name="ROBOTS" content="index,follow"/>
    <meta name="REVISIT-AFTER" content="4 weeks"/>
    <meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-9" /> 

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
	<script type="text/javascript">

        var stockChart;
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
		            text: '<%=ticker.Text %> Price',
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
                <%=stockDataString %>
		    });
        }

		$(document).ready(function () 
        {
            renderStockChart();
        });

        function turnDarkGreenLocal(idButton, divSelect) 
        {
            var customButton = document.getElementById('MainContent_' + idButton);
            var divSelected = $("#" + divSelect);

            if (customButton.style.backgroundColor == "") 
            {
                customButton.style.backgroundColor = "green";
                customButton.style.color = "white";       
                divSelected.show();
            }
            else 
            {
                customButton.style.backgroundColor = "";
                customButton.style.color = "black";
                divSelected.hide();
            }
            return false;
        };

        function openStockGraphs()
        {
            var url = "chart.aspx?ticker=<%=ticker.text %>";
            var win = window.open(url);           
            return false;  
        } 
    </script>      
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">  
    <div style="top:0px; width:13%; float: left; font-size: medium; text-align: center; background-color: #CCFF99;">
        &nbsp;<b>Stock 
        <br />
        Features</b>
        <br />
        <asp:Button ID="graphButton" runat="server" Text="Graphs" Width="100px" 
            onClientClick="openStockGraphs(); return false;" BorderColor="Black" 
            BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium"/>
        <br />
        <input id="helpButton" type="button" value="Help"          
            onclick="window.open('http://www.letstrend.com/theblog/category/Stock.aspx');" 
            style="border: 2px solid #000000; width: 100px; font-size: medium;" /><br />
        <input id="Button1" 
            style="border: 2px solid #000000; width: 100px; font-size: medium;" 
            type="button" value="Contact"
            onclick="window.open('http://www.letstrend.com/theblog/contact.aspx');" />
        <br />
        <br />      
    </div>

    <div style="position :relative; margin-left:auto; width:85%; top: 0px;
        font-size: medium; text-align: center;" >   
        <%-- SUMMARY TABLE--%>
        <br />
        <asp:Table ID="quoteTable" runat="server" Height="38px" Width="95%" Font-Bold="true"
            ForeColor="Black" Caption="Latest Info" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow1" runat="server" Font-Bold="false">
                <asp:TableCell runat="server" HorizontalAlign="Center">Name</asp:TableCell>
                <asp:TableCell ID="TableCell1" runat="server" HorizontalAlign="Center">Last Trade</asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server" HorizontalAlign="Center">Prev Close</asp:TableCell>
                <asp:TableCell ID="TableCell3" runat="server" HorizontalAlign="Center">Change</asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server" HorizontalAlign="Center">% Change</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Volume</asp:TableCell>
                <asp:TableCell ID="TableCell5" runat="server" HorizontalAlign="Center">Period % Gain</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell7" runat="server" ForeColor="Green" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server" ForeColor="Green" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell9" runat="server" ForeColor="Green" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server" ForeColor="Green" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell11" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell12" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell13" runat="server" HorizontalAlign="Center"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Panel runat="server" DefaultButton="month">
            <h2 style="font-weight: bold;">Ticker</h2>
            <asp:TextBox ID="ticker" runat="server" Width="70px" Font-Size="Medium">^DJI</asp:TextBox>&nbsp;
        </asp:Panel>    
        <div id="stockGraph" 
            style="position :relative; margin-left:auto; margin-right:auto; width:100%; font-size: medium; text-align: center;">
        </div>  

        <%-- INPUT BUTTONS --%>
        <div style="position :relative; margin-left:auto; margin-right:auto; width:100%; font-size: medium; text-align: center;">
            <asp:Button ID="week" runat="server" Text="Week" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button ID="month" runat="server" Text="Month" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px" />
            <asp:Button ID="month3" runat="server" Text="3 Month" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button ID="month6" runat="server" Text="6 Month" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <asp:Button ID="year1" runat="server" Text="1 Year" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px"/>
            <br />
            <asp:Label ID="errorLabel" runat="server" Visible="False" ForeColor="Red" Font-Size="Medium"></asp:Label>  
        </div>
        <div style="position :relative; margin-left:auto; margin-right:auto; width:100%; font-size: medium; text-align: center;">
            <%--- STATISTICS  ---%>
            <div id="statsDiv">
                <br />
                <asp:Label ID="fundLabel" runat="server" Text="&lt;b&gt;Fundamentals&lt;/b&gt;"></asp:Label><br />
                <asp:HyperLink ID="fundGuideButton" runat="server" text="Guidelines" NavigateUrl="~/Info/guidelines.aspx" 
                Target="_blank"/>
                <asp:Table ID="funds" runat="server" Font-Bold="true" 
                    Width="100%" ForeColor="Black" HorizontalAlign="Center">
                </asp:Table>
            </div>
        </div>
        <asp:HyperLink ID="relicLink" runat="server" NavigateUrl="~/Info/relic.aspx" 
            Target="_blank">Recreational Information</asp:HyperLink>
    </div>
    <br />
    <br />
    <br />
</asp:Content>
