<%@ Page Title="Log In" Language="VB"  AutoEventWireup="false"
    CodeFile="Login.aspx.vb" Inherits="Account_Login" MasterPageFile="loginMaster.master" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" >
    
    <center style="font-size: medium">
    <h2>
        Log In
    </h2>
    <p>
        Please enter your username and password.<br />
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="False" 
            NavigateUrl="~/Account/Register.aspx">Register</asp:HyperLink> &nbsp;If you don't have an account.</p>
        <p>
        &nbsp;<br />
        
    </p>
    <asp:Login ID="LoginUser" runat="server" EnableViewState="false" 
            RenderOuterTable="false">
        <LayoutTemplate>
            <span class="failureNotification">
                <asp:Literal ID="FailureText" runat="server"></asp:Literal>
            </span>
            <asp:ValidationSummary ID="LoginUserValidationSummary" runat="server" CssClass="failureNotification" 
                 ValidationGroup="LoginUserValidationGroup"/>
                <fieldset style="position: relative; height:250px; width:400px; font-size: medium;">
                    <p>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" 
                            Font-Size="Medium">Username:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server" CssClass="textEntry" Font-Size="Medium"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" 
                             CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Font-Size="Medium">Password:</asp:Label>
                        <asp:TextBox ID="Password" runat="server" CssClass="passwordEntry" TextMode="Password" Font-Size="Medium"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" 
                             CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required." 
                             ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                    </p>
                    <p>
                        <asp:CheckBox ID="RememberMe" runat="server"/>
                        <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="RememberMe" CssClass="inline" Font-Size="Medium">Keep me logged in</asp:Label>
                    </p>
                    <asp:Button ID="Button1" 
                        runat="server" 
                        CommandName="Login" 
                        Text="Log In" 
                        Font-Size="Medium"
                        ValidationGroup="LoginUserValidationGroup"
                       />

                </fieldset>           
        </LayoutTemplate>
    </asp:Login>
                <asp:HiddenField ID="pageReturn" runat="server" />
    </center>
</asp:Content>