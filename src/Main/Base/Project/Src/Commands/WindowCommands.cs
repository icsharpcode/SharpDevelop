// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SelectNextWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null ||
			   		WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Workbench.WorkbenchWindowCollection[(index + 1) % WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null ||
					WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = WorkbenchSingleton.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Workbench.WorkbenchWindowCollection[(index + WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count - 1) % WorkbenchSingleton.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.CloseAllViews();
		}
	}
	
}
