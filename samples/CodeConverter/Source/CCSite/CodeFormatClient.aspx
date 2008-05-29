<%@ Page Language="C#" MasterPageFile="~/ProzacAfternoon.master" 
    AutoEventWireup="true" CodeBehind="CodeFormatClient.aspx.cs" 
    Inherits="CCSite.CodeFormatClient" Title="Windows Forms Client Demo Application for Code Formatting" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <p class="page_title">Code Formatting Client</p>
    <p>
    This is a demo of how to use the Code Formatting Web Service - implemented as a Windows Forms application.
    Source code is included.
    </p>
    <p>
    <img src="/screenshots/FormatCode_Source.png" width="759" height="495" alt="Providing Input" />
    </p>
    <p>
    <img src="/screenshots/FormatCode_Preview.png" width="759" height="495" alt="The generated HTML is rendered as a preview" />
    </p>
    <p>
    <img src="/screenshots/FormatCode_Html.png" width="759" height="495" alt="Copy the HTML to your Web site / application" />
    </p>
    
    <p class="page_title">Downloads</p>
    <ul>
        <li><a href="/downloads/CodeFormatServiceClient.zip">Application</a></li>
        <li><a href="/downloads/CodeFormatServiceClient_Source.zip">Source Code</a> (C#)</li>
    </ul>
   
</asp:Content>