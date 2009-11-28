// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Command that adds a new Wix element to the selected WixPackageTreeView node.
	/// </summary>
	public class AddElementCommand : ToolStripMenuItem
	{
		string name;
		IWorkbench workbench;
		ActivePackageFilesView activePackageFilesView;
		
		public AddElementCommand(string name) 
			: this(name, WorkbenchSingleton.Workbench)
		{
		}
		
		public AddElementCommand(string name, IWorkbench workbench)
		{
			this.name = name;
			this.workbench = workbench;
			activePackageFilesView = new ActivePackageFilesView(workbench);
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			
			PackageFilesView view = activePackageFilesView.GetActiveView();
			if (view != null) {
				view.AddElement(name);
			}
		}
	}
}
