Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class moveWidget
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If User.Identity.Name = "" Then
            Response.Redirect("~/loginrequired.aspx")
        Else : findWidgets()
        End If

    End Sub

    Sub findWidgets()
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim checkValue As String = ""

        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT position FROM bobpar.marketWidgets WHERE (username = '" & User.Identity.Name & "')"
        myreader = mycom.ExecuteReader
        oldPosition.Items.Clear()
        While myreader.Read()
            oldPosition.Items.Add(myreader("position"))
        End While
        con.Close()
    End Sub

    Protected Sub openWidgetButton_Click(sender As Object, e As System.EventArgs) Handles openWidgetButton.Click
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim widgetData As String = ""
        Dim theWidget As New widgets

        Try
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT widgetTitle, widgetData FROM bobpar.marketWidgets " & _
                                "WHERE (username = '" & User.Identity.Name & "') AND (position ='" & oldPosition.SelectedValue & "')"
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
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub moveWidgetButton_Click(sender As Object, e As System.EventArgs) Handles moveWidgetButton.Click
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim finished As Integer = 0
        Dim myreader As SqlDataReader
        Dim dataStrings As New List(Of String)
        con.Open()
        mycom.Connection = con

        mycom.CommandText = "SELECT widgetTitle, position FROM bobpar.marketWidgets " & _
                            "WHERE (username = '" & User.Identity.Name & "') AND (position = '" & oldPosition.SelectedValue & "')" & _
                            "OR(position = '" & newPosition.SelectedValue & "')"
        myreader = mycom.ExecuteReader
        'get values that are in new cell
        While myreader.Read()
            dataStrings.Add(myreader("widgetTitle"))
            dataStrings.Add(myreader("position"))
        End While
        con.Close()
        con.Open()
        'oldt oldp newt newp
        If dataStrings.Count > 2 Then
            mycom.CommandText = "UPDATE bobpar.marketWidgets SET position= '" + dataStrings(1) +
                                "' WHERE (username = '" + User.Identity.Name + "') AND (widgetTitle = '" + dataStrings(2) + "')"
            finished = mycom.ExecuteNonQuery()

            mycom.CommandText = "UPDATE bobpar.marketWidgets SET position= '" + dataStrings(3) +
                    "' WHERE (username = '" + User.Identity.Name + "') AND (widgetTitle = '" + dataStrings(0) + "')"
            finished = mycom.ExecuteNonQuery()
        Else
            mycom.CommandText = "UPDATE bobpar.marketWidgets SET position= '" + newPosition.SelectedValue +
                                "'WHERE (username = '" + User.Identity.Name + "') AND (widgetTitle = '" + dataStrings(0) + "')"
            finished = mycom.ExecuteNonQuery()
        End If
        con.Close()
        Response.Redirect("~/Default.aspx")
    End Sub

    Protected Sub cancelButton_Click(sender As Object, e As System.EventArgs) Handles cancelButton.Click
        Response.Redirect("~/Default.aspx")
    End Sub

End Class
