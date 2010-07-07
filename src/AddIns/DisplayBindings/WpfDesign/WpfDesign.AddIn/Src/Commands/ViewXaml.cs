// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn.Commands
{
	/// <summary>
	/// Switches to the XAML source code tab.
	/// </summary>
	public class ViewXaml : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SwitchView(0);
		}
	}
}
