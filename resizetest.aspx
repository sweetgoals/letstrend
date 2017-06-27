<%@ Page Language="VB" AutoEventWireup="false" CodeFile="resizetest.aspx.vb" Inherits="resizetest" %>

<HTML>
<HEAD>
<TITLE>jQuery Resizable and Draggable Demo - ViralPatel.net</TITLE>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.js"></script>
	<script type="text/javascript" src="highcharts/highcharts.js"></script>          
    <script type="text/javascript" src="Scripts/controls.js"></script>  
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
        function renderStockChart()
        {
            var dates = [<%=graphDate %>];
            stockChart = new Highcharts.Chart(
            {
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
                <%=series_string %>
		    });
        }

        $(document).ready(function () 
        {      
            var stockChart;
            renderStockChart();

            $("#gainButton").click(function () {
                var buyInput = $("#buyValue").val();
                var sellInput = $("#sellValue").val();
                var cashOut = $("#cashGain");
                var percentOut = $("#percentGain");
                var cashGainResult = 0;
                var percentGainResult = 0;
                cashGainResult = sellInput - buyInput;
                cashGainResult = cashGainResult.toFixed(2);
                cashOut.text(cashGainResult + "$");
                percentGainResult = ((sellInput - buyInput) / buyInput) * 100;
                percentGainResult = percentGainResult.toFixed(2);
                percentOut.text(percentGainResult + "%"); 
            });
        });
    </script>
    <script type="text/javascript">
        $(function () {
            $('.demo').resizable({
                resize: function () {
                    stockChart.setSize(
                        this.offsetWidth - 20,
                        this.offsetHeight - 20,
                        false
                );
                }
            });
        });

        function customButtonClicky() {
            $("#customRangeDiv").slideToggle("slow");
        };

        function chartButtonClicky() {
            $("#chartMenu").slideToggle("slow");
        };
    </script>

    <STYLE>
        .cssRows
		    {
			    float: left;
			    margin: 5px 5px 5px 5px;
			    border-style:solid;
			    border-width: 2px;
		    }
    
        .demo {   
	        border-Style:solid;
	        border-width: 2px;
        }
    </STYLE>

