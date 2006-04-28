// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ImportProjectItem.
	/// </summary>
	public class ImportProjectItem : ProjectItem
	{
		public ImportProjectItem(IProject project) : base(project)
		{
		}
		
		public override ItemType ItemType {
			get {
				return ItemType.Import;
			}
		}
		
		public override ProjectItem Clone()
		{
			ImportProjectItem n = new ImportProjectItem(this.Project);
			n.Include = this.Include;
			this.CopyExtraPropertiesTo(n);
			return n;
		}
	}
}
