// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	public class ActivePackageFilesView
	{
		IWorkbench workbench;
		
		public ActivePackageFilesView(IWorkbench workbench)
		{
			this.workbench = workbench;
		}
		
		public PackageFilesView GetActiveView()
		{
			return workbench.ActiveContent as PackageFilesView;
		}
	}
}
