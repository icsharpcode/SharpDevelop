// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
