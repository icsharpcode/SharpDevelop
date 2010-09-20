// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
