/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 13.08.2005
 * Time: 15:36
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using ICSharpCode.SharpDevelop.Dom;

namespace HtmlHelp2
{
	public class MsHelpProvider : HelpProvider
	{
		// TODO: Implement MsHelpProvider
		
		public override bool TryShowHelp(string fullTypeName)
		{
			LoggingService.Info("MsHelpProvider.TryShowHelp");

			try {
				PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
				return ((HtmlHelp2SearchPad)search.PadContent).PerformF1FTS(fullTypeName, true);
			}
			catch {
				return false;
			}
		}

		public override bool TryShowHelpByKeyword(string keyword)
		{
			LoggingService.Info("MsHelpProvider.TryShowHelpByKeyword");

			try {
				PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
				return ((HtmlHelp2SearchPad)search.PadContent).PerformF1FTS(keyword);
			}
			catch {
				return false;
			}
		}
	}
}
