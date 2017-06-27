Imports Microsoft.VisualBasic

Public Class commons
    'Inherits System.Web.UI.Page

    Public Function twoDecimal(ByVal number As Double) As String
        Return String.Format("{0:n2}", number)
    End Function

    Public Function twoDecimal(ByVal number As List(Of Double)) As List(Of String)
        Dim formattedGraph As New List(Of String)
        Dim i As Integer = 0
        For i = 0 To number.Count - 1 Step 1
            formattedGraph.Add(twoDecimal(number(i)))
        Next
        Return formattedGraph
    End Function

    Public Function convertToDoubleList(ByVal inString As String) As List(Of Double)
        Dim doubleList As New List(Of Double)
        Dim strArr() As String
        Dim i As Integer = 0
        strArr = inString.Split(",")

        For i = 0 To strArr.Count - 1 Step 1
            doubleList.Add(strArr(i))
        Next

        Return doubleList
    End Function



End Class
