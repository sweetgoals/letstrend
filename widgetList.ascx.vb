Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports Microsoft.VisualBasic

Partial Class wallst
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim position As Label = Page.FindControl("position")
        Dim userName As Label = Page.FindControl("userName")
        loadLists(position.Text, userName.Text)
    End Sub

    Sub loadLists(ByVal pos As String, _
                  ByVal userName As String)
        Dim dataValues As New List(Of String)
        Dim generalString As String = ""
        Dim userString As String = ""
        Dim mywid As New widgets

        generalString = "SELECT widgetData, widgetTitle, position FROM bobpar.marketWidgets " +
                            "WHERE (username = '') AND (position = '" & pos & "')"
        userString = "SELECT widgetData, widgetTitle, position FROM bobpar.marketWidgets " +
                            "WHERE (username = '" & userName & "') AND (position = '" & pos & "')"
        If userName <> "" Then
            getWidgets(userString, dataValues)
        End If
        getWidgets(generalString, dataValues)
        generalString = "SELECT widgetData, widgetTitle, position FROM bobpar.marketWidgets " +
                    "WHERE (username = 'jakkjakk') AND (position = '" & pos & "')"
        getWidgets(generalString, dataValues)
        If dataValues.Count <> 0 Then
            widgetTitle.Text = dataValues(1)
            mywid.loadTable(widgetTable, dataValues(2).Split(" "))
        Else : widPanel.Visible = False
        End If       
    End Sub

    Sub getWidgets(ByVal sqlString As String, _
                    ByRef data As List(Of String))
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=www.letstrend.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim dataValues As List(Of String)
        con.Open()
        mycom.Connection = con
        mycom.CommandText = sqlString
        myreader = mycom.ExecuteReader
        dataValues = New List(Of String)
        While myreader.Read()
            If Not data.Contains(myreader("position")) Then
                data.Add(myreader("position"))          '3
                data.Add(myreader("widgetTitle"))       '2
                data.Add(myreader("widgetData"))        '1
            End If
        End While
    End Sub
End Class
