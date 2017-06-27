Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml

Partial Public Class _Default
    Inherits System.Web.UI.Page
    Dim dow(,) As String = {{"MMM", "3M"}, _
                            {"AA", "Alcoa"}, _
                            {"AXP", "American Express"}, _
                            {"T", "AT&T"}, _
                            {"BAC", "Bank of America"}, _
                            {"BA", "Boeing"}, _
                            {"CAT", "Caterpillar"}, _
                            {"CVX", "Chevron"}, _
                            {"CSCO", "Cisco Systems"}, _
                            {"KO", "Coca-Cola"}, _
                            {"DD", "Dupont"}, _
                            {"XOM", "ExxonMobil"}, _
                            {"GE", "General Electric"}, _
                            {"HPQ", "Hewlett-Packard"}, _
                            {"HD", "Home Depot"}, _
                            {"INTC", "Intel"}, _
                            {"IBM", "International Business Machines"}, _
                            {"JNJ", "Johnson & Johnson"}, _
                            {"JPM", "JP Morgan Chase"}, _
                            {"KFT", "Kraft Foods"}, _
                            {"MCD", "McDonald's"}, _
                            {"MRK", "Merck & Co"}, _
                            {"MSFT", "Microsoft"}, _
                            {"PFE", "Pfizer Inc"}, _
                            {"PG", "Procter & Gamble"}, _
                            {"TRV", "Travelers Corp"}, _
                            {"UTX", "United Technologies"}, _
                            {"VZ", "Verizon"}, _
                            {"WMT", "Walmart"}, _
                            {"DIS", "Walt Disney"}}

    Dim dowTranInd(,) As String = {{"ALEX", "Alexander & Baldwin Inc"}, _
                                     {"AMR", "AMR Corporation"}, _
                                     {"CHRW", "C.H. Robinson Worldwide Inc"}, _
                                     {"CNW", "Con-Way Inc"}, _
                                     {"CSX", "CSX Corp"}, _
                                     {"DAL", "Delta Air Lines"}, _
                                     {"EXPD", "Expeditors International"}, _
                                     {"FDX", "FedEx Corporation"}, _
                                     {"GMT", "GATX Corp"}, _
                                     {"JBHT", "JB Hunt Transportation Services"}, _
                                     {"JBLU", "Jetblue Airways Corp"}, _
                                     {"KSU", "Kansas City Southern"}, _
                                     {"LSTR", "Landstar Systems Inc"}, _
                                     {"LUV", "Southwest Airlines Inc"}, _
                                     {"NSC", "Norfolk Southern Corp"}, _
                                     {"OSG", "Overseas Shipholding Group"}, _
                                     {"R", "Ryder Systems Inc"}, _
                                     {"UAL", "Union Continental Holdings"}, _
                                     {"UNP", "Union Pacific Corp"}, _
                                     {"UPS", "United Parcel Services Inc"}}

    Dim indexs(,) As String = {{"^DJI", "Dow Jones Industrial"}, _
                               {"^IXIC", "NASDAQ"}, _
                               {"^GSPC", "S&P 500"}, _
                               {"^DJT", "Dow Transportation"}}
    Public rangeData As rangeStockData
    Public stockData As yahooData

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        load_table(indexTable, indexs, "index") 'load the indexs table
        dowIndTable.Visible = False
        dowTransTable.Visible = False
    End Sub

    'closes the requested panel
    Sub openPanel(ByRef panelExtender As AjaxControlToolkit.CollapsiblePanelExtender)
        panelExtender.Collapsed = False
        panelExtender.ClientState = "false"
    End Sub

    Protected Sub dowList_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles dowButton.Click
        '  Dim runCount As Integer = Session("runCount")
        Dim rowCount As Integer = 0
        rowCount = Convert.ToInt32(Session("dowIndTable_rowCount"))

        If (dowPanelExtender.ClientState = "false") Then 'And (runCount <> 99) Then
            If rowCount = 0 Then
                load_table(dowIndTable, dow, "dowIndTable") 'load the dow 30 table

            Else : loadTableFromSession(dowIndTable, dow, "dowIndTable")
            End If
            dowIndTable.Visible = True
        End If
    End Sub

    Protected Sub dowTrans_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles dowTransButton.Click
        Dim rowCount As Integer = 0
        rowCount = Convert.ToInt32(Session("dowTransTable_rowCount"))

        If dowTransPanelExtender.ClientState = "false" Then
            If rowCount = 0 Then
                load_table(dowTransTable, dowTranInd, "dowTransTable") 'load dow trans table
            Else : loadTableFromSession(dowTransTable, dowTranInd, "DowTransTable")
            End If
            dowTransTable.Visible = True
        End If

    End Sub

    Function calculate_return(ByVal today_price As String, ByVal past_date As String) As Double
        Dim ret As Double
        Dim tprice As Double = Convert.ToDouble(today_price)
        Dim pdate As Double = Convert.ToDouble(past_date)
        ret = ((tprice - pdate) / pdate) * 100
        Return ret
    End Function

    Sub periodRun(ByVal refDay As Integer,
                  ByVal ticker As String)
        Dim startCell As Integer = 0
        Dim refTime As New TimeSpan
        Dim startDate As New Date

        Try
            refTime = New TimeSpan(refDay, 0, 0, 0)
            startDate = Today.Subtract(refTime)
            stockData = New yahooData(ticker, startDate)
            If refDay = 5 Then
                'week click needs exactly 5 days no more or less
                rangeData = New rangeStockData(stockData, 5)
            Else 'all other ranges need to go from the refDay to today
                startCell = stockData.find_date(Today.AddDays(-refDay))
                rangeData = New rangeStockData(stockData, stockData.close.Count - startCell)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Sub load_table(ByRef thetable As Table, _
                   ByVal stocks(,) As String, _
                   ByVal tableName As String)
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim ups(6) As Integer
        Dim downs(6) As Integer
        Dim avgs(6) As Double
        Dim stock_names As String = ""
        Dim dataSite As String
        Dim beginDate As Date
        Dim endDate As Date = Today
        Dim weekDate As Date = Today.AddDays(-5)
        Dim monthDate As Date = Today.AddMonths(-1)
        Dim month3Date As Date = Today.AddMonths(-3)
        Dim month6Date As Date = Today.AddMonths(-6)
        Dim yearDate As Date = Today.AddYears(-1)

        Dim arow As New TableRow

        Dim cell0 As New TableCell
        Dim cell1 As New TableCell
        Dim cell2 As New TableCell
        Dim cell3 As New TableCell
        Dim cell4 As New TableCell
        Dim cell5 As New TableCell
        Dim cell6 As New TableCell
        Dim cell7 As New TableCell
        Dim cell8 As New TableCell

        beginDate = Today.AddMonths(-1)
        beginDate = beginDate.AddYears(-1)

        For i = 0 To (stocks.Length \ 2) - 2 Step 1
            stock_names &= stocks(i, 0) & ","
        Next
        'set ups and downs to 0
        For i = 0 To 5 Step 1
            ups(i) = 0
            downs(i) = 0
            avgs(i) = 0
        Next
        For i = 0 To (stocks.Length \ 2) - 1 Step 1
            Dim theData As New yahooData
            Dim dateCell As Integer
            theData.ticker = stocks(i, 0)
            dataSite = theData.urlBuilderRangeDate(beginDate, endDate)
            theData.GetDataByURL(dataSite)
            Dim original As Double = 0
            Dim current As Double = 0
            Dim thereturn As Double = 0

            arow = New TableRow
            cell0 = New TableCell
            cell1 = New TableCell
            cell2 = New TableCell
            cell3 = New TableCell
            cell4 = New TableCell
            cell5 = New TableCell
            cell6 = New TableCell
            cell7 = New TableCell
            cell8 = New TableCell

            Dim thelink As New HyperLink
            cell0.Text = stocks(i, 1) 'name
            Session(tableName & "_row_" & i & "cell_0") = cell0.Text

            thelink.Text = "<center>" & stocks(i, 0) & "</center>"
            thelink.NavigateUrl = "Default.aspx?ticker=" & stocks(i, 0)
            thelink.Target = "_blank"
            cell1.Controls.Add(thelink)
            cell8.Text = "<center>" & theData.close.Last & "</center>" 'dow30_year(0) 'todays price
            Session(tableName & "_row_" & i & "cell_8") = cell8.Text

            'days gain
            cell2.Text = String.Format("{0:n2}", theData.per_gain.Last) 'thereturn)
            Session(tableName & "_row_" & i & "cell_2") = cell2.Text
            If cell2.Text > 0 Then
                cell2.ForeColor = Drawing.Color.Green
                ups(0) += 1
            Else
                cell2.ForeColor = Drawing.Color.Red
                downs(0) += 1
            End If
            cell2.HorizontalAlign = HorizontalAlign.Center
            avgs(0) += theData.per_gain.Last

            'weeks return
            dateCell = theData.find_date(weekDate) 'find where last week is
            thereturn = calculate_return(theData.close.Last, theData.close(theData.close.Count - 5))
            cell3.Text = String.Format("{0:n2}", thereturn)
            Session(tableName & "_row_" & i & "cell_3") = cell3.Text

            cell3.HorizontalAlign = HorizontalAlign.Center
            If cell3.Text > 0 Then
                cell3.ForeColor = Drawing.Color.Green
                ups(1) += 1
            Else
                cell3.ForeColor = Drawing.Color.Red
                downs(1) += 1
            End If
            avgs(1) += thereturn

            'Months return
            dateCell = theData.findDate(30) 'find where last month is
            thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
            cell4.Text = String.Format("{0:n2}", thereturn)
            Session(tableName & "_row_" & i & "cell_4") = cell4.Text
            cell4.HorizontalAlign = HorizontalAlign.Center
            If cell4.Text > 0 Then
                cell4.ForeColor = Drawing.Color.Green
                ups(2) += 1
            Else
                downs(2) += 1
                cell4.ForeColor = Drawing.Color.Red
            End If
            avgs(2) += thereturn

            '3 Months return
            dateCell = theData.findDate(90) 'find where last month is
            thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
            cell5.Text = String.Format("{0:n2}", thereturn)
            Session(tableName & "_row_" & i & "cell_5") = cell5.Text
            cell5.HorizontalAlign = HorizontalAlign.Center
            If cell5.Text > 0 Then
                ups(3) += 1
                cell5.ForeColor = Drawing.Color.Green
            Else
                downs(3) += 1
                cell5.ForeColor = Drawing.Color.Red
            End If
            avgs(3) += thereturn

            '6 Months return
            dateCell = theData.findDate(180) 'find where last month is
            thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
            cell6.Text = String.Format("{0:n2}", thereturn)
            Session(tableName & "_row_" & i & "cell_6") = cell6.Text
            cell6.HorizontalAlign = HorizontalAlign.Center
            If cell6.Text > 0 Then
                ups(4) += 1
                cell6.ForeColor = Drawing.Color.Green
            Else
                downs(4) += 1
                cell6.ForeColor = Drawing.Color.Red
            End If
            avgs(4) += thereturn

            'Year return
            dateCell = theData.findDate(365) 'find where last month is
            thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
            cell7.Text = String.Format("{0:n2}", thereturn)
            Session(tableName & "_row_" & i & "cell_7") = cell7.Text
            cell7.HorizontalAlign = HorizontalAlign.Center
            'calculate the ups and downs
            If cell7.Text > 0 Then
                ups(5) += 1
                cell7.ForeColor = Drawing.Color.Green
            Else
                downs(5) += 1
                cell7.ForeColor = Drawing.Color.Red
            End If
            avgs(5) += thereturn

            'load the row with the gains
            arow.Cells.Add(cell0)
            arow.Cells.Add(cell1)
            arow.Cells.Add(cell8) ' todays price
            arow.Cells.Add(cell2)
            arow.Cells.Add(cell3)
            arow.Cells.Add(cell4)
            arow.Cells.Add(cell5)
            arow.Cells.Add(cell6)
            arow.Cells.Add(cell7)
            thetable.Rows.Add(arow)

        Next

        For i = 0 To 2 Step 1
            arow = New TableRow
            cell1 = New TableCell
            If i = 0 Then
                cell1.Text = "Overall UP"
                cell1.ForeColor = Drawing.Color.Green
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)

                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.ForeColor = Drawing.Color.Green
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = ups(j)
                    arow.Cells.Add(cell1)
                Next
            ElseIf i = 1 Then
                cell1.Text = "Overall DOWN"
                cell1.ForeColor = Drawing.Color.Red
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.ForeColor = Drawing.Color.Red
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = downs(j)
                    arow.Cells.Add(cell1)
                Next
            ElseIf i = 2 Then
                cell1.Text = "Averages"
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    If avgs(j) < 0 Then
                        cell1.ForeColor = Drawing.Color.Red
                    Else
                        cell1.ForeColor = Drawing.Color.Green
                    End If
                    cell1.Text = String.Format("{0:n2}", avgs(j) / ((stocks.Length \ 2)))
                    arow.Cells.Add(cell1)
                Next
            End If
            thetable.Rows.Add(arow)
        Next
        Session(tableName & "_rowCount") = thetable.Rows.Count
    End Sub

    'Sub load_table(ByRef thetable As Table, ByVal stocks(,) As String)
    Sub loadTableFromSession(ByRef theTable As Table, _
                             ByVal stocks(,) As String, _
                             ByVal tableName As String)
        Dim i As Integer = 0
        Dim k As Integer = 0
        Dim ups(6) As Integer
        Dim downs(6) As Integer
        Dim avgs(6) As Double

        Dim acell As TableCell
        Dim arow As TableRow
        Dim theLink As HyperLink

        k = Session(tableName & "_rowCount")
        For i = 0 To 5 Step 1
            ups(i) = 0
            downs(i) = 0
            avgs(i) = 0
        Next
        For i = 0 To (stocks.Length \ 2) - 1 Step 1
            acell = New TableCell
            arow = New TableRow
            acell.HorizontalAlign = HorizontalAlign.Center
            acell.Text = Session(tableName & "_row_" & i & "cell_0")
            arow.Cells.Add(acell)

            acell = New TableCell
            theLink = New HyperLink
            acell.HorizontalAlign = HorizontalAlign.Center
            theLink.Text = "<center>" & stocks(i, 0) & "</center>"
            theLink.Target = "_blank"
            theLink.NavigateUrl = "Default.aspx?ticker=" & stocks(i, 0)
            acell.Controls.Add(theLink)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.HorizontalAlign = HorizontalAlign.Center
            acell.Text = Session(tableName & "_row_" & i & "cell_8")
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.HorizontalAlign = HorizontalAlign.Center
            acell.Text = Session(tableName & "_row_" & i & "cell_2")
            setCellColor(acell, ups(0), downs(0))
            avgs(0) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.HorizontalAlign = HorizontalAlign.Center
            acell.Text = Session(tableName & "_row_" & i & "cell_3")
            setCellColor(acell, ups(1), downs(1))
            avgs(1) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.HorizontalAlign = HorizontalAlign.Center
            acell.Text = Session(tableName & "_row_" & i & "cell_4")
            setCellColor(acell, ups(2), downs(2))
            avgs(2) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = Session(tableName & "_row_" & i & "cell_5")
            acell.HorizontalAlign = HorizontalAlign.Center
            setCellColor(acell, ups(3), downs(3))
            avgs(3) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = Session(tableName & "_row_" & i & "cell_6")
            acell.HorizontalAlign = HorizontalAlign.Center
            setCellColor(acell, ups(4), downs(4))
            avgs(4) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = Session(tableName & "_row_" & i & "cell_7")
            acell.HorizontalAlign = HorizontalAlign.Center
            setCellColor(acell, ups(5), downs(5))
            avgs(5) += Convert.ToDouble(acell.Text)
            arow.Cells.Add(acell)

            theTable.Rows.Add(arow)
        Next

        gatherSummaryInfo(ups, downs, avgs, theTable, stocks)
    End Sub

    Sub setCellColor(ByRef theCell As TableCell, _
                     ByRef ups As Integer, _
                     ByRef downs As Integer)

        If theCell.Text > 0 Then
            theCell.ForeColor = Drawing.Color.Green
            ups += 1
        Else
            theCell.ForeColor = Drawing.Color.Red
            downs += 1
        End If
    End Sub

    Sub gatherSummaryInfo(ByVal ups() As Integer, _
                          ByVal downs() As Integer, _
                          ByVal avgs() As Double, _
                          ByRef theTable As Table, _
                          ByVal stocks(,) As String)

        Dim arow As TableRow
        Dim cell1 As TableCell
        Dim i As Integer = 0
        Dim j As Integer = 0

        For i = 0 To 2 Step 1
            arow = New TableRow
            cell1 = New TableCell
            If i = 0 Then
                cell1.Text = "Overall UP"
                cell1.ForeColor = Drawing.Color.Green
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)

                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.ForeColor = Drawing.Color.Green
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = ups(j)
                    arow.Cells.Add(cell1)
                Next
            ElseIf i = 1 Then
                cell1.Text = "Overall DOWN"
                cell1.ForeColor = Drawing.Color.Red
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.ForeColor = Drawing.Color.Red
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = downs(j)
                    arow.Cells.Add(cell1)
                Next
            ElseIf i = 2 Then
                cell1.Text = "Averages"
                arow.Cells.Add(cell1)
                'spacer
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                cell1 = New TableCell
                arow.Cells.Add(cell1)
                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    If avgs(j) < 0 Then
                        cell1.ForeColor = Drawing.Color.Red
                    Else
                        cell1.ForeColor = Drawing.Color.Green
                    End If
                    cell1.Text = String.Format("{0:n2}", avgs(j) / ((stocks.Length \ 2)))
                    arow.Cells.Add(cell1)
                Next
            End If
            theTable.Rows.Add(arow)
        Next
    End Sub

End Class