// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class UpdateReferencesVisitor : ProjectBrowserTreeNodeVisitor
	{
		ProjectItemEventArgs e;
		
		public UpdateReferencesVisitor(ProjectItemEventArgs e)
		{
			this.e = e;
		}
		
		public override object Visit(ReferenceFolder referenceFolder, object data)
		{
			if (referenceFolder.Project == e.Project) {
				referenceFolder.ShowReferences();
				referenceFolder.EnsureVisible();
				referenceFolder.Expand();
			}
			return data;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			return data;
		}
	}
}
