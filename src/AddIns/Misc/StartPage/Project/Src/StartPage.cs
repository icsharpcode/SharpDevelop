// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.StartPage
{
	public class StartPageScheme : ISchemeExtension
	{
		ICSharpCodePage page;
		
		public void InterceptNavigate(HtmlViewPane pane, WebBrowserNavigatingEventArgs e)
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
	}
}
