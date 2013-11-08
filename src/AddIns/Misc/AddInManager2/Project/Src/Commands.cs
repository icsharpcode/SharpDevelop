// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.AddInManager2.View;

namespace ICSharpCode.AddInManager2
{
	public class ShowAddInManagerCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Open AddInManager2 main dialog
			using (AddInManagerView view = AddInManagerView.Create())
			{
				view.ShowDialog();
			}
		}
	}
	
	public class AddInManagerInitializationCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Remove all unreferenced NuGet packages
			AddInManagerServices.Setup.RemoveUnreferencedNuGetPackages();
		}
	}
	
	public class AddInManagerVisualInitializationCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			if (AddInManagerServices.Settings.AutoSearchForUpdates)
			{
				// Initialize UpdateNotifier and let it check for available updates
				UpdateNotifier updateNotifier = new UpdateNotifier();
				updateNotifier.StartUpdateLookup();
			}
		}
	}
}
