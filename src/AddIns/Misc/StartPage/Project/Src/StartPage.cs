// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	public class StartPageScheme : DefaultSchemeExtension
	{
		ICSharpCodePage page;
		
		public override void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e)
		{
			e.Cancel = true;
			if (page == null) {
				page = new ICSharpCodePage();
				page.Title = ICSharpCode.Core.StringParser.Parse("${res:StartPage.StartPageContentName}");
			}
			string host = e.Url.Host;
			if (host == "project") {
				string projectFile = page.projectFiles[int.Parse(e.Url.LocalPath.Trim('/'))];
				FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), projectFile);
			} else if (host == "opencombine") {
				new ICSharpCode.SharpDevelop.Project.Commands.LoadSolution().Run();
			} else if (host == "newcombine") {
				new ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution().Run();
			} else {
				pane.WebBrowser.DocumentText = page.Render(host);
			}
		}
		
		public override void GoHome(HtmlViewPane pane)
		{
			pane.Navigate("startpage://start/");
		}
	}
}
