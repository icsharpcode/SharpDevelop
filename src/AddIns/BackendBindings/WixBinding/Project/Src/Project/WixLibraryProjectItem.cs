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
	public class WixLibraryProjectItem : ProjectItem
	{
		public WixLibraryProjectItem(IProject project) : base(project)
		{
		}
		
		public override ProjectItem Clone()
		{
			ProjectItem n = new WixLibraryProjectItem(Project);
			n.Include = Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
		
		public override ItemType ItemType {
			get {
				return ItemType.None;
			}
		}
		
		public override string Tag {
			get {
				return "WixLibrary";
			}
		}
	}
}
