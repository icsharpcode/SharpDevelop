using System;
using System.Globalization;
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
			LoggingService.Info("Help 3.0: Calling browser (\"GoHome()\")");
			DisplayHelp.Catalog();
		}

		public override void GoSearch(HtmlViewPane pane)
		{
			if (pane == null) {
				throw new ArgumentNullException("pane");
			}
			LoggingService.Info("Help 3.0: Calling browser (\"GoSearch()\")");
			pane.Navigate(new Uri(string.Format(@"http://social.msdn.microsoft.com/Search/{0}", CultureInfo.CurrentCulture.Name.ToLower())));
		}
	}
}
