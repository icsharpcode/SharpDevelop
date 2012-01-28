// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class WebReferencesProjectItem : FileProjectItem
	{
		public WebReferencesProjectItem(IProject project)
			: base(project, ItemType.WebReferences)
		{
		}
		
		internal WebReferencesProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		[Browsable(false)]
		public string Directory {
			get { return Path.Combine(Project.Directory, Include).Trim('\\', '/'); }
		}
		
		/// <summary>
		/// Determines if the specified <paramref name="folder"/> is a
		/// web reference folder in the specified <paramref name="project"/>.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <param name="folder">The full folder path.</param>
		public static bool IsWebReferencesFolder(IProject project, string folder)
		{
			foreach (ProjectItem item in project.GetItemsOfType(ItemType.WebReferences)) {
				var webReference = item as WebReferencesProjectItem;
				if (webReference.IsDirectoryMatch(folder)) {
					return true;
				}
			}
			return false;
		}
		
		public bool IsDirectoryMatch(string directory)
		{
			return FileUtility.IsEqualFileName(this.Directory, directory);
		}
	}
}
