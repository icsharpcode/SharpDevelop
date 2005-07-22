// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	}
}
