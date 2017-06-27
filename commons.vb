Imports Microsoft.VisualBasic

Public Class commons

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

End Class
