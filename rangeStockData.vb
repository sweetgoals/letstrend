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


Public Class rangeStockData
    Inherits commons

    Public ticker As String
    Public close, highs, lows, volume, opens, cashGain, percentGain As New List(Of Double)
    Public sDate As New List(Of String)

    Public Sub New(ByVal stockData As yahooData, ByVal range As Integer)
        Dim i As Integer = 0
        Dim startIndex As Integer = 0
        Try
            startIndex = stockData.close.Count - range
            ticker = stockData.ticker
            close = stockData.close.GetRange(startIndex, range)
            highs = stockData.highs.GetRange(startIndex, range)
            lows = stockData.lows.GetRange(startIndex, range)
            volume = stockData.volume.GetRange(startIndex, range)
            opens = stockData.opens.GetRange(startIndex, range)
            sDate = stockData.sDate.GetRange(startIndex, range)
            If range > 1 Then
                cashGain = stockData.cash_gain.GetRange(startIndex, range - 1)
                percentGain = stockData.per_gain.GetRange(startIndex, range - 1)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub New(ByVal stockData As yahooData, ByVal startCell As Integer, ByVal endCell As Integer)
        Dim i As Integer = 0
        Dim range As Integer = 0
        Dim startIndex As Integer = 0
        Try
            range = endCell - startCell + 1
            startIndex = startCell
            ticker = stockData.ticker
            close = stockData.close.GetRange(startIndex, range)
            highs = stockData.highs.GetRange(startIndex, range)
            lows = stockData.lows.GetRange(startIndex, range)
            volume = stockData.volume.GetRange(startIndex, range)
            opens = stockData.opens.GetRange(startIndex, range)
            sDate = stockData.sDate.GetRange(startIndex, range)
            If range > 1 Then
                cashGain = stockData.cash_gain.GetRange(startIndex - 1, range)
                percentGain = stockData.per_gain.GetRange(startIndex - 1, range)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub New(ByVal stockData As yahooData)
        ticker = stockData.ticker
        close = stockData.close
        highs = stockData.highs
        lows = stockData.lows
        volume = stockData.volume
        opens = stockData.opens
        sDate = stockData.sDate
        cashGain = stockData.cash_gain
        percentGain = stockData.per_gain
    End Sub

    Public Function findBestDay(ByVal beginCell As Integer, ByVal endCell As Integer) As Integer
        Dim i As Integer = 0
        Dim bestCell As Integer = 0
        Dim bestGain As Double = 0

        For i = beginCell To i = endCell Step 1
            If percentGain(i) > bestGain Then
                bestGain = percentGain(i)
                bestCell = i
            End If
        Next
        Return bestCell
    End Function


    Public Function graphDouble(ByVal value As List(Of Double)) As String
        Dim graphData As String = ""
        Dim startIndex As Integer = 0

        Try
            startIndex = value.Count - close.Count
            For i = startIndex To value.Count - 2 Step 1
                graphData &= value(i).ToString & ", "
            Next
            graphData &= value(value.Count - 1).ToString
        Catch ex As Exception
        End Try
        Return graphData
    End Function

    Public Function graphString(ByVal value As List(Of String)) As String
        Dim graphData As String = ""
        Dim startIndex As Integer = 0

        Try
            startIndex = value.Count - close.Count

            For i = startIndex To value.Count - 2 Step 1
                graphData &= value(i).ToString & ", "
            Next
            graphData &= value(value.Count - 1).ToString
        Catch ex As Exception
        End Try
        Return graphData
    End Function

    Public Function graphDate(ByVal value As List(Of String)) As String
        Dim graphData As String = ""

        Try
            For i = 0 To value.Count - 2 Step 1
                graphData &= "'" & Convert.ToDateTime(value(i)).ToString("MMM-dd") & "'" & ", "
            Next
            graphData &= "'" & Convert.ToDateTime(value.Last).ToString("MMM-dd") & "'"
        Catch ex As Exception
        End Try
        Return graphData
    End Function

    Public Function graphRangeTitle(ByVal value As List(Of String)) As String
        Dim title As String = ""

        Try
            title = "'" & Convert.ToDateTime(value.First).ToString("MMM-dd-yy") & " To " & _
                          Convert.ToDateTime(value.Last).ToString("MMM-dd-yy") & "'"
        Catch ex As Exception
        End Try
        Return title
    End Function

    Public Function graphMACD(ByVal stockData As yahooData) As String

        Dim i As Integer = 0
        Dim sma12 As New List(Of Double)
        Dim ema12 As New List(Of Double)
        Dim sma26 As New List(Of Double)
        Dim ema26 As New List(Of Double)
        Dim MACD As New List(Of Double)
        Dim smaMACD As New List(Of Double)
        Dim emaMACD As New List(Of Double)
        Dim divergeMACD As New List(Of Double)

        Dim macdString As String = ""
        Dim divergeMacdString As String = ""
        Dim emaMacdString As String = ""
        Dim macdSeries As String = ""

        Dim formatMACD As New List(Of String)
        Dim formatDivergeMACD As New List(Of String)
        Dim formatEmaMacd As New List(Of String)

        Try
            sma12 = stockData.calculateSMA(12)
            ema12 = stockData.calculateEMA(12)

            sma26 = stockData.calculateSMA(26)
            ema26 = stockData.calculateEMA(26)

            'MacD = (12EMA- 26EMA)
            For i = 0 To ema26.Count - 1 Step 1
                MACD.Add(ema12(i) - ema26(i))
            Next

            'Now calculate the MacD SMA9 and EMA9
            smaMACD = stockData.calculateSMA(9, MACD)
            emaMACD = stockData.calculateEMA(9, MACD)

            'Calculate divergence MacD-EMA9
            For i = 0 To emaMACD.Count - 1 Step 1
                divergeMACD.Add(MACD(i) - emaMACD(i))
            Next

            'format data to two decimal pts
            formatMACD = twoDecimal(MACD)
            formatEmaMacd = twoDecimal(emaMACD)
            formatDivergeMACD = twoDecimal(divergeMACD)

            'turn the data into strings 
            macdString = graphString(formatMACD)
            emaMacdString = graphString(formatEmaMacd)
            divergeMacdString = graphString(formatDivergeMACD)

            macdSeries = "series: [{ name:'MACD', data:[" & macdString & "]},{color: 'rgb(0,128,0)', name:'EMA9', data:[" _
                            & emaMacdString & "]},{color: 'rgb(255,128,0)', name:'Div', data:[" & divergeMacdString & "]}]"
        Catch ex As Exception
        End Try
        Return macdSeries
    End Function
End Class
