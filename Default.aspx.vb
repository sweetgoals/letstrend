Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml


Partial Public Class _Default
    Inherits System.Web.UI.Page
    Public theUser As String = ""

    Dim first As Double = 5
    Dim blah As Double = 4


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            errorLabel.Visible = False
            errorLabel.Text = ""
            theUser = User.Identity.Name
            If User.Identity.Name = "" Then
                operations.Visible = False
            Else : operations.Visible = True
            End If
        Catch ex As WebException
            errorLabel.Visible = True
            errorLabel.Text = ex.Message
        End Try

    End Sub
End Class