// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
