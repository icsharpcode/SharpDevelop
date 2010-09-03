// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class NavigateBack : AbstractMenuCommand
	{
		SplitButton splitButton = null;
		
		public override bool IsEnabled {
			get {
				UpdateEnabledState();
				return NavigationService.CanNavigateBack;
			}
		}
		
		public override void Run()
		{
			//LoggingService.Debug("Jumping back...");
			NavigationService.Go(-1);
		}

		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);

			// grab the owner so we can manipulate the buttons later
			splitButton = (SplitButton)Owner;
			
			// wire up our event handlers
			NavigationService.HistoryChanged += NavHistoryChanged;
			
			// trigger state change to match initial state
			NavHistoryChanged(this, EventArgs.Empty);
		}
		
		public void NavHistoryChanged(object sender, EventArgs e)
		{
			INavigationPoint p = NavigationService.CurrentPosition;
//			LoggingService.DebugFormatted("NavHistoryChanged ({0}): {1}",
//			                              NavigationService.Count,
//			                              (p==null?"null":p.ToString()));
			
			UpdateEnabledState();
		}
		
		public void UpdateEnabledState()
		{
			//splitButton.IsEnabled = NavigationService.CanNavigateBack;
			//splitButton.IsDropDownEnabled = NavigationService.Count>1;
		}
	}
	
	public class NavigateForward : AbstractMenuCommand
	{
		public override bool IsEnabled {
			get {
				return NavigationService.CanNavigateForwards;
			}
		}

		public override void Run()
		{
			//LoggingService.Debug("Jumping forwards...");
			NavigationService.Go(+1);
		}

		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
		}
	}
}
