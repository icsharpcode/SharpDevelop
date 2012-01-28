// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
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
			string helpCatalogUrl = string.Format(@"ms-xhelp://?method=page&id=-1&{0}", Help3Service.ActiveCatalog.AsMsXHelpParam);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpCatalogUrl));
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
			string helpPageUrl = string.Format(@"ms-xhelp://?method=page&id={1}&{0}", Help3Service.ActiveCatalog.AsMsXHelpParam, pageId);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpPageUrl));
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
			string helpContextualUrl = string.Format(@"ms-xhelp://?method=f1&query={1}&{0}", Help3Service.ActiveCatalog.AsMsXHelpParam, contextual);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpContextualUrl));
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
			string helpSearchUrl = string.Format(@"ms-xhelp://?method=search&query={1}&{0}", Help3Service.ActiveCatalog.AsMsXHelpParam, searchWords.Replace(" ", "+"));
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpSearchUrl));
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
			string helpKeywordsUrl = string.Format(@"ms-xhelp://?method=keywords&query={1}&{0}", Help3Service.ActiveCatalog.AsMsXHelpParam, keywords.Replace(" ", "+"));
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpKeywordsUrl));
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
			string helpUrl = string.Format(@"{0}{1}{2}",
			                               arguments, ProjectLanguages.GetCurrentLanguageAsHttpParam(), (embedded)?"&embedded=true":string.Empty);

			if (Help3Service.Config.ExternalHelp) {
				DisplayHelpWithShellExecute(helpUrl);
				return;
			}
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("Help 3.0: Navigating to {0}", helpUrl));
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
				LoggingService.Error(string.Format("Help 3.0: {0}", ex.ToString()));
			}
		}

		static void DisplayHelpOnMSDN(string keyword)
		{
			if (string.IsNullOrEmpty(keyword)) {
				throw new ArgumentNullException("keyword");
			}
			string msdnUrl = string.Format(@"http://msdn.microsoft.com/library/{0}.aspx", keyword);

			if (Help3Service.Config.ExternalHelp) {
				DisplayHelpWithShellExecute(msdnUrl);
				return;
			}
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("Help 3.0: Navigating to {0}", msdnUrl));
				browser.Navigate(msdnUrl);
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static void DisplaySearchOnMSDN(string searchWords)
		{
			if (string.IsNullOrEmpty(searchWords)) {
				throw new ArgumentNullException("searchWords");
			}
			string msdnUrl = string.Format(@"http://social.msdn.microsoft.com/Search/{0}/?query={1}&ac=3", CultureInfo.CurrentUICulture.ToString(), searchWords.Replace(" ", "+"));
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("Help 3.0: Navigating to {0}", msdnUrl));
				browser.Navigate(msdnUrl);
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static BrowserPane ActiveHelp3Browser()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null)
			{
				BrowserPane browser = window.ActiveViewContent as BrowserPane;
				if (browser != null && browser.Url.Scheme == "http") return browser;
			}
			foreach (IViewContent view in WorkbenchSingleton.Workbench.ViewContentCollection)
			{
				BrowserPane browser = view as BrowserPane;
				if (browser != null && browser.Url.Scheme == "http") return browser;
			}
			BrowserPane tmp = new BrowserPane();
			WorkbenchSingleton.Workbench.ShowView(tmp);
			return tmp;
		}
	}
}
