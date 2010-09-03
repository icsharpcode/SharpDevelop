// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: trecio
 * Data: 2009-06-30
 * Godzina: 18:52
 * 
 */
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// ProjectConfiguration item that occur in the vcxproj files (c++ projects)
	/// </summary>
	public class ProjectConfigurationProjectItem : ProjectItem
	{
		public ProjectConfigurationProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		/// <summary>
		/// Returns an empty string as a filename. 
		/// Project configuration is specific to the whole project, not a specific item.
		/// </summary>
		public override string FileName
		{
			get
			{
				return "";
			}
		}
	}
}
