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
	public class OptionsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			
			using (TreeViewOptions optionsDialog = new TreeViewOptions((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()),
			                                                           AddInTree.GetTreeNode("/SharpDevelop/Dialogs/OptionsDialog"))) {
				optionsDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
				
				optionsDialog.Owner = (Form)WorkbenchSingleton.Workbench;
				optionsDialog.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
			}
		}
	}
	
	public class ToggleFullscreenCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen = !((DefaultWorkbench)WorkbenchSingleton.Workbench).FullScreen;
		}
	}
	
	
}
