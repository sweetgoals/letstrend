
Partial Class Account_Register
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            RegisterUser.ContinueDestinationPageUrl = Request.UrlReferrer.ToString
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub RegisterUser_CreatedUser(ByVal sender As Object, ByVal e As EventArgs) Handles RegisterUser.CreatedUser
        FormsAuthentication.SetAuthCookie(RegisterUser.UserName, False)

        Dim continueUrl As String = RegisterUser.ContinueDestinationPageUrl
	
        sendVerificationEmail()
        If String.IsNullOrEmpty(continueUrl) Then
            continueUrl = "~/Default.aspx"
        End If

        Response.Redirect("~/Default.aspx")
    End Sub
    Sub sendVerificationEmail()
        Dim newUserMembership = Membership.GetUser(RegisterUser.UserName)
        Dim userGuid As New Guid
        Dim MailMsg As New MailMessage
        Dim currentUser As User = Nothing
        Dim currentMember As Membershipdb = Nothing
        Dim goalSupportForm As _
            New StreamReader(System.Web.HttpContext.Current.Server.MapPath("/emailTemplates/accountActivation.html"))

        userGuid = DirectCast(newUserMembership.ProviderUserKey, Guid)
        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress(RegisterUser.Email))
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True
        MailMsg.Subject = "Letstrend - Account Activation"
        MailMsg.Body = goalSupportForm.ReadToEnd()
        MailMsg.Body = MailMsg.Body.Replace("[verifyAccountNumber]", userGuid.ToString)
        MailMsg.Body = MailMsg.Body.Replace("[supporterEmailAddress]", RegisterUser.Email)
        setMailServer(MailMsg)
        sendMeAnEmail(RegisterUser.UserName)

    End Sub

    Sub sendMeAnEmail(ByVal username As String)
        Dim MailMsg As New MailMessage

        MailMsg = New MailMessage(New MailAddress("dreamchaser@sweetgoals.com"), New MailAddress("apchampagne@gmail.com"))
        MailMsg.Priority = MailPriority.Normal
        MailMsg.IsBodyHtml = True
        MailMsg.Subject = "Sweet Goals - Account Activation"
        MailMsg.Body = username & " created an account on sweetgoals.com"
        setMailServer(MailMsg)
    End Sub

End Class
