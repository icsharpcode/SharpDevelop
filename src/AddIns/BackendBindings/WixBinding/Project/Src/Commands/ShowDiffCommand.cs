// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.WixBinding
{
	public class ShowDiffCommand : AbstractActivePackageFilesViewCommand
	{
		public ShowDiffCommand()
		{
		}
		
		public ShowDiffCommand(IWorkbench workbench)
			: base(workbench)
		{
		}
		
		protected override void Run(PackageFilesView view)
		{
			view.CalculateDiff();
		}
	}
}
