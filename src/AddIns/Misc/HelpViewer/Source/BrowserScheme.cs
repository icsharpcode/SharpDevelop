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
