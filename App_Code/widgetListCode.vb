Imports System.IO
Imports System.Web
Imports System.Web.Services
Imports System.Web.UI


' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class widgetListCode
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function widgetListCode(ByVal pos As String, ByVal userName As String) As String
        Dim page As New Page()
        Dim widPos As New Label
        Dim userN As New Label

        widPos.Text = pos
        widPos.Visible = False
        widPos.ID = "position"
        page.Controls.Add(widPos)

        userN.Text = userName
        userN.Visible = False
        userN.ID = "userName"
        page.Controls.Add(userN)

        Dim ctl As UserControl = DirectCast(page.LoadControl("~/widgetList.ascx"), UserControl)

        page.Controls.Add(ctl)

        Dim writer As New StringWriter()
        HttpContext.Current.Server.Execute(page, writer, False)

        Return writer.ToString()
    End Function
End Class

