using System;
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
	internal sealed class DisplayHelp
	{
		DisplayHelp()
		{
		}

		public static void Catalog()
		{
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show("You requested an offline feature in the online help mode. You have to change the mode in order to use this feature.",
				                "Help Viewer",
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return;
			}
			Help3Catalog catalog = Help3Service.ActiveCatalog;
			if (catalog == null) {
				throw new ArgumentNullException("c");
			}
			string helpCatalogUrl = string.Format(@"ms-xhelp://?method=page&id=-1&product={0}&productVersion={1}&locale={2}",
			                                      catalog.ProductCode, catalog.ProductVersion, catalog.Locale);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpCatalogUrl));
			DisplayLocalHelp(helpCatalogUrl);
		}

		public static void Page(string pageId)
		{
			if (string.IsNullOrEmpty(pageId)) {
				throw new ArgumentNullException("pageId");
			}
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show("You requested an offline feature in the online help mode. You have to change the mode in order to use this feature.",
				                "Help Viewer",
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return;
			}
			Help3Catalog catalog = Help3Service.ActiveCatalog;
			if (catalog == null) {
				throw new ArgumentNullException("c");
			}
			string helpPageUrl = string.Format(@"ms-xhelp://?method=page&id={3}&product={0}&productVersion={1}&locale={2}",
			                                   catalog.ProductCode, catalog.ProductVersion, catalog.Locale, pageId);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpPageUrl));
			DisplayLocalHelp(helpPageUrl);
		}

		public static void ContextualHelp(string contextual)
		{
			if (string.IsNullOrEmpty(contextual)) {
				throw new ArgumentNullException("contextual");
			}
			if (!Help3Environment.IsLocalHelp) {
				DisplayHelpOnMSDN(contextual);
				return;
			}
			Help3Catalog catalog = Help3Service.ActiveCatalog;
			if (catalog == null) {
				throw new ArgumentNullException("c");
			}
			string helpContextualUrl = string.Format(@"ms-xhelp://?method=f1&query={3}&product={0}&productVersion={1}&locale={2}",
			                                         catalog.ProductCode, catalog.ProductVersion, catalog.Locale, contextual);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpContextualUrl));
			DisplayLocalHelp(helpContextualUrl);
		}

		public static void Search(string searchWords)
		{
			if (string.IsNullOrEmpty(searchWords)) {
				throw new ArgumentNullException("searchWords");
			}
			if (!Help3Environment.IsLocalHelp) {
				DisplaySearchOnMSDN(searchWords);
				return;
			}
			Help3Catalog catalog = Help3Service.ActiveCatalog;
			if (catalog == null) {
				throw new ArgumentNullException("c");
			}
			string helpSearchUrl = string.Format(@"ms-xhelp://method=search&query={3}&product={0}&productVersion={1}&locale={2}",
			                                     catalog.ProductCode, catalog.ProductVersion, catalog.Locale, searchWords.Replace(" ", "+"));
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpSearchUrl));
			DisplayLocalHelp(helpSearchUrl);
		}

		public static void Keywords(string keywords)
		{
			if (string.IsNullOrEmpty(keywords)) {
				throw new ArgumentNullException("keywords");
			}
			if (!Help3Environment.IsLocalHelp) {
				MessageBox.Show("You requested an offline feature in the online help mode. You have to change the mode in order to use this feature.",
				                "Help Viewer",
				                MessageBoxButtons.OK,
				                MessageBoxIcon.Error);
				return;
			}
			Help3Catalog catalog = Help3Service.ActiveCatalog;
			if (catalog == null) {
				throw new ArgumentNullException("c");
			}
			string helpKeywordsUrl = string.Format(@"ms-xhelp://?method=keywords&query={3}&product={0}&productVersion={1}&locale={2}",
			                                       catalog.ProductCode, catalog.ProductVersion, catalog.Locale, keywords);
			LoggingService.Debug(string.Format("Help 3.0: {0}", helpKeywordsUrl));
			DisplayLocalHelp(helpKeywordsUrl);
		}


		static void DisplayLocalHelp(string arguments)
		{
			// TODO: set "embedded" to TRUE if we have a TOC control or something similar
			DisplayLocalHelp(arguments, false);
		}

		static void DisplayLocalHelp(string arguments, bool embedded)
		{
			if (!Help3Environment.IsLocalHelp) { return; 	}
			if (!HelpLibraryAgent.IsRunning) {
				HelpLibraryAgent.Start();
				Thread.Sleep(0x3e8);
			}
			string helpUrl = string.Format(@"{0}{1}{2}",
			                               arguments, ProjectLanguages.GetCurrentLanguageAsHttpParam(), (embedded)?"&embedded=true":string.Empty);

			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("Help 3.0: Navigating to {0}", helpUrl));
				browser.Navigate(Help3Environment.GetHttpFromMsXHelp(helpUrl));
				browser.WorkbenchWindow.SelectWindow();
			}			
		}

		static void DisplayHelpOnMSDN(string keyword)
		{
			string msdnUrl = string.Format(@"http://msdn.microsoft.com/library/{0}.aspx", keyword);
			BrowserPane browser = ActiveHelp3Browser();
			if (browser != null) {
				LoggingService.Info(string.Format("Help 3.0: Navigating to {0}", msdnUrl));
				browser.Navigate(msdnUrl);
				browser.WorkbenchWindow.SelectWindow();
			}
		}

		static void DisplaySearchOnMSDN(string searchWords)
		{
			string msdnUrl = string.Format(@"http://social.social.msdn.microsoft.com/Search/{0}/?query={1}&ac=3", CultureInfo.CurrentUICulture.ToString(), searchWords.Replace(" ", "+"));
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
