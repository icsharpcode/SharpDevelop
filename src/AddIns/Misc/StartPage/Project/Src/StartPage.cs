using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.Core;

namespace ICSharpCode.StartPage 
{
	/// <summary>
	/// This is the ViewContent implementation for the Start Page.
	/// </summary>
	public class StartPageView : AbstractViewContent
	{
		// defining the control variables used
		WebBrowser  webBrowser;
		
		// return the panel that contains all of our controls
		public override Control Control
		{
			get {
				return webBrowser;
			}
		}
		
		// the content cannot be modified
		public override bool IsViewOnly
		{
			get {
				return true;
			}
		}
		
		public override bool IsReadOnly
		{
			get {
				return false;
			}
		}
		
		// these methods are unused in this view
		public override void Save(string fileName) 
		{
		}
		
		public override void Load(string fileName) 
		{
		}
		
		// the redraw should get new add-in tree information
		// and update the view, the language or layout manager
		// may have changed.
		public override void RedrawContent()
		{
		}
		
		// Dispose all controls contained in this panel
		public override void Dispose()
		{
			try {
				webBrowser.Dispose();
			} catch {}
		}
		
		string curSection = "Start";
		ICSharpCodePage page = new ICSharpCodePage();
		
		// Default constructor: Initialize controls and display recent projects.
		public StartPageView()
		{
			webBrowser = new WebBrowser();
			webBrowser.Dock = DockStyle.Fill;
			
			
			webBrowser.DocumentText = page.Render(curSection);
			webBrowser.Navigating += new WebBrowserNavigatingEventHandler(HtmlControlBeforeNavigate);
			
			
			// Description of the tab shown in #develop
			TitleName = StringParser.Parse("${res:StartPage.StartPageContentName}");
			
			ProjectService.SolutionLoaded += new SolutionEventHandler(HandleCombineOpened);
		}
		
		void HandleCombineOpened(object sender, SolutionEventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		void HtmlControlBeforeNavigate(object sender, WebBrowserNavigatingEventArgs e)
		{
			e.Cancel = true;
			// bug in webbrowser control?
			string url = e.Url.Replace("about:blank", "");
			if (url.StartsWith("project://")) {
				try {
					ICSharpCode.Core.RecentOpen recOpen = (ICSharpCode.Core.RecentOpen)FileService.RecentOpen;
					
					string prjNumber = e.Url.Substring("project://".Length);
					prjNumber = prjNumber.Substring(0, prjNumber.Length - 1);
					
					string projectFile = page.projectFiles[int.Parse(prjNumber)];
					
					FileUtility.ObservedLoad(new NamedFileOperationDelegate(ProjectService.LoadSolution), projectFile);
				} catch (Exception ex) {
					MessageBox.Show("Could not access project service or load project:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			} else if (url.EndsWith("/opencombine")) {
				OpenBtnClicked(this, EventArgs.Empty);
			} else if (url.EndsWith("/newcombine")) {
				NewBtnClicked(this, EventArgs.Empty);
			} else if (url.EndsWith("/newcombine")) {
				NewBtnClicked(this, EventArgs.Empty);
			} else if (url.EndsWith("/opensection")) {
				Regex section = new Regex(@".*/(?<section>.+)/opensection", RegexOptions.Compiled);
				Match match = section.Match(e.Url);
				if (match.Success) {
					curSection = match.Result("${section}");
					webBrowser.DocumentText = page.Render(curSection);
				}
			} else {
				System.Diagnostics.Process.Start(e.Url);
			}
			e.Cancel = true;
		}
		
		public void OpenBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Project.Commands.LoadSolution cmd = new ICSharpCode.SharpDevelop.Project.Commands.LoadSolution();
				cmd.Run();
			} catch (Exception ex) {
				MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		
		public void NewBtnClicked(object sender, EventArgs e) 
		{
			try {
				ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution cmd = new ICSharpCode.SharpDevelop.Project.Commands.CreateNewSolution();
				cmd.Run();
			} catch (Exception ex) {
				MessageBox.Show("Could not access command:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
