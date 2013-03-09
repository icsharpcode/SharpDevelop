// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using ICSharpCode.AddInManager2.ViewModel;

namespace ICSharpCode.AddInManager2.Tests
{
	/// <summary>
	/// Extension methods for AddInManager2 view models supporting unit tests.
	/// </summary>
	internal static class ViewModelTestingExtensions
	{
		internal static void ReadPackagesAndWaitForUpdate(this AddInsViewModelBase viewModel)
		{
			ManualResetEvent updateDone = new ManualResetEvent(false);
			EventHandler addInsListUpdatedHandler = delegate { updateDone.Set(); };
			viewModel.AddInsListUpdated += addInsListUpdatedHandler;
			viewModel.ReadPackages();
			updateDone.WaitOne(5000);
			
			// Clean up
			viewModel.AddInsListUpdated -= addInsListUpdatedHandler;
		}
		
		internal static void SetPageAndWaitForUpdate(this AddInsViewModelBase viewModel, int page)
		{
			ManualResetEvent updateDone = new ManualResetEvent(false);
			EventHandler addInsListUpdatedHandler = delegate { updateDone.Set(); };
			viewModel.AddInsListUpdated += addInsListUpdatedHandler;
			viewModel.SelectedPageNumber = page;
			updateDone.WaitOne(5000);
			
			// Clean up
			viewModel.AddInsListUpdated -= addInsListUpdatedHandler;
		}
	}
}
