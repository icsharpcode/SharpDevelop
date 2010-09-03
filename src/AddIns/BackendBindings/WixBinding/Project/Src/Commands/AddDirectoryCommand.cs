// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Adds a directory and all its contents to the currently selected directory
	/// node.
	/// </summary>
	public class AddDirectoryCommand : AbstractActivePackageFilesViewCommand
	{
		public AddDirectoryCommand()
		{
		}
		
		public AddDirectoryCommand(IWorkbench workbench)
			: base(workbench)
		{
		}
		
		protected override void Run(PackageFilesView view)
		{
			view.AddDirectory();
		}
	}
}
