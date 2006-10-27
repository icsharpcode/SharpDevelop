// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace HtmlHelp2
{
	using System;
	using ICSharpCode.Core;
	using ICSharpCode.SharpDevelop;
	using ICSharpCode.SharpDevelop.Gui;

	public class MSHelpProvider : HelpProvider
	{
		public override bool TryShowHelp(string fullTypeName)
		{
			LoggingService.Info("Help 2.0: MsHelpProvider.TryShowHelp");

//			try
//			{
				PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
				return ((HtmlHelp2SearchPad)search.PadContent).PerformF1Fts(fullTypeName, true);
//			}
//			catch
//			{
//				return false;
//			}
		}

		public override bool TryShowHelpByKeyword(string keyword)
		{
			LoggingService.Info("Help 2.0: MsHelpProvider.TryShowHelpByKeyword");

//			try
//			{
				PadDescriptor search = WorkbenchSingleton.Workbench.GetPad(typeof(HtmlHelp2SearchPad));
				return ((HtmlHelp2SearchPad)search.PadContent).PerformF1Fts(keyword);
//			}
//			catch
//			{
//				return false;
//			}
		}
	}
}
