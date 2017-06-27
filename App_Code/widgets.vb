Imports Microsoft.VisualBasic
Imports yahooData

Public Class widgets

    Function calculate_return(ByVal today_price As String, ByVal past_date As String) As Double
        Dim ret As Double
        Dim tprice As Double = Convert.ToDouble(today_price)
        Dim pdate As Double = Convert.ToDouble(past_date)
        ret = ((tprice - pdate) / pdate) * 100
        Return ret
    End Function

    Public Sub loadTable(ByRef thetable As Table, _
               ByVal stocks() As String, _
               Optional loadSqlServer As Boolean = False)
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

        Dim acell As New TableCell
        Dim columnTitles() As String = {"Ticker", "Price", "Today", "Week", "Month", "3 Month", "6 Month", "Year"}


        For i = 0 To 4 Step 1
            acell = New TableCell
            arow.Cells.Add(acell)
        Next
        acell = New TableCell
        acell.Text = "Gains(%)"
        acell.HorizontalAlign = HorizontalAlign.Center
        arow.Cells.Add(acell)
        thetable.Rows.Add(arow)

        arow = New TableRow
        For i = 0 To 7 Step 1
            acell = New TableCell
            acell.Text = columnTitles(i)
            acell.HorizontalAlign = HorizontalAlign.Center
            arow.Cells.Add(acell)
        Next
        thetable.Rows.Add(arow)

        beginDate = Today.AddMonths(-1)
        beginDate = beginDate.AddYears(-1)
        For i = 0 To 5 Step 1
            ups(i) = 0
            downs(i) = 0
            avgs(i) = 0
        Next
        For i = 0 To stocks.Length - 1 Step 1
            Dim theData As New yahooData
            Dim dateCell As Integer
            Dim badData As Integer = 0

            If loadSqlServer = False Then
                theData.ticker = stocks(i)
                dataSite = theData.urlBuilderRangeDate(beginDate, endDate)
                getTickerData(theData, dataSite)
            Else : theData = New yahooData(stocks(i), beginDate, endDate)
            End If
            If theData.close.Count <> 0 Then

                Dim original As Double = 0
                Dim current As Double = 0
                Dim thereturn As Double = 0

                arow = New TableRow
                'cell0 = New TableCell
                cell1 = New TableCell
                cell2 = New TableCell
                cell3 = New TableCell
                cell4 = New TableCell
                cell5 = New TableCell
                cell6 = New TableCell
                cell7 = New TableCell
                cell8 = New TableCell

                Dim thelink As New HyperLink
                thelink.Text = "<center>" & stocks(i).ToUpper & "</center>"
                thelink.NavigateUrl = "stock.aspx?ticker=" & stocks(i).ToUpper
                thelink.Target = "_blank"
                cell1.Controls.Add(thelink)
                cell8.Text = "<center>" & String.Format("{0:n2}", theData.close.Last) & "</center>" 'Today's price

                'days gain
                Try
                    cell2.Text = String.Format("{0:n2}", theData.per_gain.Last)
                    avgs(0) += theData.per_gain.Last
                Catch ex As Exception
                    cell2.Text = "0.00"
                End Try

                If cell2.Text > 0 Then
                    cell2.ForeColor = Drawing.Color.Green
                    ups(0) += 1
                Else
                    cell2.ForeColor = Drawing.Color.Red
                    downs(0) += 1
                End If
                cell2.HorizontalAlign = HorizontalAlign.Center


                'weeks return
                dateCell = theData.find_date(weekDate) 'find where last week is
                Try
                    thereturn = calculate_return(theData.close.Last, theData.close(theData.close.Count - 5))
                    avgs(1) += thereturn
                Catch ex As Exception
                    thereturn = 0.0
                End Try

                cell3.Text = String.Format("{0:n2}", thereturn)
                cell3.HorizontalAlign = HorizontalAlign.Center
                If cell3.Text > 0 Then
                    cell3.ForeColor = Drawing.Color.Green
                    ups(1) += 1
                Else
                    cell3.ForeColor = Drawing.Color.Red
                    downs(1) += 1
                End If


                'Months return
                dateCell = theData.findDate(30) 'find where last month is
                Try
                    thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
                    avgs(2) += thereturn
                Catch ex As Exception
                    thereturn = 0.0
                End Try
                cell4.Text = String.Format("{0:n2}", thereturn)
                cell4.HorizontalAlign = HorizontalAlign.Center
                If cell4.Text > 0 Then
                    cell4.ForeColor = Drawing.Color.Green
                    ups(2) += 1
                Else
                    downs(2) += 1
                    cell4.ForeColor = Drawing.Color.Red
                End If


                '3 Months return
                dateCell = theData.findDate(90) 'find where last month is
                Try
                    thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
                    avgs(3) += thereturn
                Catch ex As Exception
                    thereturn = 0.0
                End Try
                cell5.Text = String.Format("{0:n2}", thereturn)
                cell5.HorizontalAlign = HorizontalAlign.Center
                If cell5.Text > 0 Then
                    ups(3) += 1
                    cell5.ForeColor = Drawing.Color.Green
                Else
                    downs(3) += 1
                    cell5.ForeColor = Drawing.Color.Red
                End If

                '6 Months return
                dateCell = theData.findDate(180) 'find where last month is
                Try
                    thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
                    avgs(4) += thereturn
                Catch ex As Exception
                    thereturn = 0.0
                End Try
                cell6.Text = String.Format("{0:n2}", thereturn)
                cell6.HorizontalAlign = HorizontalAlign.Center
                If cell6.Text > 0 Then
                    ups(4) += 1
                    cell6.ForeColor = Drawing.Color.Green
                Else
                    downs(4) += 1
                    cell6.ForeColor = Drawing.Color.Red
                End If

                'Year return
                dateCell = theData.findDate(365) 'find where last month is
                Try
                    thereturn = calculate_return(theData.close.Last, theData.close(dateCell))
                    avgs(5) += thereturn
                Catch ex As Exception
                    thereturn = 0.0
                End Try
                cell7.Text = String.Format("{0:n2}", thereturn)
                cell7.HorizontalAlign = HorizontalAlign.Center
                'calculate the ups and downs
                If cell7.Text > 0 Then
                    ups(5) += 1
                    cell7.ForeColor = Drawing.Color.Green
                Else
                    downs(5) += 1
                    cell7.ForeColor = Drawing.Color.Red
                End If

                'load the row with the gains
                'arow.Cells.Add(cell0)
                arow.Cells.Add(cell1)
                arow.Cells.Add(cell8) ' todays price
                arow.Cells.Add(cell2)
                arow.Cells.Add(cell3)
                arow.Cells.Add(cell4)
                arow.Cells.Add(cell5)
                arow.Cells.Add(cell6)
                arow.Cells.Add(cell7)
                thetable.Rows.Add(arow)
            End If

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
                'cell1 = New TableCell
                'arow.Cells.Add(cell1)
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
                'cell1 = New TableCell
                'arow.Cells.Add(cell1)
                For j = 0 To 5 Step 1
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    If avgs(j) < 0 Then
                        cell1.ForeColor = Drawing.Color.Red
                    Else
                        cell1.ForeColor = Drawing.Color.Green
                    End If
                    cell1.Text = String.Format("{0:n2}", (avgs(j) / stocks.Length))
                    arow.Cells.Add(cell1)
                Next
            End If
            thetable.Rows.Add(arow)
        Next
    End Sub

    Sub getTickerData(ByRef tickerData As yahooData, _
                      ByVal dataSite As String)
        tickerData.GetDataByURL(dataSite)
    End Sub
End Class

