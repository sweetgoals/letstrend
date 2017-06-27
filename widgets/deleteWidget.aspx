<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="deleteWidget.aspx.vb" Inherits="deleteWidget" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <title>Delete Widget - Lets Trend</title>
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
        <h1> DELETE WIDGET </h1>

        <h2> Widget Select </h2>

        <asp:DropDownList ID="widgetSelect" runat="server">
            <asp:ListItem>Select Widget</asp:ListItem>
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
        </asp:Panel><br />
        <asp:Button ID="deleteWidgetButton" runat="server" Text="Delete Widget" />
        <asp:Button ID="cancelButton" runat="server" Text="Cancel" />

    </div>


</asp:Content>

