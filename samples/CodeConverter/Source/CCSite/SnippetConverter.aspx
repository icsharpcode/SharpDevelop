<%@ Page Language="C#" MasterPageFile="~/ProzacAfternoon.master" AutoEventWireup="true"
    Inherits="SnippetConverterPage" 
    Title="Snippet Converter for .NET 2.0" 
    ValidateRequest="false" Codebehind="SnippetConverter.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <p class="page_title">Step 1: Choose source language and destination language for conversion</p>
    <p>
        <asp:RadioButtonList ID="languageChoice" runat="server" RepeatColumns="4">
            <asp:ListItem Selected="True" Value="cs2vbnet">C# to VB.NET</asp:ListItem>
            <asp:ListItem Value="vbnet2cs">VB.NET to C#</asp:ListItem>
            <asp:ListItem Value="cs2boo">C# to Boo</asp:ListItem>
            <asp:ListItem Value="vbnet2boo">VB.NET to Boo</asp:ListItem>
            <asp:ListItem Value="cs2python">C# to Python</asp:ListItem>
            <asp:ListItem Value="vbnet2python">VB.NET to Python</asp:ListItem>
            <asp:ListItem Value="cs2ruby">C# to Ruby</asp:ListItem>
            <asp:ListItem Value="vbnet2ruby">VB.NET to Ruby</asp:ListItem>
        </asp:RadioButtonList>
    <br />
    </p>
    
    <p class="page_title">Step 2: Paste the source code snippet you want to convert</p>
    <p>
        <asp:TextBox ID="inputTextBox" runat="server" Height="171px" TextMode="MultiLine"
            Width="737px"></asp:TextBox>
            <br /> (Note: We do not store the code you submit in any way!)
    </p>
    <p class="page_title">Step 3: Perform the actual conversion</p>
    <p>
        <asp:Button ID="convertCode" runat="server" Text="Perform conversion" OnClick="convertCode_Click" /><br />
    </p>
    <p class="page_title">Step 4: Output</p>
    <p>
        <asp:Label ID="OutputLabel" runat="server" Text="Converted Sourcecode" Width="741px"></asp:Label><br />
        <asp:TextBox ID="outputTextBox" runat="server" Height="171px" TextMode="MultiLine"
            Width="737px"></asp:TextBox>
    </p>
    
    <p class="page_title">IMPORTANT: Please help us improve!</p>
    <p>
    If you encounter a problem such as a wrongly converted statement (<a href="default.aspx">read about conversion limitations first</a>), please let us know via the 
    <a href="http://community.sharpdevelop.net/forums/19/ShowForum.aspx">Bug Reporting Forum</a>. 
    By providing us with a sample of what's not working as expected, we can improve the converter.
    </p>
</asp:Content>