</HEAD>
<BODY>
    <form id="form1" runat="server">
        <div>
            <img src="images/letstrend%20logo.png"
                style="display:block; margin-right: auto; margin-left: auto;" alt=""/>      
        </div>
        <div style="width: 100%; 
                background-color: #cf9; 
                height:30px;           
                margin-right: auto; 
                text-align: center;
                font-size:medium;
                margin-left: auto;">       
            <asp:Button ID="chartMenuButton" runat="server" Text="Chart Menu" Width="117px" 
                BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" OnClientClick="chartButtonClicky(); return false;"
                Font-Size="Medium"/>
            Ticker<asp:TextBox ID="ticker" runat="server" Width="48px"></asp:TextBox>
            <asp:Button ID="day" runat="server" Text="Day" BorderColor="Black" BorderStyle="Solid"
                BorderWidth="2px" Font-Size="Medium" Width="70px" Visible="False"/>
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
            <asp:Button ID="customButton" runat="server" Text="Custom Period" Width="117px" 
                BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" OnClientClick="customButtonClicky(); return false;" 
                Font-Size="Medium"/>
            &nbsp;<span style="font-size: large; font-weight: bold">B </span>
            <input id="buyValue" type="text" 
                style="font-size: medium; width: 60px;"/>
            <span style="font-size: large; font-weight: bold">S </span>
            <input id="sellValue" type="text"               
                style="font-size: medium; width: 60px;"/>
            <input id="gainButton" type="button" value="G" 
                style="font-size: medium; text-align:center; width: 40px;"/>
            <asp:Label ID="cashGain" runat="server" Text=""
                style="font-size: medium; width: 30px;"></asp:Label>
            <asp:Label ID="percentGain" runat="server" Text=""
                style="font-size: medium; width: 30px;"></asp:Label>
        </div>
        <div id="chartMenu" style="display:none; margin-right: auto; margin-left: auto;">
            <ul style="width: 99%;">
                <div id="multiYear" class="cssRows">
                    <h4 style="color: #000000;">Multi Year</h4>
                    <asp:CheckBox ID="multiEnable" runat="server" Text="Enable" /><br />
                    <asp:TextBox ID="my1" runat="server" Width="58px" BackColor="White" Font-Size="Medium">2009</asp:TextBox>
                    <asp:TextBox ID="my2" runat="server" Width="58px" BackColor="White" Font-Size="Medium">2005</asp:TextBox>
                    <asp:TextBox ID="my3" runat="server" Width="58px" BackColor="White" Font-Size="Medium">2000</asp:TextBox>           
                </div>
                <div id="sma" class="cssRows">
                    <h4>Simple Moving Averages</h4>
                    <asp:CheckBox ID="smaEnable" runat="server" Text="Enable" /><br />
                    <asp:TextBox ID="sma1" runat="server" Width="58px" BackColor="White" Font-Size="Medium">15</asp:TextBox>
                    <asp:TextBox ID="sma2" runat="server" Width="58px" Font-Size="Medium">50</asp:TextBox>
                    <asp:TextBox ID="sma3" runat="server" Width="58px" Font-Size="Medium"></asp:TextBox>           
                </div>
                <div id="ema" class="cssRows">
                    <h4>Exponentail Moving Averages</h4>
                    <asp:CheckBox ID="emaEnable" runat="server" Text="Enable" /><br />
                    <asp:TextBox ID="ema1" runat="server" Width="58px" BackColor="White" Font-Size="Medium">15</asp:TextBox>
                    <asp:TextBox ID="ema2" runat="server" Width="58px" Font-Size="Medium">50</asp:TextBox>
                    <asp:TextBox ID="ema3" runat="server" Width="58px" Font-Size="Medium">200</asp:TextBox>
                </div>          
            </ul>
            <ul style="width: 99%;">
                <div id="rsi" class="cssRows">
                    <h4>RSI Period</h4>
                    <asp:CheckBox ID="rsiEnable" runat="server" Text="Enable" /><br />
                    <asp:TextBox ID="rsiBox" runat="server" Width="50px" BackColor="White" Font-Size="Medium">14</asp:TextBox>
                </div>
                <div class="cssRows">
                    <h4>ROC Period</h4>
                    <asp:CheckBox ID="rocEnable" runat="server" Text="Enable" /><br />
                    <asp:TextBox ID="rocBox" runat="server" Width="50px" BackColor="White" Font-Size="Medium">12</asp:TextBox>           
                </div>
                <div class="cssRows">
                    <h4>Stochastic Period</h4>
                    <asp:CheckBox ID="stochEnable" runat="server" Text="Enable" /><br />
                    %K
                    <asp:TextBox ID="kStoch" runat="server" BackColor="White" Font-Size="Medium" 
                        Width="50px">12</asp:TextBox>
                    &nbsp;%D
                    <asp:TextBox ID="dStoch" runat="server" BackColor="White" Font-Size="Medium" 
                        Width="50px">3</asp:TextBox>
                    &nbsp;
                    <br />
                    <asp:CheckBox ID="slowStoch" runat="server" Text="Slow" Width="100px" />
                    <asp:CheckBox ID="fastStoch" runat="server" Text="Fast" Width="100px" 
                        Checked="True" />           
                </div>           
            </ul>
        </div>

        <div id="customRangeDiv" style="display:none; float: right;">
                <div><h4 style="color: #000000; font-size: medium;">Enter Custom Range</h4></div>
                <asp:TextBox ID="customRange" runat="server" Width="150px" Font-Size="Medium">01/26/09-12/10/10</asp:TextBox>
                <asp:Button ID="analyze" runat="server" Text="Analyze" BorderColor="Black" 
                    BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium"/>
                <br />
                Example:
                <br />
                10/10/09-01/06/10<br /> <br />
        </div>
        <div>
            <asp:Label ID="errorLabel" runat="server" Visible="False"></asp:Label>
        </div>
        <%--- GRAPH SECTIONS ---%>
        <div id="stockGraph" class="demo" style="width:98%; height:100px;">
        </div>  
    </form>



</BODY>
</HTML>