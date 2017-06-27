<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="moveWidget.aspx.vb" Inherits="moveWidget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Move Widget - Lets Trend</title>
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
        <h1> MOVE WIDGET </h1>

        <h2> Widget Select </h2>

        <asp:DropDownList ID="oldPosition" runat="server">
        </asp:DropDownList>
        <br />
        <asp:Button ID="openWidgetButton" runat="server" Text="Open Widget" /><br />

        <asp:Panel ID="widgetPanel" runat="server" Visible="False"> 
            <li class="widget" style="width: 37.7%;  margin-left: auto; margin-right: auto; ">
                <div class="widgetHeadBackground">
                    <h3> 
                        <asp:Label ID="widetTitlePreview" runat="server" 
                            Text="" Font-Bold="True"></asp:Label>
                    </h3>
			    </div>
                <div>
                    <asp:Table ID="previewTable" runat="server" Width="100%" forecolor="black" HorizontalAlign="Center" 
                        ViewStateMode="Enabled" >
                    </asp:Table>
                </div>
            </li>
        </asp:Panel>
        <br />
        <h2> New Position </h2>
        <asp:DropDownList ID="newPosition" runat="server">
            <asp:ListItem>1</asp:ListItem>
            <asp:ListItem>2</asp:ListItem>
            <asp:ListItem>3</asp:ListItem>
            <asp:ListItem>4</asp:ListItem>
            <asp:ListItem>5</asp:ListItem>
            <asp:ListItem>6</asp:ListItem>
            <asp:ListItem>7</asp:ListItem>
            <asp:ListItem>8</asp:ListItem>
            <asp:ListItem>9</asp:ListItem>
        </asp:DropDownList>

        <br />
        <asp:Button ID="moveWidgetButton" runat="server" Text="Move Widget" />
        <asp:Button ID="cancelButton" runat="server" Text="Cancel" />

    </div>


</asp:Content>

