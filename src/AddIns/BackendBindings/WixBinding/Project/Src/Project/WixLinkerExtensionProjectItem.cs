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
	public class WixLinkerExtensionProjectItem : WixExtensionProjectItem
	{
		public WixLinkerExtensionProjectItem(IProject project)
			: base(project, WixItemType.LinkExtension)
		{
		}
		
		public WixLinkerExtensionProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem item)
			: base(project, item)
		{
		}
	}
}
