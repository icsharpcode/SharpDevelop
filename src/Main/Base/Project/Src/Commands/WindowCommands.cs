// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SelectNextWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
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
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
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
