using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class UpdateReferencesVisitor : ProjectBrowserTreeNodeVisitor
	{
		ProjectReferenceEventArgs e;
		
		public UpdateReferencesVisitor(ProjectReferenceEventArgs e)
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
