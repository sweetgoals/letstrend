'Imports Microsoft.VisualBasic
'Imports System.IO
'Imports System.Data.SqlClient
'Imports System.Data
'Imports System.Net
'Imports System.Text
'Imports System.Xml.XPath
'Imports System.Xml
'Imports System.Globalization
'Imports System.Collections.Specialized

Public Class simulate
    Inherits commons

    Structure movingAverage
        Dim name As String
        Dim values As String
    End Structure
    'Public summaryResults(,) As String = {{"Ticker", ""}           0
    '                                  {"Ini Invest", ""}           1
    '                                  {"Current Invest", ""}       2
    '                                  {"Time Vested", ""}          3
    '                                  {"Total Trading Days", ""}   4
    '                                  {"Date Range", ""}           5
    '                                  {"Current Status", ""}       6
    '                                  {"Buy Signal", ""}           7
    '                                  {"Sell Signal", ""}}         8

    Public summaryResults(,) As String = {{"Ticker", ""}, _
                                          {"Ini Invest", ""}, _
                                          {"Current Invest", ""}, _
                                          {"Time Vested", ""}, _
                                          {"Total Trading Time", ""}, _
                                          {"Date Range", ""}, _
                                          {"Buy Signal", ""}, _
                                          {"Sell Signal", ""}, _
                                          {"Total Trade Cost", ""}}

    'Public gainsResults(,) As String = {{"Total Cash Gain", ""}, _     0
    '                                    {"Cash Gain stopError Day", ""}, _   1
    '                                    {"Cash Gain stopError Month", ""}, _ 2
    '                                    {"Cash Gain stopError Year", ""}, _  3
    '                                    {"Total % Gain", ""}, _        4
    '                                    {"Best Day", ""}, _            5
    '                                    {"Best Month", ""}, _          6
    '                                    {"Best year", ""}, _           7
    '                                    {"Worst Day", ""}, _           8
    '                                    {"Worst Month", ""}, _         9
    '                                    {"Worst Year", ""}}            10


    Public gainsResults(,) As String = {{"Total Gain", ""}, _
                                        {"Best", ""}, _
                                        {"Worst", ""}, _
                                        {"Cash Gain stopError Per Day", ""}, _
                                        {"Cash Gain stopError Per Month", ""}, _
                                        {"Cash Gain stopError Per Year", ""}}
    'Do this later if you want.
    '{"Best Day", ""}, _
    '{"Best Month", ""}, _
    '{"Best year", ""}, _
    '{"Worst Day", ""}, _
    '{"Worst Month", ""}, _
    '{"Worst Year", ""}}

    'Public tradeSummary(,) As String = {{"#Trades", ""}, _     0
    '                                    {"Winning", ""}, _     1
    '                                    {"Losing", ""}, _      2
    '                                    {"Win Ratio", ""}, _   3
    '                                    {"Entry Trades", ""}   4
    '                                    {"Exit Trades", ""}    5
    '                                    {"Longest Held", ""}   6
    '                                    {"Shortest Held", ""}  7
    '                                    {"Best", ""}, _        8
    '                                    {"Worst", ""}, _       9
    '                                    {"Winning Streak       10
    '                                    {"Losing Streak"       11
    '                                    {"Avg Gain stopError Trade   12  
    '                                    {"Avg Held Time"       13
    '                                    {"Avg Win Held Time"   14
    '                                    {"Avg Lose Held Time"  15

    Public tradeSummary(,) As String = {{"#Trades", ""}, _
                                        {"Winning", ""}, _
                                        {"Losing", ""}, _
                                        {"Win Percent", ""}, _
                                        {"Longest Held", ""}, _
                                        {"Shortest Held", ""}, _
                                        {"Best Trade", ""}, _
                                        {"Worst Trade", ""}, _
                                        {"Winning Streak", ""}, _
                                        {"Losing Streak", ""}, _
                                        {"Avg Gain Per Trade", ""}, _
                                        {"Avg Held Time", ""}, _
                                        {"Avg Win Held Time", ""}, _
                                        {"Avg Lose Held Time", ""}}

    Public ticker, buyOption, buySignal, sellOption, sellSignal, capital As String
    Public tradeCost As Double
    Public lastTradePosition As String = "Out"
    Public lastTradeDate As String = "No Trades"
    Public dates As DateTime
    Public stockData As yahooData
    Public periodData As rangeStockData
    Public tradeResults As New List(Of String)
    Public maList As New List(Of movingAverage)
    Public simError As String = ""
    Public signalSyncOption As Boolean = False

    Public Sub New()

    End Sub

    Public Sub New(ByVal initilizationValues As List(Of String), _
                   Optional ByVal dates As DateTime = Nothing, _
                   Optional ByVal range As Integer = Nothing, _
                   Optional ByVal sigSync As Boolean = False)
        Dim diffDate As Integer
        Try
            ticker = initilizationValues(0)
            buyOption = initilizationValues(1)
            buySignal = initilizationValues(2)
            sellOption = initilizationValues(3)
            sellSignal = initilizationValues(4)
            Try
                tradeCost = Convert.ToDouble(initilizationValues(5))
            Catch ex As Exception
                tradeCost = 0
            End Try
            capital = initilizationValues(6)
            If sigSync <> Nothing Then
                signalSyncOption = sigSync
            End If

            If dates <> Nothing Then
                diffDate = DateDiff(DateInterval.Day, dates, Today.Date)
                setupTrade(diffDate, False)
            Else : setupTrade(range, False)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub automate(ByVal symbol As String, _
                        ByRef maxGain As Double, _
                        ByRef bestBuySMA As String, _
                        ByRef bestSellSMA As String, _
                        ByRef goldenCross As String, _
                        ByRef deathCross As String)
        ticker = symbol
        stockData = New yahooData(ticker, Today.AddMonths(-18))
        periodData = New rangeStockData(stockData, 120)
        capital = "1000.00"
        findBestSMA(maxGain, bestBuySMA, bestSellSMA, goldenCross, deathCross)
    End Sub

    Sub findBestSMA(ByRef maxGain As Double, _
                    ByRef bestBuySMA As String, _
                    ByRef bestSellSMA As String, _
                    ByRef gCross As String, _
                    ByRef dCross As String)
        'Dim bestSMA As String = ""
        Dim i As Integer = 0
        Dim j As Integer = 0
        'Dim maxGain As Double = 0
        Dim runGain As Double = 0
        Dim buySignals As New List(Of String)
        Dim sellSignals As New List(Of String)
        Dim maxTradeResults As New List(Of String)
        Dim bestPercentGain As Double = 0

        Try
            buyOption = "maAvg"
            sellOption = "maAvg"
            For i = 1 To 200 Step 1
                buySignal = "Price>SMA" & i
                buySignals = maSignalsSetup(buySignal)
                For j = 1 To 200 Step 1
                    sellSignal = "Price<SMA" & j
                    sellSignals = maSignalsSetup(sellSignal)
                    runGain = 0
                    tradeResults = New List(Of String)
                    '        If signalSyncOption = True Then
                    signalSync(buySignals, sellSignals)
                    'End If
                    processSignals(buySignals, sellSignals, tradeResults, runGain)
                    If runGain > maxGain Then
                        maxGain = runGain
                        bestBuySMA = buySignal
                        bestSellSMA = sellSignal
                        maxTradeResults = tradeResults
                    End If
                    If i = 50 And j = 200 Then
                        findCrosses(gCross, dCross)
                    End If
                Next
            Next
        Catch ex As Exception
        End Try
        bestSellSMA = bestSellSMA.Replace("<", "&lt;")
        bestPercentGain = (((1000 + maxGain) - 1000) / 1000) * 100
        bestPercentGain = String.Format("{0:n2}", bestPercentGain)
        maxGain = bestPercentGain
    End Sub

    Sub findCrosses(ByRef gCross As String, _
                    ByRef dCross As String)
        Dim ma50 As New List(Of Double)
        Dim ma200 As New List(Of Double)
        Dim startCell As Integer = 0
        Dim i As Integer = 0
        Dim gCrossPoint As Integer = 0
        Dim dCrossPoint As Integer = 0
        Try
            gCross = ""
            dCross = ""
            ma50 = periodData.rangeDouble(stockData.calculateSMA(50))
            ma200 = periodData.rangeDouble(stockData.calculateSMA(200))
            i = ma200.Count - 1
            While (((gCross = "") Or (dCross = "")) And (i > 1))
                'golden cross
                '		periodData.sDate(i)	"2011-11-15"	String
                If i = 23 Then
                    Dim blah As Integer
                    blah = 1
                End If
                If (ma50(i) > ma200(i)) And (ma50(i - 1) < ma200(i - 1)) Then 'gold cross
                    gCross = periodData.close(i).ToString & "@" & periodData.sDate(i)
                ElseIf (ma50(i) < ma200(i)) And (ma50(i - 1) > ma200(i - 1)) Then 'black cross
                    dCross = periodData.close(i).ToString & "@" & periodData.sDate(i)
                End If
                i -= 1
            End While
        Catch ex As Exception
        End Try
    End Sub
    'TRADE SECTION
    'Selects the Buy Signal and runs the approate setup
    Sub setupTrade(ByVal range As Integer, _
                   ByVal automate As Boolean, _
                   Optional ByVal startDateOp As Date = Nothing, _
                   Optional ByVal endDateOp As Date = Nothing)
        Dim buyTriggerValue() As Double = {0, 0}
        Dim sellTriggerValue() As Double = {0, 0}
        Dim buyCommand As Integer = 0
        Dim sellCommand As Integer = 0
        Dim buySignals As New List(Of String)
        Dim sellSignals As New List(Of String)
        Dim highMa As Integer = 0
        Dim startDate As New Date
        Dim macdDoubles As New List(Of Double)
        Dim macdEmaDoubles As New List(Of Double)
        Dim macdDivDoubles As New List(Of Double)

        Dim rsiOrRocBuyData As New List(Of Double)
        Dim rsiOrRocSellData As New List(Of Double)
        Dim rsiOrRocBuyPeriod As Integer = 0
        Dim rsiOrRocBuyGraph As String = ""
        Dim rsiOrRocSellPeriod As Integer = 0
        Dim rsiOrRocSellGraph As String = ""

        Dim macdString As String = ""
        Dim macdEmaString As String = ""
        Dim macdDivString As String = ""

        loadStockData(range, startDateOp, endDateOp)

        If buyOption = "Price" Then
            priceSignalSetup(buyCommand, buyTriggerValue, buySignal)
            buySignals = getSignals(buyCommand, buyTriggerValue)
        ElseIf buyOption = "maAvg" Then
            buySignals = maSignalsSetup(buySignal)
        ElseIf buyOption = "macd" Then
            periodData.graphMACD(stockData, macdString, macdEmaString, macdDivString)
            macdDoubles = periodData.convertToDoubleList(macdString)
            macdEmaDoubles = periodData.convertToDoubleList(macdEmaString)
            macdDivDoubles = periodData.convertToDoubleList(macdDivString)
            buySignals = macdSignalSetup(buySignal, macdDoubles, macdEmaDoubles, macdDivDoubles)
        ElseIf buyOption = "rsi" Then
            rsiOrRocBuyPeriod = findRsiorRocPeriod("I", buySignal)
            rsiOrRocBuyGraph = periodData.graphRsi(stockData, rsiOrRocBuyPeriod)
            rsiOrRocBuyData = periodData.convertToDoubleList(rsiOrRocBuyGraph)
            buySignals = rsiorRocSignalSetup(buySignal, rsiOrRocBuyData)
        ElseIf buyOption = "roc" Then
            rsiOrRocBuyPeriod = findRsiorRocPeriod("C", buySignal)
            rsiOrRocBuyGraph = periodData.graphRoc(stockData, rsiOrRocBuyPeriod)
            rsiOrRocBuyData = periodData.convertToDoubleList(rsiOrRocBuyGraph)
            buySignals = rsiorRocSignalSetup(buySignal, rsiOrRocBuyData)
        End If
        '7 - Sell Signal
        If sellOption = "Price" Then
            priceSignalSetup(sellCommand, sellTriggerValue, sellSignal)
            sellSignals = getSignals(sellCommand, sellTriggerValue)
        ElseIf sellOption = "maAvg" Then
            sellSignals = maSignalsSetup(sellSignal)
        ElseIf sellOption = "macd" Then
            periodData.graphMACD(stockData, macdString, macdEmaString, macdDivString)
            macdDoubles = periodData.convertToDoubleList(macdString)
            macdEmaDoubles = periodData.convertToDoubleList(macdEmaString)
            macdDivDoubles = periodData.convertToDoubleList(macdDivString)
            sellSignals = macdSignalSetup(sellSignal, macdDoubles, macdEmaDoubles, macdDivDoubles)
        ElseIf sellOption = "rsi" Then
            rsiOrRocSellPeriod = findRsiorRocPeriod("I", sellSignal)
            rsiOrRocSellGraph = periodData.graphRsi(stockData, rsiOrRocSellPeriod)
            rsiOrRocSellData = periodData.convertToDoubleList(rsiOrRocSellGraph)
            sellSignals = rsiorRocSignalSetup(sellSignal, rsiOrRocSellData)
        ElseIf sellOption = "roc" Then
            rsiOrRocSellPeriod = findRsiorRocPeriod("C", sellSignal)
            rsiOrRocSellGraph = periodData.graphRoc(stockData, rsiOrRocSellPeriod)
            rsiOrRocSellData = periodData.convertToDoubleList(rsiOrRocSellGraph)
            sellSignals = rsiorRocSignalSetup(sellSignal, rsiOrRocSellData)
        End If
        If buySignals.Count > 0 Then
            If signalSyncOption = True Then
                signalSync(buySignals, sellSignals)
            End If
            processSignals(buySignals, sellSignals, tradeResults)
        End If
        loadResults() 'tradeResults)
    End Sub

    Function constantSetup(ByVal days As Integer, _
                   ByVal constant As Integer) As DateTime
        Dim macd_start As Double = 0
        Dim aday As New TimeSpan(0, 0, 0, 0)

        macd_start = constant + days
        aday = New TimeSpan(macd_start, 0, 0, 0)
        Return Today.Subtract(aday)
    End Function

    'Finds the period of the RSI entered. box.text=RSI14>30 returns 14
    Function findRsiorRocPeriod(ByVal indicator As String, _
                           ByVal box As String) As Integer
        Dim rsiPeriod As Integer = 0
        Dim rsiSplit() As String
        Dim rsiI As Integer = 0
        Try
            If box.Contains("<") Then
                rsiSplit = box.Split("<")
            Else : rsiSplit = box.Split(">")
            End If
            rsiI = rsiSplit(0).IndexOf(indicator)
            rsiPeriod = Convert.ToInt32(rsiSplit(0).Substring(rsiI + 1, rsiSplit(0).Length - 1 - rsiI))
        Catch ex As Exception
        End Try
        Return rsiPeriod
    End Function

    Sub loadStockData(ByVal range As Integer, _
                      Optional ByVal startDateOp As Date = Nothing, _
                      Optional ByVal endDateOP As Date = Nothing)
        Dim highMa As Integer = 0
        Dim startDate As New Date
        Dim rangeSpan As New TimeSpan
        Dim rangeStartDate As New Date
        Dim buyRsiOrRocPeriod As Integer = 0
        Dim sellRsiOrRocPeriod As Integer = 0
        Dim highRsiOrRocPeriod As Integer = 0
        Dim highestStartDate As Integer = 0
        Dim highMacd As Integer = 0
        Dim hasMA As Boolean = False
        Dim hasRsiRoc As Boolean = False
        Dim hasMACD As Boolean = False
        Dim sampleSize As Integer = 0
        Dim i As Integer = 0

        'buySelect = signalOperation(buyOption)
        'sellSelect = signalOperation(sellOption)
        rangeSpan = New TimeSpan(range, 0, 0, 0)
        'invested = Convert.ToDouble(investValue.Text)
        If endDateOP = Nothing Then
            endDateOP = Today
        End If
        rangeStartDate = endDateOP.Subtract(rangeSpan)
        If buyOption = "maAvg" Or sellOption = "maAvg" Then
            hasMA = True
            highMa = findHighMa()
        End If
        If buyOption = "rsi" Or _
            buyOption = "roc" Or _
            sellOption = "rsi" Or _
            sellOption = "roc" Then

            hasRsiRoc = True
            If buyOption = "rsi" Then
                buyRsiOrRocPeriod = findRsiorRocPeriod("I", buySignal)
            ElseIf buyOption = "roc" Then
                buyRsiOrRocPeriod = findRsiorRocPeriod("C", buySignal)
            End If
            If sellOption = "rsi" Then
                sellRsiOrRocPeriod = findRsiorRocPeriod("I", sellSignal)
            ElseIf sellOption = "roc" Then
                sellRsiOrRocPeriod = findRsiorRocPeriod("C", sellSignal)
            End If
            If buyRsiOrRocPeriod < sellRsiOrRocPeriod Then
                highRsiOrRocPeriod = sellRsiOrRocPeriod
            Else : highRsiOrRocPeriod = buyRsiOrRocPeriod
            End If
        End If
        If buyOption = "macd" Or sellOption = "macd" Then
            hasMACD = True
            highMacd = 26
        End If
        If highMa > highRsiOrRocPeriod Then
            If highMa > highMacd Then
                sampleSize = highMa
            Else : sampleSize = highMacd
            End If
        ElseIf highRsiOrRocPeriod > highMacd Then
            sampleSize = highRsiOrRocPeriod
        Else : sampleSize = highMacd
        End If

        'Loop because sometimes yahoo doesn't get the data so we're going to try five times then give up.
        For i = 0 To 5 Step 1
            If hasMA = True And sampleSize = highMa Then
                'get data for EMA & SMA simulations
                If startDateOp = Nothing Then
                    startDate = calculateMaStartDate(sampleSize, range, Today)
                    stockData = New yahooData(ticker, startDate)
                Else
                    startDate = calculateMaStartDate(sampleSize, range, startDateOp)
                    stockData = New yahooData(ticker, startDate, endDateOP)
                End If
            ElseIf hasMACD = True And sampleSize = highMacd Then
                'use the MACD as the max
                If startDateOp = Nothing Then
                    stockData = New yahooData
                    '                stockData.loadDataByNumberDays(ticker, range + 30)
                    startDate = constantSetup(range, range + 30)
                    stockData = New yahooData(ticker, startDate)
                Else
                    startDateOp = startDateOp.AddMonths(-2)
                    stockData = New yahooData(ticker, startDateOp, endDateOP)
                End If
            ElseIf hasRsiRoc = True And sampleSize = highRsiOrRocPeriod Then
                'use the RSI or ROC as the max
                If startDateOp = Nothing Then
                    startDate = constantSetup(range, (highRsiOrRocPeriod * 2 + 50))
                    stockData = New yahooData(ticker, startDate)
                Else
                    startDateOp = startDateOp.AddDays(-(highRsiOrRocPeriod * 2 + 50))
                    stockData = New yahooData(ticker, startDateOp, endDateOP)
                End If
            End If
            If stockData.close.Count > 0 Then
                Exit For
            End If
        Next
        getRangeData(rangeStartDate, endDateOP, range)
    End Sub

    Sub getRangeData(ByVal rangeStartDate As Date, _
                 ByVal endDate As Date, _
                 ByVal range As Integer)

        Dim startCell As Integer = 0
        Dim endCell As Integer = 0

        If endDate = Nothing Then
            endDate = Today
        End If
        startCell = stockData.find_date(rangeStartDate.ToString("yyyy-MM-dd"))
        endCell = stockData.find_date(endDate.ToString("yyyy-MM-dd"))
        If range = 5 Then
            periodData = New rangeStockData(stockData, range)
        Else : startCell = stockData.find_date(Today.AddDays(-range))
            periodData = New rangeStockData(stockData, stockData.close.Count - startCell)
        End If
        If periodData.close.Count = 0 Then
            periodData = New rangeStockData(stockData, stockData.close.Count)
            simError = "From IPO"
        End If
    End Sub

    'Finds the Highest Moving Average
    Function findHighMa() As Integer
        Dim buyMa As Integer = 0
        Dim sellMa As Integer = 0

        If buyOption = "maAvg" Then
            buyMa = getHighestMaValue(buySignal)
        End If
        If sellOption = "maAvg" Then
            sellMa = getHighestMaValue(sellSignal)
        End If
        If buyMa < sellMa Then
            Return sellMa
        Else : Return buyMa
        End If
    End Function

    Function calculateMaStartDate(ByVal longestMa As Integer, _
                              ByVal days As Integer, _
                              ByVal firstDate As Date) As Date
        Dim startDate As Date
        Dim daySpan As New TimeSpan

        longestMa += (longestMa \ 5) * 2 + 20
        longestMa += (2 * longestMa) + ((longestMa \ 365) * longestMa) + days
        daySpan = New TimeSpan(longestMa, 0, 0, 0)
        startDate = firstDate.Subtract(daySpan)
        Return startDate
    End Function

    Function getHighestMaValue(ByVal maValue As String) As Integer
        Dim highMa As Integer = 0
        Dim range() As String = {"", ""}
        Dim ma1 As Integer = 0
        Dim ma2 As Integer = 0
        If maValue.Contains(">") Then
            range = maValue.Split(">")
        ElseIf maValue.Contains("<") Then
            range = maValue.Split("<")
        ElseIf maValue.Contains("%lt;") Then
            range = maValue.Split("%lt;")
        End If
        ma1 = getMaValue(range(0))
        ma2 = getMaValue(range(1))
        If ma1 < ma2 Then
            Return ma2
        Else : Return ma1
        End If
    End Function

    Function getMaValue(ByVal maString As String) As Integer
        Dim value As Integer = 0
        Try
            value = Convert.ToInt32(maString.Substring(3, maString.Count - 3))
        Catch ex As Exception
        End Try
        Return value
    End Function

    Function maSignalsSetup(ByVal istring As String) As List(Of String)
        Dim i As Integer = 0
        Dim startCell As Integer = 0
        Dim inputs As String() = {"", ""}
        Dim frontData As New List(Of Double) 'List(Of movingAverage)
        Dim backData As New List(Of Double)
        Dim maNumber As Integer = 0
        Dim switch As Boolean = False
        Dim foundSignals As New List(Of String)
        Try
            istring = istring.ToUpper
            If istring.Contains("<") Then
                inputs = istring.Split("<")
            ElseIf istring.Contains(">") Then
                inputs = istring.Split(">")
                switch = True
            End If
            If inputs(0).Contains("PRICE") Then
                frontData = stockData.close
            Else
                frontData = getMaInput(inputs(0))
            End If
            If inputs(1).Contains("PRICE") Then
                backData = stockData.close
            Else
                backData = getMaInput(inputs(1))
            End If

            startCell = stockData.sDate.IndexOf(periodData.sDate.First)

            'front and back data should be the same lenght and they should also be the lenght of stockData
            For i = startCell To frontData.Count - 1 Step 1
                'If backData(i) > 0 And
                '    frontData(i) > 0 Then
                If switch = True Then
                    If frontData(i) > backData(i) Then
                        'signals.Add(i & " " & periodData.close(i) & " " & periodData.sDate(i))
                        foundSignals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                    End If
                ElseIf frontData(i) < backData(i) Then
                    foundSignals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                End If
                'End If
            Next
        Catch ex As Exception
        End Try
        Return foundSignals
    End Function

    Function macdSignalSetup(ByVal signal As String, _
                             ByVal macd As List(Of Double), _
                             ByVal ema As List(Of Double), _
                             ByVal div As List(Of Double)) As List(Of String)
        Dim foundSignals As New List(Of String)
        Dim op1Select As Integer = -1
        Dim op2Select As Integer = -1
        Dim condition As Integer = -1
        Dim i As Integer = 0
        Dim startCell As Integer = 0
        Dim operands() As String
        Dim op1Data As New List(Of Double)
        Dim op2Data As New List(Of Double)
        Dim op2Number As Double = -99999999

        Try
            If signal.Contains("<") Then
                operands = signal.Split("<")
                condition = 0
            ElseIf signal.Contains(">") Then
                operands = signal.Split(">")
                condition = 1
            Else
                operands = signal.Split("=")
                condition = 2
            End If
            op1Select = opSelector(operands(0))
            'load op1Data with either macd, ema, or div
            op1Data = opDataLoad(op1Select, macd, ema, div)
            op2Select = opSelector(operands(1))
            'load op2Data with either macd, ema, div. If it's a number use op2Number
            If op2Select = -999 Then
                op2Number = Convert.ToDouble(operands(1))
            Else
                op2Data = opDataLoad(op2Select, macd, ema, div)
            End If
            startCell = stockData.sDate.IndexOf(periodData.sDate.First)
            If op1Data.Count > 0 And op2Data.Count > 0 Then
                For i = 0 To op1Data.Count - 1 Step 1
                    If condition = 0 Then 'op1data < op2data
                        If op1Data(i) < op2Data(i) Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If
                    ElseIf condition = 1 Then 'op1data > op2data
                        If op1Data(i) > op2Data(i) Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If

                    ElseIf condition = 2 Then 'op1data = op2data   
                        If op1Data(i) = op2Data(i) Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If
                    End If
                Next
            ElseIf op2Number <> -99999999 And op1Data.Count > 0 Then
                For i = 0 To op1Data.Count - 1 Step 1
                    If condition = 0 Then 'op1data < op2Number
                        If op1Data(i) < op2Number Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If
                    ElseIf condition = 1 Then 'op1data > op2Number
                        If op1Data(i) > op2Number Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If
                    ElseIf condition = 2 Then 'op1data = op2Number
                        If op1Data(i) = op2Number Then
                            foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
        Return foundSignals
    End Function

    Function opSelector(ByVal op As String) As Integer
        Dim opSelect As Integer = -1
        If op = "MACD" Then
            Return 0
        ElseIf op = "EMA" Then
            Return 1
        ElseIf op = "DIV" Then
            Return 2
        Else : Return -999
        End If
    End Function

    Function opDataLoad(ByVal op1 As Integer, _
                        ByVal macd As List(Of Double), _
                        ByVal ema As List(Of Double), _
                        ByVal div As List(Of Double)) As List(Of Double)
        If op1 = 0 Then
            Return macd
        ElseIf op1 = 1 Then
            Return ema
        ElseIf op1 = 2 Then
            Return div
        Else : Return Nothing
        End If
    End Function

    Function rsiorRocSignalSetup(ByVal signal As String, _
                                 ByVal rsiData As List(Of Double)) As List(Of String)
        Dim foundSignals As New List(Of String)
        Dim sigValue As Double = 0
        Dim condition As Integer = -1
        Dim startCell As Integer = 0
        Dim i As Integer = 0

        If signal.Contains("<") Then
            condition = 1
        Else : condition = 2
        End If
        sigValue = findRsiorRocValue(signal)
        startCell = stockData.sDate.IndexOf(periodData.sDate.First)
        For i = 0 To rsiData.Count - 1 Step 1
            If condition = 1 Then
                If rsiData(i) <= sigValue Then
                    foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                End If
            Else
                If rsiData(i) >= sigValue Then
                    foundSignals.Add(startCell + i & " " & twoDecimal(stockData.close(startCell + i)) & " " & stockData.sDate(startCell + i))
                End If
            End If
        Next
        Return foundSignals
    End Function

    Function findRsiorRocValue(ByVal signal As String) As Double
        Dim rsiValue As Integer = 0
        Dim rsiSplit() As String
        Dim rsiI As Integer = 0

        If signal.Contains("<") Then
            rsiSplit = signal.Split("<")
        Else : rsiSplit = signal.Split(">")
        End If
        Return Convert.ToDouble(rsiSplit(1))
    End Function

    Function getMaInput(ByVal maString As String) As List(Of Double)
        Dim maData As New List(Of Double)
        Dim maNumber As Integer = 0
        Dim mode As String = ""
        Dim maMember As New movingAverage
        Dim i As Integer = 0
        Dim hasValue = False

        mode = maString.Substring(0, 3)
        maNumber = getMaValue(maString)
        If mode = "SMA" Then
            maData = stockData.calculateSMA(maNumber)
        ElseIf mode = "EMA" Then
            maData = stockData.calculateEMA(maNumber)
        End If
        For i = 0 To maList.Count - 1 Step 1
            If maString = maList(i).name Then
                hasValue = True
            End If
        Next
        If hasValue = False Then
            maMember.name = maString
            maMember.values = periodData.graphDouble(maData)
            maList.Add(maMember)
        End If
        Return maData
    End Function

    Sub priceSignalSetup(ByRef lgCommand As Integer, ByRef trigger() As Double, ByVal value As String)
        Dim hasDash As Boolean = False

        If value <> "" Then
            If value.Contains("<") Then
                lgCommand = 1
            ElseIf value.Contains(">") Then
                lgCommand = 2
                'ElseIf value.Contains(",") Then
                '    lgCommand = 3
                'ElseIf value.Contains("%") Then
                '    lgCommand = 4
            End If
            If value.Contains("-") Then
                hasDash = True
            End If
            If lgCommand > 0 Then
                If hasDash = True Then
                    getTriggerValue(value.Substring(1), trigger)
                Else : trigger(0) = Convert.ToDouble(value.Substring(1))
                End If
            ElseIf hasDash = True Then
                getTriggerValue(value, trigger)
            Else
                trigger(0) = Convert.ToDouble(value)
            End If
        End If
    End Sub

    Sub getTriggerValue(ByVal inputValue As String, ByRef trigger() As Double)
        Dim inputSplit() As String
        Try
            inputSplit = inputValue.Split("-")
            trigger(0) = Convert.ToDouble(inputSplit(0))
            trigger(1) = Convert.ToDouble(inputSplit(1))
        Catch ex As Exception
        End Try
    End Sub

    'loads the stock value into the array buy/sell array 
    'Keeps track of where it is in periodData, close, and date, invest amount, sold for amount, cash gain, pecent gain
    'output: <cell # in periodData>, <Price>, <date>, <invested value>
    'output: if it's an exit entry then it will have <current investment> <cash gain> <percent gain> 
    '                             0                 1        2            3               4                 5            6
    'output: signal calls:       CELL             PRICE     DATE     BOUGHT               SOLD           CASHGAIN    PERCENTGAIN
    'output: buy signal:  <cell # in periodData>, <Price>, <date>, <invested value>
    'output: sell signal: <cell # in periodData>, <Price>, <date>, <invested value> <current investment> <cash gain> <percent gain> 
    'example output:buy -> "5 13 10/25/2010 1000"
    'example output:sell-> "10 16 11/12/2010 1000 1500 500 50"
    Function getSignals(ByVal lgCommand As Integer, ByVal btValue() As Double) As List(Of String)
        Dim signals As New List(Of String)
        Dim i As Integer
        Dim startCell As Integer
        Dim blah As Integer = 1
        ' lgcommand = 1 for less
        '           = 2 for greater
        ' value examples 10, <10, >10, 10-20
        Try
            startCell = stockData.sDate.IndexOf(periodData.sDate.First)
            For i = startCell To stockData.close.Count - 1 Step 1
                If stockData.sDate(i) = "2011-09-09" Then
                    blah = 1
                End If
                If lgCommand = 0 Then
                    If btValue(1) = 0 Then
                        If stockData.close(i) = btValue(0) Then
                            signals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                        End If
                    ElseIf stockData.close(i) >= btValue(0) And _
                            stockData.close(i) <= btValue(1) Then
                        signals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                    End If
                ElseIf lgCommand = 1 Then
                    If stockData.close(i) <= btValue(0) Then
                        signals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                    End If
                ElseIf lgCommand = 2 Then
                    If stockData.close(i) >= btValue(0) Then
                        signals.Add(i & " " & twoDecimal(stockData.close(i)) & " " & stockData.sDate(i))
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
        Return signals
    End Function

    Sub signalSync(ByRef bSigs As List(Of String), _
                   ByRef sSigs As List(Of String))
        'remove duplicates in both buy and sell signals
        Dim bDups As New List(Of Integer)
        Dim sDups As New List(Of Integer)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Try

            For i = 0 To bSigs.Count - 1 Step 1
                For j = 0 To sSigs.Count - 1 Step 1
                    If bSigs(i) = sSigs(j) Then
                        bDups.Add(i)
                        sDups.Add(j)
                    End If
                Next
            Next

            For i = bDups.Count - 1 To 0 Step -1
                bSigs.RemoveAt(bDups(i))
            Next
            For i = sDups.Count - 1 To 0 Step -1
                sSigs.RemoveAt(sDups(i))
            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub processSignals(ByVal bSig As List(Of String), _
                            ByVal sSig As List(Of String), _
                            ByRef results As List(Of String), _
                            Optional ByRef maxGain As Double = 0)

        Dim buyIndex As Integer = 0
        Dim sellIndex As Integer = 0
        Dim buyTrigger As Boolean = False
        Dim sellTrigger As Boolean = False
        'Dim results As New List(Of String)
        Dim buyLastDate As Date
        Dim sellLastDate As Date
        Dim lastDate As Date
        Dim currentDate As Date
        Dim currentResultsDate As Date
        Dim buyValue As Double = 0
        Dim sellValue As Double = 0
        Dim buyInvest As Double = 0
        Dim soldInvest As Double = 0
        Dim cashGain As Double = 0
        Dim percentGain As Double = 0
        Dim trade As String = ""
        Dim tradeSplit() As String
        Dim cashGainPerShare As Double = 0
        Dim percentGainPerShare As Double = 0
        Dim shareNum As Double = 0
        Dim lastSignalDate As String = ""
        Dim insertSellSignal As Boolean = False
        maxGain = 0
        Try
            lastTradePosition = "In"
            buyLastDate = signalDate(bSig.Last)
            trade = stockData.close.Count - 1 & " " & twoDecimal(stockData.close.Last) & " " & stockData.sDate.Last
            lastSignalDate = stockData.sDate.Last
            If sSig.Count = 0 Then
                insertSellSignal = True
                sSig.Add(trade)
            ElseIf buyLastDate > signalDate(sSig.Last) Then
                insertSellSignal = True
                sSig.Add(trade)
            End If
            sellLastDate = stockData.sDate.Last
            If buyLastDate > sellLastDate Then
                lastDate = buyLastDate
            Else : lastDate = sellLastDate
            End If
            currentDate = signalDate(bSig.First)
            'if they have trade cost enabled account for the cost of each trade
            If tradeCost <> 0 Then
                trade = bSig.First & " " & twoDecimal(Convert.ToDouble(capital) - Convert.ToDouble(tradeCost))
            Else
                trade = bSig.First & " " & twoDecimal(Convert.ToDouble(capital))
            End If
            results.Add(trade)
            buyTrigger = True
            lastTradeDate = currentDate.ToShortDateString

            While (currentDate < lastDate)
                If (buyTrigger = True) Then 'looking for sell signal
                    currentResultsDate = signalDate(results.Last)
                    currentDate = signalDate(sSig(sellIndex))
                    If currentDate > currentResultsDate Then
                        'process sell signal
                        buyTrigger = False
                        buyValue = signalPrice(results.Last) 'buy stock price
                        buyInvest = signalBought(results.Last) 'investment
                        sellValue = signalPrice(sSig(sellIndex)) 'sell stock price
                        shareNum = buyInvest / buyValue '# shares
                        cashGainPerShare = sellValue - buyValue
                        percentGainPerShare = ((sellValue - buyValue) / buyValue) * 100
                        If tradeCost > 0 Then
                            soldInvest = buyInvest + (buyInvest * (percentGainPerShare / 100)) - Convert.ToDouble(tradeCost)
                        Else
                            soldInvest = buyInvest + (buyInvest * (percentGainPerShare / 100))
                        End If
                        cashGain = soldInvest - buyInvest
                        percentGain = ((soldInvest - buyInvest) / buyInvest) * 100
                        maxGain += cashGain
                        trade = sSig(sellIndex) & " " & twoDecimal(buyInvest) & " " & twoDecimal(soldInvest) _
                                & " " & twoDecimal(cashGain) & " " & twoDecimal(percentGain)
                        results.Add(trade)
                        tradeSplit = trade.Split(" ")
                        If insertSellSignal = True Then
                            lastTradePosition = "In"
                            ' lastTradeDate = signalDate(results.Last).ToShortDateString
                        Else
                            lastTradePosition = "Out"
                            lastTradeDate = Convert.ToDateTime(signalDate(trade).ToString("MM/dd/yyyy"))
                        End If
                        'If tradeSplit(2) = lastSignalDate Then
                        '    lastTradePosition = "In"
                        'Else
                        'If bSig.Last = sSig(sellIndex) Then
                        '    lastTradePosition = "In"
                        '    lastTradeDate = signalDate(trade)
                        'If sSig.Count = 1 Then
                        '    lastTradePosition = "In"
                        'Else
                        '    lastTradePosition = "Out"
                        '    lastTradeDate = signalDate(trade)
                        'End If
                        'End If
                    End If
                    sellIndex += 1
                ElseIf (buyIndex < bSig.Count) Then                    'Looking for buy Signal
                    currentResultsDate = signalDate(results.Last)
                    currentDate = signalDate(bSig(buyIndex))
                    If currentDate > currentResultsDate Then
                        'process buy signal
                        buyTrigger = True
                        If tradeCost > 0 Then
                            buyInvest = signalSold(results.Last) - Convert.ToDouble(tradeCost) 'investment
                        Else
                            buyInvest = twoDecimal(signalSold(results.Last))
                        End If
                        results.Add(bSig(buyIndex) & " " & twoDecimal(buyInvest))
                        tradeSplit = results.Last.Split(" ")

                        lastTradeDate = Convert.ToDateTime(tradeSplit(2)).ToString("MM/dd/yyyy")
                        lastTradePosition = "In"
                    End If
                    buyIndex += 1
                Else : currentDate = signalDate(sSig.Last)
                End If
                If currentDate = signalDate(sSig.Last) Then
                    Exit While
                End If
            End While
        Catch ex As Exception
        End Try
        'Return results
    End Sub

    'Function processSignals2(ByVal bSig As List(Of String), ByVal sSig As List(Of String)) As List(Of String)
    '    Dim buyIndex As Integer = 0
    '    Dim sellIndex As Integer = 0
    '    Dim buyTrigger As Boolean = False
    '    Dim sellTrigger As Boolean = False
    '    Dim results As New List(Of String)
    '    Dim buyLastDate As Date
    '    Dim sellLastDate As Date
    '    Dim lastDate As Date
    '    Dim currentDate As Date
    '    Dim currentResultsDate As Date
    '    Dim buyValue As Double = 0
    '    Dim sellValue As Double = 0
    '    Dim buyInvest As Double = 0
    '    Dim soldInvest As Double = 0
    '    Dim cashGain As Double = 0
    '    Dim percentGain As Double = 0
    '    Dim trade As String = ""
    '    Dim tradeSplit() As String
    '    Dim cashGainPerShare As Double = 0
    '    Dim percentGainPerShare As Double = 0
    '    Dim shareNum As Double = 0
    '    Dim lastSignalDate As String = ""
    '    Dim x As Integer = 0
    '    Try
    '        lastTradePosition = "Out"
    '        buyLastDate = signalDate(bSig.Last)
    '        trade = stockData.close.Count - 1 & " " & twoDecimal(stockData.close.Last) & " " & stockData.sDate.Last
    '        lastSignalDate = stockData.sDate.Last
    '        sSig.Add(trade)
    '        sellLastDate = stockData.sDate.Last
    '        If buyLastDate > sellLastDate Then
    '            lastDate = buyLastDate
    '        Else : lastDate = sellLastDate
    '        End If
    '        currentDate = signalDate(bSig.First)
    '        'if they have trade cost enabled account for the cost of each trade
    '        If tradeCost <> 0 Then
    '            trade = bSig.First & " " & twoDecimal(Convert.ToDouble(capital) - Convert.ToDouble(tradeCost))
    '        Else
    '            trade = bSig.First & " " & twoDecimal(Convert.ToDouble(capital))
    '        End If
    '        results.Add(trade)
    '        buyTrigger = True
    '        lastTradeDate = currentDate.ToShortDateString

    '        While (sellIndex <> -1)
    '            sellIndex = signalSearchDate(sSig, signalDate(results.Last))
    '            If sellIndex <> -1 Then
    '                'process sell signal
    '                buyTrigger = False
    '                buyValue = signalPrice(results.Last) 'buy stock price
    '                buyInvest = signalBought(results.Last) 'investment
    '                sellValue = signalPrice(sSig(sellIndex)) 'sell stock price
    '                shareNum = buyInvest / buyValue '# shares
    '                cashGainPerShare = sellValue - buyValue
    '                percentGainPerShare = ((sellValue - buyValue) / buyValue) * 100
    '                If tradeCost <> 0 Then
    '                    soldInvest = buyInvest + (buyInvest * (percentGainPerShare / 100)) - Convert.ToDouble(tradeCost)
    '                Else
    '                    soldInvest = buyInvest + (buyInvest * (percentGainPerShare / 100))
    '                End If
    '                cashGain = soldInvest - buyInvest
    '                percentGain = ((soldInvest - buyInvest) / buyInvest) * 100
    '                trade = sSig(sellIndex) & " " & twoDecimal(buyInvest) & " " & twoDecimal(soldInvest) _
    '                        & " " & twoDecimal(cashGain) & " " & twoDecimal(percentGain)
    '                results.Add(trade)
    '                tradeSplit = trade.Split(" ")
    '                'If tradeSplit(2) = lastSignalDate Then
    '                '    lastTradePosition = "In"
    '                'Else
    '                If bSig.Last = sSig(sellIndex) Then
    '                    lastTradePosition = "In"
    '                Else
    '                    lastTradePosition = "Out"
    '                End If
    '                'sellIndex += 1
    '                'ElseIf (buyIndex < bSig.Count) Then                    'Looking for buy Signal
    '                'currentResultsDate = signalDate(results.Last)
    '                'currentDate = signalDate(bSig(buyIndex))
    '                '                If currentDate > currentResultsDate Then
    '                'process buy signal
    '            End If

    '            buyIndex = signalSearchDate(bSig, signalDate(results.Last))
    '            If buyIndex <> -1 Then
    '                buyTrigger = True
    '                If tradeCost <> 0 Then
    '                    buyInvest = signalSold(results.Last) - Convert.ToDouble(tradeCost) 'investment
    '                Else
    '                    buyInvest = twoDecimal(signalSold(results.Last))
    '                End If
    '                results.Add(bSig(buyIndex) & " " & twoDecimal(buyInvest))
    '                tradeSplit = results.Last.Split(" ")
    '                lastTradeDate = tradeSplit(2)
    '                lastTradePosition = "In"
    '            End If

    '            'End If
    '            'buyIndex += 1
    '            'Else : currentDate = signalDate(sSig.Last)
    '            'End If
    '        End While
    '    Catch ex As Exception
    '    End Try
    '    Return results
    'End Function

    Function signalSearchDate(ByVal sig As List(Of String), _
                              ByVal dateSearch As String) As Integer
        Dim foundSignal As Integer = 0
        Dim i As Integer = 0
        Dim sigDate As Date = signalDate(sig(i))
        Dim search As Date = Convert.ToDateTime(dateSearch)

        While sigDate <= search
            i += 1
            Try
                sigDate = signalDate(sig(i))
            Catch ex As Exception
                i = -1
                Exit While
            End Try
        End While
        Return i

    End Function


    Sub loadResults() 'ByVal tradeResults As List(Of String))
        Dim i As Integer = 0
        Dim firstDate As DateTime
        Dim lastDate As DateTime
        Dim timeVested As TimeSpan
        Dim cashGains As Double = 0
        Dim percentGains As Double = 0
        Dim currentInvest As Double = 0
        Dim winTrade As Integer = 0
        Dim loseTrade As Integer = 0
        Dim bestTrade As String = ""
        Dim worstTrade As String = ""
        Dim bestGain As Integer = 0
        Dim worstGain As Integer = 0
        Dim winStreak As Integer = 0
        Dim loseStreak As Integer = 0
        Dim winTime As TimeSpan = TimeSpan.Zero
        Dim loseTime As TimeSpan = TimeSpan.Zero
        Dim longTime As TimeSpan = TimeSpan.Zero
        Dim shortTime As TimeSpan = TimeSpan.Zero
        Dim totalPercentGain As Double = 0

        Try
            If tradeResults.Count > 0 Then
                'calculate different values for output
                summaryValues(timeVested, cashGains, percentGains, winTrade, loseTrade, bestTrade, _
                              worstTrade, bestGain, worstGain, winStreak, loseStreak, winTime, loseTime, longTime, shortTime)

                '**************** SUMMARY OUTPUT **************************
                '0 - Ticker
                summaryResults(0, 1) = ticker

                '1 - Initial Investment           
                summaryResults(1, 1) = twoDecimal(Convert.ToDouble(capital))

                '2 - Current Investment
                If signalSold(tradeResults.Last) = 0 Then
                    currentInvest = twoDecimal(Convert.ToDouble(signalBought(tradeResults.Last)))
                Else
                    currentInvest = twoDecimal(Convert.ToDouble(signalSold(tradeResults.Last)))
                End If
                summaryResults(2, 1) = twoDecimal(currentInvest)
                'If currentInvest = 0.0 Then

                'End If
                'For i = 0 To percentGains.Count - 1 Step 1
                '    currentInvest += currentInvest * cashGains(i)
                'Next
                'summaryResults(2, 1) = signalSold(tradeResults.Last)

                '3 - Time Vested
                summaryResults(3, 1) = timeVested.Days & " Days"

                '4 - total Trading Time
                'Calculate time
                firstDate = Convert.ToDateTime(signalDate(tradeResults.First))
                lastDate = Convert.ToDateTime(signalDate(tradeResults.Last))
                If firstDate = lastDate Then
                    lastDate = Today
                End If
                Dim daysSpan As Long
                daysSpan = DateDiff(DateInterval.Day, firstDate, lastDate)
                summaryResults(4, 1) = daysSpan.ToString & " Days"

                '5 - Date Range
                summaryResults(5, 1) = firstDate.ToString("MMM-dd-yyyy") & " To " & lastDate.ToString("MMM-dd-yyyy")

                'If stillVested = True Then
                '    '6 - Current Status
                '    summaryResults(6, 1) = "Vested"
                'Else
                '    '6 - Current Status
                '    summaryResults(6, 1) = "Not Vested"
                'End If

                '*********************BUY SELL SIGNALS ARE ASSIGNED IN PROCEDURE setupTrade
                ''7 - Buy Signal
                summaryResults(6, 1) = buySignal 'twoDecimal(Convert.ToDouble(priceBuy.Text))

                ''8 - Sell Signal
                summaryResults(7, 1) = sellSignal 'twoDecimal(Convert.ToDouble(priceSell.Text))

                '9 - Total Trade Cost
                Try
                    summaryResults(8, 1) = twoDecimal(tradeResults.Count * Convert.ToDouble(tradeCost))
                Catch ex As Exception
                    summaryResults(8, 1) = ""
                End Try

                '******************** GAIN OUTPUT ***********************************************
                'gainsResults
                '0 - total Gain
                'Dim totalCashGain = currentInvest - Convert.ToDouble(capital)
                Dim outputCashGain As Double = 0
                Dim outputPercentGain As Double = 0
                outputCashGain = currentInvest - Convert.ToDouble(summaryResults(1, 1)) 
                outputPercentGain = (outputCashGain / Convert.ToDouble(summaryResults(1, 1))) * 100
                cashGains = outputCashGain
                percentGains = outputPercentGain
                'cashGains, percentGains
                gainsResults(0, 1) = twoDecimal(outputCashGain) & "   ( " & twoDecimal(outputPercentGain) & " %)"

                '1 - Best Gain
                gainsResults(1, 1) = twoDecimal(signalCashGain(tradeResults(bestGain))) & "   ( " & twoDecimal(signalPercentGain(tradeResults(bestGain))) & " %)"

                '2 - Worst Gain
                gainsResults(2, 1) = twoDecimal(signalCashGain(tradeResults(worstGain))) & "   ( " & twoDecimal(signalPercentGain(tradeResults(worstGain))) & " %)"

                '3 - Cash Gain stopError Day
                gainsResults(3, 1) = twoDecimal(cashGains / timeVested.TotalDays) & "   ( " & twoDecimal(percentGains / timeVested.TotalDays) & " %)"

                '4 - Cash Gain stopError Month
                Dim totalMonths = timeVested.TotalDays \ 30
                Dim totalYears = totalMonths \ 12
                If totalMonths > 0 Then
                    gainsResults(4, 1) = twoDecimal(cashGains / totalMonths) & "   ( " & twoDecimal(percentGains / totalMonths) & " %)"
                Else
                    gainsResults(4, 1) = twoDecimal(cashGains) & "   ( " & twoDecimal(percentGains) & " %)"
                End If

                '3 - Cash Gain stopError Year
                If totalYears > 0 Then
                    gainsResults(5, 1) = twoDecimal(cashGains / totalYears) & "   ( " & twoDecimal(percentGains / totalYears) & " %)"
                Else : gainsResults(5, 1) = twoDecimal(cashGains) & "   ( " & twoDecimal(percentGains) & " %)"
                End If

                ''4 - Total percent gain
                'totalPercentGain = ((currentInvest - Convert.ToDouble(investValue.Text)) / Convert.ToDouble(investValue.Text)) * 100
                'gainsResults(4, 1) = String.Format("{0:n2}", totalPercentGain)

                '************************ TRADE SUMMARY **********************************************
                '0 # Trades
                tradeSummary(0, 1) = tradeResults.Count

                '1 Winning 
                tradeSummary(1, 1) = winTrade * 2

                '2 Losing
                tradeSummary(2, 1) = loseTrade * 2

                '3 Win Percent
                tradeSummary(3, 1) = twoDecimal((winTrade / (winTrade + loseTrade)) * 100)

                '4 Longest 
                tradeSummary(4, 1) = longTime.TotalDays & " Days"

                '5 Shortest
                tradeSummary(5, 1) = shortTime.TotalDays & " Days"

                '6 Best
                tradeSummary(6, 1) = bestTrade

                '7 Worst
                tradeSummary(7, 1) = worstTrade

                '8 Winning Streak
                tradeSummary(8, 1) = winStreak

                '9 Losing Streak
                tradeSummary(9, 1) = loseStreak

                'If stillVested = False Then
                '10 Avg Gain stopError Trade
                tradeSummary(10, 1) = twoDecimal(cashGains / tradeResults.Count) & "  ( " & twoDecimal(percentGains / tradeResults.Count) & " %)"

                '11 Avg Held Time
                tradeSummary(11, 1) = twoDecimal(timeVested.TotalDays / (tradeResults.Count / 2)) & " " & "Days"
                'Else
                ''10 Avg Gain stopError Trade
                'tradeSummary(10, 1) = twoDecimal(cashGains / (tradeResults.Count - 1)) & "  ( " & twoDecimal(percentGains / (tradeResults.Count - 1)) & " %)"

                ''11 Avg Held Time
                'tradeSummary(11, 1) = twoDecimal(timeVested.TotalDays / (tradeResults.Count - 1))
                'End If

                '12 Avg Win Held Time
                If winTrade <> 0 Then
                    tradeSummary(12, 1) = twoDecimal(winTime.TotalDays / winTrade) & " " & "Days"
                Else : tradeSummary(12, 1) = "0.00"
                End If

                '13 Avg Lose Held Time
                If loseTrade <> 0 Then
                    tradeSummary(13, 1) = twoDecimal(loseTime.TotalDays / loseTrade) & " " & "Days"
                Else : tradeSummary(13, 1) = "0.00"
                End If

                'Some sort of bug can't use "<" in a table need to replace it with &lt;
                'the value will still have the "<" but when displayed in a tablecell it will
                'just show whatever is before the "<" blah2<blah1 becomes blah2. to get 
                'blah2<blah1 you need to do blah2&lt;blah1
                If summaryResults(6, 1).Contains("<") Then
                    summaryResults(6, 1) = summaryResults(6, 1).Replace("<", "&lt;")
                End If
                If summaryResults(7, 1).Contains("<") Then
                    summaryResults(7, 1) = summaryResults(7, 1).Replace("<", "&lt;")
                End If

                'load table with gain results
                For i = 11 To 16 Step 1
                    If gainsResults(i - 11, 0).Contains("stopError") Then
                        gainsResults(i - 11, 0) = gainsResults(i - 11, 0).Replace("stopError", "")
                    End If
                Next

                'load table with trade results
                For i = 1 To 14 Step 1
                    If tradeSummary(i - 1, 0).Contains("stopError") Then
                        tradeSummary(i - 1, 0) = tradeSummary(i - 1, 0).Replace("stopError", "")
                    End If
                Next
            Else
            End If
        Catch ex As Exception
        End Try
    End Sub

    Sub signalSeperater(ByVal signal As String, ByRef sValue As Double, ByRef sDate As Date)
        Dim signalSplit() As String
        Try
            signalSplit = signal.Split(" ")
            sValue = Convert.ToDouble(signalSplit(1))
            sDate = Convert.ToDateTime(signalSplit(2))
        Catch ex As Exception
        End Try
    End Sub

    Function signalCell(ByVal signal As String) As Integer
        Dim signalSplit() As String
        Dim cell As Integer
        Try
            signalSplit = signal.Split(" ")
            cell = Convert.ToInt32(signalSplit(0))
        Catch ex As Exception
        End Try
        Return cell
    End Function

    Function signalPrice(ByVal signal As String) As Double
        Dim signalSplit() As String
        Dim price As Double
        Try
            signalSplit = signal.Split(" ")
            price = Convert.ToDouble(signalSplit(1))
        Catch ex As Exception
        End Try
        Return price
    End Function

    Function signalDate(ByVal signal As String) As Date
        Dim signalSplit() As String
        Dim sDate As Date
        Try
            signalSplit = signal.Split(" ")
            sDate = Convert.ToDateTime(signalSplit(2))
        Catch ex As Exception
        End Try
        Return sDate
    End Function

    Function signalBought(ByVal signal As String) As Double
        Dim signalSplit() As String
        Dim price As Double
        Try
            signalSplit = signal.Split(" ")
            price = Convert.ToDouble(signalSplit(3))
        Catch ex As Exception
        End Try
        Return price
    End Function

    Function signalSold(ByVal signal As String) As Double
        Dim signalSplit() As String
        Dim price As Double
        Try
            signalSplit = signal.Split(" ")
            price = Convert.ToDouble(signalSplit(4))
        Catch ex As Exception
        End Try
        Return price
    End Function

    Function signalCashGain(ByVal signal As String) As Double
        Dim signalSplit() As String
        Dim price As Double
        Try
            signalSplit = signal.Split(" ")
            price = Convert.ToDouble(signalSplit(5))
        Catch ex As Exception
        End Try
        Return price
    End Function

    Function signalPercentGain(ByVal signal As String) As Double
        Dim signalSplit() As String
        Dim price As Double
        Try
            signalSplit = signal.Split(" ")
            price = Convert.ToDouble(signalSplit(6))
        Catch ex As Exception
        End Try
        Return price
    End Function

    Sub summaryValues(ByRef vestTime As TimeSpan, _
                   ByRef cGains As Double, _
                   ByRef pGains As Double, _
                   ByRef wTrade As Integer, _
                   ByRef lTrade As Integer, _
                   ByRef beTrade As String, _
                   ByRef loTrade As String, _
                   ByRef bGain As Integer, _
                   ByRef lGain As Integer, _
                   ByRef wStreak As Double, _
                   ByRef lStreak As Double, _
                   ByRef wTime As TimeSpan, _
                   ByRef lTime As TimeSpan, _
                   ByRef lonTime As TimeSpan, _
                   ByRef shoTime As TimeSpan)

        Dim i As Integer = 0
        Dim currentTime As TimeSpan = TimeSpan.Zero
        Dim curLongTime As TimeSpan = TimeSpan.Zero
        Dim curShortTime As TimeSpan = TimeSpan.Zero
        Dim last As Integer = 0

        Dim currentCashGain As Double = 0
        Dim currentPercentGain As Double = 0
        Dim onWin As Boolean = False
        Dim onLose As Boolean = False
        Dim currentWinStreak As Integer = 0
        Dim currentLoseStreak As Integer = 0
        Dim bestCashGain As Double = 0
        Dim bestGain As Double = 0
        Dim worstGain As Double = 100
        Try
            For i = 1 To tradeResults.Count - 1 Step 2
                vestTime += signalDate(tradeResults(i)) - signalDate(tradeResults(i - 1))
                currentCashGain = signalCashGain(tradeResults(i))
                currentPercentGain = signalPercentGain(tradeResults(i))
                currentTime = signalDate(tradeResults(i)) - signalDate(tradeResults(i - 1))
                cGains += currentCashGain
                pGains += currentPercentGain
                If currentTime > lonTime Then
                    lonTime = currentTime
                End If
                If currentTime < shoTime Or shoTime = TimeSpan.Zero Then
                    shoTime = currentTime
                End If
                If currentPercentGain > 0 Then
                    onWin = True
                    onLose = False
                    If i = 1 Then
                        wStreak = 1
                    End If
                    If lStreak < currentLoseStreak Then
                        lStreak = currentLoseStreak
                    End If
                    currentLoseStreak = 0
                    wTrade += 1
                    wTime = wTime + currentTime
                Else
                    onWin = False
                    onLose = True
                    If i = 1 Then
                        lStreak = 1
                    End If
                    If wStreak < currentWinStreak Then
                        wStreak = currentWinStreak
                    End If
                    currentWinStreak = 0
                    lTrade += 1
                    lTime = lTime + currentTime
                End If
                If onWin = True Then
                    currentWinStreak += 1
                ElseIf onLose = True Then
                    currentLoseStreak += 1
                End If
                If bestGain < currentPercentGain Then
                    bestGain = currentPercentGain
                    bGain = i
                    beTrade = signalPrice(tradeResults(i - 1)) & " " & signalDate(tradeResults(i - 1)).ToString("MMM-dd-yyyy") & " To " _
                                & signalPrice(tradeResults(i)) & " " & signalDate(tradeResults(i)).ToString("MMM-dd-yyyy")
                End If
                If worstGain > currentPercentGain Then
                    worstGain = currentPercentGain
                    lGain = i
                    loTrade = signalPrice(tradeResults(i - 1)) & " " & signalDate(tradeResults(i - 1)).ToString("MMM-dd-yyyy") & " To " _
                                & signalPrice(tradeResults(i)) & " " & signalDate(tradeResults(i)).ToString("MMM-dd-yyyy")
                End If
            Next
            If beTrade = "" Then
                beTrade = loTrade
            ElseIf loTrade = "" Then
                loTrade = beTrade
            End If
            If currentWinStreak > wStreak Then
                wStreak = currentWinStreak
            End If
            If currentLoseStreak > lStreak Then
                lStreak = currentLoseStreak
            End If
        Catch ex As Exception
        End Try
    End Sub
End Class
