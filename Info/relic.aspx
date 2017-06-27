<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="relic.aspx.vb" Inherits="Info_relic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div style="position :relative; margin-left:auto; margin-right:auto; width:100%; font-size: medium; text-align: center;">
        <h2 style="font-weight: bold;">Ticker</h2>
        <asp:TextBox ID="TextBox1" runat="server" Width="70px" Font-Size="Medium">^DJI</asp:TextBox>&nbsp;

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
            <asp:Button ID="customButton" runat="server" Text="Custom Period" Width="117px" Font-Size="Medium"
                BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" OnClientClick="turnDarkGreen(MainContent_customButton);"/>
            <br />

            <%--- CUSTOM PERIOD ---%>
            <asp:Panel ID="customPanel" runat="server">
                <div><h4 style="color: #000000; font-size: medium;">Enter Custom Range</h4></div>
                <asp:TextBox ID="customRange" runat="server" Width="150px" Font-Size="Medium">01/26/09-12/10/10</asp:TextBox>
                <asp:Button ID="analyze" runat="server" Text="Analyze" BorderColor="Black" 
                    BorderStyle="Solid" BorderWidth="2px" Font-Size="Medium"/>
                <br />
                Example:
                <br />
                10/10/09-01/06/10<br /> <br />
             </asp:Panel>
             <br />
        
        <%--- SUPPORT & RESISTANCE  ---%>
        <asp:Table ID="supports" runat="server" Caption="Support" Font-Bold="true"
            ForeColor="Green" width="100%" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow1" runat="server" Font-Bold="false" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell1" runat="server">Level </asp:TableCell>
                <asp:TableCell ID="TableCell2" runat="server">Value</asp:TableCell>
                <asp:TableCell ID="TableCell3" runat="server">%Gain From Current Price</asp:TableCell>
                <asp:TableCell ID="TableCell4" runat="server">Confirmed</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow2" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell5" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell6" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell7" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell8" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow3" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell9" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell10" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell11" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell12" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow4" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell13" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell14" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell15" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell16" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow5" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell17" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell18" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell19" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell20" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow6" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell21" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell22" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell23" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell24" runat="server"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <asp:Table ID="Resistance" runat="server" Caption="Resistance" Font-Bold="True" 
            ForeColor="Red" width="100%" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow7" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell25" runat="server">Level</asp:TableCell>
                <asp:TableCell ID="TableCell26" runat="server">Value</asp:TableCell>
                <asp:TableCell ID="TableCell27" runat="server">%Gain From Current price</asp:TableCell>
                <asp:TableCell ID="TableCell28" runat="server">Confirmed</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow8" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell29" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell30" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell31" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell32" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow9" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell33" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell34" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell35" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell36" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow10" runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell ID="TableCell37" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell38" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell39" runat="server"></asp:TableCell>
                <asp:TableCell ID="TableCell40" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow11" runat="server" HorizontalAlign="Center" Font-Bold="false"> 
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false">
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
                <asp:TableCell runat="server"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        Support / Resistance Range<br />
        <asp:TextBox ID="TextBox4" runat="server" Width="60px" Font-Size="Medium"></asp:TextBox>
        <br />
        (price * 0.005)<br />
        <br />
        <br/>

        <%--- HIGH&LOW  ---%>
        <asp:Table ID="range_table" runat="server" Caption="High - Low" Font-Bold="True"
            Height="116px" width="100%" ForeColor="Black" HorizontalAlign="Center">
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false" ForeColor="Black">
                <asp:TableCell runat="server" HorizontalAlign="Center">Description</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center" >Date</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Value</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Price</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center">Price Gain</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"> % Gain</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false" ForeColor="Black">
                <asp:TableCell runat="server" HorizontalAlign="Center">Low for Period</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false" ForeColor="Black">
                <asp:TableCell runat="server" HorizontalAlign="Center">High For Period</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false" ForeColor="Black">
                <asp:TableCell runat="server" HorizontalAlign="Center">Latest 4wk Low</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow runat="server" HorizontalAlign="Center" Font-Bold="false" ForeColor="Black">
                <asp:TableCell runat="server" HorizontalAlign="Center">Latest 4wk High</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br/>
            <%--- ADVANCES & DECLINE  ---%>
        <asp:Table ID="advanceDeclineTable" runat="server" Caption="Advances & Declines" Font-Bold="True" ForeColor="Black" 
            width="100%" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow12" runat="server" Font-Bold="false" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell41" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell42" runat="server" HorizontalAlign="Center">Mondays</asp:TableCell>
                <asp:TableCell ID="TableCell43" runat="server" HorizontalAlign="Center">Tuesdays</asp:TableCell>
                <asp:TableCell ID="TableCell44" runat="server" HorizontalAlign="Center">Wednesdays</asp:TableCell>
                <asp:TableCell ID="TableCell45" runat="server" HorizontalAlign="Center">Thursdays</asp:TableCell>
                <asp:TableCell ID="TableCell46" runat="server" HorizontalAlign="Center">Fridays</asp:TableCell>
                <asp:TableCell ID="TableCell47" runat="server" HorizontalAlign="Center">Overall</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow13" runat="server" Font-Bold="false" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell48" runat="server" HorizontalAlign="Center">Advances</asp:TableCell>
                <asp:TableCell ID="TableCell49" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell50" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell51" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell52" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell53" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell54" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow14" runat="server" Font-Bold="false" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell55" runat="server" HorizontalAlign="Center">Declines</asp:TableCell>
                <asp:TableCell ID="TableCell56" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell57" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell58" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell59" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell60" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell61" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow15" runat="server" Font-Bold="false" HorizontalAlign="Center">
                <asp:TableCell ID="TableCell62" runat="server" HorizontalAlign="Center">Totals</asp:TableCell>
                <asp:TableCell ID="TableCell63" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell64" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell65" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell66" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell67" runat="server" HorizontalAlign="Center"></asp:TableCell>
                <asp:TableCell ID="TableCell68" runat="server"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Table ID="advanceDeclineStatsTable" runat="server" Font-Bold="false" ForeColor="Black" 
            Width="100%" HorizontalAlign="Center">
            <asp:TableRow ID="TableRow16" runat="server">
                <asp:TableCell ID="TableCell69" runat="server">Longest Advance</asp:TableCell>
                <asp:TableCell ID="TableCell70" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow17" runat="server">
                <asp:TableCell ID="TableCell71" runat="server">Longest Decline</asp:TableCell>
                <asp:TableCell ID="TableCell72" runat="server"></asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="TableRow18" runat="server">
                <asp:TableCell ID="TableCell73" runat="server">Current Direction</asp:TableCell>
                <asp:TableCell ID="TableCell74" runat="server"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>
            <asp:Label ID="errorLabel" runat="server" Text="Label"></asp:Label>
        <br /><br />
    </div>
</asp:Content>

