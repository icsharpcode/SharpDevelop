// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of WorkspaceTreeNode.
	/// </summary>
	public class WorkspaceTreeNode : ModelCollectionTreeNode
	{
		ClassBrowserWorkspace workspace;
		
		public WorkspaceTreeNode(ClassBrowserWorkspace workspace)
		{
			if (workspace == null)
				throw new ArgumentNullException("workspace");
			this.workspace = workspace;
		}
		
		protected override object GetModel()
		{
			return workspace;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return workspace.LoadedAssemblies; }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		public override object Text {
			get {
				return "Workspace " + workspace.Name;
			}
		}
		
		protected override bool IsSpecialNode()
		{
			return true;
		}
		
		protected override void InsertSpecialNodes()
		{
			if (workspace.IsAssigned) {
				InsertChildren(new[] { workspace.AssignedSolution });
			}
		}
	}
	
	public class SolutionTreeNode : ModelCollectionTreeNode
	{
		ISolution solution;
		
		public SolutionTreeNode(ISolution solution)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			this.solution = solution;
		}
		
		protected override object GetModel()
		{
			return solution;
		}
		
		public override object Text {
			get { return "Solution " + solution.Name; }
		}
		
		public override object Icon {
			get { return IconService.GetImageSource("Icons.16x16.SolutionIcon"); }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return solution.Projects; }
		}
	}
	
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
			get { return project.Name; }
		}
		
		public override object Icon {
			get { return IconService.GetImageSource(IconService.GetImageForProjectType(project.Language)); }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return project.AssemblyModel.Namespaces; }
		}
	}
	
	public class AssemblyTreeNode : ModelCollectionTreeNode
	{
		IAssemblyModel model;
		
		public AssemblyTreeNode(IAssemblyModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.model = model;
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return model.Namespaces; }
		}
	}
	
	public class NamespaceTreeNode : ModelCollectionTreeNode
	{
		INamespaceModel model;
		
		public NamespaceTreeNode(INamespaceModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.model = model;
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return model.Types; }
		}
		
		public override object Icon {
			get { return ClassBrowserIconService.Namespace.ImageSource; }
		}
		
		public override object Text {
			get { return model.FullName; }
		}
	}
	
	public class TypeDefinitionTreeNode : ModelCollectionTreeNode
	{
		ITypeDefinitionModel definition;
		
		public TypeDefinitionTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
		}
		
		protected override object GetModel()
		{
			return definition;
		}
		
		public override object Icon {
			// TODO why do I have to resolve this?
			get { return ClassBrowserIconService.GetIcon(definition.Resolve()).ImageSource; }
		}
		
		public override object Text {
			get { return definition.Name; }
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get { return NodeTextComparer; }
		}
		
		protected override IModelCollection<object> ModelChildren {
			get { return definition.Members; }
		}
	}
}
