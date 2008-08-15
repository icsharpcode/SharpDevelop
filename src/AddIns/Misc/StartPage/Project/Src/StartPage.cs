// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
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
			} else {
				pane.WebBrowser.DocumentText = page.Render(host);
			}
		}
		
		public override void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e)
		{
			HtmlElement btn;
			btn = pane.WebBrowser.Document.GetElementById("opencombine");
			if (btn != null) {
				LoggingService.Debug("Attached event handler to opencombine button");
				btn.Click += delegate {new ICSharpCode.SharpDevelop.Project.Commands.LoadSolution().Run();};
			}
			btn = pane.WebBrowser.Document.GetElementById("newcombine");
			if (btn != null) {
				btn.Click += delegate {new ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution().Run();};
			}
			
			pane.WebBrowser.Navigating += pane_WebBrowser_Navigating;
			pane.WebBrowser.Navigated += pane_WebBrowser_Navigated;
		}

		void pane_WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			try {
				WebBrowser webBrowser = (WebBrowser)sender;
				webBrowser.Navigating -= pane_WebBrowser_Navigating;
				webBrowser.Navigated -= pane_WebBrowser_Navigated;
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}

		void pane_WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			try {
				if (e.Url.IsFile) {
					e.Cancel = true;
					string file = e.Url.LocalPath;
					IProjectLoader loader = ProjectService.GetProjectLoader(file);
					if (loader != null) {
						FileUtility.ObservedLoad(new NamedFileOperationDelegate(loader.Load), file);
					} else {
						FileService.OpenFile(file);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		public override void GoHome(HtmlViewPane pane)
		{
			pane.Navigate("startpage://start/");
		}
	}
}
