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
	public class PackageFilesViewFactory : IPackageFilesViewFactory
	{
		public PackageFilesViewFactory()
		{
		}
		
		public PackageFilesView Create(WixProject project, IWorkbench workbench)
		{
			return new PackageFilesView(project, workbench);
		}
	}
}
