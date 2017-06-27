<%@ Page Title="" Language="VB" MasterPageFile="~/Account/loginMaster.master" AutoEventWireup="false" CodeFile="broken.aspx.vb" Inherits="broken" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <center>
    <h1> BROKEN! </h1>
    <br />
    <P style="font-size: medium">Grats you broke it!!! <br />
        Ahhh another bug... <br /> 
        I'll have to fix that. <br />
        Try not to do what you did before. 
    </P>
        <P style="font-size: medium">Here&#39;s what happend:</P>
        <P style="font-size: medium">
            <asp:Label ID="errorLabel" runat="server" Text=""></asp:Label>
            <br />   
    
    <br />
    Can you tell me what you were doing? <br />
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.letstrend.com/theblog/contact.aspx">Contact</asp:HyperLink>
    </P>
    <br />

    <h1> BROKEN! </h1>
    </center>
</asp:Content>

