// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
