Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI


' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class signalGuideCode
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function signalGuideCode(ByVal ticker As String, ByVal userName As String) As String
        Dim page As New Page()
        Dim writer As New StringWriter()
        Dim userN As New Label
        Dim tickerN As New Label
        Dim ctl As UserControl = DirectCast(page.LoadControl("~/signalGuide.ascx"), UserControl)

        userN.Text = userName
        userN.Visible = False
        userN.ID = "userName"
        page.Controls.Add(userN)

        tickerN.Text = ticker
        tickerN.Visible = False
        tickerN.ID = "ticker"
        page.Controls.Add(tickerN)

        page.Controls.Add(ctl)
        HttpContext.Current.Server.Execute(page, writer, False)
        Return writer.ToString()
    End Function
End Class

