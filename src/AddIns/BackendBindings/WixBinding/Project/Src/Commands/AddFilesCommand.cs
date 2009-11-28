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
	public class AddFilesCommand : AbstractActivePackageFilesViewCommand
	{
		public AddFilesCommand()
		{
		}
		
		public AddFilesCommand(IWorkbench workbench)
			: base(workbench)
		{
		}
		
		protected override void Run(PackageFilesView view)
		{
			view.AddFiles();
		}
	}
}

