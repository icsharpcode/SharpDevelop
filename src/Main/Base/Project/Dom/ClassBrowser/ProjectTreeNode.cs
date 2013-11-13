// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ProjectTreeNode : ModelCollectionTreeNode
	{
		IProject project;
		
		public ProjectTreeNode(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
		}
		
		protected override object GetModel()
		{
			return project;
		}
		
		public override object Text {
			get {
				return project.Name;
			}
		}
		
		public override object Icon {
			get {
				return IconService.GetImageSource(IconService.GetImageForProjectType(project.Language));
			}
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return project.AssemblyModel.Namespaces;
			}
		}
		
		protected override bool IsSpecialNode()
		{
			return true;
		}
		
		protected override void InsertSpecialNodes()
		{
			UpdateReferencesNode();
		}
		
		void UpdateReferencesNode()
		{
			this.Children.RemoveAll(n => n is AssemblyReferencesTreeNode);
			var refsTreeNode = new AssemblyReferencesTreeNode(project.AssemblyModel);
			Children.Insert(0, refsTreeNode);
		}
	}
}


