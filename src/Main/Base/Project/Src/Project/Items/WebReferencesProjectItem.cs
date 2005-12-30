// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferencesProjectItem : FileProjectItem
	{
		public WebReferencesProjectItem(IProject project) : base(project, ItemType.WebReferences)
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
