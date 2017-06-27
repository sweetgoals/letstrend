Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class deleteWidget
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If User.Identity.Name = "" Then
            Response.Redirect("~/info/loginrequired.aspx")
        Else : findWidgets()
        End If
    End Sub

    Sub findWidgets()
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader

        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT position FROM bobpar.marketWidgets WHERE (username = '" & User.Identity.Name & "')"
        myreader = mycom.ExecuteReader
        While myreader.Read()
            widgetSelect.Items.Add(myreader("position"))
        End While
        con.Close()
    End Sub

    Protected Sub openWidgetButton_Click(sender As Object, e As System.EventArgs) Handles openWidgetButton.Click
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader      
        Dim widgetData As String = ""
        Dim theWidget As New widgets

        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT widgetTitle, widgetData FROM bobpar.marketWidgets " & _
                            "WHERE (username = '" & User.Identity.Name & "') AND (position ='" & widgetSelect.SelectedValue & "')"
        myreader = mycom.ExecuteReader
        While myreader.Read()
            widetTitlePreview.Text = myreader("widgetTitle")
            widgetData = myreader("widgetData")
        End While
        con.Close()

        If widgetData <> "" Then
            theWidget.loadTable(previewTable, widgetData.Split(" "))
            widgetPanel.Visible = True
        End If
    End Sub

    Protected Sub deleteWidgetButton_Click(sender As Object, e As System.EventArgs) Handles deleteWidgetButton.Click
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim finished As Integer = 0
        con.Open()
        mycom.Connection = con
        mycom.CommandText = "DELETE FROM bobpar.marketWidgets WHERE (username = '" + User.Identity.Name + _
                            "') AND (position = '" + widgetSelect.SelectedValue + "')"
        finished = mycom.ExecuteNonQuery()
        Response.Redirect("~/Default.aspx")
    End Sub

    Protected Sub cancelButton_Click(sender As Object, e As System.EventArgs) Handles cancelButton.Click
        Response.Redirect("~/Default.aspx")
    End Sub
End Class
