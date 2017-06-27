Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports Microsoft.VisualBasic
Imports simulate

Partial Class signalGuide
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim i As Integer = 0
        Dim arow As New TableRow
        Dim acell As New TableCell
        Dim sbox As New TextBox
        Dim columns() As String = {"Signal", "Days"}
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim guideData As New List(Of String)
        Dim sqlCols As String = "signal, days"
        Dim sqlColSplit() As String = sqlCols.Split(",")
        Dim userName As Label = Page.FindControl("userName")
        Dim ticker As Label = Page.FindControl("ticker")
        Dim simulateSigGuide As New simulate

        If userName.Text <> "" Then
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT " & sqlCols & " FROM bobpar.signalGuides WHERE (username='" & userName.Text & "')"
            myreader = mycom.ExecuteReader
            guideData = New List(Of String)
            While myreader.Read()
                guideData.Add(myreader("signal"))
                guideData.Add(myreader("days"))
            End While
            con.Close()

            For i = 0 To 1 Step 1
                acell.Text = columns(i)
                arow.Cells.Add(acell)
                acell = New TableCell
            Next

            signalDataTable.Rows.Add(arow)
            For i = 0 To guideData.Count - 2 Step 2
                '                simulateSigGuide.findSignalGuide(ticker.Text, guideData(i), guideData(i + 1))
                signalDataTable.Rows.Add(makeRow(New List(Of String)(New String() {ticker.Text, guideData(i), guideData(i + 1)})))
            Next
        End If

    End Sub

    Function makeRow(ByVal cellValues As List(Of String)) As TableRow
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim i As Integer = 0
        Dim iniSimValues As New List(Of String)
        Dim signalSim As New simulate
        Dim buySellValues(2) As String

        If cellValues(1).Contains("<") Then
            buySellValues = cellValues(1).Split("<")
        ElseIf cellValues(1).Contains(">") Then
            buySellValues = cellValues(1).Split(">")
        ElseIf cellValues(1).Contains("=") Then
            buySellValues = cellValues(1).Split("=")
        End If

        iniSimValues.Add(cellValues(0))
        iniSimValues.Add(buySellValues(0))
        iniSimValues.Add(buySellValues(1))

        For i = 1 To cellValues.Count - 1 Step 1
            acell = New TableCell
            acell.Text = cellValues(i).Replace("<", "&lt")
            arow.Cells.Add(acell)
        Next
        Return arow
    End Function
End Class
