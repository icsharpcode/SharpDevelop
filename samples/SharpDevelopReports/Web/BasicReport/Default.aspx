<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Basic Report</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Please click the "Create Report" Button to create the report.<br />
        <br />
        Requirements: SQL Server 2005 or SQL Server 2005 Express Edition with instance (local)\SQLEXPRESS<br />
        <a href="http://www.microsoft.com/downloads/details.aspx?familyid=06616212-0356-46a0-8da2-eebc53a68034&displaylang=en" target="_blank">Northwind Sample Database</a> installed.<br />
        <br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Create Report" /></div>
    </form>
</body>
</html>
