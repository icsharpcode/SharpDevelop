// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Remove the selected element from the Wix document.
	/// </summary>
	public class RemoveElementCommand : AbstractActivePackageFilesViewCommand
	{
		public RemoveElementCommand()
		{
		}
		
		public RemoveElementCommand(IWorkbench workbench)
			: base(workbench)
		{
		}
		
		protected override void Run(PackageFilesView view)
		{
			view.RemoveSelectedElement();
		}
	}
}
