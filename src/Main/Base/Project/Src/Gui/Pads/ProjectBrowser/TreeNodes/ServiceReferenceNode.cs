// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace Gui.Pads.ProjectBrowser.TreeNodes
{
	public class ServiceReferenceNode : DirectoryNode
	{
		public ServiceReferenceNode(string directory)
			: base(directory)
		{
			this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ServiceReferenceNode";
			this.SpecialFolder = SpecialFolder.ServiceReference;
		}
	}
}
