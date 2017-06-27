Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports System.Globalization
Imports System.Collections.Specialized
Imports commons


Partial Class _Default
    Inherits System.Web.UI.Page
    Dim stockData As yahooData
    Dim rangeData As rangeStockData
    Public stockDataString As String = ""
    Public graphRange As String = ""
    Public graphDate As String = ""

    Public graphClose As String = ""
    Public graphVolume As String = ""
    Public graphStockName As String = ""

    Public stock_info(,) As String = {{"Name", ""},
                                      {"Last Trade", ""},
                                      {"Prev Close", ""},
                                      {"Change", ""},
                                      {"% Change", ""},
                                      {"Volume", ""}}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        errorLabel.Visible = False
        reset_colors()
        If User.Identity.Name = "" Then
            fundGuideButton.Visible = False
            fundLabel.Visible = False
        End If
        If Page.IsPostBack = False Then
            If Request.QueryString("ticker") <> "" Then
                ticker.Text = Request.QueryString("ticker")
            End If
            week_Click(sender, e)
        End If
        ticker.Text = ticker.Text.ToUpper
    End Sub

    'resets the display buttons back to default colors.
    Sub reset_colors()
        week.BackColor = Drawing.Color.Empty
        month.BackColor = Drawing.Color.Empty
        month3.BackColor = Drawing.Color.Empty
        month6.BackColor = Drawing.Color.Empty
        year1.BackColor = Drawing.Color.Empty
    End Sub

    Protected Sub week_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles week.Click
        periodRun(5, 1, week)
    End Sub

    Protected Sub month_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles month.Click
        periodRun(30, 2, month)
    End Sub

    Protected Sub month3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles month3.Click
        periodRun(90, 3, month3)
    End Sub

    Protected Sub month6_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles month6.Click
        periodRun(180, 4, month6)
    End Sub

    Protected Sub year1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles year1.Click
        periodRun(365, 5, year1)
    End Sub

    Sub periodRun(ByVal refDay As Integer, _
              ByVal sessionValue As Integer, _
              ByRef buttonClicked As Button)
        Dim startCell As Integer = 0
        Dim refTime As New TimeSpan

        Try
            refTime = New TimeSpan(refDay, 0, 0, 0)
            reset_colors()
            buttonClicked.BackColor = Drawing.Color.Lime
            If ticker.Text <> "" Then
                Try
                    stockData = New yahooData(ticker.Text, Today.AddYears(-2))
                    If stockData.close.Count = 0 Then
                        errorLabel.Text = "Bad Ticker"
                        errorLabel.Visible = True
                        Exit Sub
                    End If
                    If refDay = 5 Then 'week click needs exactly 5 days no more or less
                        rangeData = New rangeStockData(stockData, 5)
                    Else 'all other ranges need to go from the refDay to today
                        startCell = stockData.find_date(Today.AddDays(-refDay))
                        If startCell <> -1 Then
                            rangeData = New rangeStockData(stockData, stockData.close.Count - startCell)
                        Else
                            rangeData = New rangeStockData(stockData)
                        End If
                    End If
                    stockGraphDisplay()
                    quoteTableDisplay()
                Catch ex As Exception
                    ticker.Text = ""
                    checkTicker()
                End Try
            End If
        Catch ex As Exception
            checkTicker()
        End Try
    End Sub

    Sub checkTicker()
        If stockData.close.Count = 0 Then
            errorLabel.Text = "Bad Ticker"
            errorLabel.Visible = True
        End If
    End Sub

    Function remove_quotes(ByVal input As String) As String
        Dim temp() As String
        temp = input.Split("""")
        Return temp(1)
    End Function

    Sub stockGraphDisplay()
        Dim graphClose As String = ""
        graphDate = rangeData.graphDate(rangeData.sDate)
        graphRange = rangeData.graphRangeTitle(rangeData.sDate)
        graphClose = rangeData.graphDouble(rangeData.close)
        stockDataString = "series: [{ name:'" & rangeData.ticker & "', data:[" & graphClose & "]}]"
    End Sub

    'displays the data for top quoteTable table. This is just the summary information. The Gain for the period is calculated down 
    'in Display.
    Sub quoteTableDisplay()
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim l As Integer = 0
        Dim dow_hist As New List(Of String)
        Dim colorSwitch As Integer = 0
        Dim cell1 As TableCell
        Dim arow As TableRow
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim guideData As New List(Of String)
        Dim sqlCols As String = "userName,spot,guideName,ask,bid,avgDailyVol_3m,dayLow,dayHigh,fiftyTwoHigh," & _
                                "changeFiftyTwoHigh,percentChangeFiftyTwoHigh,fiftyTwoLow,changeFiftyTwoLow," & _
                                "percentChangeFiftyTwoLow,epsDiluted_ttm,epsEstNextYear,epsEstNextQrtr,epsEstCurYear," & _
                                "divPerShare,marketCap,pricePerSales,pricePerBook,PERatio,PEGRatio,pricePerEpsEstCurrYr," & _
                                "yearTarget,exchange,dividendYield"
        Dim sqlColSplit() As String = sqlCols.Split(",")

        Try
            If stockData.ticker <> "^DJI" Then
                If stockData.stock_info(3, 1).IndexOf("+") > -1 Then
                    colorSwitch = 1
                ElseIf stockData.stock_info(3, 1).IndexOf("-") > -1 Then
                    colorSwitch = -1
                End If

                'name, last trade, prev close, change, %change, volume, Period % gain
                'check to make sure we have the info for the quoteTable, if so then load table1 with data.
                If stockData.quoteInfo = True Then
                    quoteTable.Rows(1).Cells(0).Text = stockData.stock_info(0, 1) 'Name
                    quoteTable.Rows(1).Cells(1).Text = twoDecimal(Convert.ToDouble(stockData.stock_info(1, 1))) 'last trade
                    quoteTable.Rows(1).Cells(2).Text = twoDecimal(Convert.ToDouble(stockData.stock_info(2, 1))) 'previous close
                    quoteTable.Rows(1).Cells(3).Text = twoDecimal(Convert.ToDouble(stockData.stock_info(3, 1))) 'Price Change
                    quoteTable.Rows(1).Cells(4).Text = String.Format("{0:0.00}", stockData.stock_info(4, 1))                   'percent Change
                    quoteTable.Rows(1).Cells(5).Text = String.Format("{0:0.00}", stockData.stock_info(5, 1))                   'volume
                    If colorSwitch = 1 Then
                        For i = 0 To 5 Step 1
                            quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Green
                        Next
                    ElseIf colorSwitch = -1 Then
                        For i = 0 To 5 Step 1
                            quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Red
                        Next
                    Else
                        For i = 0 To 5 Step 1
                            quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Black
                        Next
                    End If
                    For i = 0 To 5 Step 1
                        quoteTable.Rows(0).Cells(i).Wrap = False
                        quoteTable.Rows(1).Cells(i).Wrap = False
                        quoteTable.Rows(0).Cells(i).BorderWidth = 5
                        quoteTable.Rows(1).Cells(i).BorderWidth = 5
                        quoteTable.Rows(0).Cells(i).BorderColor = Drawing.Color.White
                        quoteTable.Rows(1).Cells(i).BorderColor = Drawing.Color.White
                    Next
                End If
                'Load the statistics table fund
                Try
                    If stockData.fundamentalInfo = True Then
                        fundLabel.Visible = True
                        con.Open()
                        mycom.Connection = con
                        If User.Identity.Name <> "" Then
                            mycom.CommandText = "SELECT " & sqlCols & " FROM bobpar.fundamentalGuides WHERE (username='" & User.Identity.Name & "')"
                        Else
                            mycom.CommandText = "SELECT " & sqlCols & " FROM bobpar.fundamentalGuides where userName='jakkjakk' and guideName='General'"
                        End If

                        myreader = mycom.ExecuteReader
                        guideData = New List(Of String)
                        While myreader.Read()
                            For i = 0 To 27 Step 1
                                guideData.Add(myreader(sqlColSplit(i)))
                            Next
                        End While
                        con.Close()
                        k = 6
                        l = 3
                        For i = 0 To 7 Step 1
                            arow = New TableRow
                            For j = 0 To 2 Step 1
                                cell1 = New TableCell
                                cell1.Wrap = False
                                cell1.BorderWidth = 5
                                cell1.BorderColor = Drawing.Color.White
                                cell1.Font.Bold = False
                                If k = 8 Then
                                    Dim num_vol As Int64 = Convert.ToInt64(stockData.stock_info(k, 1))
                                    Dim InvC As CultureInfo = CultureInfo.InvariantCulture
                                    cell1.Text = stockData.stock_info(k, 0) & ": " & num_vol.ToString("#,#", InvC) 'puts commas into daily volume
                                Else
                                    cell1.Text = stockData.stock_info(k, 0) & ": " & stockData.stock_info(k, 1)
                                End If
                                arow.Cells.Add(cell1)
                                cell1 = New TableCell
                                If guideData.Count > 0 Then 'make sure user has a guide
                                    cell1.Text = guideData(l)
                                    l += 1
                                End If
                                arow.Cells.Add(cell1)
                                arow.Font.Bold = False
                                k += 1
                            Next
                            funds.Rows.Add(arow) 'populates the statistics table
                        Next
                    End If
                Catch ex As Exception
                End Try
                If stockData.ticker = "^IXIC" Then
                    Dim hisstock As String = stockData.urlBuilderStartDate(Today)
                    stockData.GetDataByURL(hisstock)
                    Dim num_vol As Int64 = Convert.ToInt64(stockData.volume.Last)
                    Dim InvC As CultureInfo = CultureInfo.InvariantCulture
                    quoteTable.Rows(1).Cells(5).Text = num_vol.ToString("#,#", InvC) 'puts commas into daily volume
                End If

            ElseIf stockData.ticker = "^DJI" Then
                quoteTable.Rows(1).Cells(0).Text = "Dow Jones IND"
                quoteTable.Rows(1).Cells(1).Text = twoDecimal(stockData.close.Last)
                quoteTable.Rows(1).Cells(2).Text = twoDecimal(stockData.close(stockData.close.Count - 2))
                quoteTable.Rows(1).Cells(3).Text = twoDecimal(stockData.cash_gain.Last)
                quoteTable.Rows(1).Cells(4).Text = twoDecimal(stockData.per_gain.Last)
                Dim num_vol As Int64 = Convert.ToInt64(stockData.volume.Last)
                Dim InvC As CultureInfo = CultureInfo.InvariantCulture
                quoteTable.Rows(1).Cells(5).Text = num_vol.ToString("#,#", InvC) 'puts commas into daily volume

                If Convert.ToDouble(stockData.cash_gain.Last) > 0 Then
                    For i = 0 To 5 Step 1
                        quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Green
                    Next
                ElseIf Convert.ToDouble(stockData.cash_gain.Last) < 0 Then
                    For i = 0 To 5 Step 1
                        quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Red
                    Next
                Else
                    For i = 0 To 5 Step 1
                        quoteTable.Rows(1).Cells(i).ForeColor = Drawing.Color.Black
                    Next
                End If
            End If

            Dim per_gain As Double = ((rangeData.close.Last - rangeData.close.First) / rangeData.close.First) * 100
            If per_gain > 0 Then
                quoteTable.Rows(1).Cells(6).ForeColor = Drawing.Color.Green
            ElseIf per_gain = 0 Then
                quoteTable.Rows(1).Cells(6).ForeColor = Drawing.Color.Black
            Else : quoteTable.Rows(1).Cells(6).ForeColor = Drawing.Color.Red
            End If
            quoteTable.Rows(1).Cells(6).Text = stockData.twoDecimal(per_gain)

        Catch ex As Exception
        End Try
    End Sub

    Function twoDecimal(ByVal theNumber As Double) As String
        Return String.Format("{0:n2}", theNumber)
    End Function

End Class
