// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class ShowComponentInspectorCommand : AbstractMenuCommand
	{	
		public override void Run()
		{
			// Switch to previously opened view.
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) {
				ComponentInspectorView openView = viewContent as ComponentInspectorView;
				if (openView != null) {
					openView.WorkbenchWindow.SelectWindow();
					return;
				}
			}
			
			// Create new view.
			ComponentInspectorView view = new ComponentInspectorView();
			WorkbenchSingleton.Workbench.ShowView(view);
		}
	}
}
