
Partial Class Info_relic
    Inherits System.Web.UI.Page

    Public stockData As yahooData

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim switch1 As Boolean = False
        Dim i As Integer = 0
        Dim k As Integer = 0

        For i = 1 To 3 Step 1
            For k = 1 To 6 Step 1
                advanceDeclineTable.Rows(i).Cells(k).Text = ""
            Next
        Next
        For i = 0 To 2 Step 1
            advanceDeclineStatsTable.Rows(i).Cells(1).Text = ""
        Next
        TextBox1.Text = TextBox1.Text.ToUpper
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
        Session("clicked") = 6
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
                stockData = New yahooData(TextBox1.Text, startd)
                If stockData.close.Count = 0 Then
                    errorLabel.Text = "Bad Ticker"
                    errorLabel.Visible = True
                    Exit Sub
                End If
                startCell = stockData.find_date(startDate)
                endCell = stockData.find_date(endDate)
            Catch ex As Exception
                TextBox1.Text = ""
                errorLabel.Text = "Bad Ticker"
                errorLabel.Visible = True
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
            reset_colors()
            buttonClicked.BackColor = Drawing.Color.Lime

            If TextBox1.Text <> "" Then
                Try
                    stockData = New yahooData(TextBox1.Text, startDate)
                    If stockData.close.Count = 0 Then
                        errorLabel.Text = "Bad Ticker"
                        errorLabel.Visible = True
                        Exit Sub
                    End If
                    display_support()
                    display_highlow()
                    advancesAndDeclines()
                Catch ex As Exception
                    TextBox1.Text = ""
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

    'resets the display buttons back to default colors.
    Sub reset_colors()
        week.BackColor = Drawing.Color.Empty
        month.BackColor = Drawing.Color.Empty
        month3.BackColor = Drawing.Color.Empty
        month6.BackColor = Drawing.Color.Empty
        year1.BackColor = Drawing.Color.Empty
        customButton.BackColor = Drawing.Color.Empty
    End Sub

    'SUPPORT AND RESISTANCE LEVELS
    'Two part system. 
    '1. Find all the high low high, or low high low points. Stick those into an array
    '2. Start at the beggining of this array. Take the first cell and find all the points that fall between the devation range.
    '   the number of points that are with in this range is the confirmed number. As you find these points count and remove. 
    '   so you do not find the same point twice. 
    '3. the points above the stock price are the resistance, below is the support. 

    Sub display_support() '(ByVal prices As List(Of Double))
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim gain As Double = 0
        Dim devation As Double = 0
        Dim dev As Double = 0
        Dim total_confirms As Integer = 0
        Dim cell1 As New TableCell
        Dim arow As New TableRow
        Dim total_gain As Double = 0
        Dim supp As New List(Of Double)
        Dim res As New List(Of Double)
        Dim conf_supp_num As New List(Of Integer)
        Dim conf_res_num As New List(Of Integer)
        Dim found_price As Integer = 0
        Dim found_vals As New List(Of Double)
        Dim found_vals_count As New List(Of Integer)
        Dim found_vals_and_count As New List(Of String)
        Dim found_vals_and_count_temp As New List(Of String)
        Dim counter As Integer = 0
        Dim val_string As String = ""
        Dim seperate() As String
        Dim value As Double = 0
        Dim times As Double = 0

        'clear the table
        For i = 1 To 5 Step 1
            For j = 0 To 3 Step 1
                supports.Rows(i).Cells(j).Text = ""
                Resistance.Rows(i).Cells(j).Text = ""
            Next
        Next
        'set the devation to .005 if there's an invalid or no value in textbox4.
        Try
            devation = Convert.ToDouble(TextBox4.Text)
        Catch ex As Exception
            devation = 0.005
        End Try

        'demark - williams method 
        'find all high lows high, low high low, or high high, low low
        Try
            If ((stockData.close(0) < stockData.close(1) + 0.05) And _
                      (stockData.close(0) > stockData.close(1) - 0.05)) Then
                found_vals.Add(stockData.close(0))
            End If

            For i = 1 To stockData.close.Count - 2 Step 1
                If (stockData.close(i - 1) <= stockData.close(i) And _
                    stockData.close(i + 1) <= stockData.close(i)) Then
                    found_vals.Add(stockData.close(i))
                ElseIf (stockData.close(i - 1) >= stockData.close(i) And _
                        stockData.close(i + 1) >= stockData.close(i)) Then
                    found_vals.Add(stockData.close(i))
                End If
                k = i
                While ((stockData.close(i) < stockData.close(k + 1) + 0.05) And _
                      (stockData.close(i) > stockData.close(k + 1) - 0.05)) And _
                       k < stockData.close.Count - 1
                    k += 1
                    found_vals.Add(stockData.close(k))
                End While
                i = k
            Next
        Catch ex As Exception
        End Try

        ' go through the found values and count up all the ones that are within the deveation
        ' and then remove them so they do not get counted twice. 
        Try
            i = 0
            While i < found_vals.Count
                dev = found_vals(i) * devation
                counter = 0
                j = i + 1
                While j < found_vals.Count
                    If found_vals(j) < (found_vals(i) + dev) And _
                       found_vals(j) > (found_vals(i) - dev) Then
                        counter += 1
                        found_vals.RemoveAt(j)
                        j -= 1
                    End If
                    j += 1
                End While
                found_vals_count.Add(counter)
                i += 1
            End While
        Catch ex As Exception
        End Try

        'Add support and Resistance lists. Values above the stock price are resistance.
        'Values below are support. Keep the support price with the number of times its been confirmed together.
        For i = 0 To found_vals.Count - 1 Step 1
            val_string = Convert.ToString(found_vals(i))
            val_string = String.Format("{0:n2}", found_vals(i))
            found_vals_and_count.Add(val_string & " " & Convert.ToString(found_vals_count(i)))
        Next
        Dim temp As String
        'Sort the list      
        For i = 0 To found_vals_and_count.Count - 1 Step 1
            For j = i + 1 To found_vals_and_count.Count - 1 Step 1
                If Convert.ToDouble(found_vals_and_count(i).Split(" ")(0)) > Convert.ToDouble(found_vals_and_count(j).Split(" ")(0)) Then
                    temp = found_vals_and_count(i)
                    found_vals_and_count(i) = found_vals_and_count(j)
                    found_vals_and_count(j) = temp
                End If
            Next
        Next

        'Now find out where the current price compared to the found values and the ones above
        'are the resistance. Ones below are support
        Try
            For i = 0 To found_vals_and_count.Count - 1 Step 1
                seperate = found_vals_and_count(i).Split(" ")
                value = Convert.ToDouble(seperate(0))
                times = Convert.ToDouble(seperate(1))
                'support level
                'If stockData.close.Last >= value Then
                If stockData.close.Last >= value Then
                    supp.Add(value)
                    conf_supp_num.Add(times)
                    'resistance level
                ElseIf stockData.close.Last < value Then
                    res.Add(value)
                    conf_res_num.Add(times)
                End If
            Next
        Catch ex As Exception
        End Try

        supp.Reverse()
        conf_supp_num.Reverse()

        'add up all the support points to display on the totals.
        total_confirms = 0
        For i = 0 To conf_supp_num.Count - 1 Step 1
            total_confirms += 1
            If conf_supp_num(i) > 0 Then
                total_confirms += conf_supp_num(i)
            End If
        Next

        'display the support points.
        If supp.Count > 0 Then
            j = conf_supp_num.Count
            k = j - 1

            If j > 0 Then
                'Show support levels 5 closest to current price the(lows)
                If k > 4 Then
                    k = 4
                End If
                Try
                    'String.Format("{0:n2}", per_gain.Last)
                    For i = 0 To k Step 1
                        supports.Rows(i + 1).Cells(0).Text = i + 1
                        supports.Rows(i + 1).Cells(1).Text = String.Format("{0:n2}", supp(k - i))
                        found_price = stockData.close.IndexOf(supp(k - i))
                        If found_price = -1 Then
                            found_price = stockData.close.IndexOf(supp(k - i))
                        End If
                        supports.Rows(i + 1).Cells(2).Text = String.Format("{0:n2}", stockData.close(found_price))
                        gain = ((stockData.close(found_price) - stockData.close.Last) / stockData.close(found_price)) * 100
                        supports.Rows(i + 1).Cells(2).Text = String.Format("{0:n2}", gain)
                        total_gain += gain
                        supports.Rows(i + 1).Cells(3).Text = conf_supp_num(k - i)
                    Next
                Catch ex As Exception
                End Try
                Try
                    cell1.Text = "Overall"
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = supports.Rows(1).Cells(1).Text & " To " & supports.Rows(k + 1).Cells(1).Text
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    arow.Cells.Add(cell1)
                    'cell1.Text = String.Format("{0:n2}", supp.Last) & " To " & String.Format("{0:n2}", supp.First)
                    cell1.Text = String.Format("{0:n2}", supp.Last) & " To " & String.Format("{0:n2}", supp.First)
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = ""
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = total_confirms
                    arow.Cells.Add(cell1)
                    arow.Font.Bold = False
                    arow.HorizontalAlign = HorizontalAlign.Center
                    supports.Rows.Add(arow)
                Catch ex As Exception
                End Try
            End If
        End If

        'Display the resistance levels
        If res.Count > 0 Then
            Try
                'count up all the resistance levels to display on the overall section.
                total_confirms = 0
                total_gain = 0
                For i = 0 To conf_res_num.Count - 1 Step 1
                    total_confirms += 1
                    If conf_res_num(i) > 0 Then
                        total_confirms += conf_res_num(i)
                    End If
                Next
                j = res.Count
                k = j - 1
                If j > 0 Then
                    If k > 4 Then
                        k = 4
                    End If
                    For i = 0 To k Step 1
                        Resistance.Rows(i + 1).Cells(0).Text = i + 1
                        Resistance.Rows(i + 1).Cells(1).Text = String.Format("{0:n2}", res(i))
                        found_price = stockData.close.IndexOf(res(i))
                        If found_price = -1 Then
                            found_price = stockData.close.IndexOf(res(i))
                        End If
                        gain = ((stockData.close(found_price) - stockData.close.Last) / stockData.close(found_price)) * 100
                        Resistance.Rows(i + 1).Cells(2).Text = String.Format("{0:n2}", gain)
                        Resistance.Rows(i + 1).Cells(3).Text = conf_res_num(i)
                    Next
                    arow = New TableRow
                    cell1 = New TableCell
                    cell1.Text = "Overall"
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = String.Format("{0:n2}", res.First) & " To " & String.Format("{0:n2}", res.Last)
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = ""
                    arow.Cells.Add(cell1)
                    cell1 = New TableCell
                    cell1.HorizontalAlign = HorizontalAlign.Center
                    cell1.Text = total_confirms
                    arow.Cells.Add(cell1)
                    arow.Font.Bold = False
                    arow.HorizontalAlign = HorizontalAlign.Center
                    Resistance.Rows.Add(arow)
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    'HIGH LOW PROCEDURES
    'shows the last 4wk high and low.
    Sub display_highlow() 'ByVal stockData.close As List(Of Double), _
        '                ByVal stockData.highs As List(Of Double), _
        '                ByVal stockData.highs As List(Of Double),
        '                ByVal stockData.sDate As List(Of String))

        Dim found_low As Boolean = False
        Dim found_high As Boolean = False
        Dim searchlist As New List(Of Double)
        Dim i As Integer = stockData.highs.Count - 1
        Dim j As Integer = 0
        Dim counter As Integer = 0
        Dim period_low_index As Integer = 0
        Dim period_high_index As Integer = 0
        Dim price_gain As Double = 0
        Dim percent_gain As Double = 0
        Dim low_index As Integer = 0
        Dim fwkh As String = ""
        Dim fwkl As String = ""

        Try
            For i = 1 To 4 Step 1
                range_table.Rows(i).ForeColor = Drawing.Color.Black
                range_table.Rows(i).Font.Bold = False
                For j = 1 To 5 Step 1
                    range_table.Rows(i).Cells(j).Text = ""
                Next
            Next
        Catch ex As Exception
        End Try
        Try
            'description date value price gain %gain
            'period low
            period_low_index = stockData.highs.IndexOf(stockData.highs.Min)
            range_table.Rows(1).Cells(1).Text = Convert.ToDateTime(stockData.sDate(period_low_index)).ToString("MMM-dd-yy")
            range_table.Rows(1).Cells(2).Text = String.Format("{0:n2}", stockData.highs.Min)
            range_table.Rows(1).Cells(3).Text = String.Format("{0:n2}", stockData.close(period_low_index))
            'If Convert.ToDateTime(stockData.sDate(period_low_index)) = Today Then
            If stockData.sDate(period_low_index) = stockData.sDate.Last Then
                range_table.Rows(1).ForeColor = Drawing.Color.Blue
                range_table.Rows(1).Font.Bold = True
            End If

            'perid high 
            period_high_index = stockData.highs.IndexOf(stockData.highs.Max)
            range_table.Rows(2).Cells(1).Text = Convert.ToDateTime(stockData.sDate(period_high_index)).ToString("MMM-dd-yy")
            range_table.Rows(2).Cells(2).Text = String.Format("{0:n2}", stockData.highs.Max)
            range_table.Rows(2).Cells(3).Text = String.Format("{0:n2}", stockData.close(period_high_index))

            'gain between high and low
            price_gain = stockData.close(period_high_index) - stockData.close(period_low_index)
            percent_gain = ((stockData.close(period_high_index) - stockData.close(period_low_index)) / stockData.close(period_high_index)) * 100
            range_table.Rows(2).Cells(4).Text = String.Format("{0:n2}", price_gain)
            range_table.Rows(2).Cells(5).Text = String.Format("{0:n2}", percent_gain)

            If stockData.sDate(period_high_index) = stockData.sDate.Last Then
                range_table.Rows(2).ForeColor = Drawing.Color.Blue
                range_table.Rows(2).Font.Bold = True
            End If

            If stockData.close.Count > 20 Then
                range_table.Rows(3).Cells(0).Text = "Latest 4wk Low"
                range_table.Rows(4).Cells(0).Text = "Latest 4wk High"

                '4wk low
                Dim ver_num As Integer = 9999
                fwkl = find_4wkl(stockData.highs, stockData.sDate)
                low_index = -1
                If fwkl <> "9999" Then
                    low_index = stockData.sDate.IndexOf(fwkl)
                End If
                If low_index > 19 Then
                    range_table.Rows(3).Cells(1).Text = Convert.ToDateTime(stockData.sDate(low_index)).ToString("MMM-dd-yy")
                    range_table.Rows(3).Cells(2).Text = String.Format("{0:n2}", stockData.lows(low_index))
                    range_table.Rows(3).Cells(3).Text = String.Format("{0:n2}", stockData.close(low_index))

                    If stockData.sDate(low_index) = stockData.sDate.Last Then
                        range_table.Rows(3).ForeColor = Drawing.Color.Blue
                        range_table.Rows(3).Font.Bold = True
                    End If
                Else
                    range_table.Rows(3).Cells(1).Text = "Not In Range"
                    range_table.Rows(3).Cells(2).Text = " "
                    range_table.Rows(3).Cells(3).Text = " "
                End If

                '4wk high
                i = -1
                fwkh = find_4wkh(stockData.highs, stockData.sDate)
                If fwkh <> "9999" Then
                    i = stockData.sDate.IndexOf(fwkh)
                End If

                If i > 19 Then
                    range_table.Rows(4).Cells(1).Text = Convert.ToDateTime(stockData.sDate(i)).ToString("MMM-dd-yy")
                    range_table.Rows(4).Cells(2).Text = String.Format("{0:n2}", stockData.highs(i))
                    range_table.Rows(4).Cells(3).Text = String.Format("{0:n2}", stockData.close(i))

                    If stockData.sDate(i) = stockData.sDate.Last Then
                        range_table.Rows(4).ForeColor = Drawing.Color.Blue
                        range_table.Rows(4).Font.Bold = True
                    End If
                Else
                    range_table.Rows(4).Cells(1).Text = "Not In Range"
                    range_table.Rows(4).Cells(2).Text = " "
                    range_table.Rows(4).Cells(3).Text = " "

                End If
                '4wk high - low gain
                'gain between high and low
                If i > 19 And low_index > 19 Then
                    price_gain = stockData.close(i) - stockData.close(low_index)
                    percent_gain = ((stockData.close(i) - stockData.close(low_index)) / stockData.close(i)) * 100
                    range_table.Rows(4).Cells(4).Text = String.Format("{0:n2}", price_gain)
                    range_table.Rows(4).Cells(5).Text = String.Format("{0:n2}", percent_gain)
                End If
            Else
                range_table.Rows(3).Cells(0).Text = ""
                range_table.Rows(4).Cells(0).Text = ""
            End If

        Catch ex As Exception
            'there's a bug here because the close, low, high, and date arrays are not the same 
            'size. If we send the originals it doesn't get the right information. As of this
            'writing there doesn't seem to be a problem with letting it except out. 8-1-2010
        End Try
    End Sub

    'find the lowest value the stock has reached in the last 4 weeks
    Function find_4wkl(ByVal thelows As List(Of Double), _
                       ByVal dates As List(Of String)) As String
        Dim pos As Integer = thelows.IndexOf(thelows.Last)
        Dim checkpos As Integer = thelows.IndexOf(thelows.Last)
        Try
            pos = dates.IndexOf(verifyl(thelows.GetRange(thelows.Count - 20, 20), dates.GetRange(thelows.Count - 20, 20)))
            While (pos - 19 > 0)
                checkpos = dates.IndexOf(verifyl(thelows.GetRange(pos - 20, 20), dates.GetRange(pos - 20, 20)))
                If thelows(checkpos) < thelows(pos) Then
                    pos = checkpos
                Else : Exit While
                End If
            End While
        Catch ex As Exception
            Return -1
        End Try
        If pos >= 19 Then
            Return dates(pos)
        Else
            Return "9999"
        End If
    End Function

    'find the loweest value the stock has reached in the last 4 weeks
    Function verifyl(ByVal num_set As List(Of Double), ByVal das As List(Of String)) As String
        Try
            Return das(num_set.IndexOf(num_set.Min))
        Catch ex As Exception
            Return "9999"
        End Try
    End Function

    'find the highest value the stock has reached in the last 4 weeks
    Function find_4wkh(ByVal thehighs As List(Of Double), _
                       ByVal dates As List(Of String)) As String
        Dim pos As Integer = thehighs.IndexOf(thehighs.Last)
        Dim checkpos As Integer = thehighs.IndexOf(thehighs.Last)
        Try
            pos = dates.IndexOf(verifyh(thehighs.GetRange(thehighs.Count - 20, 20), dates.GetRange(thehighs.Count - 20, 20)))
            While (pos - 19 > 0)
                checkpos = dates.IndexOf(verifyh(thehighs.GetRange(pos - 20, 20), dates.GetRange(pos - 20, 20)))
                If thehighs(checkpos) > thehighs(pos) Then
                    pos = checkpos
                Else : Exit While
                End If
            End While
        Catch ex As Exception
            Return -1
        End Try
        If pos >= 19 Then
            Return dates(pos)
        Else
            Return "9999"
        End If
    End Function

    'verify that the high value selected is really the high in the next 4 weeks
    Function verifyh(ByVal num_set As List(Of Double), ByVal das As List(Of String)) As String
        Try
            Return das(num_set.IndexOf(num_set.Max))
        Catch ex As Exception
            Return ""
        End Try
    End Function

    'makes sure the 4 week high found is really it by checking the next 20 day trading periods.
    Function verify_4wkh(ByVal search As List(Of Double), _
                ByVal dates As List(Of String), _
                ByVal prices As List(Of Double)) As Integer
        Dim i As Integer
        Dim tlc As Integer
        Dim sl As New List(Of Double)
        Dim found As Boolean = True
        Dim store As Integer = 0
        Dim testlow As Double = 0
        Dim fail_safe As Integer = 0
        i = search.Count - 1
        Try
            sl = search.GetRange(i - 20, 20)
            store = i
            While search(i) = sl.Max And fail_safe < 1000
                fail_safe += 1
                sl = search.GetRange(i - 20, 20)
                testlow = sl.Max
                tlc = sl.IndexOf(testlow)
                store = i
                i -= 1
            End While
        Catch ex As Exception
        End Try
        Return store
    End Function
    'ADVANCES & DECLINES PROCEDURES
    'calculates the number of times a stock has advanced and declined
    Sub advancesAndDeclines()
        '        Dim stockData As rangeStockData

        'total times the stock has traded on the week day
        Dim monTotal, tuesTotal, wedTotal, thurTotal, friTotal, i As Integer
        'total times stock was up on the week day
        Dim monUp, tuesUp, wedUp, thurUp, friUp As Integer

        Dim lad As Integer = 0
        Dim ladt As Integer = 0
        Dim ad_start As Integer = 0
        Dim ad_startt As Integer = 0
        Dim up As Boolean = False
        Dim lde As Integer = 0
        Dim ldet As Integer = 0
        Dim de_start As Integer = 0
        Dim de_startt As Integer = 0
        Dim down As Boolean = False
        Dim curdate As DateTime
        Dim cur_dir As Boolean = False
        Dim cur_dir_count As Integer = 0
        Dim cur_pos As Integer = 0
        Dim arow As TableRow
        Dim acell As TableCell
        Dim startCell As Integer = 0
        Dim advEndDate As Integer = 0
        Dim deEndDate As Integer = 0
        Dim blah As Integer = 0

        Try
            startCell = stockData.sDate.IndexOf(stockData.sDate(0))
            'stockData = New yahooData(stockData, startCell - 1, startCell + stockData.sDate.Count - 1)
            i = 0
            monTotal = 0
            tuesTotal = 0
            wedTotal = 0
            thurTotal = 0
            friTotal = 0
            monUp = 0
            tuesUp = 0
            wedUp = 0
            thurUp = 0
            friUp = 0

            If stockData.per_gain.Last >= 0 Then
                cur_dir = True
                advanceDeclineStatsTable.Rows(2).Cells(0).Text = "Current Direction Up"
            Else
                cur_dir = False
                advanceDeclineStatsTable.Rows(2).Cells(0).Text = "Current Direction Down"
            End If
            Try
                Dim changed As Boolean = False
                Dim tchange As Boolean = False
                i = stockData.per_gain.Count - 1
                While (changed = False And i > 0)
                    If stockData.per_gain(i) >= 0 Then
                        tchange = True
                    Else : tchange = False
                    End If
                    If tchange = cur_dir Then
                        cur_dir_count += 1
                        i -= 1
                    Else
                        changed = True
                        cur_pos = i + 1
                    End If
                End While
            Catch ex As Exception
            End Try

            Try
                For i = 1 To stockData.per_gain.Count - 1 Step 1
                    curdate = Convert.ToDateTime(stockData.sDate(i))
                    'find how many of each day there is in the period
                    'find how many times on each day it advanced. 
                    If curdate.DayOfWeek = 1 Then 'monday
                        monTotal += 1
                        If stockData.per_gain(i) > 0 Then
                            monUp += 1
                        End If
                    ElseIf curdate.DayOfWeek = 2 Then
                        tuesTotal += 1
                        If stockData.per_gain(i) > 0 Then
                            tuesUp += 1
                        End If
                    ElseIf curdate.DayOfWeek = 3 Then
                        wedTotal += 1
                        If stockData.per_gain(i) > 0 Then
                            wedUp += 1
                        End If
                    ElseIf curdate.DayOfWeek = 4 Then
                        thurTotal += 1
                        If stockData.per_gain(i) > 0 Then
                            thurUp += 1
                        End If
                    ElseIf curdate.DayOfWeek = 5 Then
                        friTotal += 1
                        If stockData.per_gain(i) > 0 Then
                            friUp += 1
                        End If
                    End If

                    'find the longest adavance
                    If stockData.per_gain(i) >= 0 Then
                        If cur_dir = False And ldet = cur_dir_count Then
                            acell = New TableCell
                            arow = New TableRow
                            acell.Text = stockData.sDate(de_startt)
                            arow.Cells.Add(acell)
                            acell = New TableCell
                            If de_startt + 5 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(de_startt + 5) - stockData.close(de_startt)) / stockData.close(de_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                            acell = New TableCell

                            If de_startt + 20 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(de_startt + 20) - stockData.close(de_startt)) / stockData.close(de_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                            acell = New TableCell

                            If de_startt + 60 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(de_startt + 60) - stockData.close(de_startt)) / stockData.close(de_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                        End If
                        de_startt = 0
                        ldet = 0
                        down = False
                        If up = False Then
                            'mark the start of the advance
                            ad_startt = i
                        End If
                        up = True
                        If up = True Then
                            'count how many days in a row it advances
                            ladt += 1
                        End If
                    ElseIf stockData.per_gain(i) < 0 Then
                        up = False
                        If cur_dir = True And ladt = cur_dir_count Then
                            acell = New TableCell
                            arow = New TableRow
                            acell.Text = stockData.sDate(ad_startt)
                            arow.Cells.Add(acell)
                            acell = New TableCell
                            If ad_startt + 5 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(ad_startt + 5) - stockData.close(ad_startt)) / stockData.close(ad_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                            acell = New TableCell
                            If ad_startt + 20 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(ad_startt + 20) - stockData.close(ad_startt)) / stockData.close(ad_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                            acell = New TableCell
                            If ad_startt + 60 < stockData.close.Count Then
                                acell.Text = String.Format("{0:n2}", ((stockData.close(ad_startt + 60) - stockData.close(ad_startt)) / stockData.close(ad_startt)) * 100)
                            Else : acell.Text = ""
                            End If
                            arow.Cells.Add(acell)
                        End If
                        ad_startt = 0
                        ladt = 0
                        If down = False Then
                            'mark the start of the decline
                            de_startt = i
                        End If
                        down = True
                        If down = True Then
                            'count how many days ina  row it declines
                            ldet += 1
                        End If
                    End If
                    If ladt > lad Then
                        lad = ladt
                        ad_start = ad_startt
                    End If
                    If ldet > lde Then
                        lde = ldet
                        de_start = de_startt
                    End If
                Next
            Catch ex As Exception
            End Try

            Dim total_up As Double = monUp + tuesUp + wedUp + thurUp + friUp
            Dim total As Double = monTotal + tuesTotal + wedTotal + thurTotal + friTotal

            'display which days it advanced on and declined.
            advanceDeclineTable.Rows(1).Cells(1).Text = monUp
            advanceDeclineTable.Rows(1).Cells(2).Text = tuesUp
            advanceDeclineTable.Rows(1).Cells(3).Text = wedUp
            advanceDeclineTable.Rows(1).Cells(4).Text = thurUp
            advanceDeclineTable.Rows(1).Cells(5).Text = friUp
            advanceDeclineTable.Rows(1).Cells(6).Text = total_up

            advanceDeclineTable.Rows(2).Cells(1).Text = monTotal - monUp
            advanceDeclineTable.Rows(2).Cells(2).Text = tuesTotal - tuesUp
            advanceDeclineTable.Rows(2).Cells(3).Text = wedTotal - wedUp
            advanceDeclineTable.Rows(2).Cells(4).Text = thurTotal - thurUp
            advanceDeclineTable.Rows(2).Cells(5).Text = friTotal - friUp
            advanceDeclineTable.Rows(2).Cells(6).Text = total - total_up

            advanceDeclineTable.Rows(3).Cells(1).Text = String.Format("{0:n2}", monUp / monTotal * 100) & "% (" & monTotal & ")"
            advanceDeclineTable.Rows(3).Cells(2).Text = String.Format("{0:n2}", tuesUp / tuesTotal * 100) & "% (" & tuesTotal & ")"
            advanceDeclineTable.Rows(3).Cells(3).Text = String.Format("{0:n2}", wedUp / wedTotal * 100) & "% (" & wedTotal & ")"
            advanceDeclineTable.Rows(3).Cells(4).Text = String.Format("{0:n2}", thurUp / thurTotal * 100) & "% (" & thurTotal & ")"
            advanceDeclineTable.Rows(3).Cells(5).Text = String.Format("{0:n2}", friUp / friTotal * 100) & "% (" & friTotal & ")"
            advanceDeclineTable.Rows(3).Cells(6).Text = String.Format("{0:n2}", total_up / total * 100) & "% (" & total & ")"

            'If lad + ad_start > stockData.sDate.Count - 1 Then
            If lad > 0 Then
                advEndDate = lad - 1
            Else : advEndDate = lad
            End If
            If lde > 0 Then 'If lde + ad_start > stockData.sDate.Count - 1 Then
                deEndDate = lde - 1
            Else : deEndDate = lde
            End If

            'display the longest advance and decline
            If lad > 0 Then
                advanceDeclineStatsTable.Rows(0).Cells(1).Text = lad & " days ; " & Convert.ToDateTime(stockData.sDate(ad_start - 1)).ToString("MMM-dd-yy") & " - " & Convert.ToDateTime(stockData.sDate(ad_start + advEndDate)).ToString("MMM-dd-yy")
            Else : advanceDeclineStatsTable.Rows(0).Cells(1).Text = "No Advance"
            End If
            If lde > 0 Then
                advanceDeclineStatsTable.Rows(1).Cells(1).Text = lde & " days ; " & Convert.ToDateTime(stockData.sDate(de_start - 1)).ToString("MMM-dd-yy") & " - " & Convert.ToDateTime(stockData.sDate(de_start + deEndDate)).ToString("MMM-dd-yy")
            Else : advanceDeclineStatsTable.Rows(1).Cells(1).Text = "No Decline"
            End If

            advanceDeclineStatsTable.Rows(2).Cells(1).Text = cur_dir_count & " days ; " & Convert.ToDateTime(stockData.sDate(cur_pos)).ToString("MMM-dd-yy") & " - " & Convert.ToDateTime(stockData.sDate.Last).ToString("MMM-dd-yy")
        Catch ex As Exception

        End Try
    End Sub

End Class
