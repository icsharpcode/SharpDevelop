/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Browser Control
 * Based on "HtmlViewPane.cs" by Mike Krueger
 *
 * ********************************************************* */

//#define ChangeTopics_BringPadToFront

namespace HtmlHelp2Browser
{
	using System;
	using System.ComponentModel;
	using System.Drawing;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
	using HtmlHelp2;
	using HtmlHelp2Service;


	public class HtmlHelp2BrowserPane : AbstractViewContent
	{
		protected HtmlHelp2BrowserControl help2Browser;

		public override Control Control {
			get {
				return help2Browser;
			}
		}

		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}

		public override bool IsViewOnly {
			get {
				return true;
			}
		}

		public HtmlHelp2BrowserPane(string fileName)
		{
			help2Browser                                    = new HtmlHelp2BrowserControl();
			help2Browser.AxWebBrowser.DocumentTitleChanged += new EventHandler(TitleChanged);
			if(fileName != null) this.Load(fileName);
				else this.Load("about:blank");
		}

		public override void Load(string url)
		{
			help2Browser.Navigate(url);
			this.FileName = url;
		}

		public override void Save(string url)
		{
			Load(url);
		}

		void TitleChanged(object sender, EventArgs e)
		{
			this.TitleName = help2Browser.AxWebBrowser.DocumentTitle;
			this.FileName  = help2Browser.AxWebBrowser.Url.ToString();
		}
	}


	public class HtmlHelp2BrowserControl : UserControl
	{
		ExtendedWebBrowser axWebBrowser = null;
		ToolStripButton goBack          = new ToolStripButton();
		ToolStripButton goForward       = new ToolStripButton();
		ToolStripButton stopSite        = new ToolStripButton();
		ToolStripButton refreshSite     = new ToolStripButton();
		ToolStripButton homePage        = new ToolStripButton();
		ToolStripButton searchPage      = new ToolStripButton();
		ToolStripButton addToFavorites  = new ToolStripButton();
		ToolStripComboBox urlTextbox    = new ToolStripComboBox();
		ToolStripButton newWindow       = new ToolStripButton();
		ToolStripButton syncToc         = new ToolStripButton();
		ToolStripButton prevTocTopic    = new ToolStripButton();
		ToolStripButton nextTocTopic    = new ToolStripButton();
//		int zoomFactor                  = 2;


		public WebBrowser AxWebBrowser
		{
			get {
				return axWebBrowser;
			}
		}

		public HtmlHelp2BrowserControl()
		{
			Size                             = new Size(800, 800);
			Dock                             = DockStyle.Fill;

			axWebBrowser                     = new ExtendedWebBrowser();
			axWebBrowser.Dock                = DockStyle.Fill;
			axWebBrowser.StatusTextChanged  += new EventHandler(this.StatusTextChanged);
			axWebBrowser.Navigating         += new WebBrowserNavigatingEventHandler(this.BeforeNavigating);
			axWebBrowser.DocumentCompleted  += new WebBrowserDocumentCompletedEventHandler(this.DocumentCompleted);
			axWebBrowser.NewWindowExtended  += new NewWindowExtendedEventHandler(this.NewBrowserWindow);
			Controls.Add(axWebBrowser);

			ToolStrip toolStrip              = new ToolStrip();
			toolStrip.Dock                   = DockStyle.Top;
			toolStrip.AllowItemReorder       = false;
			Controls.Add(toolStrip);
			toolStrip.Items.Add(goBack);
			goBack.Click                    += new EventHandler(this.GoBack);
			goBack.Enabled                   = false;
			goBack.ImageIndex                = 0;
			toolStrip.Items.Add(goForward);
			goForward.Click                 += new EventHandler(this.GoForward);
			goForward.Enabled                = false;
			goForward.ImageIndex             = 1;
			toolStrip.Items.Add(stopSite);
			stopSite.Click                  += new EventHandler(this.StopPageLoading);
			stopSite.Enabled                 = false;
			stopSite.ImageIndex              = 2;
			toolStrip.Items.Add(refreshSite);
			refreshSite.Click               += new EventHandler(this.RefreshPage);
			refreshSite.ImageIndex           = 3;
			toolStrip.Items.Add(homePage);
			homePage.Click                  += new EventHandler(this.CallHomepage);
			homePage.ImageIndex              = 4;
			toolStrip.Items.Add(new ToolStripSeparator());
			toolStrip.Items.Add(searchPage);
			searchPage.Click                += new EventHandler(this.CallSearchpage);
			searchPage.ImageIndex            = 5;
			toolStrip.Items.Add(addToFavorites);
			addToFavorites.Click            += new EventHandler(this.AddToHelpFavorites);
			addToFavorites.Enabled           = false;
			addToFavorites.ImageIndex        = 6;
			toolStrip.Items.Add(new ToolStripSeparator());
			toolStrip.Items.Add(urlTextbox);
			urlTextbox.AutoCompleteMode      = AutoCompleteMode.Suggest;
			urlTextbox.AutoCompleteSource    = AutoCompleteSource.HistoryList;
			urlTextbox.DropDownWidth         = 300;
			urlTextbox.Size                  = new Size(300, toolStrip.Items[0].Height);
			urlTextbox.KeyDown              += new KeyEventHandler(this.UrlTextboxKeyDown);
			urlTextbox.SelectedIndexChanged += new EventHandler(this.UrlTextboxSelectedIndexChanged);
			toolStrip.Items.Add(new ToolStripSeparator());
			toolStrip.Items.Add(newWindow);
			newWindow.Click                 += new EventHandler(this.NewHelpWindow);
			newWindow.ImageIndex             = 7;
			toolStrip.Items.Add(syncToc);
			syncToc.Click                   += new EventHandler(this.SyncToc);
			syncToc.Enabled                  = false;
			syncToc.ImageIndex               = 8;
			toolStrip.Items.Add(new ToolStripSeparator());
			toolStrip.Items.Add(prevTocTopic);
			prevTocTopic.Click              += new EventHandler(this.PreviousTocTopic);
			prevTocTopic.Enabled             = false;
			prevTocTopic.ImageIndex          = 9;
			toolStrip.Items.Add(nextTocTopic);
			nextTocTopic.Click              += new EventHandler(this.NextTocTopic);
			nextTocTopic.Enabled             = false;
			nextTocTopic.ImageIndex          = 10;

			// TODO: name those buttons :o)

			toolStrip.ImageList              = new ImageList();
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Back.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Forward.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Stop.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Refresh.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.Home.gif"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.SearchSite.gif"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.AddToFavorites.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.NewWindow.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.SyncToc.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.PrevTopic.png"));
			toolStrip.ImageList.Images.Add(ResourcesHelper.GetBitmap("HtmlHelp2.16x16.NextTopic.png"));
		}

		#region WebBrowser Events
		private void StatusTextChanged(object sender, EventArgs e)
		{
			StatusBarService.SetMessage(axWebBrowser.StatusText);
		}

		private void BeforeNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			stopSite.Enabled = true;
		}

		private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
