Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class money
    Inherits System.Web.UI.Page

    Public stockData As yahooData
    Public periodData As rangeStockData
    Public simData As simulate
    Public macdSeries As String = ""
    Public daysWorthData As Integer
    Public invested As Double = 0
    Public graphDate As String = "''"
    Public graphClose As String = "''"
    Public graphSeries As String = "series: [{ name: '', data: []}]"
    Public graphRange As String = "''"
    Public graphStockName As String = ""
    Public rsiSeries As String = ""
    Public rocSeries As String = ""
    Public sigSyncOption As Boolean = False

    'Public maList As New List(Of movingAverage)
    '****************PAGE SECTION ***********************************************************
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'trendButton.Attributes.Add("onClick", "this.form.target='_blank'")
        Dim dataValues As New List(Of String)
        Dim fromUrl As String = ""

        If User.Identity.Name <> "" Then
            moneySavePanel.Visible = True
            trendButton.Visible = True
            Try
                fromUrl = Request.UrlReferrer.ToString
                If fromUrl.Contains("trending") Then
                    dataValues.Add(Request.QueryString("ticker"))
                    dataValues.Add(Request.QueryString("capital"))
                    dataValues.Add(Request.QueryString("buyOpt"))
                    dataValues.Add(Request.QueryString("buySignal"))
                    dataValues.Add(Request.QueryString("sellOpt"))
                    dataValues.Add(Request.QueryString("sellSignal"))
                    dataValues.Add(Request.QueryString("tradeCost"))
                    loadFields(dataValues)
                End If
            Catch ex As Exception
            End Try
        Else : moneySavePanel.Visible = False
        End If
        Try
            displaySavedTable()
        Catch ex As Exception
        End Try
        resetErrorVisible()
        lookup.Text = lookup.Text.ToUpper
        maBuy.Text = maBuy.Text.ToUpper
        maSell.Text = maSell.Text.ToUpper
        maStop.Text = maStop.Text.ToUpper
        rsiBuy.Text = rsiBuy.Text.ToUpper
        rsiSell.Text = rsiSell.Text.ToUpper
        rocBuy.Text = rocBuy.Text.ToUpper
        rocSell.Text = rocSell.Text.ToUpper

        'Try
        '    If tradeCostPanel.Visible = True Then
        '        '                If Convert.ToDouble(tradeCostBox.Text) > 0 Then
        '        tradeCostButton.ForeColor = Drawing.Color.White
        '        tradeCostButton.BackColor = Drawing.Color.Green
        '        'End If
        '    Else : tradeCostButton.ForeColor = Drawing.Color.Black
        '        tradeCostButton.BackColor = Drawing.Color.Empty
        '    End If
        'Catch ex As Exception
        '    tradeCostButton.ForeColor = Drawing.Color.Black
        '    tradeCostButton.BackColor = Drawing.Color.Empty
        'End Try
        buttonOn(tradeCostPanelExtender, tradeCostButton)
        buttonOn(tradePanelExtender, tradeButton)
        buttonOn(resultsPanelExtender, resultsButton)
        If syncHide.Value = "on" Then
            signalSync.ForeColor = Drawing.Color.White
            signalSync.BackColor = Drawing.Color.Green
            sigSyncOption = True
        Else
            signalSync.ForeColor = Drawing.Color.Black
            signalSync.BackColor = Drawing.Color.Empty
            sigSyncOption = False
        End If
        resetColors()
        If stockData Is Nothing Then
            graphButton.Enabled = False
            graphMacdButton.Enabled = False
            graphRocButton.Enabled = False
            graphRsiButton.Enabled = False
        End If
    End Sub

    'Button Controls
    Sub resetColors()
        weekButton.BackColor = Drawing.Color.Empty
        monthButton.BackColor = Drawing.Color.Empty
        threeMonthButton.BackColor = Drawing.Color.Empty
        sixMonthButton.BackColor = Drawing.Color.Empty
        yearButton.BackColor = Drawing.Color.Empty
        customEnable.BackColor = Drawing.Color.Empty
        analyze.BackColor = Drawing.Color.Empty
    End Sub

    Sub buttonOn(ByVal panelExt As AjaxControlToolkit.CollapsiblePanelExtender, _
                ByRef gButton As Button)
        If panelExt.ClientState = "true" Then
            gButton.BackColor = Drawing.Color.Empty
            gButton.ForeColor = Drawing.Color.Empty
        ElseIf panelExt.ClientState = "false" Then
            gButton.BackColor = Drawing.Color.Green
            gButton.ForeColor = Drawing.Color.White
        End If
    End Sub

    'BUTTON SECTION
    Protected Sub weekButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles weekButton.Click
        Dim hasErrors As Boolean = False
        Dim stockName As String = ""
        weekButton.BackColor = Drawing.Color.Lime
        customEnable_Click(sender, e)
        hasErrors = checkForErrors()
        If hasErrors = False Then
            setupTrade(5, hasErrors)
            If hasErrors = False Then
                graphSetup()
            End If
        End If
    End Sub

    Protected Sub monthButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles monthButton.Click
        Dim hasErrors As Boolean = False
        customEnable_Click(sender, e)
        monthButton.BackColor = Drawing.Color.Lime
        hasErrors = checkForErrors()
        If hasErrors = False Then
            setupTrade(30, hasErrors)
            If hasErrors = False Then
                graphSetup()
            End If
        End If
    End Sub

    Protected Sub threeMonthButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles threeMonthButton.Click
        Dim hasErrors As Boolean = False
        hasErrors = checkForErrors()
        customEnable_Click(sender, e)
        threeMonthButton.BackColor = Drawing.Color.Lime
        If hasErrors = False Then
            setupTrade(90, hasErrors)
            If hasErrors = False Then
                graphSetup()
            End If
        End If
    End Sub

    Protected Sub sixMonthButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles sixMonthButton.Click
        Dim hasErrors As Boolean = False
        hasErrors = checkForErrors()
        customEnable_Click(sender, e)
        sixMonthButton.BackColor = Drawing.Color.Lime
        If hasErrors = False Then
            setupTrade(180, hasErrors)
            If hasErrors = False Then
                graphSetup()
            End If
        End If
    End Sub

    Protected Sub yearButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles yearButton.Click
        Dim hasErrors As Boolean = False
        hasErrors = checkForErrors()
        customEnable_Click(sender, e)
        yearButton.BackColor = Drawing.Color.Lime
        If hasErrors = False Then
            setupTrade(365, hasErrors)
            If hasErrors = False Then
                graphSetup()
            End If
        End If
    End Sub

    Protected Sub analyzeClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles analyze.Click
        Dim hasErrors As Boolean = False
        Dim startDate As Date
        Dim endDate As Date
        Dim webLink As String = ""
        Dim dateDifference As Integer = 0
        hasErrors = checkForErrors()
        customEnable.BackColor = Drawing.Color.Lime
        analyze.BackColor = Drawing.Color.Lime
        Try
            If customRange.Text.Contains("-") Then
                startDate = Convert.ToDateTime(customRange.Text.Substring(0, customRange.Text.IndexOf("-")))
                endDate = Convert.ToDateTime(customRange.Text.Substring(customRange.Text.IndexOf("-") + 1, customRange.Text.Count - customRange.Text.IndexOf("-") - 1))
            End If
        Catch ex As Exception
            customEr.Visible = True
            customEr.Text = "Bad Range"
            hasErrors = True
        End Try
        If hasErrors = False Then
            dateDifference = DateDiff(DateInterval.Day, startDate, endDate)
            setupTrade(dateDifference, hasErrors, startDate, endDate)
            graphSetup()
        End If
    End Sub

    Protected Sub customEnable_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles customEnable.Click
        Me.customPanelExtender.Collapsed = True
        Me.customPanelExtender.ClientState = "true"
    End Sub

    Protected Sub moneySave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles moneySave.Click
        saveMoney("moneySaved")
        displaySavedTable()
        moneyName.Text = ""
        note.Text = ""
    End Sub

    Protected Sub moneyLoad_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles moneyLoad.Click
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim values() As String = {"name", "ticker", "buyOpt", "buySignal", "sellOpt", "sellSignal", "capital", "date", "notes"}
        Dim i As Integer = 0
        '  Dim valueString As String
        Dim buyOpt As String = ""
        Dim buySignal As String = ""
        Dim sellOpt As String = ""
        Dim sellSignal As String = ""
        Dim dataValues As List(Of String)
        Try
            panelExtenderOff(Me.BuyPanelExtender)
            panelExtenderOff(Me.SellPanelExtender)
            panelExtenderOff(Me.maAvgBuyPanelExtender)
            panelExtenderOff(Me.maAvgSellPanelExtender)
            panelExtenderOff(Me.macdBuyPanelExtender)
            panelExtenderOff(Me.macdSellPanelExtender)
            panelExtenderOff(Me.rsiBuyPanelExtender)
            panelExtenderOff(Me.rsiSellPanelExtender)
            panelExtenderOff(Me.rocBuyPanelExtender)
            panelExtenderOff(Me.rocSellPanelExtender)

            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("Select Signal"))
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("Select Signal"))
            lookup.Text = ""
            investValue.Text = ""
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT username, name, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, date, tradeCost, notes FROM bobpar.moneySaved " +
                                "WHERE (username = '" & User.Identity.Name & "') AND (name = '" & moneyName.Text & "')"
            myreader = mycom.ExecuteReader
            While myreader.Read()
                dataValues = New List(Of String)
                dataValues.Add(myreader("ticker"))     '0
                dataValues.Add(myreader("capital"))    '1
                dataValues.Add(myreader("buyOpt"))     '2
                dataValues.Add(myreader("buySignal"))  '3
                dataValues.Add(myreader("sellOpt"))    '4
                dataValues.Add(myreader("sellSignal")) '5
                dataValues.Add(myreader("tradeCost"))  '6
                loadFields(dataValues)
            End While
            con.Close()
        Catch ex As Exception

        End Try
        displaySavedTable()
        moneyName.Text = ""
        note.Text = ""
    End Sub

    Protected Sub moneyDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles moneyDelete.Click
        deleteSavedTest("save")
        displaySavedTable()
        moneyName.Text = ""
        note.Text = ""
    End Sub

    Protected Sub trendButton_Click(sender As Object, e As System.EventArgs) Handles trendButton.Click
        Dim hasErrors As Boolean = False
        hasErrors = checkForErrors()
        If hasErrors = False Then
            setupTrade(30, hasErrors)
            If hasErrors = False Then
                saveMoney("trends")
                Response.Redirect("~/trending.aspx")
            End If
        End If
    End Sub

    'CHECK INPUTS FOR ERRORS
    Function checkForErrors() As Boolean
        Dim hasErrors As Boolean = False

        Try
            resetErrorVisible()
            If checkForTickCostErrors() Then 'Check Ticker and Cost Errors
                Return True
            ElseIf checkSigForErrors(buyOption) Then 'Check Buy Signal
                Return True
            ElseIf checkSigForErrors(sellOption) Then 'check Sell Signal
                Return True
            ElseIf checkStopForErrors(stopOption) Then
                Return True
            End If
        Catch ex As Exception
        End Try
        Return False
    End Function

    'Clears the errors.
    Sub resetErrorVisible()
        lookupError.Visible = False
        capitalEr.Visible = False
        buyError.Visible = False
        sellError.Visible = False
        stopError.Visible = False
        resultsTable.Rows.Clear()
        tradeList.Rows.Clear()
        resultsTable.Visible = False
        tradeList.Visible = False
    End Sub

    'Checks for Errors in  the Ticker and Cost Textboxes.
    Function checkForTickCostErrors() As Boolean
        Dim hasErrors As Boolean = False
        'Dim lookupCheck As Char
        Dim checkChar As Char
        Dim checkNum As Double
        Dim i As Integer

        'Make sure Ticker isn't too long
        If lookup.Text = "" Then
            lookupError.Text = "Need a Ticker Symbol"
            lookupError.Visible = True
            Return True
        ElseIf lookup.Text.Count > 7 Then
            lookupError.Text = "Ticker Too Long"
            lookupError.Visible = True
            Return True
        End If
        'Check trade cost to make sure it's a number
        If tradeCostPanelExtender.ClientState = "false" Then
            Try
                checkNum = Convert.ToDouble(tradeCostBox.Text)
                checkNum = 0
            Catch ex As Exception
                lookupError.Text = "Trade Cost Must Be A Number"
                lookupError.Visible = True
                Return True
            End Try
        End If
        'Check for non characters in Ticker Symbol
        If lookup.Text.Contains("^") Then
            If lookup.Text.IndexOf("^") <> 0 Then
                lookupError.Text = "^"
                lookupError.Visible = "^ Must Be First Letter"
                Return True
            End If
            'hasErrors = 
            If checkForMultiCommand(lookup.Text, "^") Then
                lookupError.Text = "Too many ^"
                lookupError.Visible = True
                Return True
            End If
        End If
        If lookup.Text.Contains(".") Then
            Dim suffix As String
            '. can be in 3, 4 or 5 
            If lookup.Text.IndexOf(".") < 3 And
                lookup.Text.IndexOf(".") > 5 Then
                lookupError.Visible = True
                lookupError.Text = ". Must Be the 4th, 5th or 6th Letter"
                Return True
            End If
            If checkForMultiCommand(lookup.Text, ".") Then
                lookupError.Visible = True
                lookupError.Text = "To many ."
                Return True
            End If
            suffix = lookup.Text.Substring(lookup.Text.IndexOf(".") + 1)
            If suffix.Count = 0 Then
                lookupError.Visible = True
                lookupError.Text = "Must of Suffix after ."
                Return True
            End If
        End If
        For i = 0 To lookup.Text.Count - 1 Step 1
            checkChar = Convert.ToChar(lookup.Text(i))
            If Char.IsLetterOrDigit(checkChar) = False Then
                If (checkChar <> (".") And checkChar <> ("^")) Then
                    'If Char.IsLetterOrDigit Then
                    lookupError.Visible = True
                    lookupError.Text = checkChar & " Bad Letter"
                    Return True
                End If
            End If
        Next
        'Check the Cost
        If investValue.Text = "" Then
            capitalEr.Visible = True
            capitalEr.Text = "Investment Value can't be blank"
            Return True
        End If
        Try
            checkNum = Convert.ToDouble(investValue.Text)
        Catch ex As Exception
            capitalEr.Visible = True
            capitalEr.Text = "Invalid Investment Value. Only Numbers Allowed."
            Return True
        End Try
        Return False
    End Function

    Function checkSigForErrors(ByVal sigOption As DropDownList) As Boolean
        Dim hasErrors As Boolean = True
        Dim sigError As New Label
        Dim sigBox As New TextBox

        If sigOption.ID = "buyOption" Then
            sigError = buyError
            If sigOption.SelectedValue = "Price" Then
                sigBox = priceBuy
                If checkPriceSignals(sigBox.Text, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "maAvg" Then
                sigBox = maBuy
                If checkMovingAvg(sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "macd" Then
                sigBox = macdBuy
                If checkMacd(sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "rsi" Then
                sigBox = rsiBuy
                If checkRsiorRoc("RSI", sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "roc" Then
                sigBox = rocBuy
                If checkRsiorRoc("ROC", sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            Else : sigError.Text = "Need to Select a Signal"
                sigError.Visible = True
                Return True
            End If
        ElseIf sigOption.ID = "sellOption" Then
            sigError = sellError
            If sigOption.SelectedValue = "Price" Then
                sigBox = priceSell
                If checkPriceSignals(sigBox.Text, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "maAvg" Then
                sigBox = maSell
                If checkMovingAvg(sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "macd" Then
                sigBox = macdSell
                If checkMacd(sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "rsi" Then
                sigBox = rsiSell
                If checkRsiorRoc("RSI", sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "roc" Then
                sigBox = rocSell
                If checkRsiorRoc("ROC", sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            Else
                sigError.Text = "Need to Select Sell Signal"
                sigError.Visible = True
                Return True
            End If
        End If
        Return False
    End Function

    Function checkStopForErrors(ByVal sigOption As DropDownList) As Boolean
        Dim sigError As New Label
        Dim sigBox As New TextBox
        sigBox = stop1
        If sigOption.ID = "stopOption" Then
            sigError = stopError
            If sigOption.SelectedValue = "Price" Then
                If checkPriceSignals(sigBox.Text, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            ElseIf sigOption.SelectedValue = "maAvg" Then
                sigBox = maStop
                If checkMovingAvg(sigBox, sigError) Then
                    sigError.Visible = True
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Function checkPriceSignals(ByVal checkValue As String, _
                               ByRef errorLabel As Label) As Boolean
        Dim hasErrors As Boolean = False
        Dim checkNum As Double = 0
        Dim hasLess As Boolean = False
        Dim hasGreat As Boolean = False
        Dim hasDash As Boolean = False
        Dim checkChar As Char = ""
        Dim i As Integer = 0

        If checkValue = "" Then
            errorLabel.Text = "Can't Be Blank"
            hasErrors = True
        End If

        If hasErrors = False Then
            If checkValue.Contains("<") Then
                hasLess = True
                If checkValue.LastIndexOf("<") <> 0 Then
                    errorLabel.Text = "< Must Be First Letter"
                    hasErrors = True
                End If
            ElseIf checkValue.Contains(">") Then
                hasGreat = True
                If checkValue.LastIndexOf(">") <> 0 Then
                    hasErrors = True
                    errorLabel.Text = "> Must Be First Letter"
                End If
            ElseIf checkValue.Contains(" ") Then
                errorLabel.Text = "Spaces Not Allowed"
                hasErrors = True
            End If

            If checkValue.Contains("-") Then
                hasDash = True
                If hasGreat = True Or hasLess = True Then
                    errorLabel.Text = "<, > Not allowed with - Command"
                    hasErrors = True
                End If
            End If
        End If

        'Check for dash
        If hasErrors = False Then
            If hasDash Then
                hasErrors = checkForMultiCommand(checkValue, "-")
                If hasErrors = False Then
                    If (hasGreat = True) Or (hasLess = True) Then
                        'checkDashCommand(checkValue.Substring(1),)
                        checkDashCommand(checkValue.Substring(1), errorLabel.Text, hasErrors)
                    Else
                        checkDashCommand(checkValue.Substring(0), errorLabel.Text, hasErrors)
                    End If
                Else : errorLabel.Text = "To Many Dashes"
                End If
            End If
        End If

        ''check for Letters and Special Letters
        If hasErrors = False Then
            If hasDash = False Then
                Try
                    If hasLess = True Or hasGreat = True Then
                        checkNum = Convert.ToDouble(checkValue.Substring(1))
                    Else : checkNum = Convert.ToDouble(checkValue.Substring(0))
                    End If
                Catch ex As Exception
                    errorLabel.Text = "Only Numbers Allowed"
                    hasErrors = True
                End Try
            End If
        End If
        'set the error message and send back
        If hasErrors = True Then
            'If buySellSwitch = True Then
            '    buyError.Text = errorLabel.Text
            '    buyError.Visible = True
            'Else : sellError.Text = errorLabel.Text
            '    sellError.Visible = True
            'End If
            Return True
        Else
            Return False
        End If
    End Function

    Function checkMovingAvg(ByRef box As TextBox, _
                            ByRef errorLabel As Label) As Boolean
        Dim hasErrors As Boolean = False
        Dim hasLess As Boolean = False
        Dim hasGreat As Boolean = False
        Dim maString As String = ""
        Dim range() As String

        Try
            If box.Text.Contains(" ") Then
                errorLabel.Text = "No Spaces Allowed"
                Return True
            End If
            If box.Text.Contains("<") Then
                hasLess = True
            ElseIf box.Text.Contains(">") Then
                hasGreat = True
            Else
                errorLabel.Text = "Must have either > or <"
                Return True
            End If

            If hasLess = True Then 'Or hasGreat = True Then
                range = box.Text.Split("<")
            Else : range = box.Text.Split(">")
            End If
            If range.Count <> 2 Then
                errorLabel.Text = "Can only contain one < or > "
                Return True
            ElseIf range(0) = range(1) Then
                errorLabel.Text = "Can't be the same value"
                Return True
            End If
            maString = range(0).Substring(0, 2)
            If Not (range(0) <> "PRICE" Or _
                range(0).Substring(0, 2) <> "SMA" Or _
                range(0).Substring(0, 2) <> "EMA") Then
                errorLabel.Text = "Must start with PRICE, SMA, or EMA"
                Return True
            End If
            If Not (range(1) <> "PRICE" Or _
                range(1).Substring(0, 2) <> "SMA" Or _
                range(1).Substring(0, 2) <> "EMA") Then
                errorLabel.Text = "Must have PRICE, SMA, or EMA after < >"
                Return True
            End If
            If range(0) <> "PRICE" Then
                hasErrors = checkmaRange(range(0).Substring(3, range(0).Count - 3), errorLabel)
                If hasErrors = True Then
                    Return hasErrors
                End If
            End If
            If range(1) <> "PRICE" Then
                hasErrors = checkmaRange(range(1).Substring(3, range(1).Count - 3), errorLabel)
                If hasErrors = True Then
                    Return hasErrors
                End If
            End If
            Return hasErrors
        Catch ex As Exception
            errorLabel.Text = "Bad Format, follow Examples"
            Return True
        End Try
    End Function

    Function checkmaRange(ByVal maString As String, _
                          ByRef errorLabel As Label) As Boolean
        Dim checkNum As Integer = 0
        Try
            checkNum = Convert.ToInt32(maString)
            If checkNum < 1 Or checkNum > 200 Then
                errorLabel.Text = "Must be between 1-200"
                Return True
            End If
            Return False
        Catch ex As Exception
            errorLabel.Text = "After funtion must contain a number."
            Return True
        End Try
    End Function

    Function checkMacd(ByRef box As TextBox,
                       ByRef errorLabel As Label) As Boolean
        Dim condition As String = ""
        Dim conditionSplit() As String

        If box.Text.Contains(" ") Then
            errorLabel.Text = "No Spaces Allowed"
            Return True
        End If
        If box.Text.Contains("<") Then
            condition = "<"
        ElseIf box.Text.Contains(">") Then
            condition = ">"
        ElseIf box.Text.Contains("=") Then
            condition = "="
        Else
            errorLabel.Text = "Must have either <, > or ="
            Return True
        End If
        box.Text = box.Text.ToUpper()
        conditionSplit = box.Text.Split(condition)
        If conditionSplit.Count > 2 Then
            errorLabel.Text = "Can't have multiple " & condition
            Return True
        ElseIf conditionSplit.Count = 1 Then
            errorLabel.Text = "Condition must be in the middle of two operands"
            Return True
        End If
        If checkMacdOperand(conditionSplit(0)) Then
            errorLabel.Text = "First operand must be MACD, DIV, EMA or a number"
            Return True
        End If
        If checkMacdOperand(conditionSplit(1)) Then
            errorLabel.Text = "Second operand must be MACD, DIV, EMA or a number"
            Return True
        End If
        Return False
    End Function

    Function checkMacdOperand(ByVal theBox As String) As Boolean
        Dim numTest As Double = 0
        Dim hasError As Boolean = False
        If theBox = "MACD" Or theBox = "DIV" Or theBox = "EMA" Then
            Return False
        End If
        Try
            numTest = Convert.ToDouble(theBox)
            Return False
        Catch ex As Exception
            Return True
        End Try
    End Function

    Function checkRsiorRoc(ByVal indicator As String, _
                           ByRef box As TextBox,
                           ByRef errorLabel As Label) As Boolean
        Dim numSplit() As String
        Dim condition As String = ""
        Dim checkNum As Double = 0
        Dim checkPeriod As Integer = 0

        If (box.Text.IndexOf("<") > -1) Then
            condition = "<"
        ElseIf (box.Text.IndexOf(">") > -1) Then
            condition = ">"
        End If

        If condition = "" Then
            errorLabel.Text = "Must have < or >"
            Return True
        End If
        numSplit = box.Text.Split(condition)
        Try
            If numSplit(0).IndexOf(indicator) <> 0 Then
                errorLabel.Text = "Must Start with " & indicator
                Return True
            Else
                checkPeriod = Convert.ToInt32(numSplit(0).Substring(3, numSplit(0).Length - 3))
            End If
        Catch ex As Exception
            errorLabel.Text = "Must have a number after" & indicator
            Return True
        End Try
        Try
            checkNum = Convert.ToDouble(numSplit(1))
            If (indicator = "RSI") And (checkNum < 1 Or checkNum > 300) Then
                errorLabel.Text = "Must be a number between 1-300"
                Return True               
            End If
        Catch ex As Exception
            errorLabel.Text = "Must be a number between 1-300"
            Return True
        End Try
        Return False
    End Function

    Function checkForMultiCommand(ByVal checkValue As String, _
                                 ByVal command As Char) As Boolean
        'If search from the left index isn't the same as search from the right then there 
        'Must be two and will return true. 

        If checkValue.IndexOf(command) <> checkValue.LastIndexOf(command) Then
            Return True
        Else : Return False
        End If
    End Function

    Sub checkDashCommand(ByVal checkValue As String, _
                         ByRef errorMsg As String, _
                         ByRef errorSig As Boolean)
        Dim dashsplit() As String = checkValue.Split("-")
        Dim startNum As Double = 0
        Dim endNum As Double = 0
        errorMsg = "No Errors"
        errorSig = False
        Try
            startNum = Convert.ToDouble(dashsplit(0))
        Catch ex As Exception
            errorMsg = "Bad Start Number"
            errorSig = True
        End Try
        Try
            endNum = Convert.ToDouble(dashsplit(1))
        Catch ex As Exception
            errorMsg = "Bad End Number"
            errorSig = True
        End Try
        If startNum > endNum Then
            errorMsg = "Start Number Greater than End Number"
            errorSig = True
        End If
    End Sub

    'checks to make sure valid numbers between comma and percent sign
    ' examples 9%4 is valid
    ' 9,400 is valid  
    Sub checkSplit(ByVal checkValue As String, _
                   ByVal command As String, _
                   ByRef errorMsg As String, _
                   ByRef errorSig As Boolean)
        Dim dashsplit() As String = checkValue.Split(command)
        Dim startNum As Double = 0
        Dim endNum As Double = 0

        errorMsg = ""
        errorSig = False
        Try
            startNum = Convert.ToDouble(dashsplit(0))
        Catch ex As Exception
            errorMsg = "Bad Start Number"
            errorSig = True
        End Try
        Try
            endNum = Convert.ToDouble(dashsplit(1))
        Catch ex As Exception
            errorMsg = "Bad End Number"
            errorSig = True
        End Try
    End Sub

    Function checkStockData() As Boolean
        Try
            If stockData.close.Count = 0 Then
                lookupError.Text = "Bad Ticker"
                lookupError.Visible = True
                Return False
            Else : Return True
            End If
        Catch ex As Exception
            lookupError.Text = "Bad Ticker"
            lookupError.Visible = True
            Return False
        End Try
    End Function

    'Selects the Buy Signal and runs the approate setup
    Sub setupTrade(ByVal range As Integer, _
                   ByRef setupError As Boolean, _
                   Optional ByVal startDateOp As Date = Nothing, _
                   Optional ByVal endDateOp As Date = Nothing)

        Dim buyTriggerValue() As Double = {0, 0}
        Dim sellTriggerValue() As Double = {0, 0}
        Dim buyCommand As Integer = 0
        Dim sellCommand As Integer = 0

        Dim buySignals As New List(Of String)
        Dim sellSignals As New List(Of String)
        Dim macdDoubles As New List(Of Double)
        Dim macdEmaDoubles As New List(Of Double)
        Dim macdDivDoubles As New List(Of Double)

        Dim rsiBuyDouble As New List(Of Double)
        Dim rsiSellDouble As New List(Of Double)

        Dim rocBuyDouble As New List(Of Double)
        Dim rocSellDouble As New List(Of Double)
        Dim simValues As New List(Of String)

        simValues.Add(lookup.Text)
        simValues.Add(buyOption.SelectedValue)

        '        simValues.Add(optionSelect(buyOption.SelectedValue))
        '7 - Buy Signal
        If buyOption.SelectedValue = "Price" Then
            simValues.Add(priceBuy.Text)
        ElseIf buyOption.SelectedValue = "maAvg" Then
            simValues.Add(maBuy.Text)
        ElseIf buyOption.SelectedValue = "macd" Then
            simValues.Add(macdBuy.Text)
        ElseIf buyOption.SelectedValue = "rsi" Then
            simValues.Add(rsiBuy.Text)
        ElseIf buyOption.SelectedValue = "roc" Then
            simValues.Add(rocBuy.Text)
        End If

        '7 - Sell Signal
        simValues.Add(sellOption.SelectedValue)
        'simValues.Add(optionSelect(sellOption.SelectedValue))

        If sellOption.SelectedValue = "Price" Then
            simValues.Add(priceSell.Text)
        ElseIf sellOption.SelectedValue = "maAvg" Then
            simValues.Add(maSell.Text)
        ElseIf sellOption.SelectedValue = "macd" Then
            simValues.Add(macdSell.Text)
        ElseIf sellOption.SelectedValue = "rsi" Then
            simValues.Add(rsiSell.Text)
        ElseIf sellOption.SelectedValue = "roc" Then
            simValues.Add(rocSell.Text)
        End If
        simValues.Add(tradeCostBox.Text)
        simValues.Add(investValue.Text)
        simData = New simulate(simValues, , range, sigSyncOption)
        If simData.periodData.close.Count = 0 Then
            lookupError.Text = "Bad Ticker"
            lookupError.Visible = True
            setupError = True
            Exit Sub
        End If
        resultsTableLoad()
    End Sub

    'This function is on hold. Trying to make the buy and sell selection signal selection smaller.
    'Function optionSelect(ByVal chooseOpt As String, _
    '                      ByVal valBox As TextBox) As String
    '    Dim selectedOpt As String = ""

    '    If chooseOpt = "Price" Then
    '        selectedOpt = priceSell.Text
    '    ElseIf chooseOpt = "maAvg" Then
    '        selectedOpt = maSell.Text
    '    ElseIf chooseOpt = "macd" Then
    '        selectedOpt = macdSell.Text
    '    ElseIf chooseOpt = "rsi" Then
    '        selectedOpt = rsiSell.Text
    '    ElseIf chooseOpt = "roc" Then
    '        selectedOpt = rocSell.Text
    '    End If
    '    Return selectedOpt
    'End Function

    Sub resultsTableLoad()
        'LOAD RESULTS TABLES 
        Dim k As Integer = 0
        Dim arow As TableRow
        Dim acell As TableCell

        If simData.periodData.close.Count = 0 Then
            resultsTable.Caption = "<b> BAD STOCK <B>"
            resultsTable.Visible = True
            Exit Sub
        End If

        resultsTable.Caption = "<b> RESULTS</b>"
        resultsTable.Visible = True
        tradeList.Visible = True
        Try
            For i = 0 To 24 Step 1
                arow = New TableRow
                For k = 0 To 3 Step 1
                    acell = New TableCell
                    If i = 0 Then
                        If k = 0 Then
                            acell.Text = "<b> Summary </b>"
                        ElseIf k = 2 Then
                            acell.Text = "<b> Trades </b>"
                        End If
                    ElseIf i = 10 And k = 0 Then
                        acell.Text = "<b> Gains </b>"
                    End If
                    'If i = 9 Then
                    '    Dim blah = 0
                    'End If
                    arow.Cells.Add(acell)
                Next
                resultsTable.Rows.Add(arow)
            Next
        Catch ex As Exception
        End Try
        Try
            arow = New TableRow
            acell = New TableCell
            acell.Text = "<b> <center> Cell # </center> </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center> Price </center> </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center>Date </center> </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center> Entry Value </center> </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center> Exit Value </center>  </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center> Cash Gain </center> </b>"
            arow.Cells.Add(acell)

            acell = New TableCell
            acell.Text = "<b> <center> Percent Gain </center>  </b>"
            arow.Cells.Add(acell)
            tradeList.Rows.Add(arow)

            'load the trade lists table
            Dim resultSplitString() As String

            If simData.simError <> "" Then
                resultsTable.Caption = "<b> RESULTS - " & simData.simError & "</b>"
            End If

            If simData.tradeResults.Count = 0 Then
                resultsTable.Caption = "<b> RESULTS - NO TRADES</b>"
                resultsTable.Visible = True
            End If

            If simData.tradeResults.Count > 0 Then
                For i = 0 To simData.tradeResults.Count - 1 Step 1
                    resultSplitString = simData.tradeResults(i).Split(" ")
                    arow = New TableRow
                    If resultSplitString.Count = 4 Then
                        For k = 0 To 3 Step 1
                            acell = New TableCell
                            If k = 2 Then
                                acell.Text = "<center>" & Convert.ToDateTime(resultSplitString(k)).ToString("MM-dd-yyyy") & "</center>"
                            Else
                                acell.Text = "<center>" & resultSplitString(k) & "</center>"
                            End If
                            arow.Cells.Add(acell)
                        Next
                    ElseIf resultSplitString.Count = 7 Then
                        For k = 0 To 6 Step 1
                            acell = New TableCell
                            If k = 2 Then
                                acell.Text = "<center>" & Convert.ToDateTime(resultSplitString(k)).ToString("MM-dd-yyyy") & "</center>"
                            Else
                                acell.Text = "<center>" & resultSplitString(k) & "</center>"
                            End If
                            arow.Cells.Add(acell)
                        Next
                    End If
                    tradeList.Rows.Add(arow)
                Next
            End If
        Catch ex As Exception
        End Try

        'Some sort of bug can't use "&lt;" in a table need to replace it with &lt;
        'the value will still have the "<" but when displayed in a tablecell it will
        'just show whatever is before the "<" blah2<blah1 becomes blah2. to get 
        'blah2<blah1 you need to do blah2&lt;blah1
        Try

            If simData.summaryResults(6, 1).Contains("<") Then
                simData.summaryResults(6, 1) = simData.summaryResults(6, 1).Replace("<", "&lt;")
            End If

            If simData.summaryResults(7, 1).Contains("<") Then
                simData.summaryResults(7, 1) = simData.summaryResults(7, 1).Replace("<", "&lt;")
            End If
            'load the table with summary results.
            For i = 1 To 9 Step 1
                resultsTable.Rows(i).Cells(0).Text = simData.summaryResults(i - 1, 0)
                resultsTable.Rows(i).Cells(1).Text = simData.summaryResults(i - 1, 1)
            Next

            'load table with gain results
            For i = 11 To 16 Step 1
                If simData.gainsResults(i - 11, 0).Contains("stopError") Then
                    simData.gainsResults(i - 11, 0) = simData.gainsResults(i - 11, 0).Replace("stopError", "")
                End If
                resultsTable.Rows(i).Cells(0).Text = simData.gainsResults(i - 11, 0)
                resultsTable.Rows(i).Cells(1).Text = simData.gainsResults(i - 11, 1)
            Next

            'load table with trade results
            For i = 1 To 14 Step 1
                If simData.tradeSummary(i - 1, 0).Contains("stopError") Then
                    simData.tradeSummary(i - 1, 0) = simData.tradeSummary(i - 1, 0).Replace("stopError", "")
                End If
                resultsTable.Rows(i).Cells(2).Text = simData.tradeSummary(i - 1, 0)
                resultsTable.Rows(i).Cells(3).Text = simData.tradeSummary(i - 1, 1)
            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub graphSetup()
        Dim i As Integer = 0
        Dim colors As List(Of String) = New List(Of String)(New String() {
                                         "0,128,0", _
                                         "255,128,0", _
                                         "0,255,255", _
                                         "0,0,255", _
                                         "128,0,64", _
                                         "128,0,255"})
        Try
            graphDate = simData.periodData.graphDate(simData.periodData.sDate)
            graphClose = simData.periodData.graphDouble(simData.periodData.close)
            graphSeries = "series: [{ name:'" & simData.periodData.ticker & "', data:[" & graphClose & "]}"
            For i = 0 To simData.maList.Count - 1 Step 1
                graphSeries &= ",{color: 'rgb(" & colors(i) & ")', name:'" & simData.maList(i).name & "',data:[" & simData.maList(i).values & "]}"
            Next
            graphSeries &= "]"
            graphRange = simData.periodData.graphRangeTitle(simData.periodData.sDate)
            graphStockName = simData.periodData.ticker
            graphButton.Enabled = True
            graphMacdButton.Enabled = True
            drawGraphs()
        Catch ex As Exception
        End Try
    End Sub

    'Graph Function 
    Sub drawGraphs()
        Dim macdString As String = ""
        Dim macdEmaString As String = ""
        Dim macdDivString As String = ""
        Dim macdDoubles As New List(Of Double)
        Dim macdEmaDoubles As New List(Of Double)
        Dim macdDivDoubles As New List(Of Double)

        Dim rsiBuyPeriod As Integer = 0
        Dim rsiSellPeriod As Integer = 0
        Dim rsiBuyGraph As String = ""
        Dim rsiSellGraph As String = ""
        Dim rsiBuyDouble As New List(Of Double)
        Dim rsiSellDouble As New List(Of Double)

        Dim rocBuyPeriod As Integer = 0
        Dim rocSellPeriod As Integer = 0
        Dim rocBuyGraph As String = ""
        Dim rocSellGraph As String = ""
        Dim rocBuyDouble As New List(Of Double)
        Dim rocSellDouble As New List(Of Double)

        simData.periodData.graphMACD(simData.stockData, macdString, macdEmaString, macdDivString)
        macdDoubles = simData.periodData.convertToDoubleList(macdString)
        macdEmaDoubles = simData.periodData.convertToDoubleList(macdEmaString)
        macdDivDoubles = simData.periodData.convertToDoubleList(macdDivString)
        macdSeries = "series: [{ name:'MACD', data:[" & macdString & "]},{color: 'rgb(0,128,0)', name:'EMA9', data:[" _
                        & macdEmaString & "]},{color: 'rgb(255,128,0)', name:'Div', data:[" & macdDivString & "]}]"
        If rsiBuy.Text <> "" Then
            rsiBuyPeriod = findRsiorRocPeriod("I", rsiBuy)
            graphRsiButton.Enabled = True
        End If
        If rsiSell.Text <> "" Then
            rsiSellPeriod = findRsiorRocPeriod("I", rsiSell)
            graphRsiButton.Enabled = True
        End If
        If rsiBuyPeriod > 0 Then
            rsiBuyGraph = simData.periodData.graphRsi(simData.stockData, rsiBuyPeriod)
            'rsiBuyDouble = simData.periodData.convertToDoubleList(rsiBuyGraph)
        End If
        If rsiSellPeriod > 0 Then
            rsiSellGraph = simData.periodData.graphRsi(simData.stockData, rsiSellPeriod)
            'rsiSellDouble = simData.periodData.convertToDoubleList(rsiSellGraph)
        End If
        rsiSeries = "series: [{ name:'RSI', data:[""]}]"
        If rsiBuyPeriod > 0 And rsiSellPeriod = 0 Then
            rsiSeries = "series: [{ name:'RSI " & rsiBuyPeriod & "', data:[" & rsiBuyGraph & "]}]"
        ElseIf rsiBuyPeriod = 0 And rsiSellPeriod > 0 Then
            rsiSeries = "series: [{ name:'RSI " & rsiSellPeriod & "', data:[" & rsiSellGraph & "]}]"
        ElseIf rsiBuyPeriod = rsiSellPeriod Then
            rsiSeries = "series: [{ name:'RSI " & rsiBuyPeriod & "', data:[" & rsiBuyGraph & "]}]"
        Else
            rsiSeries = "series: [{ name:'RSI " & rsiBuyPeriod & "', data:[" & rsiBuyGraph & _
                        "]},{color: 'rgb(255,128,0)', name:'RSI " & rsiSellPeriod & "', data:[" & rsiSellGraph & "]}]"
        End If
        If rocBuy.Text <> "" Then
            rocBuyPeriod = findRsiorRocPeriod("C", rocBuy)
            graphRocButton.Enabled = True
        End If
        If rocSell.Text <> "" Then
            rocSellPeriod = findRsiorRocPeriod("C", rocSell)
            graphRocButton.Enabled = True
        End If
        If rocBuyPeriod > 0 Then
            rocBuyGraph = simData.periodData.graphRoc(simData.stockData, rocBuyPeriod)
            rocBuyDouble = simData.periodData.convertToDoubleList(rocBuyGraph)
        End If
        If rocSellPeriod > 0 Then
            rocSellGraph = simData.periodData.graphRoc(simData.stockData, rocSellPeriod)
            rocSellDouble = simData.periodData.convertToDoubleList(rocSellGraph)
        End If
        rocSeries = "series: [{ name:'ROC', data:[""]}]"
        If rocBuyPeriod > 0 And rocSellPeriod = 0 Then
            rocSeries = "series: [{ name:'ROC " & rocBuyPeriod & "', data:[" & rocBuyGraph & "]}]"
        ElseIf rocBuyPeriod = 0 And rocSellPeriod > 0 Then
            rocSeries = "series: [{ name:'ROC " & rocSellPeriod & "', data:[" & rocSellGraph & "]}]"
        ElseIf rocBuyPeriod = rocSellPeriod Then
            rocSeries = "series: [{ name:'ROC " & rocBuyPeriod & "', data:[" & rocBuyGraph & "]}]"
        Else
            rocSeries = "series: [{ name:'ROC " & rocBuyPeriod & "', data:[" & rocBuyGraph & _
                        "]},{color: 'rgb(255,128,0)', name:'ROC " & rocSellPeriod & "', data:[" & rocSellGraph & "]}]"
        End If
    End Sub

    '****************** DATABASE FUNCTIONS ****************************************************
    Sub saveMoney(ByVal tableName As String)
        Dim insert_into_table As String
        Dim added As Integer
        Dim comp_num As Integer
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim buySig As String = ""
        Dim buyOpt As String = ""
        Dim sellSig As String = ""
        Dim sellOpt As String = ""

        If User.Identity.Name <> "" Then ' And moneyName.Text <> "" Then
            If buyOption.SelectedValue = "Price" Then
                buySig = priceBuy.Text
                buyOpt = buyOption.SelectedValue
            ElseIf buyOption.SelectedValue = "maAvg" Then
                buySig = maBuy.Text
                buyOpt = "maAvg"
            ElseIf buyOption.SelectedValue = "macd" Then
                buySig = macdBuy.Text
                buyOpt = "macd"
            ElseIf buyOption.SelectedValue = "rsi" Then
                buySig = rsiBuy.Text
                buyOpt = "rsi"
            ElseIf buyOption.SelectedValue = "roc" Then
                buySig = rocBuy.Text
                buyOpt = "roc"
            End If
            If sellOption.SelectedValue = "Price" Then
                sellSig = priceSell.Text
                sellOpt = sellOption.SelectedValue
            ElseIf sellOption.SelectedValue = "maAvg" Then
                sellSig = maSell.Text
                sellOpt = "maAvg"
            ElseIf sellOption.SelectedValue = "macd" Then
                sellSig = macdSell.Text
                sellOpt = "macd"
            ElseIf sellOption.SelectedValue = "rsi" Then
                sellSig = rsiSell.Text
                sellOpt = "rsi"
            ElseIf sellOption.SelectedValue = "roc" Then
                sellSig = rocSell.Text
                sellOpt = "roc"
            End If
            Try
                deleteSavedTest("save")
            Catch ex As Exception
            End Try
            con.Open()
            If tableName = "moneySaved" Then
                mycom.Connection = con
                mycom.CommandText = "SELECT COUNT(*) FROM bobpar.moneySaved"
                comp_num = mycom.ExecuteScalar()
                insert_into_table = "INSERT INTO bobpar.moneySaved(" &
                          "username, name, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, date, Notes, tradeCost) " & _
                        "VALUES (@username, @name, @ticker, @buyOpt, @buySignal, @sellOpt, @sellSignal, @capital, @date, @Notes, @tradeCost)"
                'insert_into_table = "INSERT INTO bobpar.moneySaved(" & _
                '          "username, name, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, date, Notes) " & _
                '        "VALUES ('" & User.Identity.Name & "', '" & moneyName.Text & "', '" & lookup.Text & "', '" & buyOpt & "', '" & buySig & "', '" & _
                '                sellOpt & "', '" & sellSig & "', '" & investValue.Text & "', '" & Today.Date.ToString("MM/dd/yyyy") & "', '" & _
                '                note.Text & "')"

                'Parameters.Add("@BID", SqlDbType.VarChar, 50).Value = strBasketID

                mycom.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = User.Identity.Name
                mycom.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = moneyName.Text
                mycom.Parameters.Add("@ticker", SqlDbType.VarChar, 50).Value = lookup.Text
                mycom.Parameters.Add("@buyOpt", SqlDbType.VarChar, 50).Value = buyOpt
                mycom.Parameters.Add("@buySignal", SqlDbType.VarChar, 50).Value = buySig
                mycom.Parameters.Add("@sellOpt", SqlDbType.VarChar, 50).Value = sellOpt
                mycom.Parameters.Add("@sellSignal", SqlDbType.VarChar, 50).Value = sellSig
                mycom.Parameters.Add("@capital", SqlDbType.VarChar, 50).Value = investValue.Text
                mycom.Parameters.Add("@date", SqlDbType.VarChar, 50).Value = Today.Date.ToString("MM/dd/yyyy")
                mycom.Parameters.Add("@notes", SqlDbType.VarChar, 50).Value = note.Text
                mycom.Parameters.Add("@tradeCost", SqlDbType.VarChar, 50).Value = tradeCostBox.Text
                mycom.CommandText = insert_into_table
                added = mycom.ExecuteNonQuery()
            ElseIf tableName = "trends" Then
                mycom.Connection = con
                mycom.CommandText = "SELECT COUNT(*) FROM bobpar.trends"
                comp_num = mycom.ExecuteScalar()
                insert_into_table = "INSERT INTO bobpar.trends(" &
                          "username, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, lastDate, tradeCost, active, startTrackDate) " & _
                        "VALUES (@username, @ticker, @buyOpt, @buySignal, @sellOpt, @sellSignal, @capital, @lastDate, @tradeCost, @active, @startTrackDate)"
                'insert_into_table = "INSERT INTO bobpar.moneySaved(" & _
                '          "username, name, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, date, Notes) " & _
                '        "VALUES ('" & User.Identity.Name & "', '" & moneyName.Text & "', '" & lookup.Text & "', '" & buyOpt & "', '" & buySig & "', '" & _
                '                sellOpt & "', '" & sellSig & "', '" & investValue.Text & "', '" & Today.Date.ToString("MM/dd/yyyy") & "', '" & _
                '                note.Text & "')"

                'Parameters.Add("@BID", SqlDbType.VarChar, 50).Value = strBasketID

                mycom.Parameters.Add("@username", SqlDbType.VarChar, 50).Value = User.Identity.Name
                mycom.Parameters.Add("@ticker", SqlDbType.VarChar, 50).Value = lookup.Text
                mycom.Parameters.Add("@buyOpt", SqlDbType.VarChar, 50).Value = buyOpt
                mycom.Parameters.Add("@buySignal", SqlDbType.VarChar, 50).Value = buySig
                mycom.Parameters.Add("@sellOpt", SqlDbType.VarChar, 50).Value = sellOpt
                mycom.Parameters.Add("@sellSignal", SqlDbType.VarChar, 50).Value = sellSig
                mycom.Parameters.Add("@capital", SqlDbType.VarChar, 50).Value = String.Format("{0:n2}", Convert.ToDouble(investValue.Text))
                mycom.Parameters.Add("@lastDate", SqlDbType.VarChar, 50).Value = DateTime.Now.ToShortDateString
                If tradeCostBox.Text = "" Then
                    mycom.Parameters.Add("@tradeCost", SqlDbType.VarChar, 50).Value = "0.00"
                Else
                    mycom.Parameters.Add("@tradeCost", SqlDbType.VarChar, 50).Value = String.Format("{0:n2}", Convert.ToDouble(tradeCostBox.Text))
                End If
                mycom.Parameters.Add("@active", SqlDbType.VarChar, 50).Value = "yes"
                mycom.Parameters.Add("@startTrackDate", SqlDbType.VarChar, 50).Value = Today.Date.ToShortDateString
                mycom.CommandText = insert_into_table
                added = mycom.ExecuteNonQuery()
            End If
            con.Close()
        End If
    End Sub

    Sub displaySavedTable()
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim values() As String = {"name", "ticker", "buyOpt", "buySignal", "sellOpt", "sellSignal", "capital", "date", "tradeCost", "notes"}
        Dim topRow() As String = {"Name", "Ticker", "Buy Option", "Buy Signal", "Sell Option", "Sell Signal", "Capital", "Date Saved", "tradeCost", "Notes"}
        Dim i As Integer = 0
        Dim valueString As String

        moneySaveTable.Rows.Clear()
        arow = New TableRow
        For i = 0 To topRow.Count - 1 Step 1
            acell = New TableCell
            acell.Text = "<b>" & topRow(i) & "</b>"
            arow.Cells.Add(acell)
        Next
        moneySaveTable.Rows.Add(arow)
        con.Open()
        mycom.Connection = con
        mycom.CommandText = "SELECT username, name, ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, date, tradeCost, notes FROM bobpar.moneySaved " +
                            "WHERE username = '" & User.Identity.Name & "'"
        'SELECT username, name, ticker, buySignal, sellSignal, capital, date, Notes, buyOpt, sellOpt FROM bobpar.moneySaved " +
        '                            "       "   WHERE username = '" + User.Identity.Name + "'"
        'mycom.CommandText = insert_into_table;
        myreader = mycom.ExecuteReader
        While myreader.Read()
            arow = New TableRow
            For i = 0 To values.Count - 1 Step 1
                acell = New TableCell
                valueString = myreader(values(i))
                If valueString.Contains(">") Then
                    valueString = valueString.Replace(">", "&gt;")
                ElseIf valueString.Contains("<") Then
                    valueString = valueString.Replace("<", "&lt;")
                End If
                acell.Text = valueString
                arow.Cells.Add(acell)
            Next
            moneySaveTable.Rows.Add(arow)
        End While
    End Sub

    Sub panelExtenderOff(ByRef panelExt As AjaxControlToolkit.CollapsiblePanelExtender)
        panelExt.Collapsed = True
        panelExt.ClientState = "true"
    End Sub

    Sub panelExtenderOn(ByRef panelExt As AjaxControlToolkit.CollapsiblePanelExtender)
        panelExt.Collapsed = False
        panelExt.ClientState = "false"
    End Sub

    Sub loadFields(ByVal dVals As List(Of String))
        Dim i As Integer = 0
        Dim buyOpt As String = ""
        Dim buySignal As String = ""
        Dim sellOpt As String = ""
        Dim sellSignal As String = ""

        lookup.Text = dVals(0)
        investValue.Text = dVals(1)
        buyOpt = dVals(2)
        If buyOpt = "Price" Then
            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("Price"))
            priceBuy.Text = dVals(3)
            panelExtenderOn(Me.BuyPanelExtender)
        ElseIf buyOpt = "maAvg" Then
            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("Moving Average"))
            maBuy.Text = dVals(3) 'buySignal
            panelExtenderOn(Me.maAvgBuyPanelExtender)
        ElseIf buyOpt = "macd" Then
            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("MACD"))
            macdBuy.Text = dVals(3)
            panelExtenderOn(Me.macdBuyPanelExtender)
        ElseIf buyOpt = "rsi" Then
            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("RSI"))
            rsiBuy.Text = dVals(3)
            panelExtenderOn(Me.rsiBuyPanelExtender)
        ElseIf buyOpt = "roc" Then
            buyOption.SelectedIndex = buyOption.Items.IndexOf(buyOption.Items.FindByText("ROC"))
            rocBuy.Text = dVals(3)
            panelExtenderOn(Me.rocBuyPanelExtender)
        End If
        sellOpt = dVals(4)
        If sellOpt = "Price" Then
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("Price"))
            priceSell.Text = dVals(5)
            panelExtenderOn(Me.SellPanelExtender)
        ElseIf sellOpt = "maAvg" Then
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("Moving Average"))
            maSell.Text = dVals(5)
            panelExtenderOn(Me.maAvgSellPanelExtender)
        ElseIf sellOpt = "macd" Then
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("MACD"))
            macdSell.Text = dVals(5)
            panelExtenderOn(Me.macdSellPanelExtender)
        ElseIf sellOpt = "rsi" Then
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("RSI"))
            rsiSell.Text = dVals(5)
            panelExtenderOn(Me.rsiSellPanelExtender)
        ElseIf sellOpt = "roc" Then
            sellOption.SelectedIndex = sellOption.Items.IndexOf(sellOption.Items.FindByText("ROC"))
            rocSell.Text = dVals(5)
            panelExtenderOn(Me.rocSellPanelExtender)
        End If
        If dVals(6) <> "0.00" And IsNothing(dVals(6)) = False Then
            panelExtenderOn(Me.tradeCostPanelExtender)
            buttonOn(Me.tradeCostPanelExtender, tradeCostButton)
            tradeCostBox.Text = dVals(6)
        End If
    End Sub

    Sub deleteSavedTest(ByVal saveSig As String)
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim finished As Integer = 0
        If saveSig = "save" Then
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "DELETE FROM bobpar.moneySaved WHERE (username = '" + User.Identity.Name + _
                                "') AND (name = '" + moneyName.Text + "')"
            finished = mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    'Finds the period of the RSI entered. box.text=RSI14>30 returns 14
    Function findRsiorRocPeriod(ByVal indicator As String, _
                           ByVal box As TextBox) As Integer
        Dim rsiPeriod As Integer = 0
        Dim rsiSplit() As String
        Dim rsiI As Integer = 0
        Try
            If box.Text.Contains("<") Then
                rsiSplit = box.Text.Split("<")
            Else : rsiSplit = box.Text.Split(">")
            End If
            rsiI = rsiSplit(0).IndexOf(indicator)
            rsiPeriod = Convert.ToInt32(rsiSplit(0).Substring(rsiI + 1, rsiSplit(0).Length - 1 - rsiI))
        Catch ex As Exception
        End Try
        Return rsiPeriod
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

    Protected Sub signalSync_Click(sender As Object, e As System.EventArgs) Handles signalSync.Click

    End Sub
End Class

