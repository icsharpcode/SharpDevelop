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
