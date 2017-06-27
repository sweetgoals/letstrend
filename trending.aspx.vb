Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml


Partial Class trending
    Inherits System.Web.UI.Page   

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If User.Identity.Name = "" Then
            Response.Redirect("~/info/loginrequired.aspx")
        Else
            loadTable()
            If Page.IsPostBack = False Then
                checkUpdateEmail()
            End If
        End If
    End Sub

    Sub loadTable()
        Dim acell As New TableCell
        Dim arow As New TableRow
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim updateCommand As New SqlCommand()
        Dim updateConnection As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim dateWriter As SqlDataReader

        Dim values() As String = {"name", "ticker", "buyOpt", "buySignal", "sellOpt", "sellSignal", "capital", "date", "notes"}
        Dim i As Integer = 0
        Dim j As Integer = 1
        Dim buyOpt As String = ""
        Dim buySignal As String = ""
        Dim sellOpt As String = ""
        Dim sellSignal As String = ""
        Dim iniValues As New List(Of String)
        Dim gainSplit() As String
        Dim sim As New simulate
        Dim link As New HyperLink
        Dim cbox As New CheckBox
        Dim iniDate As DateTime = New DateTime
        Dim rowVals As List(Of String) = New List(Of String)

        Try
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT ticker, buyOpt, buySignal, sellOpt, sellSignal, capital, startTrackDate, tradeCost, lastDate, active FROM bobpar.trends " +
                                "WHERE (username = '" & User.Identity.Name & "') AND (active = 'yes')"
            myreader = mycom.ExecuteReader
            While myreader.Read()
                arow = New TableRow
                sim = New simulate
                acell = New TableCell
                link = New HyperLink
                iniValues = New List(Of String)
                iniDate = New DateTime
                rowVals = New List(Of String)

                iniValues.Add(myreader("ticker"))               
                iniDate = Convert.ToDateTime(myreader("startTrackDate"))
                iniValues.Add(myreader("buyOpt"))
                iniValues.Add(myreader("buySignal"))
                iniValues.Add(myreader("sellOpt"))
                iniValues.Add(myreader("sellSignal"))
                iniValues.Add(myreader("tradeCost"))
                iniValues.Add(String.Format("{0:n2}", Convert.ToDouble(myreader("capital"))))
                Try
                    iniValues.Add(myreader("lastDate"))
                Catch ex As Exception
                    iniValues.Add("")
                End Try

                acell = New TableCell
                cbox = New CheckBox
                cbox.ID = "box_" & j
                acell.Controls.Add(cbox)
                arow.Cells.Add(acell)

                acell = New TableCell
                link.Text = iniValues(0)
                acell.Text = link.Text
                link.ID = "link_" & j
                link.Target = "_Blank"
                link.NavigateUrl = "~/money.aspx?ticker=" & iniValues(0) & "&capital=" & iniValues(6) _
                                    & "&buyOpt=" & iniValues(1) & "&buySignal=" & iniValues(2) _
                                    & "&sellOpt=" & iniValues(3) & "&sellSignal=" & iniValues(4) & "&tradeCost=" & iniValues(5)
                acell.Controls.Add(link)
                arow.Cells.Add(acell)
                rowVals.Add(link.Text)

                'buy Option
                'buySignal
                'sell option
                'sell signal
                'trade cost'
                'capital
                For i = 0 To 6 Step 1
                    acell = New TableCell
                    If i = 0 Then
                        acell.Text = iniDate.Date.ToShortDateString
                    ElseIf i = 5 Then
                        If iniValues(5) = "" Then
                            acell.Text = "0.00"
                        Else : acell.Text = String.Format("{0:n2}", Convert.ToDouble(iniValues(5)))
                        End If
                    Else
                        acell.Text = replaceSign1(iniValues(i))
                    End If
                    rowVals.Add(acell.Text)
                    arow.Cells.Add(acell)
                Next
                If iniValues(0) = "TSLA" Then
                    Dim blah As Integer
                    blah = 1
                End If
                sim = New simulate(iniValues, iniDate)
                If sim.gainsResults(0, 1) <> "" Then
                    gainSplit = sim.gainsResults(0, 1).Split(" ")

                    'Value
                    acell = New TableCell
                    acell.Text = sim.summaryResults(2, 1)
                    setColor(acell, gainSplit(0), "-")
                    If gainSplit(0) = "0.00" Then
                        acell.Text = sim.summaryResults(1, 1)
                        acell.ForeColor = Drawing.Color.Black
                    End If
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    '$Gain
                    acell = New TableCell
                    acell.Text = gainSplit(0)
                    setColor(acell, acell.Text, "-")
                    If acell.Text = "" Or acell.Text = "0.00" Then
                        acell.Text = "0.00"
                        acell.ForeColor = Drawing.Color.Black
                    End If
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    '%Gain
                    acell = New TableCell
                    acell.Text = gainSplit(4)
                    setColor(acell, acell.Text, "-")
                    If acell.Text = "" Or acell.Text = "0.00" Then
                        acell.Text = "0.00"
                        acell.ForeColor = Drawing.Color.Black
                    End If
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    'Last Trade Position
                    acell = New TableCell
                    acell.Text = sim.lastTradePosition
                    If acell.Text = "" Then
                        acell.Text = "Out"
                    End If
                    setColor(acell, acell.Text, "Out")
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)
                Else
                    acell = New TableCell
                    acell.Text = String.Format("{0:n2}", iniValues(6))
                    acell.ForeColor = Drawing.Color.Black
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    acell = New TableCell
                    acell.Text = "0.00"
                    acell.ForeColor = Drawing.Color.Black
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    acell = New TableCell
                    acell.Text = "0.00"
                    acell.ForeColor = Drawing.Color.Black
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)

                    acell = New TableCell
                    acell.Text = "Out"
                    acell.ForeColor = Drawing.Color.Black
                    arow.Cells.Add(acell)
                    rowVals.Add(acell.Text)
                End If

                'Last Trade Date
                acell = New TableCell
                Dim newSimDate As New DateTime
                Dim oldDate As Date = Convert.ToDateTime("01/01/1900")
                '.ToDateTime(iniValues.Last)
                If sim.lastTradeDate <> "No Trades" Then
                    newSimDate = Convert.ToDateTime(sim.lastTradeDate)
                    acell.Text = newSimDate.ToShortDateString
                Else : acell.Text = "No Trades"
                End If
                If (newSimDate.Date > oldDate.Date) Or acell.Text = "No Trades" Then
                    Try
                        updateConnection.Open()
                        updateCommand.Connection = updateConnection
                        updateCommand.CommandText = "UPDATE bobpar.trends set lastDate='" & newSimDate.ToShortDateString & _
                            "' WHERE ticker='" & iniValues(0) & "' AND buyOpt='" & iniValues(1) & "' AND buySignal='" & iniValues(2) & _
                            "' AND sellOpt='" & iniValues(3) & "' AND sellSignal='" & iniValues(4) & "' AND capital='" & iniValues(6) &
                            "' AND startTrackDate='" & iniDate.ToShortDateString & "' AND tradeCost='" & iniValues(5) & _
                            "' AND username='" & User.Identity.Name & "' AND active='yes'"
                        dateWriter = updateCommand.ExecuteReader
                        updateConnection.Close()
                    Catch ex As Exception
                        updateConnection.Close()
                    End Try
                End If
                arow.ID = "row_" & j
                j += 1
                arow.Cells.Add(acell)
                If newSimDate = Today.Date Then
                    arow.Font.Bold = True
                End If
                rowVals.Add(acell.Text)
                trendTable.Rows.Add(arow)
            End While
            con.Close()
        Catch ex As Exception
        End Try
    End Sub

    Sub runSimulation(ByVal iniVals As List(Of String))
        Dim sim As New simulate(iniVals)
        Dim arow As New TableRow
        Dim acell As New TableCell
    End Sub

    'Check for "<" will mess up the html page turn it into a &lt;
    Function replaceSign1(ByVal inString As String) As String
        If inString.Contains("<") Then
            inString = inString.Replace("<", "&lt;")
        End If
        Return inString
    End Function

    Function replaceSign2(ByVal inString As String) As String
        If inString.Contains("&lt;") Then
            inString = inString.Replace("&lt;", "<")
        End If
        Return inString
    End Function

    Function setColor(ByVal acell As TableCell, _
                      ByVal value As String, _
                      ByVal condition As String) As TableCell
        'False is true here because it's easier to detect "-" for signed numbers and to detect strings.
        If value.Contains(condition) Then 'False is true here 
            acell.ForeColor = Drawing.Color.Red
        Else : acell.ForeColor = Drawing.Color.Green
        End If
        Return acell
    End Function

    Protected Sub refreshButton_Click(sender As Object, e As System.EventArgs) Handles refreshButton.Click
        Dim pageContent As New ContentPlaceHolder
        Dim workTable As New Table
        Dim i As Integer = 0
        Dim thebox As New CheckBox
        Dim newTrendTable As New Table
        Dim rowValues As New List(Of String)
        Dim thelink As New HyperLink
        Dim rowList As New List(Of Integer)
        Dim rowRemove As Integer = 0

        setUpdateEmail()
        pageContent = Master.FindControl("mainContent")
        workTable = pageContent.FindControl("trendTable")
        newTrendTable = workTable
        For i = 1 To workTable.Rows.Count - 1 Step 1
            thebox = New CheckBox
            'thelink = New HyperLink
            thebox = workTable.FindControl("box_" & i)
            'thelink = workTable.FindControl("link_" & i)
            If thebox.Checked = True Then
                rowList.Add(i)
            End If
        Next
        For i = rowList.Count To 1 Step -1
            rowRemove = rowList(i - 1)
            thelink = New HyperLink
            thelink = workTable.FindControl("link_" & rowRemove)
            rowValues = New List(Of String)
            rowValues.Add(thelink.Text) 'ticker
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(2).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(3).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(4).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(5).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(6).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(7).Text))
            rowValues.Add(replaceSign2(workTable.Rows(rowRemove).Cells(8).Text))
            deleteFromDbase(rowValues)
            newTrendTable.Rows.Remove(workTable.FindControl("row_" & rowRemove))
        Next

    End Sub

    Sub deleteFromDbase(ByVal rowv As List(Of String))
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim mystring As String = ""

        con.Open()
        mycom.Connection = con
        mystring = "DELETE FROM bobpar.trends WHERE (userName = '" & User.Identity.Name & _
                            "') AND (ticker = '" & rowv(0) & "')" & "AND (startTrackDate = '" & rowv(1) & _
                            "') AND (buyOpt = '" & rowv(2) & "') AND (buySignal = '" & rowv(3) & _
                            "') AND (sellOpt = '" & rowv(4) & "')" & _
                            "AND (sellSignal = '" & rowv(5) & "') AND (capital = '" & rowv(7) & _
                             "') AND (tradeCost = '" & rowv(6) & "')"
        mycom.CommandText = mystring
        myreader = mycom.ExecuteReader
        con.Close()
    End Sub

    Sub checkUpdateEmail()
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim mystring As String = ""
        con.Open()
        mycom.Connection = con
        'SELECT email from dbo.aspnet_Users where userName='jakkjakk'
        mystring = "SELECT email FROM dbo.aspnet_users WHERE userName='" & User.Identity.Name & "'"
        mycom.CommandText = mystring
        myreader = mycom.ExecuteReader
        myreader.Read()
        Try
            If myreader("email") = "yes" Then
                mailCheck.Checked = True
            Else : mailCheck.Checked = False

            End If
        Catch ex As Exception
            mailCheck.Checked = False
        End Try

        con.Close()
    End Sub

    Sub setUpdateEmail()
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim mystring As String = ""

        con.Open()
        Try

            mycom.Connection = con
            If mailCheck.Checked = True Then
                mycom.CommandText = "UPDATE dbo.aspnet_users SET email='yes' WHERE userName='" & User.Identity.Name & "'"
            Else
                mycom.CommandText = "UPDATE dbo.aspnet_users SET email='no' WHERE userName='" & User.Identity.Name & "'"
            End If
            myreader = mycom.ExecuteReader()
        Catch ex As Exception

        End Try

        con.Close()
    End Sub
End Class
