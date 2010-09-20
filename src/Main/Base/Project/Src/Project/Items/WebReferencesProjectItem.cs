// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class WebReferencesProjectItem : FileProjectItem
	{
		public WebReferencesProjectItem(IProject project) : base(project, ItemType.WebReferences)
		{
		}
		
		internal WebReferencesProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		[Browsable(false)]
		public string Directory {
			get {
				return Path.Combine(Project.Directory, Include).Trim('\\', '/');
			}
		}
	}
}
