// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent
	{
		HtmlViewPane htmlViewPane;
		
		public HtmlViewPane HtmlViewPane {
			get {
				return htmlViewPane;
			}
		}
		
		public override object Control {
			get {
				return htmlViewPane;
			}
		}
		
		protected BrowserPane(bool showNavigation)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			htmlViewPane.WebBrowser.DocumentTitleChanged += new EventHandler(TitleChange);
			htmlViewPane.Closed += PaneClosed;
			TitleChange(null, null);
		}
		
		public BrowserPane(Uri uri) : this(true)
		{
			htmlViewPane.Navigate(uri);
		}
		
		public BrowserPane() : this(true)
		{
		}
		
		public override void Dispose()
		{
			base.Dispose();
			htmlViewPane.Dispose();
		}
		
		public void Navigate(string url)
		{
			htmlViewPane.Navigate(url);
		}
		
		public Uri Url {
			get {
				return htmlViewPane.Url;
			}
		}
		
		void PaneClosed(object sender, EventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		void TitleChange(object sender, EventArgs e)
		{
			string title = htmlViewPane.WebBrowser.DocumentTitle;
			if (title != null)
				title = title.Trim();
			if (title == null || title.Length == 0)
				SetLocalizedTitle("${res:ICSharpCode.SharpDevelop.BrowserDisplayBinding.Browser}");
			else
				TitleName = title;
		}
	}
	
	public class HtmlViewPane : UserControl
	{
		ExtendedWebBrowser webBrowser = null;
		
		ToolStrip toolStrip;
		
//		string lastUrl     = null;
		
		public ExtendedWebBrowser WebBrowser {
			get {
				return webBrowser;
			}
		}
		
		public event EventHandler Closed;
		
		/// <summary>
		/// Closes the ViewContent that contains this HtmlViewPane.
		/// </summary>
		public void Close()
		{
			if (Closed != null) {
				Closed(this, EventArgs.Empty);
			}
		}
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				webBrowser.Dispose();
			}
		}
		
		public HtmlViewPane(bool showNavigation)
		{
			Dock = DockStyle.Fill;
			Size = new Size(500, 500);
			
			webBrowser = new ExtendedWebBrowser();
			webBrowser.Dock = DockStyle.Fill;
			webBrowser.Navigating += WebBrowserNavigating;
			webBrowser.NewWindowExtended += NewWindow;
			webBrowser.Navigated  += WebBrowserNavigated;
			webBrowser.StatusTextChanged += WebBrowserStatusTextChanged;
			webBrowser.DocumentCompleted += WebBrowserDocumentCompleted;
			Controls.Add(webBrowser);
			
			if (showNavigation) {
				toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/ViewContent/Browser/Toolbar");
				toolStrip.GripStyle = ToolStripGripStyle.Hidden;
				Controls.Add(toolStrip);
			}
		}
		
		void NewWindow(object sender, NewWindowExtendedEventArgs e)
		{
			e.Cancel = true;
			WorkbenchSingleton.Workbench.ShowView(new BrowserPane(e.Url));
		}
		
		void WebBrowserStatusTextChanged(object sender, EventArgs e)
		{
			IWorkbenchWindow workbench = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (workbench == null) return;
			BrowserPane browser = workbench.ActiveViewContent as BrowserPane;
			if (browser == null) return;
			if (browser.HtmlViewPane == this) {
				WorkbenchSingleton.StatusBar.SetMessage(webBrowser.StatusText);
			}
		}
		
		static List<SchemeExtensionDescriptor> descriptors;
		
		public static ISchemeExtension GetScheme(string name)
		{
			if (descriptors == null) {
				descriptors = AddInTree.BuildItems<SchemeExtensionDescriptor>("/SharpDevelop/Views/Browser/SchemeExtensions", null, false);
			}
			foreach (SchemeExtensionDescriptor descriptor in descriptors) {
				if (string.Equals(name, descriptor.SchemeName, StringComparison.OrdinalIgnoreCase)) {
					return descriptor.Extension;
				}
			}
			return null;
		}
		
		void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			try {
				ISchemeExtension extension = GetScheme(e.Url.Scheme);
				if (extension != null) {
					extension.InterceptNavigate(this, e);
					if (e.TargetFrameName.Length == 0) {
						if (e.Cancel == true) {
							dummyUrl = e.Url.ToString();
						} else if (e.Url.ToString() != "about:blank") {
							dummyUrl = null;
						}
					}
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void WebBrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			try {
				if (dummyUrl != null && e.Url.ToString() == "about:blank") {
					e = new WebBrowserDocumentCompletedEventArgs(new Uri(dummyUrl));
				}
				ISchemeExtension extension = GetScheme(e.Url.Scheme);
				if (extension != null) {
					extension.DocumentCompleted(this, e);
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		public void Navigate(string url)
		{
			Navigate(new Uri(url));
		}
		
		public void Navigate(Uri url)
		{
			try {
				webBrowser.Navigate(url);
			} catch (Exception ex) {
				LoggingService.Warn("Error navigating to " + url.ToString(), ex);
			}
		}
		
		public const string DefaultHomepage = "http://www.icsharpcode.net/";
		public const string DefaultSearchUrl = "http://www.google.com/";
		
		public void GoHome()
		{
			ISchemeExtension extension = GetScheme(Url.Scheme);
			if (extension != null) {
				extension.GoHome(this);
			} else {
				Navigate(DefaultHomepage);
			}
		}
		
		public void GoSearch()
		{
			ISchemeExtension extension = GetScheme(Url.Scheme);
			if (extension != null) {
				extension.GoSearch(this);
			} else {
				Navigate(DefaultSearchUrl);
			}
		}
		
		Control urlBox;
		
		public void SetUrlComboBox(ComboBox comboBox)
		{
			SetUrlBox(comboBox);
			comboBox.DropDownStyle = ComboBoxStyle.DropDown;
			comboBox.Items.Clear();
			comboBox.Items.AddRange(PropertyService.Get("Browser.URLBoxHistory", new string[0]));
			comboBox.AutoCompleteMode      = AutoCompleteMode.Suggest;
			comboBox.AutoCompleteSource    = AutoCompleteSource.HistoryList;
		}
		
		public void SetUrlBox(Control urlBox)
		{
			this.urlBox = urlBox;
			urlBox.KeyUp += UrlBoxKeyUp;
		}
		
		void UrlBoxKeyUp(object sender, KeyEventArgs e)
		{
			Control ctl = (Control)sender;
			if (e.KeyData == Keys.Return) {
				e.Handled = true;
				UrlBoxNavigate(ctl);
			}
		}
		
		void UrlBoxNavigate(Control ctl)
		{
			string text = ctl.Text.Trim();
			if (text.IndexOf(':') < 0) {
				text = "http://" + text;
			}
			Navigate(text);
			ComboBox comboBox = ctl as ComboBox;
			if (comboBox != null) {
				comboBox.Items.Remove(text);
				comboBox.Items.Insert(0, text);
				// Add to URLBoxHistory:
				string[] history = PropertyService.Get("Browser.URLBoxHistory", new string[0]);
				int pos = Array.IndexOf(history, text);
				if (pos < 0 && history.Length >= 20) {
					pos = history.Length - 1; // remove last entry and insert new at the beginning
				}
				if (pos < 0) {
					// insert new item
					string[] newHistory = new string[history.Length + 1];
					history.CopyTo(newHistory, 1);
					history = newHistory;
				} else {
					for (int i = pos; i > 0; i--) {
						history[i] = history[i - 1];
					}
				}
				history[0] = text;
				PropertyService.Set("Browser.URLBoxHistory", history);
			}
		}
		
		string dummyUrl;
		
		public Uri Url {
			get {
				if (webBrowser.Url == null)
					return new Uri("about:blank");
				if (dummyUrl != null && webBrowser.Url.ToString() == "about:blank") {
					return new Uri(dummyUrl);
				} else {
					return webBrowser.Url;
				}
			}
		}
		
		void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			// do not use e.Url (frames!)
			string url = webBrowser.Url.ToString();
			if (dummyUrl != null && url == "about:blank") {
				urlBox.Text = dummyUrl;
			} else {
				urlBox.Text = url;
			}
			// Update toolbar:
			foreach (object o in toolStrip.Items) {
				IStatusUpdate up = o as IStatusUpdate;
				if (up != null)
					up.UpdateStatus();
			}
		}
	}
}
