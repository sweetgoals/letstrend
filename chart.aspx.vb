
Partial Class chart
    Inherits System.Web.UI.Page
    Dim stockData As yahooData
    Dim rangeData As rangeStockData
    Public stockDataString As String = ""
    Public graphRange As String = ""
    Public graphDate As String = ""
    Public graphVolume As String = ""
    Public volumeDataString As String = ""
    Public volumeDataUnitTitle As String = ""
    Public volumeDataUnitShortTitle As String = ""
    Public vol_disp As String = ""
    Public stochDataString As String = ""
    Public macdDataString As String = ""
    Public rsiDataString As String = ""
    Public rocDataString As String = ""

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        errorLabel.Visible = False
        If Page.IsPostBack = False Then
            If Request.QueryString("ticker") <> "" Then
                ticker.Text = Request.QueryString("ticker")
            End If
            week_Click(sender, e)
        End If
        ticker.Text = ticker.Text.ToUpper
        resetColors()
    End Sub

    'resets the display buttons back to default colors.
    Sub resetColors()
        week.BackColor = Drawing.Color.Empty
        month.BackColor = Drawing.Color.Empty
        month3.BackColor = Drawing.Color.Empty
        month6.BackColor = Drawing.Color.Empty
        year1.BackColor = Drawing.Color.Empty
        customButton.BackColor = Drawing.Color.Empty
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
                    checkTicker()
                End If
                startCell = stockData.find_date(startDate)
                endCell = stockData.find_date(endDate)
                rangeData = New rangeStockData(stockData, startCell, endCell)
                graphDisplay()
            Catch ex As Exception
                checkTicker()
            End Try
        End If
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
                getStartDay(refDay, startDate)
                Try
                    stockData = New yahooData(ticker.Text, startDate)
                    If stockData.close.Count = 0 Then
                        checkTicker()
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
                    checkTicker()
                End Try
            End If
        Catch ex As Exception
            checkTicker()
        End Try
    End Sub

    'finds the first day trading session to request from yahoo. 
    'Checks the ema and sma inputs. Then finds gets enough sessions to be able to calculate
    'the sma and ema and macd.
    Sub getStartDay(ByVal periodDays As Integer, _
                         ByRef sDate As Date)
        Dim emaStartDate As New Date
        Dim smaStartDate As New Date
        Dim macdStartDate As New Date
        Dim rsiStartDate As New Date
        Dim rocStartDate As New Date
        Dim multiStartDate As New Date

        Try
            emaStartDate = Today
            smaStartDate = Today
            multiStartDate = Today
            If smaEnable.Checked = True Then
                checkMovingAvg(periodDays, sma1, sma2, sma3, smaStartDate)
            End If
            If smaStartDate < sDate Then
                sDate = smaStartDate
            End If
            If emaEnable.Checked = True Then
                checkMovingAvg(periodDays, ema1, ema2, ema3, emaStartDate)
            End If
            If emaStartDate < sDate Then
                sDate = emaStartDate
            End If
            If multiYearEnable.Checked = True Then
                checkMultiYear(periodDays, my1, my2, my3, multiStartDate)
            End If
            If multiStartDate < sDate Then
                sDate = multiStartDate
            End If
            macdStartDate = constantSetup(periodDays, 90)
            rsiStartDate = constantSetup(periodDays, (Convert.ToInt32(rsiBox.Text) * 2) + 50)
            rocStartDate = constantSetup(periodDays, (Convert.ToInt32(rocBox.Text) * 2) + 50)
            If (macdStartDate < sDate) Or (rsiStartDate < sDate) Then
                If rsiStartDate < macdStartDate Then
                    sDate = rsiStartDate
                Else : sDate = macdStartDate
                End If
            End If
            If rocStartDate < sDate Then
                sDate = rocStartDate
            End If
        Catch ex As Exception
        End Try
    End Sub

    'adds 90 days to the start date to make sure there is enough data points to do calculations
    Function constantSetup(ByVal days As Integer, _
                           ByVal constant As Integer) As DateTime
        Dim macd_start As Double = 0
        Dim aday As New TimeSpan(0, 0, 0, 0)

        macd_start = constant + days
        aday = New TimeSpan(macd_start, 0, 0, 0)
        Return Today.Subtract(aday)
    End Function

    Sub checkTicker()
        errorLabel.Text = "Bad Ticker"
        errorLabel.Visible = True
    End Sub

    Sub checkMovingAvg(ByVal days As Integer, _
                  ByRef maBox1 As TextBox, _
                  ByRef maBox2 As TextBox, _
                  ByRef maBox3 As TextBox,
                  ByRef startDay As DateTime)
        Dim movingAvg1 As Integer = 0
        Dim movingAvg2 As Integer = 0
        Dim movingAvg3 As Integer = 0
        Dim longestMA As Integer = 0
        Dim daySpan As New TimeSpan
        checkMaInput(maBox1, movingAvg1)
        checkMaInput(maBox2, movingAvg2)
        checkMaInput(maBox3, movingAvg3)
        If movingAvg1 > movingAvg2 Then
            If movingAvg1 > movingAvg3 Then
                longestMA = movingAvg1
            Else : longestMA = movingAvg3
            End If
        ElseIf movingAvg2 > movingAvg3 Then
            longestMA = movingAvg2
        Else : longestMA = movingAvg3
        End If
        longestMA += (longestMA \ 5) * 2 + 20
        longestMA += (2 * longestMA) + ((longestMA \ 365) * longestMA) + days
        daySpan = New TimeSpan(longestMA, 0, 0, 0)
        startDay = Today.Subtract(daySpan)
    End Sub

    Sub checkMaInput(ByRef maBox As TextBox, _
                     ByRef maAvg As Integer)
        Try
            maAvg = Convert.ToInt16(maBox.Text)
            If maAvg < 1 Or maAvg > 300 Then
                maBox.BackColor = Drawing.Color.White
                maBox.Text = ""
                maAvg = -1
            End If
        Catch
            maBox.BackColor = Drawing.Color.White
            maBox.Text = ""
            maAvg = -1
        End Try
    End Sub

    Sub checkMultiYear(ByVal period As Integer, _
                       ByRef yearBox1 As TextBox, _
                       ByRef yearBox2 As TextBox, _
                       ByRef yearBox3 As TextBox,
                       ByRef startDate As Date)
        Dim year1 As Integer = 0
        Dim year2 As Integer = 0
        Dim year3 As Integer = 0
        Dim lowestYear As Integer = 0
        Dim lowDate As String = ""
        Dim periodTime As New TimeSpan(period, 0, 0, 0)

        'check for bad input
        checkYear(yearBox1, year1)
        checkYear(yearBox2, year2)
        checkYear(yearBox3, year3)

        'find the lowest year
        If year1 < year2 Then
            If year3 < year1 Then
                lowestYear = year3 - 2
            Else : lowestYear = year1 - 2
            End If
        ElseIf year3 < year2 Then
            lowestYear = year3 - 2
        Else : lowestYear = year2 - 2
        End If
        If lowestYear <> 9999 Then
            lowDate = Today.Month.ToString & "/" & Today.Day.ToString & "/" & lowestYear.ToString
        Else
            multiYearEnable.Checked = False
        End If
        startDate = Convert.ToDateTime(lowDate)
    End Sub

    Sub checkYear(ByRef yearbox As TextBox, _
                  ByRef year As Integer)
        Try
            year = Convert.ToInt16(yearbox.Text)
            If year < 1984 Or year > Today.Date.Year - 1 Then
                year = 9999
            End If
        Catch
            year = 9999
        End Try
    End Sub

    Function checkRsiorROC(ByVal input As String, _
                      ByVal indicator As String) As Boolean
        Dim numCheck As Integer = 0
        Try
            numCheck = Convert.ToInt32(input)
            If (numCheck > 300) Or (numCheck < 1) Then
                errorLabel.Text = indicator & " must be between 1-300"
                Return True
            End If
        Catch ex As Exception
            errorLabel.Text = indicator & " must be a number"
            Return True
        End Try
        Return False
    End Function

    '********************** MAIN GRAPHING FUNCTION ******************************************
    'Assemble all the graphs together to be displayed
    Sub graphDisplay()
        graphDate = rangeData.graphDate(rangeData.sDate)
        graphRange = rangeData.graphRangeTitle(rangeData.sDate)

        graphStockDisplay()
        graphVolumeDisplay()
        graphStochDisplay()
        graphMacdDisplay()
        graphRsiDisplay()
        graphRocDisplay()
    End Sub

    Sub graphStockDisplay()
        Dim graphClose As String = ""

        graphClose = rangeData.graphDouble(rangeData.close)
        stockDataString = "series: [{ name:'" & rangeData.ticker & "', data:[" & graphClose & "]}"
        If smaEnable.Checked = True Then
            stockDataString &= smaGraphs()
        End If
        If emaEnable.Checked = True Then
            stockDataString &= emaGraphs()
        End If
        If multiYearEnable.Checked = True Then
            stockDataString &= multiYearGraphs()
        End If
        stockDataString &= "]"
    End Sub

    Function smaGraphs() As String
        Dim smaGraphDataString As String = ""
        Dim smaGraphRawData(3) As List(Of Double)
        Dim smaGraphString() As String = {"", "", ""}

        movingAvg(False, sma1, smaGraphRawData(0))
        movingAvg(False, sma2, smaGraphRawData(1))
        movingAvg(False, sma3, smaGraphRawData(2))
        If smaGraphRawData(0).Count > 0 Then
            setupGraph(smaGraphRawData(0), _
                         "SMA - ", _
                         "0,128,0", _
                         System.Drawing.Color.FromArgb(0, 128, 0), _
                         sma1, _
                         smaGraphString(0))
        End If
        If smaGraphRawData(1).Count > 0 Then
            setupGraph(smaGraphRawData(1), _
                         "SMA - ", _
                         "255,128,0", _
                         System.Drawing.Color.FromArgb(255, 128, 0), _
                         sma2, _
                         smaGraphString(1))
        End If
        If smaGraphRawData(2).Count > 0 Then
            setupGraph(smaGraphRawData(2), _
                         "SMA - ", _
                         "0,255,255", _
                         System.Drawing.Color.FromArgb(0, 255, 255), _
                         sma3, _
                         smaGraphString(2))
        End If
        smaGraphDataString = smaGraphString(0) & smaGraphString(1) & smaGraphString(2)
        Return smaGraphDataString
    End Function

    Function emaGraphs() As String
        Dim emaGraphDataString = ""
        Dim emaGraphRawData(3) As List(Of Double)
        Dim emaGraphString() As String = {"", "", ""}

        movingAvg(True, ema1, emaGraphRawData(0))
        movingAvg(True, ema2, emaGraphRawData(1))
        movingAvg(True, ema3, emaGraphRawData(2))
        If emaGraphRawData(0).Count > 0 Then
            setupGraph(emaGraphRawData(0), _
                         "EMA - ", _
                         "0,0,255", _
                         System.Drawing.Color.FromArgb(0, 0, 255), _
                         ema1, _
                         emaGraphString(0))
        End If
        If emaGraphRawData(1).Count > 0 Then
            setupGraph(emaGraphRawData(1), _
                         "EMA - ", _
                         "128,0,64", _
                         System.Drawing.Color.FromArgb(128, 0, 64), _
                         ema2, _
                         emaGraphString(1))
        End If
        If emaGraphRawData(2).Count > 0 Then
            setupGraph(emaGraphRawData(2), _
                         "EMA - ", _
                         "128,0,255", _
                         System.Drawing.Color.FromArgb(128, 0, 255), _
                         ema3, _
                         emaGraphString(2))
        End If
        emaGraphDataString = emaGraphString(0) & emaGraphString(1) & emaGraphString(2)
        Return emaGraphDataString
    End Function

    'mode - false for SMA, 
    '       true for EMA
    Sub movingAvg(ByVal mode As Boolean, _
                  ByRef maBox As TextBox, _
                  ByRef maList As List(Of Double))
        Dim maNumber As Integer = 0
        maList = New List(Of Double)
        Try
            maNumber = Convert.ToInt16(maBox.Text)
        Catch ex As Exception
            maNumber = 0
        End Try
        If maNumber > 0 Then
            If mode = False Then
                maList = stockData.calculateSMA(maNumber)
            Else
                maList = stockData.calculateEMA(maNumber)
            End If
        Else
            maBox.Text = ""
            maBox.BackColor = Drawing.Color.White
        End If
    End Sub

    Function multiYearGraphs() As String
        Dim multiYearGraphDataString = ""
        Dim multiYearGraphRawData(3) As List(Of Double)
        Dim multiYearGraphString() As String = {"", "", ""}

        Try
            multiYearGraphRawData(0) = stockData.getMultiYear(Convert.ToInt32(my1.Text), rangeData.close.Count)
            If multiYearGraphRawData(0).Count > 0 Then
                setupGraph(multiYearGraphRawData(0), _
                             "Year - ", _
                             "255,0,128", _
                             System.Drawing.Color.FromArgb(255, 0, 128), _
                             my1, _
                             multiYearGraphString(0))
            End If
        Catch ex As Exception
            multiYearGraphString(0) = ""
        End Try

        Try
            multiYearGraphRawData(1) = stockData.getMultiYear(Convert.ToInt32(my2.Text), rangeData.close.Count)
            If multiYearGraphRawData(1).Count > 0 Then
                setupGraph(multiYearGraphRawData(1), _
                             "Year - ", _
                             "0,128,192", _
                             System.Drawing.Color.FromArgb(0, 128, 192), _
                             my2, _
                             multiYearGraphString(1))
            End If
        Catch ex As Exception
            multiYearGraphString(1) = ""
        End Try
        Try
            multiYearGraphRawData(2) = stockData.getMultiYear(Convert.ToInt32(my3.Text), rangeData.close.Count)
            If multiYearGraphRawData(2).Count > 0 Then
                setupGraph(multiYearGraphRawData(2), _
                             "Year - ", _
                             "64,128,128", _
                             System.Drawing.Color.FromArgb(64, 128, 128), _
                             my3, _
                             multiYearGraphString(2))
            End If
        Catch ex As Exception
            multiYearGraphString(2) = ""
        End Try

        multiYearGraphDataString = multiYearGraphString(0) & multiYearGraphString(1) & multiYearGraphString(2)
        Return multiYearGraphDataString
    End Function

    Sub setupGraph(ByVal maDouble As List(Of Double), _
                 ByVal maName As String, _
                 ByVal maColor As String, _
                 ByVal maBoxBackColor As Drawing.Color, _
                 ByRef maBox As TextBox, _
                 ByRef maSeries As String)
        Dim maGraph As String = ""

        maName &= maBox.Text
        maGraph = rangeData.graphDouble(maDouble)
        maBox.BackColor = maBoxBackColor
        maSeries = ",{color: 'rgb(" & maColor & ")', name:'" & maName & "',data:[" & maGraph & "]}"
    End Sub

    Sub graphVolumeDisplay()
        Dim volumeDataFormatted As New List(Of String)
        volumeDataFormatted = formatVolume(rangeData.volume)
        graphVolume = rangeData.graphString(volumeDataFormatted)
        volumeDataString = "series: [{ name:'" & rangeData.ticker & "', data:[" & graphVolume & "]}]"
    End Sub

    'VOLUME PROCEDURES
    'formats the volume so it will fit on the graph page. 
    Function formatVolume(ByVal vol As List(Of Double)) As List(Of String)
        Dim fvol As New List(Of String)
        Dim i As Integer = 0
        Dim digits As Integer = 0
        Dim vol_string As String
        Dim trailer As String = ""
        Dim cata As String = ""
        Dim dig_set As Boolean = False

        Try
            formatVolumeDigits(vol(0).ToString.Count)
            For i = 0 To vol.Count - 1 Step 1
                vol_string = vol(i).ToString
                If volumeDataUnitShortTitle = "Tho" Then
                    vol_string = (vol(i) / 1000).ToString
                ElseIf volumeDataUnitShortTitle = "Mil" Then
                    vol_string = (vol(i) / 1000000).ToString
                ElseIf volumeDataUnitShortTitle = "Bil" Then
                    vol_string = (vol(i) / 1000000000).ToString
                End If
                If vol_string.Contains(".") = False Then
                    vol_string &= "."
                End If
                fvol.Add(vol_string.Substring(0, 4))
            Next
        Catch ex As Exception
        End Try
        Return fvol
    End Function

    'finds the right place holder for the graph 
    Sub formatVolumeDigits(ByVal digit As Integer)
        Try
            If digit > 3 And digit < 7 Then
                volumeDataUnitTitle = "Thousands"
                volumeDataUnitShortTitle = "Tho"
            ElseIf digit > 6 And digit < 10 Then
                volumeDataUnitTitle = "Millions"
                volumeDataUnitShortTitle = "Mil"
            ElseIf digit > 9 And digit < 13 Then
                volumeDataUnitTitle = "Billions"
                volumeDataUnitShortTitle = "Bil"
            End If
        Catch ex As Exception
        End Try
    End Sub

    Sub graphStochDisplay()
        Dim kFastOutput As New List(Of Double)
        Dim dFastOutput As New List(Of Double)
        Dim kSlowOutput As New List(Of Double)
        Dim dSlowOutput As New List(Of Double)

        stockData.calculateStoch(kStoch.Text, kFastOutput)
        If fastStoch.Checked Then
            stockData.stochKandD(kFastOutput, "fast", dStoch.Text, kFastOutput, dFastOutput)
        End If
        If slowStoch.Checked Then
            stockData.calculateStoch(kStoch.Text, kFastOutput)
            stockData.stochKandD(kFastOutput, "slow", dStoch.Text, kSlowOutput, dSlowOutput)
        End If
        stochDataString = rangeData.graphStoch(kFastOutput, dFastOutput, kSlowOutput, dSlowOutput)
    End Sub

    Sub graphMacdDisplay()
        Dim macd As String = ""
        Dim macdEma As String = ""
        Dim macdDivergence As String = ""

        rangeData.graphMACD(stockData, macd, macdEma, macdDivergence)
        macdDataString = "series: [{ name:'MACD', data:[" & macd & "]},{color: 'rgb(0,128,0)', name:'EMA9', data:[" _
                        & macdEma & "]},{color: 'rgb(255,128,0)', name:'Div', data:[" & macdDivergence & "]}]"
    End Sub

    Sub graphRsiDisplay()
        Dim rsiGraphData As String = ""

        rsiGraphData = rangeData.graphRsi(stockData, rsiBox.Text)
        rsiDataString = "series: [{ name:'RSI', data:[" & rsiGraphData & "]}]"
    End Sub

    Sub graphRocDisplay()
        Dim rocGraphData As String = ""

        rocGraphData = rangeData.graphRoc(stockData, rocBox.Text)
        rocDataString = "series: [{ name:'ROC', data:[" & rocGraphData & "]}]"
    End Sub
End Class
