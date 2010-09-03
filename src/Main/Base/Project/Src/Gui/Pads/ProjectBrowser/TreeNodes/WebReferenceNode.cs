// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceNode : DirectoryNode
	{
		public WebReferenceNode(WebReference webReference) : this(webReference.Directory)
		{
			ProjectItem = webReference.WebReferenceUrl;
		}
		
		public WebReferenceNode(string directory) : base(directory)
		{
			SpecialFolder = SpecialFolder.WebReference;
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WebReferenceNode";
		}
	}
}
