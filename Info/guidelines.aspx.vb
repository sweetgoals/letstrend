Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class Info_guidelines
    Inherits System.Web.UI.Page
    Dim guides() As String = {"Guide Name", "Ask", "Bid", "Avg Daily vol(3m)", "Day's Low", "Day's High", _
                            "52wk High", "Chg fm 52wk High", "%Chg fm 52wk High", "52wk Low", _
                            "Chg fm 52wk Low", "%Chg fm 52wk Low", "EPS Diluted (ttm)", _
                            "EPS est next year", "EPS est next qrtr", "EPS est cur year", _
                            "Div Per Share", "Market Cap", "Price / Sales", "Price / Book", _
                            "P/E Ratio", "PEG Ratio", "Price / EPS est curr yr", "Year Target", _
                            "Stock Exchange", "Dividend Yield"}

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim acell As TableCell = New TableCell
        Dim arow As TableRow = New TableRow
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim guideData As New List(Of String)
        Dim sqlCols As String = "guideName,ask,bid,avgDailyVol_3m,dayLow,dayHigh,fiftyTwoHigh," & _
                        "changeFiftyTwoHigh,percentChangeFiftyTwoHigh,fiftyTwoLow,changeFiftyTwoLow," & _
                        "percentChangeFiftyTwoLow,epsDiluted_ttm,epsEstNextYear,epsEstNextQrtr,epsEstCurYear," & _
                        "divPerShare,marketCap,pricePerSales,pricePerBook,PERatio,PEGRatio,pricePerEpsEstCurrYr," & _
                        "yearTarget,exchange,dividendYield"
        Dim sqlColSplit() As String = sqlCols.Split(",")
        Dim i As Integer = 0

        updated.Visible = False
        If User.Identity.Name = "" Then
            Response.Redirect("~/info/loginrequired.aspx")
        Else
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT " & sqlCols & " FROM bobpar.fundamentalGuides WHERE (username='" & User.Identity.Name & "')"
            myreader = mycom.ExecuteReader
            guideData = New List(Of String)
            While myreader.Read()
                For i = 0 To 25 Step 1
                    guideData.Add(myreader(sqlColSplit(i)))
                Next
            End While
            con.Close()

            For i = 1 To guides.Count Step 1
                acell = New TableCell
                acell.Text = guides(i - 1)
                arow.Cells.Add(acell)
                If guideData.Count > 0 Then
                    arow.Cells.Add(createTextBox(i - 1, guideData(i - 1)))
                Else : arow.Cells.Add(createTextBox(i - 1, ""))
                End If

                If i Mod 3 = 0 Then
                    guideTable.Rows.Add(arow)
                    arow = New TableRow
                End If
            Next
            guideTable.Rows.Add(arow)
        End If
    End Sub

    Function createTextBox(ByVal i As Integer,
                           ByVal note As String) As TableCell
        Dim acell As New TableCell
        Dim box As New TextBox
        box.ID = "box" & i
        box.Text = note
        acell.Controls.Add(box)
        Return acell
    End Function

    Protected Sub submitButton_Click(sender As Object, e As System.EventArgs) Handles SubmitButton.Click
        Dim i As Integer = 0
        Dim storeValues As New List(Of String)

        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim sqlValuesString As String = ""
        Dim sqlNameString As String = ""
        Dim sqlInsertString As String = ""
        Dim parameterNames() As String = {"userName", "spot", "guideName", "ask", "bid", "avgDailyVol_3m", "dayLow", _
                                          "dayHigh", "fiftyTwoHigh", "changeFiftyTwoHigh", "percentChangeFiftyTwoHigh", _
                                          "fiftyTwoLow", "changeFiftyTwoLow", "percentChangeFiftyTwoLow", "epsDiluted_ttm", _
                                          "epsEstNextYear", "epsEstNextQrtr", "epsEstCurYear", "divPerShare", "marketCap",
                                          "pricePerSales", "pricePerBook", "PERatio", "PEGRatio", _
                                          "pricePerEpsEstCurrYr", "yearTarget", "exchange", "dividendYield"}
        Dim testlist As New List(Of String)

        storeValues.Add(User.Identity.Name)
        mycom.CommandText = "DELETE FROM bobpar.fundamentalGuides WHERE userName='" & User.Identity.Name & "'"
        mycom.Connection = con
        con.Open()
        i = mycom.ExecuteScalar()
        con.Close()

        mycom.CommandText = "SELECT COUNT(*) FROM bobpar.fundamentalGuides"
        mycom.Connection = con
        con.Open()
        i = mycom.ExecuteScalar()
        con.Close()

        storeValues.Add(i)
        For i = 0 To guides.Count - 1 Step 1
            storeValues.Add(Convert.ToString(Request.Form("box" & i)))
        Next

        For i = 0 To storeValues.Count - 2 Step 1
            mycom.Parameters.Add("@" & parameterNames(i), SqlDbType.VarChar, 50).Value = storeValues(i)
            testlist.Add(parameterNames(i))
            sqlNameString &= parameterNames(i) & ", "
            sqlValuesString &= "@" & parameterNames(i) & ", "
        Next
        mycom.Parameters.Add("@" & parameterNames(i), SqlDbType.VarChar, 50).Value = storeValues(i)
        testlist.Add(parameterNames(i))
        sqlNameString &= parameterNames(i)
        sqlValuesString &= "@" & parameterNames(i)
        sqlInsertString = "INSERT INTO bobpar.fundamentalGuides(" & sqlNameString & ") VALUES (" & sqlValuesString & ")"
        mycom.Connection = con
        con.Open()
        mycom.CommandText = sqlInsertString
        i = mycom.ExecuteNonQuery()
        con.Close()
        updated.Visible = True

    End Sub
End Class
