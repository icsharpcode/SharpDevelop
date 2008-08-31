// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceFolder : CustomFolderNode
	{
		IProject project;
		
		public ReferenceFolder(IProject project)
		{
			sortOrder = 0;
			
			this.project = project;

			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceFolderNode";
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					new CustomNode().AddTo(this);
					break;
				}
			}
		}
		
		public void ShowReferences()
		{
			Nodes.Clear();
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					ReferenceNode referenceNode = new ReferenceNode((ReferenceProjectItem)item);
					referenceNode.InsertSorted(this);
				}
			}
			UpdateIcon();
		}
		
		protected override void Initialize()
		{
			ShowReferences();
			base.Initialize();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
