// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class WixLibraryExtensionProjectItem : WixExtensionProjectItem
	{
		public WixLibraryExtensionProjectItem(IProject project)
			: base(project, WixItemType.LibExtension)
		{
		}
		
		public WixLibraryExtensionProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem item)
			: base(project, item)
		{
		}
	}
}
