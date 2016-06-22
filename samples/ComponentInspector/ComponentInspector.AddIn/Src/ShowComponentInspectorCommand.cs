// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class ShowComponentInspectorCommand : AbstractMenuCommand
	{	
		public override void Run()
		{
			// Switch to previously opened view.
			foreach (IViewContent viewContent in SD.Workbench.ViewContentCollection) {
				ComponentInspectorView openView = viewContent as ComponentInspectorView;
				if (openView != null) {
					openView.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			
			// Create new view.
			ComponentInspectorView view = new ComponentInspectorView();
			SD.Workbench.ShowView(view);
		}
	}
}