//			try {
//				object arg   = System.Reflection.Missing.Value;
//				object dummy = 0;
//
//				axWebBrowser.ExecWB(SHDocVw.OLECMDID.OLECMDID_ZOOM,
//				                    SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
//				                    ref arg,
//				                    ref dummy);
//
//				zoomFactor   = (int)dummy;
//			}
//			catch {
//			}

			PadDescriptor toc      = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));

			// update URL textbox
			string currentUrl      = e.Url.ToString();
			urlTextbox.Text        = currentUrl;

			// update toolbar
			goBack.Enabled         = axWebBrowser.CanGoBack;
			goForward.Enabled      = axWebBrowser.CanGoForward;
			stopSite.Enabled       = false;
			addToFavorites.Enabled = currentUrl.StartsWith("ms-help://");
			syncToc.Enabled        = addToFavorites.Enabled;

			prevTocTopic.Enabled   = (toc == null || ((HtmlHelp2TocPad)toc.PadContent).IsNotFirstNode);
			nextTocTopic.Enabled   = (toc == null || ((HtmlHelp2TocPad)toc.PadContent).IsNotLastNode);

			// hilite (full-text search only!)
			ShowHelpBrowser.HighlightDocument();
		}

		private void NewBrowserWindow(object sender, NewWindowExtendedEventArgs e)
		{
			HtmlHelp2BrowserPane help2Browser = new HtmlHelp2BrowserPane("");
			WorkbenchSingleton.Workbench.ShowView(help2Browser);
			help2Browser.WorkbenchWindow.SelectWindow();
			ExtendedWebBrowser newBrowser     = (ExtendedWebBrowser)((HtmlHelp2BrowserControl)help2Browser.Control).AxWebBrowser;
			newBrowser.Navigate(e.Url);
			e.Cancel                          = true;
		}
		#endregion

		#region Toolbar Command
		public void Navigate(string url)
		{
			this.AddTermToList(url);
			axWebBrowser.Navigate(url);
		}

		private void GoBack(object sender, EventArgs e)
		{
			if(axWebBrowser.CanGoBack) axWebBrowser.GoBack();
		}

		private void GoForward(object sender, EventArgs e)
		{
			if(axWebBrowser.CanGoForward) axWebBrowser.GoForward();
		}

		private void StopPageLoading(object sender, EventArgs e)
		{
			axWebBrowser.Stop();
		}

		private void RefreshPage(object sender, EventArgs e)
		{
			axWebBrowser.Refresh();
		}

		private void CallHomepage(object sender, EventArgs e)
		{
			this.Navigate(HtmlHelp2Environment.DefaultPage);
		}

		private void CallSearchpage(object sender, EventArgs e)
		{
			this.Navigate(HtmlHelp2Environment.SearchPage);
		}

		private void AddToHelpFavorites(object sender, EventArgs e)
		{
			PadDescriptor favorites = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2FavoritesPad));
			if(favorites != null) {
				((HtmlHelp2FavoritesPad)favorites.PadContent).AddToFavorites(axWebBrowser.DocumentTitle, axWebBrowser.Url.ToString());
			}
		}

		private void NewHelpWindow(object sender, EventArgs e)
		{
			if(axWebBrowser.Url != null) {
				HtmlHelp2BrowserPane newPage = ShowHelpBrowser.CreateNewHelp2BrowserView();
				newPage.Load(axWebBrowser.Url.ToString());
			}
		}

		private void SyncToc(object sender, EventArgs e)
		{
			if(axWebBrowser.Url != null) {
				PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
				if(toc != null) {
					((HtmlHelp2TocPad)toc.PadContent).SyncToc(axWebBrowser.Url.ToString());
					toc.BringPadToFront();
				}
			}
		}

		private void PreviousTocTopic(object sender, EventArgs e)
		{
			PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
			if(toc != null) {
				((HtmlHelp2TocPad)toc.PadContent).GetPrevFromNode();
				#if ChangeTopics_BringPadToFront
				((HtmlHelp2TocPad)toc.PadContent).BringPadToFront();
				#endif
			}
		}

		private void NextTocTopic(object sender, EventArgs e)
		{
			PadDescriptor toc = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
			if(toc != null) {
				((HtmlHelp2TocPad)toc.PadContent).GetNextFromNode();
				#if ChangeTopics_BringPadToFront
				((HtmlHelp2TocPad)toc.PadContent).BringPadToFront();
				#endif
			}
		}

