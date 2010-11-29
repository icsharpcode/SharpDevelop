// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			: base(name)
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
