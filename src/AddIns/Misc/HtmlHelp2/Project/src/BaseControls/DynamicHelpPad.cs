/* ***********************************************************
 *
 * Help 2.0 Environment for SharpDevelop
 * Dynamic Help Pad
 * Copyright (c) 2005, Mathias Simmack. All rights reserved.
 *
 * ********************************************************* */
namespace HtmlHelp2
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Reflection;
	using System.IO;
	using ICSharpCode.Core.AddIns;
	using ICSharpCode.Core.AddIns.Codons;
	using ICSharpCode.SharpDevelop.Gui;
	using ICSharpCode.Core.Services;
	using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
	using ICSharpCode.SharpDevelop.Services;
	using HtmlHelp2Service;
	using HtmlHelp2Browser;
	using MSHelpServices;
	using mshtml;


	/*
	 * Note: The dynamic help is disabled at the moment. Technically it's
	 * possible. You can patch the default editor, trying to integrate the
	 * dynamic help. :o) But if you are doing so, don't forget to use the
	 * second version of "HtmlHelp2.addin", located in the "DynamicHelpResources"
	 * folder.
	 *
	 * Mathias.
	 */

	public class ShowDynamicHelpMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			HtmlHelp2DynamicHelpPad dynamicHelp = (HtmlHelp2DynamicHelpPad)WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2DynamicHelpPad));
			if(dynamicHelp != null) dynamicHelp.BringPadToFront();
		}
	}


	public class HtmlHelp2DynamicHelpPad : AbstractPadContent
	{
		protected HtmlHelp2DynamicHelpBrowserControl dynamicHelpBrowser;
		HtmlHelp2Environment h2env = null;
		int internalIndex = 0;

		public override Control Control
		{
			get {
				return dynamicHelpBrowser;
			}
		}

		public override void Dispose()
		{
			try {
				dynamicHelpBrowser.Dispose();
			}
			catch {
			}
		}

		public override void RedrawContent()
		{
			dynamicHelpBrowser.RedrawContent();
		}

		public HtmlHelp2DynamicHelpPad() : base("${res:AddIns.HtmlHelp2.DynamicHelp}", "HtmlHelp2.16x16.DynamicHelp")
		{
			h2env              = (HtmlHelp2Environment)ServiceManager.Services.GetService(typeof(HtmlHelp2Environment));
			dynamicHelpBrowser = new HtmlHelp2DynamicHelpBrowserControl();
		}

		#region WebBrowser Scripting
		public void BuildDynamicHelpList(string dynamicHelpString, string expectedLanguage)
		{
			if(h2env == null || !h2env.Help2EnvironmentIsReady || h2env.DynamicHelpIsBusy) {
				return;
			}

			this.BringPadToFront();
			try {
				this.RemoveAllChildren();

				Cursor.Current = Cursors.WaitCursor;
				IHxTopicList topics = h2env.GetMatchingTopicsForDynamicHelp(dynamicHelpString);
				Cursor.Current = Cursors.Default;

				if(topics.Count > 0) {
					for(int i = 1; i <= topics.Count; i++) {
						IHxTopic topic = topics.ItemAt(i);

						if(expectedLanguage == null || expectedLanguage == "" || topic.HasAttribute("DevLang", expectedLanguage)) {
							this.BuildNewChild(topic.Location,
							                   topic.get_Title(HxTopicGetTitleType.HxTopicGetRLTitle,HxTopicGetTitleDefVal.HxTopicGetTitleFileName),
							                   topic.URL);
						}
					}
				}
			}
			catch {
			}
		}

		private void RemoveAllChildren()
		{
			try {
				dynamicHelpBrowser.Document.body.innerHTML = "";
				this.internalIndex = 0;
			}
			catch {
			}
		}

		private void BuildNewChild(string sectionName, string topicName, string topicUrl)
		{
			mshtml.IHTMLElementCollection children = (mshtml.IHTMLElementCollection)((mshtml.IHTMLElement2)dynamicHelpBrowser.Document.body).getElementsByTagName("span");
			for(int i = 0; i < children.length; i++) {
				mshtml.IHTMLElement span = (mshtml.IHTMLElement)children.item(i,0);

				if((string)span.getAttribute("className", 0) == "section") {
					try {
						mshtml.IHTMLElementCollection spanChildren = (mshtml.IHTMLElementCollection)span.children;
						mshtml.IHTMLElement firstChild = (mshtml.IHTMLElement)spanChildren.item(1,0);
						mshtml.IHTMLElement spanChild  = (mshtml.IHTMLElement)spanChildren.item(3,0);

						if(firstChild.tagName == "B" && firstChild.innerText == sectionName) {
							if(spanChild.tagName == "SPAN" && (string)spanChild.getAttribute("className", 0) == "content") {
								spanChild.insertAdjacentHTML("beforeEnd",
								                             String.Format("<span class=\"link\" onmouseover=\"window.status='{0}'\" onmouseout=\"window.status=''\" onclick=\"Click('{0}');\">{1}</span><br>",
								                                           topicUrl, topicName)
								                            );

								return;
							}
						}
					}
					finally {
					}
				}
			}

			try {
				if(children.length > 0) {
					dynamicHelpBrowser.Document.body.insertAdjacentHTML("beforeEnd", "<br>");
				}

				dynamicHelpBrowser.Document.body.insertAdjacentHTML("beforeEnd",
				                                                    String.Format("<span class=\"section\"><img id=\"image_{0}\" src=\"OpenBook.png\" style=\"width:16px;height:16px;margin-right:5px;\">" +
				                                                                  "<b style=\"cursor:pointer\" onclick=\"ExpandCollapse({0})\">{1}</b><br>" +
				                                                                  "<span class=\"content\" id=\"content_{0}\">" +
				                                                                  "<span class=\"link\" onmouseover=\"window.status='{2}'\" onmouseout=\"window.status=''\" onclick=\"Click('{2}');\">{3}</span><br></span></span>",
				                                                                  this.internalIndex.ToString(), sectionName, topicUrl, topicName)
				                                                   );

				this.internalIndex++;
			}
			finally {
			}
		}
		#endregion
	}


	public class HtmlHelp2DynamicHelpBrowserControl : UserControl
	{
		AxWebBrowser axWebBrowser  = null;
		ToolBar dynamicHelpToolbar = new ToolBar();
		mshtml.HTMLDocument doc    = null;
		string[] toolbarButtons    = new string[] {
			"${res:AddIns.HtmlHelp2.Contents}",
			"${res:AddIns.HtmlHelp2.Index}",
			"${res:AddIns.HtmlHelp2.Search}"
		};

		public mshtml.HTMLDocument Document
		{
			get {
				return doc;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (doc != null) {
				mshtml.HTMLDocumentEvents2_Event docEvents = (mshtml.HTMLDocumentEvents2_Event)doc;
				docEvents.oncontextmenu -= new HTMLDocumentEvents2_oncontextmenuEventHandler(DocumentEventsOnContextMenu);
				int c = Marshal.ReleaseComObject(doc);
				doc = null;
			}
			base.Dispose(disposing);
			if (disposing) axWebBrowser.Dispose();
		}

		public void RedrawContent()
		{
			StringParserService sps = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			for(int i = 0; i < toolbarButtons.Length; i++) {
				dynamicHelpToolbar.Buttons[i].ToolTipText = sps.Parse(toolbarButtons[i]);
			}
		}

		public HtmlHelp2DynamicHelpBrowserControl()
		{
			HtmlHelp2Environment h2env      = (HtmlHelp2Environment)ServiceManager.Services.GetService(typeof(HtmlHelp2Environment));
			bool Help2EnvIsReady            = (h2env != null && h2env.Help2EnvironmentIsReady);

			Dock                            = DockStyle.Fill;
			Size                            = new Size(500, 500);

			axWebBrowser                    = new AxWebBrowser();
			axWebBrowser.BeginInit();
			axWebBrowser.Dock               = DockStyle.Fill;
			axWebBrowser.Enabled            = Help2EnvIsReady;
			axWebBrowser.HandleCreated     += new EventHandler(this.CreatedWebBrowserHandle);
			axWebBrowser.NewWindow2        += new DWebBrowserEvents2_NewWindow2EventHandler(this.NewBrowserWindow);
			axWebBrowser.StatusTextChange  += new DWebBrowserEvents2_StatusTextChangeEventHandler(this.StatusBarChanged);
			axWebBrowser.DocumentComplete  += new DWebBrowserEvents2_DocumentCompleteEventHandler(this.DocumentComplete);
			axWebBrowser.EndInit();
			Controls.Add(axWebBrowser);

			Controls.Add(dynamicHelpToolbar);
			dynamicHelpToolbar.Dock         = DockStyle.Top;
			dynamicHelpToolbar.Appearance   = ToolBarAppearance.Flat;
			dynamicHelpToolbar.Divider      = false;
			dynamicHelpToolbar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			StringParserService sps  = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			for(int i = 0; i < toolbarButtons.Length; i++) {
				ToolBarButton button = new ToolBarButton();
				button.ToolTipText   = sps.Parse(toolbarButtons[i]);
				button.ImageIndex    = i;
				dynamicHelpToolbar.Buttons.Add(button);
			}

			dynamicHelpToolbar.ImageList = new ImageList();
			dynamicHelpToolbar.ImageList.ColorDepth = ColorDepth.Depth32Bit;
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetImage("HtmlHelp2.16x16.Toc"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetImage("HtmlHelp2.16x16.Index"));
			dynamicHelpToolbar.ImageList.Images.Add(ResourcesHelper.GetImage("HtmlHelp2.16x16.Search"));
		}

		private void CreatedWebBrowserHandle(object sender, EventArgs evArgs)
		{
			object arg = System.Reflection.Missing.Value;
			try {
				string url = "about:blank";

				if(File.Exists(String.Format("{0}\\context.html",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))) {
					url = String.Format("file://{0}\\context.html",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				}
				axWebBrowser.Navigate(url, ref arg, ref arg, ref arg, ref arg);
			} catch {
			}
		}

		private void NewBrowserWindow(object sender, DWebBrowserEvents2_NewWindow2Event e)
		{
			HtmlHelp2BrowserPane help2Browser = ShowHelpBrowser.GetActiveHelp2BrowserView();
			help2Browser.WorkbenchWindow.SelectWindow();
			AxWebBrowser newBrowser           = (AxWebBrowser)((HtmlHelp2BrowserControl)help2Browser.Control).AxWebBrowser;
			e.ppDisp                          = newBrowser.Application;
			newBrowser.RegisterAsBrowser      = true;
		}

		private void DocumentComplete(object sender, DWebBrowserEvents2_DocumentCompleteEvent e)
		{
			try {
				mshtml.HTMLDocumentEvents2_Event docEvents;
				if (doc != null) {
					docEvents = (mshtml.HTMLDocumentEvents2_Event)doc;
					docEvents.oncontextmenu -= new HTMLDocumentEvents2_oncontextmenuEventHandler(DocumentEventsOnContextMenu);
					Marshal.ReleaseComObject(doc);
				}
				doc = (mshtml.HTMLDocument)axWebBrowser.Document;
				docEvents = (mshtml.HTMLDocumentEvents2_Event)doc;
				docEvents.oncontextmenu += new HTMLDocumentEvents2_oncontextmenuEventHandler(DocumentEventsOnContextMenu);
			}
			catch {
			}
		}

		private void StatusBarChanged(object sender, DWebBrowserEvents2_StatusTextChangeEvent e)
		{
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage(e.text);
		}

		private bool DocumentEventsOnContextMenu(IHTMLEventObj e)
		{
			e.cancelBubble = true;
			e.returnValue = false;
			return false;
		}

		private void ToolBarButtonClicked(object sender, ToolBarButtonClickEventArgs e)
		{
			switch(dynamicHelpToolbar.Buttons.IndexOf(e.Button)) {
				case 0:
					HtmlHelp2TocPad toc = (HtmlHelp2TocPad)WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2TocPad));
					if(toc != null) toc.BringPadToFront();
					break;
				case 1:
					HtmlHelp2IndexPad index = (HtmlHelp2IndexPad)WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2IndexPad));
					if(index != null) index.BringPadToFront();
					break;
				case 2:
					HtmlHelp2SearchPad search = (HtmlHelp2SearchPad)WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
					if(search != null) search.BringPadToFront();
					break;
			}
		}
	}
}