//		public void ZoomText()
//		{
////			try {
////				zoomFactor = (zoomFactor == 4)?0:zoomFactor+1;
////				object zoomArg = zoomFactor;
////				object dummy   = System.Reflection.Missing.Value;
////
////				axWebBrowser.ExecWB(SHDocVw.OLECMDID.OLECMDID_ZOOM,
////				                    SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
////				                    ref zoomArg,
////				                    ref dummy);
////			}
////			catch {
////			}
//		}
		#endregion

		#region UrlTextbox
		private void UrlTextboxKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Enter && urlTextbox.Text != "") {
				this.AddTermToList(urlTextbox.Text);
				this.Navigate(urlTextbox.Text);
			}
		}

		private void UrlTextboxSelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedUrl = urlTextbox.SelectedItem.ToString();
			if(selectedUrl != "") {
				this.Navigate(selectedUrl);
			}
		}

		private void AddTermToList(string searchText)
		{
			if(searchText == "") return;

			if(urlTextbox.Items.IndexOf(searchText) == -1) {
				urlTextbox.Items.Insert(0, searchText);
				if(urlTextbox.Items.Count > 10) urlTextbox.Items.RemoveAt(10);
				urlTextbox.SelectedIndex = 0;
			}
		}
		#endregion
	}
}
