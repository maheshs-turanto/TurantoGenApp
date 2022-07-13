<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="GeneratorBase.MVC.Report.ReportViewer" EnableViewState="true" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845DCD8080CC91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        html,body,form,#div1 {
            height: 100%; 
        }
    </style>
</head>
<body style="margin:0px !important;padding:0px !important;overflow:hidden !important">
    <form id="form1" runat="server">
         <div id="div1">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="MyReportViewer" ShowParameterPrompts="true" ShowFindControls="false" EnableTelemetry="false"
            ShowPageNavigationControls="true" ShowPrintButton="true" ShowZoomControl="false"
            ShowRefreshButton="false" Width="100%" Height="100%" AsyncRendering="false" runat="server" ProcessingMode="Remote">
        </rsweb:ReportViewer>
             </div>
    </form>
</body>
</html>
