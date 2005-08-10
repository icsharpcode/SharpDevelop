/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 06.08.2005
 * Time: 16:13
 */

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace HtmlHelp2
{
	public class BrowserScheme : DefaultSchemeExtension
	{
		public override void GoHome(HtmlViewPane pane)
		{
			pane.Navigate(HtmlHelp2Service.HtmlHelp2Environment.DefaultPage);
		}
		
		public override void GoSearch(HtmlViewPane pane)
		{
//			new ShowSearchMenuCommand().Run();
			pane.Navigate(HtmlHelp2Service.HtmlHelp2Environment.SearchPage);
		}

		public override void DocumentCompleted(HtmlViewPane pane, WebBrowserDocumentCompletedEventArgs e)
		{
			ShowHelpBrowser.HighlightDocument(pane);
		}
	}
}
