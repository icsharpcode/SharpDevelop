// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
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
			int index = WorkbenchSingleton.Workbench.ViewContentCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent);
			WorkbenchSingleton.Workbench.ViewContentCollection[(index + 1) % WorkbenchSingleton.Workbench.ViewContentCollection.Count].WorkbenchWindow.SelectWindow();
		}
	}
	
	public class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			int index = WorkbenchSingleton.Workbench.ViewContentCollection.IndexOf(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent);
			WorkbenchSingleton.Workbench.ViewContentCollection[(index + WorkbenchSingleton.Workbench.ViewContentCollection.Count - 1) % WorkbenchSingleton.Workbench.ViewContentCollection.Count].WorkbenchWindow.SelectWindow();
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
