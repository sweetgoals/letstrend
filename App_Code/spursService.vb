Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.Net
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml
Imports System.Globalization
Imports System.Collections.Specialized
Imports System.Drawing.Image

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class spursService
     Inherits System.Web.Services.WebService
    Dim mycom As New SqlCommand()
    Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")

    <WebMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()> _
    Public Function createAccount(ByVal username As String, _
                                  ByVal password As String, _
                                  ByVal email As String) As Boolean

        Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        'Dim sqlCols As String = "userName"
        Dim mystr As String = ""
        Dim nameString = "username, password, email, userNumber"
        Dim valueString = "@userName, @password, @email, @userNumber"
        Dim result As Integer
        Dim userNumber As Integer = 0
        Dim userExists As Integer = 0
        Dim i As Integer = 0

        'mycom.CommandText = "SELECT COUNT('username') from bobpar.spursLogin WHERE (username='" & username.ToLower & "')"
        'mycom.Connection = con
        'con.Open()
        'userExists = mycom.ExecuteScalar()
        'con.Close()

        'If checkUser(username, password) = 0 Then
        mycom.CommandText = "SELECT COUNT(*) FROM bobpar.spursLogin"
        mycom.Connection = con
        con.Open()
        userNumber = mycom.ExecuteScalar()
        con.Close()

        mycom.Parameters.AddWithValue("@username", username.ToLower)
        mycom.Parameters.AddWithValue("@password", password)
        mycom.Parameters.AddWithValue("@email", email)
        mycom.Parameters.AddWithValue("@userNumber", userNumber)
        mycom.Connection = con
        con.Open()
        mycom.CommandText = "INSERT INTO bobpar.spursLogin(" & nameString & ") VALUES (" & valueString & ")"
        result = mycom.ExecuteNonQuery()
        con.Close()
        If result = 1 Then
            Return True
        Else : Return False
        End If
        'Else : Return False
        'End If
    End Function

    <WebMethod()> _
    Function checkUserAvail(ByVal user As String) As Boolean
        'if user exists then return false because that user is not avail

        'Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim result As Integer = 0

        mycom.Parameters.AddWithValue("@username", user)
        mycom.CommandText = "SELECT COUNT('username') from bobpar.spursLogin WHERE username=@username"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()

        If result > 0 Then
            Return False
        Else : Return True
        End If
    End Function

    <WebMethod()> _
    Sub checkUser(ByVal user As String, _
                  ByVal pass As String, _
                  ByVal email As String, _
                  ByRef userVerify As Boolean, _
                  ByRef passVerify As Boolean, _
                  ByRef emailVerify As Boolean)
        'Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim result As Integer = 0
        userVerify = False
        passVerify = False
        emailVerify = False

        mycom.Parameters.AddWithValue("@username", user)
        mycom.CommandText = "SELECT COUNT('username') from bobpar.spursLogin WHERE username=@username"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()
        If result > 0 Then
            userVerify = True
        End If
        If userVerify = True Then
            mycom.Parameters.AddWithValue("@password", pass)
            mycom.CommandText = "SELECT COUNT('username,password') from bobpar.spursLogin WHERE username=@username AND password=@password"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                passVerify = True
            End If
        End If

        If (userVerify = True And passVerify = True) Then
            mycom.Parameters.AddWithValue("@email", email)
            mycom.CommandText = "SELECT COUNT('email') from bobpar.spursLogin WHERE username=@username AND password=@password AND email=@email"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                emailVerify = True
            End If
        End If

    End Sub

    <WebMethod()> _
    Function verifyUserPass(ByVal user As String, _
                       ByVal pass As String) As Boolean
        'Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim result As Integer = 0
        Dim userVerify As Boolean = False
        Dim passVerify As Boolean = False

        mycom.Parameters.AddWithValue("@username", user)
        mycom.CommandText = "SELECT COUNT('username') from bobpar.spursLogin WHERE username=@username"
        mycom.Connection = con
        con.Open()
        result = mycom.ExecuteScalar()
        con.Close()
        If result > 0 Then
            userVerify = True
        End If
        If userVerify = True Then
            mycom.Parameters.AddWithValue("@password", pass)
            mycom.CommandText = "SELECT COUNT('username,password') from bobpar.spursLogin WHERE username=@username AND password=@password"
            mycom.Connection = con
            con.Open()
            result = mycom.ExecuteScalar()
            con.Close()
            If result > 0 Then
                passVerify = True
            End If
        End If
        If userVerify = True And passVerify = True Then
            Return True
        Else : Return False
        End If
    End Function

    <WebMethod()> _
    Public Function createGoal(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalTitle As String, _
                        ByVal goalDueDate As String, _
                        ByVal scheduleDays As String, _
                        ByVal timeLength As String, _
                        ByVal timeUnit As String) As Boolean
        Dim result As Boolean
        'Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim goalNumber As Integer = 0
        Dim userExists As Integer = 0
        Dim nameString = "userName, goalNumber, goalTitle, goalDueDate, scheduleDays, timeLength, timeUnit"
        Dim valueString = "@username, @goalNumber, @goalTitle, @goalDueDate, @scheduleDays, @timeLength, @timeUnit"
        Dim i As Integer = 0

        If verifyUserPass(userName, password) = True Then

            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.goals"
            mycom.Connection = con
            con.Open()
            goalNumber = mycom.ExecuteScalar() + 1
            con.Close()
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", userName.ToLower)
            mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
            mycom.Parameters.AddWithValue("@goalNumber", goalNumber)
            mycom.Parameters.AddWithValue("@goalDueDate", goalDueDate)
            mycom.Parameters.AddWithValue("@scheduleDays", scheduleDays)
            mycom.Parameters.AddWithValue("@timeLength", timeLength)
            mycom.Parameters.AddWithValue("@timeUnit", timeUnit)

            mycom.Connection = con
            con.Open()
            mycom.CommandText = "INSERT INTO bobpar.goals(" & nameString & ") VALUES (" & valueString & ")"
            result = mycom.ExecuteNonQuery()
            con.Close()
        End If
        Return result
    End Function

    <WebMethod()> _
    Public Sub writeActivity(ByVal userName As String, _
                        ByVal password As String,
                        ByVal goalNumber As Integer,
                        ByVal actTitle As String, _
                        ByVal adesc As String, _
                        ByVal startTime As String, _
                        ByVal actDate As String)


        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", userName.ToLower)
            mycom.Parameters.AddWithValue("@goalNumber", goalNumber)
            mycom.Parameters.AddWithValue("@actTitle", actTitle)
            mycom.Parameters.AddWithValue("@desc", adesc)
            mycom.Parameters.AddWithValue("@startTime", startTime)
            mycom.Parameters.AddWithValue("@actDate", actDate)

            mycom.Connection = con
            con.Open()
            mycom.CommandType = CommandType.StoredProcedure
            mycom.CommandText = "dbo.insertActivityItem"
            mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Public Function listActivity(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalTitle As String) As String

        Dim myreader As SqlDataReader
        Dim activityList As String = ""

        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", userName.ToLower)
            mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
            mycom.Connection = con
            con.Open()
            mycom.CommandType = CommandType.StoredProcedure
            mycom.CommandText = "dbo.listActivity"
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                activityList += myreader("Title") + "___"
            End While
            con.Close()
        End If
        If activityList <> "" Then
            Return activityList
        Else : Return "No Activities"
        End If

    End Function

    <WebMethod()> _
    Public Sub finishActivity(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal actNumber As String, _
                        ByVal good As String, _
                        ByVal bad As String, _
                        ByVal stopTime As String, _
                        ByVal timeDiff As String)
        Dim nameString = "actNumber, good, bad, stopTime, timeDiff"
        Dim valueString = "@actNumber, @good, @bad, @stopTime, @timeDiff"
        Dim result As Integer = 0
        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            mycom.Parameters.AddWithValue("@good", good)
            mycom.Parameters.AddWithValue("@bad", bad)
            mycom.Parameters.AddWithValue("@stopTime", stopTime)
            mycom.Parameters.AddWithValue("@timeDiff", timeDiff)

            con.Open()
            mycom.CommandText = "UPDATE bobpar.activityItems SET good=@good, bad=@bad, stopTime=@stoptime, " +
                                "timeDiff=@timediff WHERE actNumber=@actNumber"
            result = mycom.ExecuteNonQuery()
            con.Close()

        End If

    End Sub
    <WebMethod()> _
    Public Sub checkStopTime(ByVal userName As String, _
                        ByVal password As String, _
                        ByVal goalNumber As Integer, _
                        ByVal actSeq As Integer, _
                        ByRef title As String, _
                        ByRef actnumber As String, _
                        ByRef actdate As String, _
                        ByRef actdesc As String, _
                        ByRef good As String, _
                        ByRef bad As String, _
                        ByRef verified As String, _
                        ByRef starttime As String, _
                        ByRef stoptime As String, _
                        ByRef tartime As String, _
                        ByRef workTime As String, _
                        ByRef timeDiff As String)

        Dim myreader As SqlDataReader
        Dim acttime As String = ""
        Dim timeUnit As String = ""
        Dim minuteDiff As Long = 0
        Dim hourDiff As Long = 0
        workTime = ""
        'Dim workTime As String = ""
        stoptime = ""
        title = ""
        acttime = ""
        timeUnit = ""
        actdesc = ""
        good = ""
        bad = ""
        verified = ""
        actdate = ""
        starttime = ""
        actnumber = ""
        acttime = ""
        timeUnit = ""

        If verifyUserPass(userName, password) = True Then
            mycom.Parameters.Clear()
            mycom.CommandText = "SELECT stopTime,title,actTime,timeUnit,actDesc,good,bad,verified,actDate,startTime,actNumber,actTime,timeUnit, timeDiff " +
                                "from bobpar.activityItems where goalnumber='" & goalNumber & "'" +
                                " AND seqNumber='" & actSeq & "'"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                stoptime = myreader("stopTime")
                title = myreader("title")
                acttime = myreader("actTime")
                timeUnit = myreader("timeUnit")
                actdesc = myreader("actDesc")
                good = myreader("good")
                bad = myreader("bad")
                verified = myreader("verified")
                actdate = myreader("actDate")
                starttime = myreader("startTime")
                actnumber = myreader("actNumber")
                acttime = myreader("actTime")
                timeUnit = myreader("timeUnit")
                timeDiff = myreader("timeDiff")
            End While
            tartime = acttime + " " + timeUnit
            con.Close()
        End If
        If stoptime <> "" Then
            'minuteDiff = DateDiff(DateInterval.Minute, Convert.ToDateTime(starttime), Convert.ToDateTime(stoptime))
            'hourDiff = DateDiff(DateInterval.Hour, Convert.ToDateTime(starttime), Convert.ToDateTime(stoptime))
            'If minuteDiff >= 60 Then
            '    hourDiff = minuteDiff \ 60
            '    minuteDiff = minuteDiff Mod 60
            '    If minuteDiff = 0 Then
            '        workTime = hourDiff.ToString + ":00"
            '    ElseIf minuteDiff < 10 Then
            '        workTime = hourDiff.ToString + ":0" + minuteDiff.ToString
            '    Else
            '        workTime = hourDiff.ToString + ":" + minuteDiff.ToString
            '    End If
            'ElseIf minuteDiff < 10 Then
            '    workTime = "0:0" + minuteDiff.ToString
            'Else : workTime = "0:" + minuteDiff.ToString
            'End If
        Else
            stoptime = "BAD"
        End If
    End Sub

    <WebMethod()> _
    Public Function goalSummary(ByVal user As String, _
                                ByVal pass As String) As String
        'Dim goalList As String = ""
        Dim myreader As SqlDataReader
        Dim goalList As String = ""

        If verifyUserPass(user, pass) = True Then
            'mycom.CommandText = "SELECT bobpar.goals.goalTitle from bobpar.spursLogin INNER JOIN bobpar.goals" +
            '                    " on bobpar.spursLogin.userNumber=bobpar.goals.usernumber" +
            '                    " where bobpar.spursLogin.userName='" & user & "'"
            mycom.CommandText = "SELECT goalTitle FROM bobpar.goals WHERE (username='" & user & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                goalList += myreader("goalTitle") + "___"
            End While
            con.Close()
        End If
        If goalList <> "" Then
            Return goalList
        Else : Return "Add Some Goals :-)"
        End If
    End Function

    <WebMethod()> _
    Sub getGoal(ByVal user As String, _
                ByVal pass As String, _
                ByVal goalTitle As String, _
                ByRef goalNumber As String, _
                ByRef goalDueDate As String, _
                ByRef scheduleDays As String, _
                ByRef timeLength As String,
                ByRef timeUnit As String)
        'Dim mycom As New SqlCommand()
        'Dim con As New SqlConnection("Data Source=jakkjakk.yobbers.com; Initial Catalog=jakkjakk; User ID=dashpar; Password='mathsucks1';")
        Dim myreader As SqlDataReader

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT goalTitle, goalNumber, goalDueDate, scheduleDays, timeLength, timeUnit from bobpar.goals where " +
                                "bobpar.goals.userName='" & user & "' AND bobpar.goals.goalTitle='" & goalTitle & "'"

            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                goalNumber = myreader("goalNumber")
                goalDueDate = myreader("goalDueDate")
                scheduleDays = myreader("scheduleDays")
                timeLength = myreader("timeLength")
                timeUnit = myreader("timeUnit")
            End While
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Function listSupporters(ByVal user As String, _
                            ByVal pass As String) As String
        Dim myreader As SqlDataReader
        Dim supportList As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supportList += myreader("supportName") + "___"
            End While
            con.Close()
        End If
        If supportList <> "" Then
            Return supportList
        Else : Return "Add Some Support :-)"
        End If
    End Function

    <WebMethod()> _
    Function listGoalSupporters(ByVal user As String, _
                                ByVal pass As String, _
                                ByVal goalTitle As String) As String
        Dim myreader As SqlDataReader
        Dim supportList As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supporters FROM bobpar.goals WHERE (username='" & user & "' AND goalTitle='" & goalTitle & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supportList = myreader("supporters")
            End While
            con.Close()
        End If
        If supportList <> "" Then
            Return supportList
        Else : Return "Add Some Support :-)"
        End If
    End Function

    <WebMethod()> _
    Function createSupporter(ByVal user As String, _
                        ByVal pass As String, _
                        ByVal supportName As String, _
                        ByVal supportEmail As String) As String
        Dim nameString = "supportNumber, userName, supportName, supportEmail"
        Dim valueString = "@supportNumber, @username, @supportName, @supportEmail"
        Dim supportNumber = 0
        Dim result As Boolean
        Dim myreader As SqlDataReader
        Dim sqlName As String = ""

        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "' AND supportName='" & supportName & "')"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                sqlName = myreader("supportName")
            End While
            con.Close()
            If sqlName.Length = 0 Then
                mycom.CommandText = "SELECT TOP 1 supportNumber FROM bobpar.supporters ORDER BY supportNumber DESC"
                mycom.Connection = con
                con.Open()
                supportNumber = mycom.ExecuteScalar() + 1
                con.Close()

                mycom.Parameters.Clear()
                mycom.Parameters.AddWithValue("@supportNumber", supportNumber)
                mycom.Parameters.AddWithValue("@userName", user.ToLower)
                mycom.Parameters.AddWithValue("@supportName", supportName)
                mycom.Parameters.AddWithValue("@supportEmail", supportEmail)

                mycom.Connection = con
                con.Open()
                mycom.CommandText = "INSERT INTO bobpar.supporters(" & nameString & ") VALUES (" & valueString & ")"
                result = mycom.ExecuteNonQuery()
                con.Close()
                Return "Created"
            Else : Return "Duplicate"
            End If
        Else : Return "Bad User"
        End If
    End Function

    <WebMethod()> _
    Function getSupportEmail(ByVal user As String, _
                             ByVal pass As String, _
                             ByVal supportName As String) As String

        Dim result As Integer = 0
        Dim myreader As SqlDataReader
        Dim supportEmail As String = ""
        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportEmail FROM bobpar.supporters WHERE (userName='" & user & "' AND supportName='" & supportName & "')"
            mycom.Connection = con
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                supportEmail = myreader("supportEmail")
            End While
            con.Close()
        End If
        If supportEmail <> "" Then
            Return supportEmail
        Else : Return ""
        End If
    End Function

    <WebMethod()> _
    Function updateSupport(ByVal user As String, _
                      ByVal pass As String, _
                      ByVal oldSupportName As String, _
                      ByVal newSupportName As String, _
                      ByVal newSupportEmail As String) As String

        Dim result As Integer = 0
        Dim supportEmail As String = ""
        Dim sqlName As String = ""
        Dim myreader As SqlDataReader
        If verifyUserPass(user, pass) = True Then
            mycom.CommandText = "SELECT supportName FROM bobpar.supporters WHERE (username='" & user & "' AND supportName='" & newSupportName & "')"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                sqlName = myreader("supportName")
            End While
            con.Close()
            If sqlName.Length = 0 Then
                mycom.Parameters.Clear()
                mycom.Parameters.AddWithValue("@userName", user)
                mycom.Parameters.AddWithValue("@oldSupportName", oldSupportName)
                mycom.Parameters.AddWithValue("@newSupportName", newSupportName)
                mycom.Parameters.AddWithValue("@newSupportEmail", newSupportEmail)

                mycom.CommandText = "UPDATE bobpar.supporters SET supportName=@newSupportName, supportEmail=@newSupportEmail " +
                                    "WHERE userName=@userName and supportName=@oldSupportName"
                mycom.Connection = con
                con.Open()
                result = mycom.ExecuteNonQuery()
                con.Close()
                Return "Modified"
            Else : Return "Duplicate"
            End If
        Else : Return "Bad User"
        End If
    End Function

    <WebMethod()> _
    Sub addGoalSupport(ByVal user As String, _
                        ByVal pass As String, _
                        ByVal goalTitle As String, _
                        ByVal goalSup As String)
        Dim nameString = "userName, goalTitle, supporters"
        Dim valueString = "@username, @goalTitle, @supporters"
        Dim result As Integer = 0

        If verifyUserPass(user, pass) = True Then
            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@userName", user.ToLower)
            mycom.Parameters.AddWithValue("@goalTitle", goalTitle)
            mycom.Parameters.AddWithValue("@supporters", goalSup)

            con.Open()
            mycom.CommandText = "UPDATE bobpar.goals SET supporters=@supporters WHERE userName=@userName AND goalTitle=@goalTitle"
            result = mycom.ExecuteNonQuery()
            con.Close()
        End If
    End Sub

    <WebMethod()> _
    Sub sendImage(ByVal user As String, _
                  ByVal pass As String, _
                  ByVal actNumber As Integer, _
                  ByVal myImage As Byte())
        'Sub sendImage(ByVal user As String, _
        '          ByVal pass As String, _
        '          ByVal actNumber As Integer, _
        '          ByVal myImage As String)

        'Find the path on the server of our apps images folder
        Dim FilePath As String = Server.MapPath("~/actpic/")
        Dim picLoc As String = ""
        Dim pictureID As Integer = 0
        Dim seqNum As Integer = 0
        Dim nameString = "pictureID, actNumber, seqNumber, picLoc"
        Dim valueString = "@pictureID, @actNumber, @seqNumber, @picLoc"
        Dim result As Integer = 0

        If verifyUserPass(user, pass) Then
            mycom.CommandText = "SELECT COUNT(*) FROM bobpar.activityPictures"
            mycom.Connection = con
            con.Open()
            pictureID = mycom.ExecuteScalar() + 1
            con.Close()
            mycom.CommandText = "SELECT TOP 1 seqNumber FROM bobpar.activityPictures ORDER BY seqNumber DESC"
            mycom.Connection = con
            con.Open()
            seqNum = mycom.ExecuteScalar() + 1
            con.Close()
            picLoc = "http://www.letstrend.com/actpic/" & "an" & actNumber & "sn" & seqNum & ".jpg"

            mycom.Parameters.Clear()
            mycom.Parameters.AddWithValue("@pictureID", pictureID)
            mycom.Parameters.AddWithValue("@actNumber", actNumber)
            mycom.Parameters.AddWithValue("@seqNumber", seqNum)
            mycom.Parameters.AddWithValue("@picLoc", picLoc)

            mycom.Connection = con
            con.Open()
            mycom.CommandText = "INSERT INTO bobpar.activityPictures(" & nameString & ") VALUES (" & valueString & ")"
            result = mycom.ExecuteNonQuery()
            con.Close()
            System.IO.File.WriteAllBytes(FilePath & "an" & actNumber & "sn" & seqNum & ".jpg", myImage)
            'System.IO.File.WriteAllText(FilePath & "an" & actNumber & "sn" & seqNum & ".jpg", myImage)
        End If
    End Sub


    Function getImage(ByVal user As String, _
              ByVal pass As String, _
              ByVal actNumber As Integer, _
              ByVal seqNumber As Integer) As String

        Dim picLoc As String = ""
        Dim myreader As SqlDataReader

        If verifyUserPass(user, pass) Then
            mycom.CommandText = "SELECT picLoc FROM bobpar.activityPictures where actNumber='" & actNumber & "' and seqNumber='" & seqNumber & "'"
            con.Open()
            myreader = mycom.ExecuteReader()
            While myreader.Read()
                picLoc = myreader("picLoc")
            End While
            con.Close()
        End If
        If picLoc <> "" Then
            Return picLoc
        Else : Return ""
        End If
    End Function
End Class