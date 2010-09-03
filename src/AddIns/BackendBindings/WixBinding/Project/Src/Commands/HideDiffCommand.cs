// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Hides the diff control from the Setup Files window.
	/// </summary>
	public class HideDiffCommand : AbstractActivePackageFilesViewCommand
	{
		public HideDiffCommand()
		{
		}
		
		public HideDiffCommand(IWorkbench workbench)
			: base(workbench)
		{
		}
		
		protected override void Run(PackageFilesView view)
		{
			view.HideDiff();
		}
	}
}
