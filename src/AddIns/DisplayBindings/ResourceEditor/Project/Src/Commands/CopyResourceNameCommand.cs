// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop;

namespace ResourceEditor
{
	class CopyResourceNameCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			ResourceEditorControl editor = (ResourceEditorControl)window.ViewContent.Control;
			
			if(editor.ResourceList.SelectedItems.Count > 0) {
				Clipboard.SetDataObject(editor.ResourceList.SelectedItems[0].Text, true);
			}
		}
	}
}
