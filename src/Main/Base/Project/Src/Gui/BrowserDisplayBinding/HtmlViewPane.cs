// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;

using ICSharpCode.SharpDevelop.Internal.Undo;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent
	{
		protected HtmlViewPane htmlViewPane;
		
		public override Control Control {
			get {
				return htmlViewPane;
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
		
		protected BrowserPane(bool showNavigation)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			htmlViewPane.WebBrowser.DocumentTitleChanged += new EventHandler(TitleChange);
			htmlViewPane.Closed += PaneClosed;
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
			htmlViewPane.Dispose();
			base.Dispose();
		}
		
		public override void Load(string url)
		{
			htmlViewPane.Navigate(url);
		}
		
		public override void Save(string url)
		{
			Load(url);
		}
		
		void PaneClosed(object sender, EventArgs e)
		{
			WorkbenchWindow.CloseWindow(true);
		}
		
		void TitleChange(object sender, EventArgs e)
		{
			string title = htmlViewPane.WebBrowser.DocumentTitle;
			if (title == null || title.Length == 0)
				TitleName = "Browser";
			else
				TitleName = title;
		}
	}
	
	public class HtmlViewPane : UserControl
	{
		ExtendedWebBrowser webBrowser = null;
		
		Panel   topPanel   = new Panel();
		ToolBar toolBar    = new ToolBar();
		TextBox urlTextBox = new TextBox();
		
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
			
			if (showNavigation) {
				
				topPanel.Size = new Size(Width, 25);
				topPanel.Dock = DockStyle.Top;
				
				Controls.Add(topPanel);
				
				toolBar.Dock = DockStyle.None;
				
				
				for (int i = 0; i < 4; ++i) {
					ToolBarButton toolBarButton = new ToolBarButton();
					toolBarButton.ImageIndex    = i;
					toolBar.Buttons.Add(toolBarButton);
				}
				
				
				toolBar.ImageList = new ImageList();
				toolBar.ImageList.ColorDepth = ColorDepth.Depth32Bit;
				toolBar.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.BrowserBefore"));
				toolBar.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.BrowserAfter"));
				toolBar.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.BrowserCancel"));
				toolBar.ImageList.Images.Add(ResourceService.GetBitmap("Icons.16x16.BrowserRefresh"));
				
				toolBar.Appearance = ToolBarAppearance.Flat;
				toolBar.Divider = false;
				toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarClick);
				
				toolBar.Location = new Point(0, 0);
				toolBar.Size = new Size(4*toolBar.ButtonSize.Width, 25);
				
				topPanel.Controls.Add(toolBar);
				
				urlTextBox.Location  = new Point(4*toolBar.ButtonSize.Width, 2);
				urlTextBox.Size      = new Size(Width - (4*toolBar.ButtonSize.Width) - 1, 21);
				urlTextBox.Anchor    = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
				urlTextBox.KeyPress += new KeyPressEventHandler(KeyPressEvent);
				
				topPanel.Controls.Add(urlTextBox);
				
			}
			
			webBrowser = new ExtendedWebBrowser();
//			axWebBrowser.BeginInit();
//			if (showNavigation) {
//				int height = 48;
//				axWebBrowser.Location = new Point(0, height);
//				axWebBrowser.Size     = new Size(Width, Height - height);
//				axWebBrowser.Anchor   = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
//				axWebBrowser.Dock     = DockStyle.Fill;
//			} else {
			webBrowser.Dock = DockStyle.Fill;
//			}
			webBrowser.Navigating += WebBrowserNavigating;
			webBrowser.NewWindowExtended += NewWindow;
			webBrowser.Navigated  += WebBrowserNavigated;
			Controls.Add(webBrowser);
			
			if (showNavigation) {
				Controls.Add(topPanel);
			}
			
//			axWebBrowser.EndInit();
		}
		
		void NewWindow(object sender, NewWindowExtendedEventArgs e)
		{
			e.Cancel = true;
			WorkbenchSingleton.Workbench.ShowView(new BrowserPane(e.Url));
		}
		
		void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			urlTextBox.Text = webBrowser.Url.ToString();
		}
		
		static ArrayList descriptors;
		
		void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			try {
				if (descriptors == null) {
					AddInTreeNode treeNode = null;
					try {
						treeNode = AddInTree.GetTreeNode("/SharpDevelop/Views/Browser/SchemeExtensions");
					} catch (Exception) {
					}
					if (treeNode != null) {
						descriptors = treeNode.BuildChildItems(null);
					} else {
						descriptors = new ArrayList();
					}
				}
				string scheme = e.Url.Scheme;
				foreach (SchemeExtensionDescriptor descriptor in descriptors) {
					if (string.Equals(scheme, descriptor.SchemeName, StringComparison.OrdinalIgnoreCase)) {
						descriptor.InterceptNavigate(this, e);
					}
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void KeyPressEvent(object sender, KeyPressEventArgs ex)
		{
			if (ex.KeyChar == '\r') {
				ex.Handled = true;
				Navigate(urlTextBox.Text);
			}
		}
		
		void ToolBarClick(object sender, ToolBarButtonClickEventArgs e)
		{
			try {
				switch(toolBar.Buttons.IndexOf(e.Button)) {
					case 0:
						webBrowser.GoBack();
						break;
					case 1:
						webBrowser.GoForward();
						break;
					case 2:
						webBrowser.Stop();
						break;
					case 3:
						webBrowser.Refresh();
						break;
				}
			} catch (Exception) {
			}
		}
		
		public void Navigate(string name)
		{
			webBrowser.Navigate(new Uri(name));
		}
		
		public void Navigate(Uri url)
		{
			webBrowser.Navigate(url);
		}
	}
}
