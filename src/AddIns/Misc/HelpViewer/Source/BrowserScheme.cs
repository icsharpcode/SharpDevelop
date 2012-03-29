// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;
using MSHelpSystem.Core;

namespace MSHelpSystem
{
	public class BrowserScheme : DefaultSchemeExtension
	{
		public override void GoHome(HtmlViewPane pane)
		{
			if (pane == null) {
				throw new ArgumentNullException("pane");
			}
			DisplayHelp.Catalog();
		}

		
		public override void GoSearch(HtmlViewPane pane)
		{
			if (pane == null) {
				throw new ArgumentNullException("pane");
			}
			pane.Navigate(new Uri(string.Format(@"http://social.msdn.microsoft.com/Search/{0}", CultureInfo.CurrentCulture.Name.ToLower())));
		}

		public override void DocumentCompleted(HtmlViewPane pane, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
		{
			if (pane == null) {
				throw new ArgumentNullException("pane");
			}

			ExtendedWebBrowser browser = pane.WebBrowser;
			if (browser == null || browser.Document == null)
				return;
			HtmlElementCollection divs = browser.Document.GetElementsByTagName("div");
			foreach (HtmlElement div in divs) {
				if (!string.IsNullOrEmpty(div.Id)) {
					if (div.Id.Equals("LeftNav", StringComparison.InvariantCultureIgnoreCase) ||
					    div.Id.Equals("TocResize", StringComparison.InvariantCultureIgnoreCase))
					{
						div.Style = "visibility:hidden;display:none;width:0px";
					}
				}
			}
		}
	}
}
