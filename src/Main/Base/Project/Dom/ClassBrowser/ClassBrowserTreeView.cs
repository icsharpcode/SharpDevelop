// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class ClassBrowserTreeView : SharpTreeView, IClassBrowserTreeView
	{
		#region IClassBrowser implementation
		
		WorkspaceModel workspace;

		public ICollection<IAssemblyList> AssemblyLists {
			get { return workspace.AssemblyLists; }
		}

		public IAssemblyList MainAssemblyList {
			get { return workspace.MainAssemblyList; }
			set { workspace.MainAssemblyList = value; }
		}
		
		public IAssemblyList UnpinnedAssemblies {
			get { return workspace.UnpinnedAssemblies; }
			set { workspace.UnpinnedAssemblies = value; }
		}
		
		public IAssemblyModel FindAssemblyModel(FileName fileName)
		{
			return workspace.FindAssemblyModel(fileName);
		}

		#endregion
		
		public ClassBrowserTreeView()
		{
			WorkspaceTreeNode root = new WorkspaceTreeNode();
			this.workspace = root.Workspace;
			ClassBrowserTreeView instance = this;
			root.Workspace.AssemblyLists.CollectionChanged += delegate {
				instance.ShowRoot = root.Workspace.AssemblyLists.Count > 0;
			};
			root.PropertyChanged += delegate {
				instance.ShowRoot = root.Workspace.AssemblyLists.Count > 0;
			};
			this.Root = root;
		}
		
		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			var treeNode = this.SelectedItem as ModelCollectionTreeNode;
			if (treeNode != null) {
				treeNode.ShowContextMenu();
			}
		}
		
		private SharpTreeNode FindAssemblyTreeNode(string fullAssemblyName)
		{
			var assemblyTreeNode = this.Root.Children.FirstOrDefault(
				node => {
					if (node is AssemblyTreeNode) {
						var asmTreeNode = (AssemblyTreeNode) node;
						if (node.Model is IAssemblyModel) {
							var asmModel = (IAssemblyModel) node.Model;
							return asmModel.FullAssemblyName == fullAssemblyName;
						}
					}
					
					return false;
				});
			
			return assemblyTreeNode;
		}
		
		public bool GoToEntity(IEntity entity)
		{
			// Try to find assembly in workspace
			var entityAssembly = entity.ParentAssembly;
			if (entityAssembly != null) {
				ITypeDefinition entityType = null;
				if (entity is ITypeDefinition) {
					entityType = (ITypeDefinition) entity;
				} else {
					entityType = entity.DeclaringTypeDefinition;
				}
				
				SharpTreeNodeCollection namespaceChildren = null;
				var root = this.Root as WorkspaceTreeNode;
				
				// Try to find assembly of passed entity among open projects in solution
				var solutionTreeNode = this.Root.Children.OfType<SolutionTreeNode>().FirstOrDefault();
				if (solutionTreeNode != null) {
					// Ensure that we have children
					solutionTreeNode.EnsureLazyChildren();
					
					var projectTreeNode = solutionTreeNode.Children.FirstOrDefault(
						node => {
							if (node is ProjectTreeNode) {
								var treeNode = (ProjectTreeNode) node;
								if (node.Model is IProject) {
									var projectModel = (IProject) node.Model;
									return projectModel.AssemblyModel.FullAssemblyName == entityAssembly.FullAssemblyName;
								}
							}
							
							return false;
						});
					if (projectTreeNode != null) {
						projectTreeNode.EnsureLazyChildren();
						namespaceChildren = projectTreeNode.Children;
					}
				}
				
				if (namespaceChildren == null) {
					// Try to find assembly of passed entity among additional assemblies
					var assemblyTreeNode = FindAssemblyTreeNode(entityAssembly.FullAssemblyName);
					if (assemblyTreeNode != null) {
						assemblyTreeNode.EnsureLazyChildren();
						namespaceChildren = assemblyTreeNode.Children;
					}
				}
				
				if (namespaceChildren == null) {
					// Add assembly to workspace (unpinned), if not available in ClassBrowser
					IAssemblyParserService assemblyParser = SD.GetService<IAssemblyParserService>();
					if (assemblyParser != null) {
						IAssemblyModel unpinnedAssemblyModel = assemblyParser.GetAssemblyModel(new FileName(entityAssembly.UnresolvedAssembly.Location));
						if (unpinnedAssemblyModel != null) {
							this.UnpinnedAssemblies.Assemblies.Add(unpinnedAssemblyModel);
							var assemblyTreeNode = FindAssemblyTreeNode(entityAssembly.FullAssemblyName);
							if (assemblyTreeNode != null) {
								assemblyTreeNode.EnsureLazyChildren();
								namespaceChildren = assemblyTreeNode.Children;
							}
						}
					}
				}
				
				if (namespaceChildren != null) {
					var nsTreeNode = namespaceChildren.FirstOrDefault(
						node =>
						(node is NamespaceTreeNode)
						&& (((NamespaceTreeNode) node).Model is INamespaceModel)
						&& (((INamespaceModel) ((NamespaceTreeNode) node).Model).FullName == entityType.Namespace)
					) as ModelCollectionTreeNode;
					
					if (nsTreeNode != null) {
						// Ensure that we have children
						nsTreeNode.EnsureLazyChildren();
						
						// Search in namespace node recursively
						var foundEntityNode = nsTreeNode.FindChildNodeRecursively(
							node => {
								var treeNode = node as ModelCollectionTreeNode;
								if (treeNode != null) {
									if ((entity is ITypeDefinition) && (treeNode.Model is ITypeDefinitionModel)) {
										// Compare directly with type
										var modelFullTypeName = ((ITypeDefinitionModel) treeNode.Model).FullTypeName;
										return modelFullTypeName == entityType.FullTypeName;
									}
									if ((entity is IMember) && (treeNode.Model is IMemberModel)) {
										// Compare parent types and member names
										IMemberModel memberModel = (IMemberModel) treeNode.Model;
										IMember member = (IMember) entity;
										return (member.DeclaringType.FullName == entityType.FullName)
											&& (member.Name == memberModel.Name)
											&& (member.SymbolKind == memberModel.SymbolKind);
									}
								}
								
								return false;
							});
						
						if (foundEntityNode != null) {
							this.FocusNode(foundEntityNode);
							this.SelectedItem = foundEntityNode;
							return true;
						}
					}
				}
			}
			
			return false;
		}
	}

	public interface IClassBrowserTreeView : IClassBrowser
	{
		
	}
}