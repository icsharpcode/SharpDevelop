// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	public abstract class AbstractActivePackageFilesViewCommand : AbstractMenuCommand
	{
		IWorkbench workbench;
		ActivePackageFilesView activePackageFilesView;
		
		public AbstractActivePackageFilesViewCommand()
			: this(WorkbenchSingleton.Workbench)
		{
		}

		public AbstractActivePackageFilesViewCommand(IWorkbench workbench)
		{
			this.workbench = workbench;
			activePackageFilesView = new ActivePackageFilesView(workbench);
		}
		
		public override void Run()
		{
			PackageFilesView view = activePackageFilesView.GetActiveView();
			if (view != null) {
				Run(view);
			}
		}
		
		 protected abstract void Run(PackageFilesView view);
	}
}
