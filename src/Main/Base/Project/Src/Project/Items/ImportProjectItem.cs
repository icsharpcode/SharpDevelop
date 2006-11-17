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
	/// Project item for default namespace import (e.g in VB)
	/// </summary>
	public sealed class ImportProjectItem : ProjectItem
	{
		public ImportProjectItem(IProject project, string include)
			: base(project, ItemType.Import, include)
		{
		}
		
		internal ImportProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
			: base(project, buildItem)
		{
		}
	}
}
