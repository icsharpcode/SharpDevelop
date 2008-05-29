<%@ Page Language="C#" MasterPageFile="~/ProzacAfternoon.master" 
    AutoEventWireup="true" CodeBehind="About.aspx.cs" 
    Inherits="CCSite.About" Title="About" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="news_right clearfix" id="news">
      <div class="news_cnr_top"><img src="images/cnr_tl.gif" alt="corner" width="8" height="8" class="cnr" style="display: none" /></div>
      <p><span class="news_title">Images and Layout</span><br />
        The design used for this site originates from <a href="http://www.oswd.org/">Open Source Web Design</a>,
        and we decided to use the <a href="http://www.oswd.org/design/preview/id/2381">prozac afternoon</a> layout.
          </p>
          <p>
          We changed the top photo, and went with one from <a href="http://www.photocase.de/">PhotoCase.de</a>. 
          The bridge metaphor is intended to symbolize a code conversion.
          </p>
      <div class="news_cnr_bottom"><img src="images/cnr_bl.gif" alt="corner" width="8" height="8" class="cnr" style="display: none" /></div>
    </div>
    <p class="page_title">Version Information</p>
    <p>The online code conversion facility is currently using version 
        <asp:Label ID="VersionLabel" runat="server" Text="Label"></asp:Label>
        of the <a href="http://laputa.sharpdevelop.net/NRefactoryTutorialVideo.aspx">NRefactory</a> 
        parser layer of <a href="http://www.icsharpcode.net/opensource/sd/">SharpDevelop</a>.
        </p>
        
    <p class="page_title">Who is Providing This Service?</p>
    <p>This Web site is operated by the SharpDevelop team to gather as much feedback on our
    code conversion feature as possible.
        </p>
        
</asp:Content>
