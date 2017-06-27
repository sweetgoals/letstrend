<%@ Control Language="VB" AutoEventWireup="false" CodeFile="widgetList.ascx.vb" Inherits="wallst" %>

<asp:Panel runat="server" ID="widPanel">
<li class="widget" >       
    <div class="widgetHeadBackground" style="text-align: center">
        <h3><asp:Label ID="widgetTitle" runat="server" Font-Bold="True"></asp:Label></h3>
	</div>
    <div>
        <asp:Table ID="widgetTable" runat="server" Width="100%" forecolor="black" HorizontalAlign="Center">
        </asp:Table>
    </div>
</li>           
</asp:Panel>

