<%@ Page Title="" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="loginrequired.aspx.vb" Inherits="loginrequired" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">   
        <h1 style="text-align: center"> LOGIN REQUIRED </h1>
        <br />
        <br />
        <p style="font-size: medium; text-align: center">To use this program you must have an account and be logged in.</p>

        <p style="font-size: medium; text-align: center"> 
            <asp:HyperLink ID="HyperLink1" runat="server" 
                NavigateUrl="~/Account/Login.aspx">Login</asp:HyperLink>
        </p>
</asp:Content>

