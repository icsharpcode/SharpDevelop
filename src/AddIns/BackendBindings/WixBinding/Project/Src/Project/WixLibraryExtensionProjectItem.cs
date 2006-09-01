// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.WixBinding
{
	public class WixLibraryExtensionProjectItem : WixExtensionProjectItem
	{
		public WixLibraryExtensionProjectItem(IProject project) : base(project)
		{
		}
				
		public override string Tag {
			get {
				return "LibExtension";
			}
		}
		
		protected override ProjectItem CreateNewInstance(IProject project)
		{
			return new WixLibraryExtensionProjectItem(project);
		}
	}
}
