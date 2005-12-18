// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
