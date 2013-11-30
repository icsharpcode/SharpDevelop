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
		class ProjectChildComparer : IComparer<SharpTreeNode>
		{
			IComparer<string> stringComparer = StringComparer.OrdinalIgnoreCase;
			
			public int Compare(SharpTreeNode x, SharpTreeNode y)
			{
				// "References" node has precedence over other nodes
				if ((x is AssemblyReferencesTreeNode) && !(y is AssemblyReferencesTreeNode))
					return -1;
				if (!(x is AssemblyReferencesTreeNode) && (y is AssemblyReferencesTreeNode))
					return 1;
				
				// All other nodes are compared by their Text property
				return stringComparer.Compare(x.Text.ToString(), y.Text.ToString());
			}
		}
		
		protected static readonly IComparer<SharpTreeNode> ChildNodeComparer = new ProjectChildComparer();
		
		IProject project;
		IAssemblyReferencesModel assemblyReferencesModel;
		
		public ProjectTreeNode(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			assemblyReferencesModel = project.AssemblyModel.References;
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
				return ChildNodeComparer;
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
			var treeNode = SD.TreeNodeFactory.CreateTreeNode(assemblyReferencesModel);
			if (treeNode != null)
				Children.OrderedInsert(treeNode, ChildNodeComparer);
		}
	}
}


