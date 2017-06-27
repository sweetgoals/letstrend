<%@ Page Title="" 
    Language="VB" 
    MasterPageFile="~/Site.master" 
    AutoEventWireup="false" 
    CodeFile="createWidget.aspx.vb" 
    Inherits="createWidget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Create Widget - Lets Trend</title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.js"></script>

    <script type="text/javascript">
        function resetClick() {
            $("#MainContent_widgetTitle").val("");
            $("#MainContent_tickers").val("");
        };
    </script>
    <style type="text/css"> 
  		.widgetHeadBackground {background-color:green}
		.widget 
		{
			border-style:solid; 
		    boder-width:1px;
		    margin: 3px 3px 3px 3px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="text-align: center">
        <h1> Create Widget Watch List </h1>
        <h2>Widget Title</h2>         
        <asp:TextBox ID="widgetTitle" runat="server" Width="120px"></asp:TextBox><br /><br /><br />
        <h2>Tickers to watch</h2>         
        <asp:TextBox ID="tickers" runat="server" Width="444px"></asp:TextBox>
        <br />
        <br />
        <br />
        <h2> Position </h2>
        <asp:DropDownList ID="Position" runat="server" Height="18px" Width="55px">
            <asp:ListItem Value="1">1</asp:ListItem>
            <asp:ListItem Value="2">2</asp:ListItem>
            <asp:ListItem Value="3">3</asp:ListItem>
            <asp:ListItem Value="4" Selected="True">4</asp:ListItem>
            <asp:ListItem Value="5">5</asp:ListItem>
            <asp:ListItem Value="6">6</asp:ListItem>
            <asp:ListItem Value="7">7</asp:ListItem>
            <asp:ListItem Value="8">8</asp:ListItem>
            <asp:ListItem Value="9">9</asp:ListItem>
        </asp:DropDownList>
        <br />
        <h2>
            <asp:Label ID="errorLabel" runat="server" Text="Already Using That Position" 
                Visible="False" Font-Bold="True"></asp:Label></h2>
        <br />
        <asp:Button ID="previewWidgetButton" runat="server" Text="Preview" />
        <asp:Button ID="resetButton"  runat="server" Text="Reset" OnClientClick="resetClick(); return false;"/><br />
        <h3>example: JKHY IBM GE NVDA</h3>
        <li class="widget" style="width: 37.7%;  margin-left: auto; margin-right: auto; ">
            <div class="widgetHeadBackground">
                <h3> 
                    <asp:Label ID="widetTitlePreview" runat="server" 
                        Text="&lt;b&gt;Title&lt;/b&gt;" Font-Bold="True"></asp:Label>
                </h3>
			</div>
            <div>
                <asp:Table ID="previewTable" runat="server" Width="100%" forecolor="black" HorizontalAlign="Center" 
                    ViewStateMode="Enabled" >
                </asp:Table>
            </div>
        </li>
        <br /><br /><br />
        <asp:Button ID="createWidgetButton" runat="server" Text="Create Widget" />
        <asp:Button ID="cancelButton" runat="server" Text="Cancel" /><br />
</div>
</asp:Content>

