// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ServiceReferencesProjectItem : FileProjectItem
	{
		public ServiceReferencesProjectItem(IProject project)
			: base(project, ItemType.ServiceReferences)
		{
		}
		
		internal ServiceReferencesProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		[Browsable(false)]
		public string Directory {
			get { return Path.Combine(Project.Directory, Include).Trim('\\'); }
		}
		
		public static bool IsServiceReferencesFolder(IProject project, string folder)
		{
			foreach (ProjectItem item in project.GetItemsOfType(ItemType.ServiceReferences)) {
				var serviceReferenceProjectItem = (ServiceReferencesProjectItem)item;
				if (serviceReferenceProjectItem.IsDirectoryMatch(folder)) {
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
