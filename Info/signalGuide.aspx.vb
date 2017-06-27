Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Net
Imports System.Text

Partial Class Info_signalconfig
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim i As Integer = 0
        Dim arow As New TableRow
        Dim acell As New TableCell
        Dim sbox As New TextBox
        Dim columns() As String = {"Signal", "Days"}
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader
        Dim guideData As New List(Of String)
        Dim sqlCols As String = "signal, days"
        Dim sqlColSplit() As String = sqlCols.Split(",")

        errorLabel.Text = ""
        If User.Identity.Name = "" Then
            Response.Redirect("~/info/loginrequired.aspx")
        Else
            con.Open()
            mycom.Connection = con
            mycom.CommandText = "SELECT " & sqlCols & " FROM bobpar.signalGuides WHERE (username='" & User.Identity.Name & "')"
            myreader = mycom.ExecuteReader
            guideData = New List(Of String)
            While myreader.Read()
                guideData.Add(myreader("signal"))
                guideData.Add(myreader("days"))
            End While
            con.Close()

            For i = 0 To 1 Step 1
                acell.Text = columns(i)
                arow.Cells.Add(acell)
                acell = New TableCell
            Next
            signalTable.Rows.Add(arow)
            arow = New TableRow
            For i = 1 To 15 Step 1
                If i <= guideData.Count Then
                    acell = createTextBox(i, guideData(i - 1))
                Else
                    acell = createTextBox(i, "")
                End If

                arow.Cells.Add(acell)
                acell = New TableCell
                If i Mod 2 = 0 Then
                    signalTable.Rows.Add(arow)
                    arow = New TableRow
                End If
            Next
        End If

    End Sub

    Function createTextBox(ByVal i As Integer,
                       ByVal note As String) As TableCell
        Dim acell As New TableCell
        Dim box As New TextBox
        box.ID = "box" & i
        box.Text = note
        acell.Controls.Add(box)
        Return acell
    End Function

    Protected Sub SubmitButton_Click(sender As Object, e As System.EventArgs) Handles SubmitButton.Click
        Dim storeValues As New List(Of String)
        Dim inputString As String = ""
        Dim inputDays As String = ""
        Dim parameterNames() = {"rowNum", "userName", "signal", "days"}
        Dim sqlValuesString As String = ""
        Dim sqlNameString As String = ""
        Dim sqlInsertString As String = ""
        Dim testlist As New List(Of String)
        Dim i As Integer = 0
        Dim result As Integer = 0
        Dim rowCount As Integer = 0
        Dim mycom As New SqlCommand()
        Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")

        For i = 1 To 7 Step 2
            inputString = Convert.ToString(Request.Form("box" & i))
            inputDays = Convert.ToString(Request.Form("box" & i + 1))
            If inputString <> "" And inputDays <> "" Then
                If checkErrors(inputString, inputDays) = False Then
                    storeValues.Add(i)
                    storeValues.Add(User.Identity.Name)
                    storeValues.Add(inputString)
                    storeValues.Add(inputDays)
                End If
            End If
        Next
        If storeValues.Count > 3 Then

            mycom.CommandText = "DELETE FROM bobpar.signalGuides WHERE userName='" & User.Identity.Name & "'"
            mycom.Connection = con
            con.Open()
            i = mycom.ExecuteScalar()
            con.Close()

            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.signalGuides"
            mycom.Connection = con
            con.Open()
            rowCount = mycom.ExecuteScalar()
            con.Close()

            For i = 0 To 3 Step 1
                mycom.Parameters.Add("@" & parameterNames(i), SqlDbType.VarChar, 50)
                '            testlist.Add(parameterNames(i))
            Next
            Dim k As Integer = 0

            sqlNameString = "rowNum, userName, signal, days"
            sqlValuesString = "@rowNum, @userName, @signal, @days"

            For i = 0 To storeValues.Count - 1 Step 1
                mycom.Parameters("@" & parameterNames(k)).Value = storeValues(i)
                testlist.Add(storeValues(i))
                k += 1
                If k = 4 Then
                    sqlInsertString = "INSERT INTO bobpar.signalGuides(" & sqlNameString & ") VALUES (" & sqlValuesString & ")"
                    mycom.Connection = con
                    con.Open()
                    mycom.CommandText = sqlInsertString
                    result = mycom.ExecuteNonQuery()
                    con.Close()
                    k = 0
                    testlist.Clear()
                End If
            Next
            'testlist.Add(parameterNames(i))
            'sqlNameString &= parameterNames(i) & ", "
            'sqlValuesString &= "@" & parameterNames(i) & ", "

            '        mycom.Parameters.Add("@" & parameterNames(i), SqlDbType.VarChar, 50).Value = storeValues(i)

            updated.Visible = True
        End If


    End Sub

    Function checkErrors(ByVal signal As String,
                         ByVal days As String) As Boolean
        Dim signalConditions() As String = {">", "<", "X"}
        Dim signalNames() As String = {"EMA", "SMA", "MACD", "RSI", "ROC"}
        Dim signalName As String = ""
        Dim selectedCondition As String = ""
        Dim signalsInvolved() As String

        Dim i As Integer = 0
        Dim checkNumber As Integer = 0
        Dim dayCheck As Integer = 0

        signal = signal.ToUpper
        For i = 0 To 2 Step 1
            If signal.Contains(signalConditions(i)) Then
                selectedCondition = signalConditions(i)
            End If
        Next
        If selectedCondition = "" Then
            errorLabel.Text = "BAD SIGNAL CONDITION"
            Return True
        End If
        For i = 0 To 4 Step 1
            If signal.Contains(signalNames(i)) Then
                signalName = signalNames(i)
            End If
        Next
        If signalName = "" Then
            errorLabel.Text = "BAD SIGNAL NAME"
            Return True
        End If

        If signal.Contains("EMA") Or signal.Contains("SMA") Then
            signalsInvolved = signal.Split(selectedCondition)
            For i = 0 To 1 Step 1
                If checkSignalName(signalsInvolved(i)) Then
                    Return True
                End If
            Next
        ElseIf signal.Contains("RSI") Or signal.Contains("ROC") Then
            signalsInvolved = signal.Split(selectedCondition)
            Try
                checkNumber = Convert.ToInt16(signalsInvolved(1))
                If checkNumber < 1 Or checkNumber > 100 Then
                    errorLabel.Text = signalName & " Limiter Out of Range (1-100)"
                    Return True
                End If               
            Catch ex As Exception
                errorLabel.Text = "Bad Input"
                Return True
            End Try
            Try
                checkNumber = Convert.ToInt16(signal.Substring(3, signal.IndexOf(selectedCondition) - 3))
                If checkNumber < 1 Or checkNumber > 100 Then
                    errorLabel.Text = signalName & " Value Out of Range (1-100)"
                    Return True
                End If
            Catch ex As Exception
                errorLabel.Text = "Bad Input"
                Return True
            End Try
        End If

        Try
            dayCheck = Convert.ToInt16(days)
            If dayCheck < 1 And dayCheck > 100 Then
                errorLabel.Text = "DAYS OUT OF RANGE (1-100)"
                Return True
            End If
        Catch ex As Exception
            errorLabel.Text = "BAD DAYS INPUT"
            Return True
        End Try
        Return False
    End Function

    Function checkSignalName(ByVal signalInvolved As String) As Boolean
        Dim signalNames() As String = {"EMA", "SMA", "MACD", "RSI", "ROC"}
        Dim signalName As String = ""
        Dim sNumber As String
        Dim periodCheck As Integer = 0
        Dim i As Integer = 0

        If signalName = "EMA" Or signalName = "EMA" Then
            sNumber = signalInvolved.Substring(signalName.Count, signalInvolved.Count - signalName.Count)
            Try
                periodCheck = Convert.ToInt16(sNumber)
                'If periodCheck > 100 And periodCheck < 1 Then
                '    errorLabel.Text = "BAD SIGNAL PERIOD (RANGE 1-100)"
                '    Return True
                'End If
            Catch ex As Exception
                errorLabel.Text = "BAD SIGNAL NUMBER"
                Return True
            End Try
        End If

        Return False
    End Function

End Class
