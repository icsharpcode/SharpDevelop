// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
	public class ClassBrowserTreeView : SharpTreeView
	{
		public ClassBrowserTreeView()
		{
			WorkspaceTreeNode root = new WorkspaceTreeNode();
			ClassBrowserTreeView instance = this;
			SD.ClassBrowser.CurrentWorkspace.AssemblyLists.CollectionChanged += delegate {
				instance.ShowRoot = SD.ClassBrowser.CurrentWorkspace.AssemblyLists.Count > 0;
			};
			root.PropertyChanged += delegate {
				instance.ShowRoot = SD.ClassBrowser.CurrentWorkspace.AssemblyLists.Count > 0;
			};
			this.Root = root;
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
		
		public bool GotoAssemblyModel(IAssemblyModel assemblyModel)
		{
			if (assemblyModel == null)
				throw new ArgumentNullException("assemblyModel");
			
			SharpTreeNode assemblyTreeNode = FindAssemblyTreeNode(assemblyModel.FullAssemblyName);
			if (assemblyTreeNode != null) {
				this.FocusNode(assemblyTreeNode);
				return true;
			}
			
			return false;
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
					IClassBrowser classBrowser = SD.GetService<IClassBrowser>();
					if (assemblyParser != null && classBrowser != null) {
						IAssemblyModel unpinnedAssemblyModel = assemblyParser.GetAssemblyModel(new FileName(entityAssembly.UnresolvedAssembly.Location));
						if (unpinnedAssemblyModel != null) {
							classBrowser.UnpinnedAssemblies.Assemblies.Add(unpinnedAssemblyModel);
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
						
						SharpTreeNode entityTypeNode = null;
						
						// Search in namespace node recursively
						var foundEntityNode = nsTreeNode.FindChildNodeRecursively(
							node => {
								var treeNode = node as SharpTreeNode;
								if (treeNode != null) {
									var treeNodeTypeModel = treeNode.Model as ITypeDefinitionModel;
									if (treeNodeTypeModel != null) {
										var modelFullTypeName = treeNodeTypeModel.FullTypeName;
										if (modelFullTypeName == entityType.FullTypeName) {
											// This is the TypeDefinitionModel of searched entity (the type itself or its member)
											entityTypeNode = treeNode;
											if (entity is ITypeDefinition) {
												// We are looking for the type itself
												return true;
											}
										}
									}
									
									if ((entity is IMember) && (treeNode.Model is IMemberModel)) {
										// Compare parent types and member names
										IMemberModel memberModel = (IMemberModel) treeNode.Model;
										IMember member = (IMember) entity;
										bool isSymbolOfTypeAndName =
											(member.DeclaringType.FullName == memberModel.UnresolvedMember.DeclaringTypeDefinition.FullName)
											&& (member.Name == memberModel.Name)
											&& (member.SymbolKind == memberModel.SymbolKind);
										
										if (isSymbolOfTypeAndName) {
											var parametrizedEntityMember = member as IParameterizedMember;
											var parametrizedTreeNodeMember = memberModel.UnresolvedMember as IUnresolvedParameterizedMember;
											if ((parametrizedEntityMember != null) && (parametrizedTreeNodeMember != null)) {
												// For methods and constructors additionally check the parameters and their types to handle overloading properly
												int treeNodeParamsCount = parametrizedTreeNodeMember.Parameters != null ? parametrizedTreeNodeMember.Parameters.Count : 0;
												int entityParamsCount = parametrizedEntityMember.Parameters != null ? parametrizedEntityMember.Parameters.Count : 0;
												if (treeNodeParamsCount == entityParamsCount) {
													for (int i = 0; i < entityParamsCount; i++) {
														// Compare full type names or at least only type names without namespace
														// This is not precise, but sufficient in most cases.
														var entityParamType = parametrizedEntityMember.Parameters[i].Type;
														var treeNodeParamType = parametrizedTreeNodeMember.Parameters[i].Type.Resolve(
															new SimpleTypeResolveContext(entityAssembly));
														if ((entityParamType.FullName != treeNodeParamType.FullName) &&
														    (entityParamType.Name != treeNodeParamType.Name)) {
															return false;
														}
													}
													
													// All parameters were equal
													return true;
												}
											} else {
												return true;
											}
										}
									}
								}
								
								return false;
							});
						
						// Special handling for default constructors: If not found, jump to type declaration instead
						if ((foundEntityNode == null) && (entity.SymbolKind == SymbolKind.Constructor)) {
							foundEntityNode = entityTypeNode;
						}
						
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
}
