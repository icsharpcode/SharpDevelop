<%@ Page Language="C#" MasterPageFile="~/ProzacAfternoon.master" 
    AutoEventWireup="true" CodeBehind="FormatCode.aspx.cs" Inherits="CCSite.FormatCode" 
    ValidateRequest="false"  Title="Format Code" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <p class="page_title">Step 1: Choose source language</p>
    <p>
        <asp:DropDownList ID="languageChoice" runat="server">
        </asp:DropDownList>
    <br />
    </p>
    
    <p class="page_title">Step 2: Paste the source code you want to HTML format</p>
    <p>
        <asp:TextBox ID="inputTextBox" runat="server" Height="171px" TextMode="MultiLine"
            Width="737px"></asp:TextBox>
            <br /> (Note: We do not store the code you submit in any way!)
    </p>
    <p class="page_title">Step 3: Format source code</p>
    <p>
        <asp:CheckBox ID="ShowLineNumbers" runat="server" Text="Show line numbers " /><br />
        <asp:CheckBox ID="UseAlternatingBackground" runat="server" Text="Use alternating line background" /><br />
        <asp:Button ID="convertCode" runat="server" Text="Format source code" OnClick="formatCode_Click" /><br />
    </p>
    <p class="page_title">Step 4: Output</p>
    <p>
        <asp:Label ID="OutputLabel" runat="server" Text="Formatted source code" Width="741px"></asp:Label><br />
        <asp:TextBox ID="outputTextBox" runat="server" Height="171px" TextMode="MultiLine"
            Width="737px"></asp:TextBox>
    </p>
    <div style="width: 100%;" runat="server" ID="PreviewPane" visible="false">
    <p class="page_title">Inline Code Preview</p>
    <p>
        <asp:Literal ID="inlineCodePreview" runat="server"></asp:Literal>
    </p>
    </div>

</asp:Content>