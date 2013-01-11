// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.AddInManager2.View;

namespace ICSharpCode.AddInManager2
{
	public class ShowCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Open AddInManager2 main dialog
			using (AddInManagerView view = CreateManagerView())
			{
				view.ShowDialog();
			}
		}
		
		private AddInManagerView CreateManagerView()
		{
			return new AddInManagerView()
			{
				Owner = SD.Workbench.MainWindow
			};
		}
	}
	
	public class AddInManagerInitializationCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// Load string resources needed by AddInManager2
			SD.ResourceService.RegisterStrings("ICSharpCode.AddInManager2.Resources.StringResources", GetType().Assembly);
			
			// Remove all unreferenced NuGet packages
			AddInManagerServices.Setup.RemoveUnreferencedNuGetPackages();
		}
	}
}
