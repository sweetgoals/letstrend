Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports System.Globalization
Imports System.Collections.Specialized


Public Class yahooData
    Public ticker As String
    Public close, highs, lows, cash_gain, per_gain, volume, opens, day_per_change As New List(Of Double)
    Public slowStoch, fastStoch, fullStoch As List(Of Double)
    Public sDate As New List(Of String)
    Public quoteInfo As Boolean = False '(Name - Volume) loaded into stock_info 
    Public fundamentalInfo As Boolean = False '(Ask - Dividend Yield) Loaded into stock_info
    Public stock_info(,) As String = {{"Name", ""},
                                      {"Last Trade", ""},
                                      {"Prev Close", ""},
                                      {"Change", ""},
                                      {"% Change", ""},
                                      {"Volume", ""},
                                      {"Ask", ""},
                                      {"Bid", ""},
                                      {"Avg Daily vol(3m)", ""},
                                      {"Day's Low", ""},
                                      {"Day's High", ""},
                                      {"52wk High", ""},
                                      {"Chg fm 52wk High", ""},
                                      {"%Chg fm 52wk High", ""},
                                      {"52wk Low", ""},
                                      {"Chg fm 52wk Low", ""},
                                      {"%Chg fm 52wk Low", ""},
                                      {"EPS Diluted (ttm)", ""},
                                      {"EPS est next year", ""},
                                      {"EPS est next qrtr", ""},
                                      {"EPS est cur year", ""},
                                      {"Div Per Share", ""},
                                      {"Market Cap", ""},
                                      {"Price / Sales", ""},
                                      {"Price / Book", ""},
                                      {"P/E Ratio", ""},
                                      {"PEG Ratio", ""},
                                      {"Price / EPS est curr yr", ""},
                                      {"Year Target", ""},
                                      {"Stock Exchange", ""},
                                      {"Dividend Yield", ""}}
    Dim StochOption As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal stockTicker As String)
        ticker = stockTicker.ToUpper
        GetDataDay()
    End Sub

    Public Sub New(ByVal stockTicker As String,
                   ByVal startDate As Date)
        Dim webLink As String

        ticker = stockTicker.ToUpper
        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & startDate.Month - 1 & "&b=" & startDate.Day & "&c=" & _
            startDate.Year & "&d=" & Today.Month - 1 & "&e=" & Today.Day & "&f=" & Today.Year & "&g=d&s=" & _
            stockTicker & "&ignore=.csv"
        GetDataByURL(webLink)
        GetDataDay()
    End Sub

    'New Function 2/11;
    Public Sub New(ByVal stockTicker As String,
               ByVal startDate As String)
        Dim webLink As String
        ticker = stockTicker.ToUpper
        webLink = urlBuilderStartDate(startDate)
        GetDataByURL(webLink)
        GetDataDay()
    End Sub

    Public Sub New(ByVal stockTicker As String, _
                   ByVal startDate As Date, _
                   ByVal endDate As Date)
        Dim weblink As String
        ticker = stockTicker.ToUpper
        weblink = urlBuilderRangeDate(startDate, endDate)
        GetDataByURL(weblink)
        GetDataDay()
    End Sub

    Public Sub New(ByVal stockTicker As String, _
               ByVal startDate As String, _
               ByVal endDate As String)
        Dim weblink As String      
        ticker = stockTicker.ToUpper
        weblink = urlBuilderRangeDate(startDate, endDate)
        GetDataByURL(weblink)
        GetDataDay()
    End Sub

    Sub breakDate(ByVal inDate As Date, _
                  ByRef monthBreak As Integer, _
                  ByRef dayBreak As Integer, _
                  ByRef yearBreak As Integer)
        monthBreak = inDate.Month
        monthBreak -= 1
        dayBreak = inDate.Day
        yearBreak = inDate.Year
    End Sub

    Function getMonth(ByVal inDate As Date) As Integer
        Dim monthBreak As Integer
        monthBreak = inDate.Month
        monthBreak -= 1
        Return monthBreak
    End Function

    'Makes the string for the data url that is used to get the data from yahoo.
    'dates need to minus one month because yahoo starts at 0 months instead of 1. 

    Function urlBuilderStartDate(ByVal startDate As String) As String
        'Dim firstDate As Date = startDate.AddMonths(-1)
        'Dim endDate As Date = Today.AddMonths(-1)
        Dim webLink As String
        Dim linkStartDate As Date = Convert.ToDateTime(startDate)

        If linkStartDate = Date.Today Then
            linkStartDate.AddMonths(-1)
        End If
        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & getMonth(linkStartDate) & "&b=" & linkStartDate.Day & "&c=" &
                    linkStartDate.Year & "&d=" & getMonth(Today) & "&e=" & Today.Day & "&f=" & Today.Year & "&g=d&s=" & ticker &
                    "&ignore=.csv"
        Return webLink
    End Function

    Function urlBuilderStartDate(ByVal startDate As Date) As String
        'Dim firstDate As Date = startDate.AddMonths(-1)
        'Dim endDate As Date = Today.AddMonths(-1)
        Dim webLink As String

        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & getMonth(startDate) & "&b=" & startDate.Day & "&c=" &
                    startDate.Year & "&d=" & getMonth(Today) & "&e=" & Today.Day & "&f=" & Today.Year & "&g=d&s=" & ticker &
                    "&ignore=.csv"
        Return webLink
    End Function

    'Makes the string for the data url that is used to get data from yahoo. 
    Function urlBuilderRangeDate(ByVal startDate As Date, _
                                 ByVal endDate As Date) As String
        Dim webLink As String
        'startDate = startDate.AddMonths(-1)
        'endDate = endDate.AddMonths(-1)
        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & getMonth(startDate) & "&b=" & startDate.Day & "&c=" &
                    startDate.Year & "&d=" & getMonth(endDate) & "&e=" & endDate.Day & "&f=" & endDate.Year & "&g=d&s=" &
                    ticker & "&ignore=.csv"
        Return webLink
    End Function

    'Overload to accept strings instead of Dates
    '***** NEW FUNCTION 2/11
    Function urlBuilderRangeDate(ByVal startDate As String, _
                                 ByVal endDate As String) As String
        Dim webLink As String
        'startDate = startDate.AddMonths(-1)
        'endDate = endDate.AddMonths(-1)
        Dim linkStartDate As Date = Convert.ToDateTime(startDate)
        Dim linkEndDate As Date = Convert.ToDateTime(endDate)

        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & getMonth(linkStartDate) & "&b=" & linkStartDate.Day & "&c=" &
                    linkStartDate.Year & "&d=" & getMonth(linkEndDate) & "&e=" & linkEndDate.Day & "&f=" & linkEndDate.Year & "&g=d&s=" &
                    ticker & "&ignore=.csv"
        Return webLink
    End Function

    'Makes a url string from today minus the numberDays provided. 
    Function urlBuilderDays(ByVal numberDays As Integer) As String
        Dim webLink As String
        Dim startDate As Date = Today.AddDays(-numberDays)
        startDate = startDate.AddMonths(-1)
        Dim endDate As Date = Today
        endDate = Today.AddMonths(-1)
        webLink = "http://ichart.finance.yahoo.com/table.csv?a=" & startDate.Month & "&b=" & startDate.Day & "&c=" & startDate.Year & _
                    "&d=" & endDate.Month & "&e=" & endDate.Day & "&f=" & endDate.Year & "&g=d&s=" & ticker & "&ignore=.csv"
        Return webLink
    End Function


    'gets the stocks quote for the day and the statistics for the stock.
    Sub GetDataDay()
        Dim success As Boolean = False
        Dim i As Integer = 0

        success = requestStockDay()
        While (success = False)
            If i = 5 Then
                Exit While
            End If
            i += 1
            System.Threading.Thread.Sleep(200)
            success = requestStockDay()
        End While
        'load the ticker into the companyList table if it's not already in it.
        If stock_info(0, 1) <> "" And stock_info(1, 1) <> "" And stock_info(1, 1) <> "0.00" Then
            sqlCallCommand.Connection = con
            con.Open()
            sqlCallCommand.CommandText = "SELECT COUNT(*) FROM bobpar.companyList WHERE Ticker='" & ticker & "'"
            sqlResult = sqlCallCommand.ExecuteScalar()
            If sqlResult = 0 Then
                sqlCallCommand.CommandText = "SELECT COUNT(*) FROM bobpar.companyList"
                sqlResult = sqlCallCommand.ExecuteScalar()
                sqlCallCommand.Parameters.Add("@companyName", SqlDbType.NVarChar)
                sqlCallCommand.Parameters.Add("@ticker", SqlDbType.NChar, 10)

                sqlCallCommand.Parameters("@companyName").Value = stock_info(0, 1)
                sqlCallCommand.Parameters("@ticker").Value = ticker
                sqlCallCommand.CommandText = "INSERT INTO bobpar.companyList(companyName, ticker) " & _
                    "VALUES (@companyName, @ticker)"
                sqlResult = sqlCallCommand.ExecuteNonQuery()
            End If
            con.Close()
        End If
    End Sub

    Function requestStockDay() As Boolean
        Dim yahoo_data As String 'holds the data from yahoo
        Dim splitYahooData() As String 'splits each piece of information from yahoo into strings
        Dim url As String = "http://finance.yahoo.com/d/quotes.csv?s=" & ticker & "&f=npl1vc1p2b2b3a2ghkk4k5jj5j6ee8e9e7dj1p5p6rr5r6t8xy"
        Dim reader As StreamReader
        Dim request As HttpWebRequest = HttpWebRequest.Create(url)
        Dim i As Integer = 0
        Dim dow_hist As New List(Of String)
        Dim color_switch As Integer = 0
        Dim response As HttpWebResponse = request.GetResponse()
        Dim splitCellTwo() As String
        Dim num_vol As Int64
        Dim InvC As CultureInfo

        request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.2) Gecko/20070219 Firefox/2.0.0.2;"
        request.Method = WebRequestMethods.Http.Get
        reader = New StreamReader(response.GetResponseStream())
        yahoo_data = reader.ReadToEnd()
        response.Close()
        splitYahooData = yahoo_data.Split("""")
        Try
            Try
                stock_info(0, 1) = splitYahooData(1) 'name
                stock_info(4, 1) = splitYahooData(3) 'Percent change
                'split the string in cell 2 
                splitCellTwo = splitYahooData(2).Split(",")
                'temp2 =
                stock_info(2, 1) = splitCellTwo(1) 'previous close
                stock_info(1, 1) = splitCellTwo(2) 'last trade
                num_vol = Convert.ToInt64(splitCellTwo(3))
                InvC = CultureInfo.InvariantCulture

                stock_info(5, 1) = num_vol.ToString("#,#", InvC) 'Volume 
                stock_info(3, 1) = splitCellTwo(4) 'price change             
                quoteInfo = True
            Catch
                quoteInfo = False
            End Try
            If stock_info(3, 1).IndexOf("+") > -1 Then
                color_switch = 1
            ElseIf stock_info(3, 1).IndexOf("-") > -1 Then
                color_switch = -1
            End If
            'split the string in cell 4 of splitYahooData
            Dim splitCellFour() As String = splitYahooData(4).Split(",")
            For i = 6 To 28 Step 1
                stock_info(i, 1) = splitCellFour(i - 5)
            Next
            stock_info(29, 1) = splitYahooData(5)
            stock_info(30, 1) = splitYahooData(6)
            'check for fundamentals were found
            If stock_info(6, 1) <> "" And stock_info(6, 1) <> "N/A" Then
                fundamentalInfo = True
            End If
            Return True
        Catch ex As Exception
            fundamentalInfo = False
            quoteInfo = False
            Return False
        End Try
        Return False
    End Function


    Sub GetDataByURL(ByVal his_stock As String)
        Dim success As Boolean = False
        Dim i As Integer = 0

        success = getStockData(his_stock)
        While (success = False)
            If i = 5 Then
                Exit While
            End If
            i += 1
            System.Threading.Thread.Sleep(200)
            success = getStockData(his_stock)
        End While
        If success = True Then
            sqlCallCommand.Connection = con
            con.Open()
            sqlCallCommand.Parameters.Add("@ticker", SqlDbType.NChar, 10)
            sqlCallCommand.Parameters("@ticker").Value = ticker
            sqlCallCommand.CommandText = "INSERT INTO bobpar.tickersEnteredForDay(ticker) VALUES (@ticker)"
            sqlResult = sqlCallCommand.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    Function getStockData(ByVal his_stock As String) As Boolean
        Dim response As HttpWebResponse
        Dim reader As StreamReader
        Dim request As HttpWebRequest
        Dim temp() As String
        Dim i As Integer = 0

        Try
            'his_stock = "http://ichart.finance.yahoo.com/table.csv?a=" & startd.Month & "&b=" & startd.Day & "&c=" & startd.Year & "&d=" & Today.Month & "&e=" & Today.Day & "&f=" & Today.Year & "&g=d&s=" & TextBox1.Text & "&ignore=.csv"
            request = HttpWebRequest.Create(his_stock)
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.2) Gecko/20070219 Firefox/2.0.0.2;"
            request.Method = WebRequestMethods.Http.Get
            response = request.GetResponse()
            reader = New StreamReader(response.GetResponseStream())
            reader.ReadLine()
            While reader.EndOfStream = False
                temp = reader.ReadLine().Split(",")
                If temp(5) <> 0 Then
                    sDate.Add(temp(0))
                    opens.Add(String.Format("{0:n2}", temp(1)))
                    highs.Add(String.Format("{0:n2}", temp(2)))
                    lows.Add(String.Format("{0:n2}", temp(3)))
                    close.Add(String.Format("{0:n2}", temp(4)))
                    volume.Add(temp(5))
                End If
            End While
            response.Close()
            sDate.Reverse()
            opens.Reverse()
            highs.Reverse()
            lows.Reverse()
            close.Reverse()
            volume.Reverse()
            'calculate the gains
            '((Sell-bought)/bought)*100
            For i = 0 To close.Count - 1 Step 1
                day_per_change.Add(String.Format("{0:n2}", ((close(i) - opens(i)) / opens(i)) * 100))
                If i > 0 And i < close.Count Then
                    cash_gain.Add(String.Format("{0:n2}", (close(i) - close(i - 1))))
                    per_gain.Add(String.Format("{0:n2}", (((close(i) - close(i - 1)) / close(i - 1)) * 100)))
                End If
            Next
            Return True
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function

    'finds the cell that the end date is in so the graph can get the same values from the close list.
    Function find_date(ByVal dateFind As String) As Integer
        Dim currentDate As DateTime
        Dim currentDateString As String = ""
        Dim findCell As Integer = -1
        Try
            currentDate = Convert.ToDateTime(dateFind)
            currentDateString = currentDate.ToString("yyyy-MM-dd")
            findCell = sDate.IndexOf(currentDateString)
            While (findCell < 0)
                currentDate = currentDate.AddDays(-1)
                currentDateString = currentDate.ToString("yyyy-MM-dd")
                findCell = sDate.IndexOf(currentDateString)
            End While
        Catch ex As Exception
        End Try
        Return findCell
    End Function

    Function findDate(ByVal daysMinus As Integer) As Integer
        Dim startCell As Integer = 0
        Dim refTime As New TimeSpan
        Dim startDate As New Date

        Try
            refTime = New TimeSpan(daysMinus, 0, 0, 0)
            startDate = Today.Subtract(refTime)
            startCell = find_date(Today.AddDays(-daysMinus))
        Catch ex As Exception
        End Try
        Return startCell
    End Function

    'loads the yahoo data from today - numberdays to today
    Sub loadDataByNumberDays(ByVal symbol As String, _
                             ByVal numberDays As Integer)
        Dim urlCall As String

        ticker = symbol
        urlCall = urlBuilderDays(numberDays)
        GetDataByURL(urlCall)
    End Sub

    Public Function twoDecimal(ByVal number As Double) As String
        Return String.Format("{0:n2}", number)
    End Function

    Public Function calculateSMA(ByVal smaValue As Integer, _
                                 Optional ByVal dataList As List(Of Double) = Nothing) As List(Of Double)
        Dim sma As New List(Of Double)
        Dim sum As Double = 0
        Dim oneSMA As New List(Of Double)
        Dim avg As Double = 0
        Dim i As Integer = 0
        Try
            If Not dataList Is Nothing Then
                For i = 0 To smaValue - 2 Step 1
                    sma.Add(0)
                Next
                For i = smaValue To dataList.Count Step 1
                    oneSMA = dataList.GetRange(i - smaValue, smaValue)
                    sum = oneSMA.Sum
                    avg = sum / smaValue
                    sma.Add(twoDecimal(avg)) 'String.Format("{0:n2}", avg))
                Next
            Else
                For i = 0 To smaValue - 2 Step 1
                    sma.Add(0)
                Next
                For i = smaValue To close.Count Step 1
                    'sm_arr = stockData.close.GetRange(i - (sm - 1), sm)
                    oneSMA = close.GetRange(i - smaValue, smaValue)
                    sum = oneSMA.Sum
                    avg = sum / smaValue
                    sma.Add(twoDecimal(avg)) 'String.Format("{0:n2}", avg))
                Next
            End If
        Catch ex As Exception
            Return New List(Of Double)
        End Try
        Return sma
    End Function

    Function calculateEMA(ByVal value As Integer, _
                          Optional ByVal dataList As List(Of Double) = Nothing) As List(Of Double)
        Dim emaList As New List(Of Double)
        Dim smaList As New List(Of Double)
        Dim weight As Double = 0
        Dim ema As Double = 0
        Dim i As Integer = 0
        Dim x As Integer = 0
        Dim limit As Integer = 0
        Try
            'start i at sm+1
            'calculation is weight * (price(i) - ema(i-1)) + ema(i-1)
            weight = 2 / (value + 1)
            x = value + 1
            If Not dataList Is Nothing Then
                smaList = calculateSMA(value, dataList)
                For i = 0 To value - 2 Step 1
                    emaList.Add(0)
                Next
                emaList.Add(smaList(value - 1))
                For i = value To dataList.Count - 1 Step 1
                    ema = weight * (dataList(i) - emaList(emaList.Count - 1)) + emaList(emaList.Count - 1)
                    emaList.Add(twoDecimal(ema))
                Next
            Else
                smaList = calculateSMA(value)
                For i = 0 To value - 2 Step 1
                    emaList.Add(0)
                Next
                emaList.Add(smaList(value - 1))
                For i = value To close.Count - 1 Step 1
                    'ema = cur_weight * (stockData.close(i - 1) - ema_avg(ema_avg.Count - 1)) + ema_avg(ema_avg.Count - 1)
                    ema = weight * (close(i) - emaList(emaList.Count - 1)) + emaList(emaList.Count - 1)
                    emaList.Add(twoDecimal(ema))
                    If emaList.Count = 35 Then
                        Dim blah As Int16 = 0
                    End If
                Next
            End If
        Catch ex As Exception
        End Try
        Return emaList
    End Function

    Function getMultiYear(ByVal year As Int32,
                          ByVal period As Integer) As List(Of Double)
        Dim multiData As New List(Of Double)
        Dim startString As String = ""
        Dim endString As String = ""
        Dim startCell As Integer = 0
        Dim endCell As Integer = 0
        Dim startTime As New TimeSpan
        Dim startDate As New Date
        Try
            endString = year & "-" & Today.Month.ToString & "-" & Today.Day.ToString
            endCell = find_date(Convert.ToDateTime(endString).ToString("yyyy-MM-dd"))
            multiData = close.GetRange(endCell - period, period)
        Catch ex As Exception
        End Try
        Return multiData
    End Function

    Function calculateRsi(ByVal rsiBox As Integer) As List(Of Double)
        Dim rsi As New List(Of Double)
        Dim rs As New List(Of Double)
        Dim i As Integer = 0
        Dim k As Integer = 0
        Dim posGain As New List(Of Double)
        Dim posAvg As New List(Of Double)
        Dim posSum As Double = 0
        Dim avgGain As New List(Of Double)
        Dim negGain As New List(Of Double)
        Dim negAvg As New List(Of Double)
        Dim negSum As Double = 0
        Dim avgLoss As New List(Of Double)

        Try
            For i = 0 To cash_gain.Count - 1 Step 1
                If cash_gain(i) > 0 Then
                    posGain.Add(cash_gain(i))
                    negGain.Add(0)
                Else
                    negGain.Add(Math.Abs(cash_gain(i)))
                    posGain.Add(0)
                End If
            Next
            posAvg = posGain.GetRange(0, rsiBox)
            posSum = posAvg.Sum() / rsiBox
            avgGain.Add(posSum)
            negAvg = negGain.GetRange(0, rsiBox)
            negSum = Math.Abs(negAvg.Sum() / rsiBox)
            avgLoss.Add(negSum)
            k = 0
            For i = rsiBox To per_gain.Count - 1 Step 1
                avgGain.Add(((avgGain.Last * (rsiBox - 1)) + posGain(i)) / rsiBox)
                avgLoss.Add(((avgLoss.Last * (rsiBox - 1)) + negGain(i)) / rsiBox)
                k += 1
            Next
            For i = 0 To avgGain.Count - 1 Step 1
                rs.Add(avgGain(i) / avgLoss(i))
                rsi.Add(twoDecimal(100 - (100 / (1 + rs(i)))))
            Next
        Catch ex As Exception
        End Try
        Return rsi
    End Function

    Function calculateRoc(ByVal rocBox As Integer) As List(Of Double)
        Dim roc As New List(Of Double)
        Dim perGain As Double = 0
        Dim i As Integer = 0
        Try
            For i = rocBox To close.Count - 1 Step 1
                perGain = close(i) - close(i - rocBox + 1)
                roc.Add(twoDecimal(perGain))
            Next
        Catch ex As Exception
        End Try
        Return roc
    End Function


    '************* CALCULATE STOCHASTIC ************************
    '%K = (Current Close - Lowest Low)/(Highest High - Lowest Low) * 100
    '%D = 3-day SMA of %K

    'Lowest Low = lowest low for the look-back period
    'Highest High = highest high for the look-back period
    '%K is multiplied by 100 to move the decimal point two places
    'Fast Stochastic Oscillator:
    '    Fast %K = %K basic calculation
    '    Fast %D = 3-period SMA of Fast %K

    'Slow Stochastic Oscillator:
    '    Slow %K = Fast %K smoothed with 3-period SMA
    '    Slow %D = 3-period SMA of Slow %K

    'The Full Stochastic Oscillator is a fully customizable version of the Slow Stochastic Oscillator. 
    'Users can set the look-back period, the number of periods to slow %K and the number of periods for 
    'the %D moving average. The default parameters were used in these examples: Fast Stochastic Oscillator (14,3), 
    'Slow Stochastic Oscillator (14,3) and Full Stochastic Oscillator (14,3,3).

    'Full Stochastic Oscillator:
    '    Full %K = Fast %K smoothed with X-period SMA
    '    Full %D = X-period SMA of Full %K

    Sub calculateStoch(ByVal kPercent As Integer, _
                       ByRef kOut As List(Of Double))

        'Dim kOut As New List(Of Double)
        'Dim dOut As New List(Of Double)
        Dim i As Integer = 0
        Dim calcK As Double = 0
        Dim rangeClose As Double
        Dim rangeHigh As List(Of Double)
        Dim rangeLow As List(Of Double)
        Dim tdhigh As Double()
        Dim thigh As New List(Of Double)

        Dim tdlow As Double()
        Dim tlow As New List(Of Double)

        Dim tdclose As Double()
        Dim tclose As New List(Of Double)

        Try
            tdhigh = {127.01, 127.62, 126.59, 127.35, 128.17, 128.43, 127.37, 126.42, 126.9, 126.85, 125.65, 125.72, 127.16, 127.72, 127.69, 128.22, 128.27, 128.09, 128.27, 127.74, 128.77, 129.29, 130.06, 129.12, 129.29, 128.47, 128.09, 128.65, 129.14, 128.64}
            tdlow = {125.36, 126.16, 124.93, 126.09, 126.82, 126.48, 126.03, 124.83, 126.39, 125.72, 124.56, 124.57, 125.07, 126.86, 126.63, 126.8, 126.71, 126.8, 126.13, 125.92, 126.99, 127.81, 128.47, 128.06, 127.61, 127.6, 127.0, 126.9, 127.49, 127.4}
            tdclose = {127.29, 127.18, 128.01, 127.11, 127.73, 127.06, 127.33, 128.71, 127.87, 128.58, 128.6, 127.93, 128.11, 127.6, 127.6, 128.69, 128.27}
            tclose = tdclose.ToList
            thigh = tdhigh.ToList
            tlow = tdlow.ToList

            rangeClose = tclose.Last
            'For i = 13 To tdhigh.Count - 1 Step 1
            '    'rangeClose = tclose(i - 13)
            '    rangeHigh = thigh.GetRange(i - 13, 13)
            '    rangeLow = tlow.GetRange(i - 13, 13)
            '    calcK = calculateStochPercentK(rangeClose, rangeHigh, rangeLow)
            '    kOut.Add(calcK)
            'Next
            ' -1 is because the array starts at 0 and not 1.
            rangeClose = close.Last
            For i = kPercent To highs.Count - 1 Step 1
                rangeClose = close(i)
                rangeHigh = highs.GetRange(i - kPercent, kPercent)
                rangeLow = lows.GetRange(i - kPercent, kPercent)
                calcK = calculateStochPercentK(rangeClose, rangeHigh, rangeLow)
                kOut.Add(calcK)
            Next
            '            If StochOption = "fast" Then
            'dOut = calculateSMA(dPercent, kOut)
            'End If

            'If StochOption = "slow" Then
            '    kOut = calculateSMA(dPercent, kOut)
            '    dOut = calculateSMA(dPercent, kOut)
            'End If

            'If StochOption = "full" Then
            '    kOut = calculateSMA(xPeriod, kOut)
            '    dOut = calculateSMA(xPeriod, kOut)
            'End If
        Catch ex As Exception

        End Try
    End Sub

    Function calculateStochPercentK(ByVal close As Double, _
                                    ByVal high As List(Of Double), _
                                    ByVal low As List(Of Double)) As Double
        '%K = (Current Close - Lowest Low)/(Highest High - Lowest Low) * 100
        Dim k As Double = 0
        Dim rlow As Double = low.Min
        Dim rhigh As Double = high.Max

        k = ((close - low.Min) / (high.Max - low.Min)) * 100
        Return twoDecimal(k)
    End Function

    Sub stochKandD(ByVal kArray As List(Of Double), _
                               ByVal stochType As String, _
                               ByVal dValue As Integer, _
                               ByRef kOut As List(Of Double), _
                               ByRef dOut As List(Of Double))

        If stochType = "fast" Then
            kOut = kArray
            dOut = calculateSMA(dValue, kArray)
        ElseIf stochType = "slow" Then
            kOut = calculateSMA(dValue, kArray)
            dOut = calculateSMA(dValue, kOut)
        End If
    End Sub

End Class
