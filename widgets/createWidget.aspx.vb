Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class createWidget
    Inherits System.Web.UI.Page

    Public rangeData As rangeStockData
    Public stockData As yahooData

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try

            errorLabel.Visible = False
            If User.Identity.Name = "" Then
                Response.Redirect("~/info/loginrequired.aspx")
            End If
        Catch ex As WebException
            errorLabel.Visible = True
            errorLabel.Text = ex.Message
        End Try

    End Sub

    Sub removePositions()
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim dataValues As List(Of String)
        Dim i As Integer = 0
        Dim j As Integer = 0

        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT position FROM bobpar.marketWidgets WHERE (username='" & User.Identity.Name & "')"
        myreader = mycom.ExecuteReader
        dataValues = New List(Of String)
        While myreader.Read()
            dataValues.Add(myreader("position"))          '3
        End While
        con.Close()
        For j = 0 To dataValues.Count - 1 Step 1
            'If dataValues(j) = Position.Items(i).Value Then
            Position.Items.Remove(dataValues(j))
            ' End If
        Next

    End Sub
    Protected Sub createWidgetButton_Click(sender As Object, e As System.EventArgs) Handles createWidgetButton.Click
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim insertIntoTable As String
        Dim comp_num As Integer
        Dim added As Integer
        Dim widgetSQLTitle As String = ""
        Dim widgetSQLData As String = ""

        readWidgetData(widgetSQLTitle, widgetSQLData)

        If widgetSQLData = "" And tickers.Text <> "" And widgetTitle.Text <> "" Then
            con.Open()
            mycom.Connection = con
            insertIntoTable = "INSERT INTO bobpar.marketWidgets(" &
                      "number, UserName, widgetTitle, widgetData, position) " & _
                      "VALUES (@number, @UserName, @widgetTitle, @widgetData, @position)"

            mycom.Parameters.Add("@number", SqlDbType.BigInt, 50).Value = comp_num + 1
            mycom.Parameters.Add("@UserName", SqlDbType.VarChar, 50).Value = User.Identity.Name
            mycom.Parameters.Add("@widgetTitle", SqlDbType.VarChar, 50).Value = widgetTitle.Text
            mycom.Parameters.Add("@widgetData", SqlDbType.VarChar).Value = tickers.Text
            mycom.Parameters.Add("@position", SqlDbType.BigInt, 50).Value = Position.SelectedValue
            mycom.CommandText = insertIntoTable

            added = mycom.ExecuteNonQuery()
            con.Close()
            Response.Redirect("~/Default.aspx")
        ElseIf widgetSQLData <> "" And tickers.Text <> "" And widgetTitle.Text <> "" Then
            con.Open()
            mycom.Connection = con
            insertIntoTable = "UPDATE bobpar.marketWidgets SET widgetTitle='" & widgetTitle.Text _
                    & "', widgetData='" & tickers.Text & "' WHERE userName = '" _
                    & User.Identity.Name & "' AND position='" & Position.SelectedValue & "'"
            mycom.CommandText = insertIntoTable
            added = mycom.ExecuteNonQuery()
            con.Close()
            Response.Redirect("~/Default.aspx")
        ElseIf widgetTitle.Text = "" Then
            errorLabel.Text = "Widget Title is blank"
            errorLabel.Visible = True
        Else
            errorLabel.Text = "bad data"
            errorLabel.Visible = True
        End If

    End Sub

    Protected Sub previewWidgetButton_Click(sender As Object, e As System.EventArgs) Handles previewWidgetButton.Click
        Dim theWidget As widgets = New widgets
        Dim widgetSQLTitle As String = ""
        Dim widgetSQLData As String = ""

        readWidgetData(widgetSQLTitle, widgetSQLData)
        If widgetSQLData <> "" And tickers.Text = "" Then
            tickers.Text = widgetSQLData
        End If
        If tickers.Text <> "" Then
            theWidget.loadTable(previewTable, tickers.Text.Split(" "), True)
            If widgetSQLTitle <> "" And widgetTitle.Text = "" Then
                widetTitlePreview.Text = widgetSQLTitle
                widgetTitle.Text = widgetSQLTitle
            ElseIf widgetTitle.Text <> "" Then
                widetTitlePreview.Text = widgetTitle.Text
            End If
        End If
    End Sub

    Sub readWidgetData(ByRef widTitle As String, ByRef widData As String)
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader

        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT widgetTitle, widgetData FROM bobpar.marketWidgets " & _
                            "WHERE (username = '" & User.Identity.Name & "') AND (position ='" & _
                            Position.SelectedValue & " ')"
        myreader = mycom.ExecuteReader
        While myreader.Read()
            widTitle = myreader("widgetTitle")
            widData = myreader("widgetData")
        End While
        con.Close()
    End Sub

    Protected Sub cancelButton_Click(sender As Object, e As System.EventArgs) Handles cancelButton.Click
        Response.Redirect("~/Default.aspx")
    End Sub
End Class
