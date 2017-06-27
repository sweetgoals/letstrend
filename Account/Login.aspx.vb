
Partial Class Account_Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.UrlReferrer.ToString.Contains("money") = True Then
                pageReturn.Value = "~/money.aspx"
            ElseIf Request.UrlReferrer.ToString.Contains("Portfolio") = True Then
                pageReturn.Value = "~/Portfolio.aspx"
            ElseIf Request.UrlReferrer.ToString.Contains("Default") = True Then
                pageReturn.Value = "~/Default.aspx"
            ElseIf Request.UrlReferrer.ToString.Contains("wallstate") = True Then
                pageReturn.Value = "~/wallstate.aspx"
            End If
        Catch ex As Exception
            pageReturn.Value = "~/Default.aspx"
        End Try

        '  LoginUser.DestinationPageUrl = pageReturn.Value
    End Sub

    'Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    '    Login.ContinueDestinationPageUrl = Request.QueryString("ReturnUrl")
    'End Sub


    Protected Sub LoginUser_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoginUser.LoggedIn
        If pageReturn.Value = "" Then
            pageReturn.Value = "~/Default.aspx"
        Else
            Response.Redirect(pageReturn.Value)
        End If
    End Sub
End Class