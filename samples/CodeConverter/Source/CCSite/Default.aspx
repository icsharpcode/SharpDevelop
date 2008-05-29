<%@ Page Language="C#" MasterPageFile="~/ProzacAfternoon.master" 
    AutoEventWireup="true" 
    Inherits="_DefaultPage" 
    Title="About the Code Converter" Codebehind="Default.aspx.cs" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <p class="page_title">The Code Converter</p>
    <p>
    Go directly to the <a href="SnippetConverter.aspx">converter</a> (supports C#, VB.NET and Boo)
    </p>
    
    <p class="page_title">The Code Formatter</p>
    <p>
    Go directly to the <a href="FormatCode.aspx">Code Formatter</a> (supports ASP/XHTML, BAT, Boo, Coco, C++.NET, C#, 
            HTML, Java, JavaScript, Patch, PHP, TeX, VBNET, XML)
    </p>
    <p>
    The code formatting capability is also exposed as a Web service. A <a href="CodeFormatClient.aspx">sample client application</a> is available 
    for download. Please note: SharpDevelop ships with \Samples\HtmlSyntaxColorizer on which this Web service is built upon.
    </p>
    
    <p class="page_title">Code Conversion as a Web Service</p>
    <p>
    If you want to use code conversion in your code, you can call the <a href="ConvertService.asmx">
        Convert Service</a>. It is really simple to use: reference the service, and
    you are good to go. Please use the method Convert (the others are deprecated), and the first
    parameter has the following options: cs2vbnet, vbnet2cs, cs2boo, vbnet2boo. If conversion
    fails, you can look at the ErrorMessage to see what went wrong. Otherwise, ConvertedSource
    will contain the source code in the target language. The service is doing the exact
    same steps as the <a href="SnippetConverter.aspx">online converter</a>.
    </p>
    <p>
    A sample Windows Forms application demonstrating the usage of the service is available:
    <a href="DotNetClientApplication.aspx">Windows Forms Web Service Client</a> (source included)
    </p>
    
    <p class="page_title">Technical Background</p>
    <p>
        The converter uses <a href="http://www.icsharpcode.net/opensource/sd/">SharpDevelop</a>'s
        NRefactory to perform the conversion. For the conversion
        to work properly, you have to paste a full class or source code file because we
        don't do "magic" RegEx's or string replacement - our code converter uses a full
        blown parser, and that's why the source code must be valid.</p>
    <p>
        Note that the conversion is not perfect (ie references to external references can
        be guessed only), however, if you encounter a problem such as a wrongly converted
        statement, please let us know - after all, the reason why we provide this online
        converter is to more easily gather feedback. You can get in touch with us via the <a href="http://community.sharpdevelop.net/forums/19/ShowForum.aspx">
            Bug Reporting Forum</a>. By providing us with a sample of what's not working
        as expected, we can improve the converter
        so you get better results the next time - as well as everyone else.
    </p>

</asp:Content>

