// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using MSHelpSystem.Helper;

namespace MSHelpSystem.Core
{
	public sealed class DisplayHelp
	{
		DisplayHelp()
		{
		}

		public static bool Catalog()
		{
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show(StringParser.Parse("${res:AddIns.HelpViewer.OfflineFeatureRequestMsg}"),
				                StringParser.Parse("${res:AddIns.HelpViewer.MicrosoftHelpViewerTitle}"),
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return false;
			}
			if (Help3Service.ActiveCatalog == null) {
				return false;
			}
			string helpCatalogUrl = string.Concat("ms-xhelp://?method=page&id=-1&", Help3Service.ActiveCatalog.AsMsXHelpParam);
			DisplayLocalHelp(helpCatalogUrl);
			return true;
		}

		public static bool Page(string pageId)
		{
			if (string.IsNullOrEmpty(pageId)) {
				return false;
			}
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show(StringParser.Parse("${res:AddIns.HelpViewer.OfflineFeatureRequestMsg}"),
				                StringParser.Parse("${res:AddIns.HelpViewer.MicrosoftHelpViewerTitle}"),
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return false;
			}
			if (Help3Service.ActiveCatalog == null) {
				return false;
			}
			string helpPageUrl = string.Concat("ms-xhelp://?method=page&id=", pageId, "&", Help3Service.ActiveCatalog.AsMsXHelpParam);
			DisplayLocalHelp(helpPageUrl);
			return true;
		}

		public static bool ContextualHelp(string contextual)
		{
			if (string.IsNullOrEmpty(contextual)) {
				return false;
			}
			if (!Help3Environment.IsLocalHelp) {
				DisplayHelpOnMSDN(contextual);
				return true;
			}
			if (Help3Service.ActiveCatalog == null) {				
				return false;
			}
			string helpContextualUrl = string.Concat("ms-xhelp://?method=f1&query=", contextual, "&", Help3Service.ActiveCatalog.AsMsXHelpParam);
			DisplayLocalHelp(helpContextualUrl);
			return true;
		}

		public static bool Search(string searchWords)
		{
			if (string.IsNullOrEmpty(searchWords)) {
				return false;
			}
			if (!Help3Environment.IsLocalHelp) {
				DisplaySearchOnMSDN(searchWords);
				return true;
			}
			if (Help3Service.ActiveCatalog == null) {
				return false;
			}
			string helpSearchUrl = string.Concat("ms-xhelp://?method=search&query=", searchWords.Replace(" ", "+"), "&", Help3Service.ActiveCatalog.AsMsXHelpParam);
			DisplayLocalHelp(helpSearchUrl);
			return true;
		}

		public static bool Keywords(string keywords)
		{
			if (string.IsNullOrEmpty(keywords)) {
				return false;
			}
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show(StringParser.Parse("${res:AddIns.HelpViewer.OfflineFeatureRequestMsg}"),
				                StringParser.Parse("${res:AddIns.HelpViewer.MicrosoftHelpViewerTitle}"),
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return false;
			}
			if (Help3Service.ActiveCatalog == null) {
				return false;
			}
			string helpKeywordsUrl = string.Concat("ms-xhelp://?method=keywords&query=", keywords.Replace(" ", "+"), "&", Help3Service.ActiveCatalog.AsMsXHelpParam);
			DisplayLocalHelp(helpKeywordsUrl);
			return true;
		}


		static void DisplayLocalHelp(string arguments)
		{
			DisplayLocalHelp(arguments, !Help3Service.Config.ExternalHelp);
		}

		static void DisplayLocalHelp(string arguments, bool embedded)
		{
			if (string.IsNullOrEmpty(arguments)) {
				throw new ArgumentNullException("arguments");
			}
			if (!Help3Environment.IsLocalHelp) { return; 	}
			if (!HelpLibraryAgent.IsRunning) {
				HelpLibraryAgent.Start();
				Thread.Sleep(0x3e8);
			}
			string helpUrl = string.Concat(arguments, ProjectLanguages.CurrentLanguageAsHttpParam, (embedded)?"&embedded=true":string.Empty);

			if (Help3Service.Config.ExternalHelp) {
				DisplayHelpWithShellExecute(helpUrl);
				return;
			}
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("HelpViewer: DisplayLocalHelp calls \"{0}\"", helpUrl));
				browser.Navigate(Help3Environment.GetHttpFromMsXHelp(helpUrl));
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static void DisplayHelpWithShellExecute(string arguments)
		{
			if (string.IsNullOrEmpty(arguments)) {
				throw new ArgumentNullException("arguments");
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = arguments;
			psi.UseShellExecute = true;
			psi.WindowStyle = ProcessWindowStyle.Normal;
			try {
				Process p = Process.Start(psi);
				p.WaitForInputIdle();
			}
			catch (Exception ex) {
				LoggingService.Error(string.Format("HelpViewer: {0}", ex.ToString()));
			}
		}

		static void DisplayHelpOnMSDN(string keyword)
		{
			if (string.IsNullOrEmpty(keyword)) {
				throw new ArgumentNullException("keyword");
			}
			string msdnUrl = string.Concat("http://msdn.microsoft.com/library/", keyword, ".aspx");

			if (Help3Service.Config.ExternalHelp) {
				DisplayHelpWithShellExecute(msdnUrl);
				return;
			}
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("HelpViewer: DisplayHelpOnMSDN calls \"{0}\"", msdnUrl));
				browser.Navigate(msdnUrl);
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static void DisplaySearchOnMSDN(string searchWords)
		{
			if (string.IsNullOrEmpty(searchWords)) {
				throw new ArgumentNullException("searchWords");
			}
			string msdnUrl = string.Concat("http://social.msdn.microsoft.com/Search/", CultureInfo.CurrentUICulture.ToString(), "/?query=", searchWords.Replace(" ", "+"), "&ac=3");
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("HelpViewer: DisplaySearchOnMSDN calls \"{0}\"", msdnUrl));
				browser.Navigate(msdnUrl);
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static BrowserPane ActiveHelp3Browser()
		{
			IWorkbenchWindow window = SD.Workbench.ActiveWorkbenchWindow;
			if (window != null)
			{
				BrowserPane browser = window.ActiveViewContent as BrowserPane;
				if (browser != null && browser.Url.Scheme == "http") return browser;
			}
			foreach (IViewContent view in SD.Workbench.ViewContentCollection)
			{
				BrowserPane browser = view as BrowserPane;
				if (browser != null && browser.Url.Scheme == "http") return browser;
			}
			BrowserPane tmp = new BrowserPane();
			SD.Workbench.ShowView(tmp);
			return tmp;
		}
	}
}
