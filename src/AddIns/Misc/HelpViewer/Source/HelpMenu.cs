using System;
using MSHelpSystem.Core;
using ICSharpCode.Core;

namespace MSHelpSystem.SharpDevelopMenu
{
	public class DisplayCatalogCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Help 3.0: Calling menu command \"DisplayHelp.Catalog()\"");
			DisplayHelp.Catalog();
		}
	}
}
