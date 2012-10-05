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
			if (SD.Workbench.ActiveWorkbenchWindow == null ||
			   		SD.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = SD.Workbench.WorkbenchWindowCollection.IndexOf(SD.Workbench.ActiveWorkbenchWindow);
			SD.Workbench.WorkbenchWindowCollection [(index + 1) % SD.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (SD.Workbench.ActiveWorkbenchWindow == null ||
					SD.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = SD.Workbench.WorkbenchWindowCollection.IndexOf(SD.Workbench.ActiveWorkbenchWindow);
			SD.Workbench.WorkbenchWindowCollection [(index + SD.Workbench.WorkbenchWindowCollection.Count - 1) % SD.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			SD.Workbench.CloseAllViews();
		}
	}
	
}
