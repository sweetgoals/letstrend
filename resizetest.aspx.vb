
Partial Class resizetest
    Inherits System.Web.UI.Page
    Public series_string As String = ""
    Public graphRange As String = ""
    Public graphDate As String = ""
    Dim stockData As yahooData
    Dim rangeData As rangeStockData


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        errorLabel.Visible = False
        ticker.Text = ticker.Text.ToUpper
        reset_colors()
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

    'custom period
    Protected Sub analyze_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles analyze.Click
        Dim startDate As Date
        Dim endDate As Date
        Dim valid As Boolean = True
        Dim aday As New TimeSpan
        Dim dateDifference As New Integer
        Dim startd As Date
        Dim splitDates() As String
        Dim startCell As Integer = 0
        Dim endCell As Integer = 0

        customButton.BackColor = Drawing.Color.Lime
        Try
            splitDates = customRange.Text.Split("-")
            startDate = Convert.ToDateTime(splitDates(0))
            endDate = Convert.ToDateTime(splitDates(1))
            dateDifference = DateDiff(DateInterval.Day, startDate, endDate)
            aday = New TimeSpan((dateDifference + (2 * 30) + ((30 \ 365) * 30)), 0, 0, 0)
            startd = endDate.Subtract(aday)
        Catch ex As Exception
            errorLabel.Text = "Bad Custom Period Format"
            errorLabel.Visible = True
            valid = False
        End Try
        If valid = True Then
            Try
                stockData = New yahooData(ticker.Text, startd)
                If stockData.close.Count = 0 Then
                    errorLabel.Text = "Bad Ticker"
                    errorLabel.Visible = True
                    Exit Sub
                End If
                startCell = stockData.find_date(startDate)
                endCell = stockData.find_date(endDate)
                rangeData = New rangeStockData(stockData, startCell, endCell)
                graphDisplay()
            Catch ex As Exception
                ticker.Text = ""
                errorLabel.Text = "Bad Ticker"
                errorLabel.Visible = True
            End Try
        End If
    End Sub

    'resets the display buttons back to default colors.
    Sub reset_colors()
        week.BackColor = Drawing.Color.Empty
        month.BackColor = Drawing.Color.Empty
        month3.BackColor = Drawing.Color.Empty
        month6.BackColor = Drawing.Color.Empty
        year1.BackColor = Drawing.Color.Empty
        customButton.BackColor = Drawing.Color.Empty
    End Sub

    Sub periodRun(ByVal refDay As Integer, _
              ByVal sessionValue As Integer, _
              ByRef buttonClicked As Button)
        Dim startCell As Integer = 0
        Dim refTime As New TimeSpan
        Dim startDate As New Date

        Try
            refTime = New TimeSpan(refDay, 0, 0, 0)
            startDate = Today.Subtract(refTime)
            Session("clicked") = sessionValue
            buttonClicked.BackColor = Drawing.Color.Lime
            If ticker.Text <> "" Then
                Try
                    stockData = New yahooData(ticker.Text, startDate)
                    If stockData.close.Count = 0 Then
                        errorLabel.Text = "Bad Ticker"
                        errorLabel.Visible = True
                        Exit Sub
                    End If
                    If refDay = 5 Then
                        'week click needs exactly 5 days no more or less
                        rangeData = New rangeStockData(stockData, 5)
                    Else 'all other ranges need to go from the refDay to today
                        startCell = stockData.find_date(Today.AddDays(-refDay))
                        If startCell <> -1 Then
                            rangeData = New rangeStockData(stockData, stockData.close.Count - startCell)
                        Else
                            rangeData = New rangeStockData(stockData)
                        End If
                    End If
                    graphDisplay()
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

    Sub graphDisplay()
        Dim graphClose As String = ""
        graphClose = rangeData.graphDouble(rangeData.close)
        graphDate = rangeData.graphDate(rangeData.sDate)
        series_string = "series: [{ name:'" & rangeData.ticker & "', data:[" & graphClose & "]}]"
        graphRange = rangeData.graphRangeTitle(rangeData.sDate)
    End Sub

End Class
