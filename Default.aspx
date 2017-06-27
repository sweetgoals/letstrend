<%@ Page Title="Market Summary - Lets Trend"
    Language="VB" 
    AutoEventWireup="false" 
    MasterPageFile="~/Site.Master" 
    CodeFile="Default.aspx.vb" 
    Inherits="_Default"
    ResponseEncoding="ISO-8859-9" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <title>Market Summary - Lets Trend</title>
    <meta name="Title" content="Market Summary - Lets Trend"/>
    <meta name="description" content="Gives a status on how the Stock Indices are doing. Lists Gains for all Dow Industrial 
                and Dow Transportation Stocks." />
    <meta name="keywords" content="stock,technical, analysis, technical analysis, Dow Industrial, Dow Transportation, 
                Indices Gains day, week, 3 month, 6 months and one year." />
    <meta http-equiv="Content-Type" content="text/html;charset=ISO-8859-1" />
    <meta name="author" content="Jakkjakk Par" />
    <meta name="RATING" content="General"/>
    <meta name="ROBOTS" content="index,follow"/>
    <meta name="REVISIT-AFTER" content="4 weeks"/>
    <meta name="GENERATOR" content="Mozilla/4.76 [en] (Win98; U) [Netscape]"/>
	<meta http-equiv="content-type" content="text/html; charset=ISO-8859-1"/>

	<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
	<script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.17/jquery-ui.min.js" type="text/javascript"></script>
    <script>
		  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
		  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
		  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
		  })(window,document,'script','https://www.google-analytics.com/analytics.js','ga');

		  ga('create', 'UA-63033431-2', 'auto');
		  ga('send', 'pageview');

    </script>
	<script language="javascript" type="text/javascript">
	    $(document).ready(function () {

	        function loadWidget(pos, username) {
	            $.ajax({
	                type: "POST",
	                url: "widgetListWebService.asmx/widgetListCode",
	                data: "{'pos': " + pos + ", 'userName': '" + username + "' }",
	                contentType: "application/json; charset=utf-8",
	                dataType: "json",
	                success: function (msg) {
	                    $('#widget' + pos).html(msg.d);
	                    $('#loading' + pos).hide();
	                }
	            });
	        };

            for (i=1;i<10;i++)
                loadWidget(i, "<%=theUser %>");

	        $("#column1, #column2, #column3").sortable({
	            connectWith: ".column"
	        }).disableSelection();

	        $('.top').click(function () {
	            $(this).parent().toggle();
	            $(this).parent().next().toggle();
	        });

	        $('.hiding').click(function () {
	            var inputText = $(this).prev();
	            $(this).parent().toggle();
	            $(this).parent().prev().find('span').replaceWith("<span><b>" + $(inputText).val() + "</b></span>")
	            $(this).parent().prev().toggle();
	            return false;
	        });
	    });
    </script>

	<style type="text/css"> 
		body,img,p,h1,h3,h4,h5,h6,ul,ol {margin:0; padding:0; list-style:none; border:none;}

		#columns .column 
		{
			float: left;
			width: 32.7%;
			min-height: 200px;
			height: inherit !important; 
			height: 200px;	
			margin: 2px 2px 2px 2px;
			
		}
		.widgetHeadBackground {background-color:green}
		.widget 
		{
			border-style: solid; 
		    boder-width: 1px;
		    margin: 3px 3px 3px 3px;
        }        
	</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:panel id="operations" runat="server">
        <asp:Button ID="createWidget" runat="server" Text="Create Widget" Width="120px" BorderColor="Black" BorderStyle="Solid" 
            BorderWidth="2px" Font-Size="Medium" PostBackUrl="~/widgets/createWidget.aspx" />  
        <asp:Button ID="deleteWidget" runat="server" Text="Delete Widget" Width="120px" BorderColor="Black" BorderStyle="Solid" 
            BorderWidth="2px" Font-Size="Medium" PostBackUrl="~/widgets/deleteWidget.aspx" />  
        <asp:Button ID="moveWidget0" runat="server" Text="Move Widget" Width="120px" BorderColor="Black" BorderStyle="Solid" 
            BorderWidth="2px" Font-Size="Medium" PostBackUrl="~/widgets/moveWidget.aspx" />  
    </asp:panel>

    <div id="columns">
        <ul id="column1" class="column">
            <li id="widget1">
                <div id="loading1"><img alt="" src="images/progress-indicator.gif" /></div>                
            </li>
            <li id="widget4">
                <div id="loading4"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
            <li id="widget7">
                <div id="loading7"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
        </ul>
        <ul id="column2" class="column">
            <li id="widget2">
                <div id="loading2"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
            <li id="widget5">
                <div id="loading5"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
            <li id="widget8">    
                <div id="loading8"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
        </ul>
        <ul id="column3" class="column">
            <li id="widget3">
                <div id="loading3"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
            <li id="widget6">
                <div id="loading6"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
            <li id="widget9">
                <div id="loading9"><img alt="" src="images/progress-indicator.gif" /></div>      
            </li>
        </ul>      
    </div>
    <asp:Label ID="errorLabel" runat="server" Text="" Visible="False"></asp:Label>
  </asp:Content>