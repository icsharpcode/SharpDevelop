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
	public class WixCompilerExtensionProjectItem : WixExtensionProjectItem
	{
		public WixCompilerExtensionProjectItem(IProject project) : base(project)
		{
		}
				
		public override string Tag {
			get {
				return "CompileExtension";
			}
		}
		
		protected override ProjectItem CreateNewInstance(IProject project)
		{
			return new WixCompilerExtensionProjectItem(project);
		}
	}
}
