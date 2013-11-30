// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Represents the "Base types" sub-node of type nodes in ClassBrowser tree.
	/// </summary>
	public class BaseTypesTreeNode : ModelCollectionTreeNode
	{
		ITypeDefinitionModel definition;
		string text;
		bool childrenLoaded;
		SimpleModelCollection<ITypeDefinitionModel> baseTypes;
		
		public BaseTypesTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.definition.Updated += (sender, e) => UpdateBaseTypes();
			this.text = SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.BaseTypes");
			baseTypes = new SimpleModelCollection<ITypeDefinitionModel>();
			childrenLoaded = false;
		}

		protected override IModelCollection<object> ModelChildren {
			get {
				if (!childrenLoaded) {
					UpdateBaseTypes();
					childrenLoaded = true;
				}
				return baseTypes;
			}
		}
		
		public override SharpTreeNode FindChildNodeRecursively(Func<SharpTreeNode, bool> predicate)
		{
			// Don't search children of this node, because they are repeating type nodes from elsewhere
			return null;
		}
		
		public override bool CanFindChildNodeRecursively {
			get { return false; }
		}
		
		void UpdateBaseTypes()
		{
			baseTypes.Clear();
			ITypeDefinition currentTypeDef = definition.Resolve();
			if (currentTypeDef != null) {
				foreach (var baseType in currentTypeDef.DirectBaseTypes) {
					ITypeDefinition baseTypeDef = baseType.GetDefinition();
					if ((baseTypeDef != null)) {
						ITypeDefinitionModel baseTypeModel = GetTypeDefinitionModel(currentTypeDef, baseTypeDef);
						if (baseTypeModel != null)
							baseTypes.Add(baseTypeModel);
					}
				}
			}
		}
		
		ITypeDefinitionModel GetTypeDefinitionModel(ITypeDefinition mainTypeDefinition, ITypeDefinition baseTypeDefinition)
		{
			ITypeDefinitionModel resolveTypeDefModel = null;
			var assemblyFileName = mainTypeDefinition.ParentAssembly.GetRuntimeAssemblyLocation();
			IAssemblyModel assemblyModel = null;
			
			try {
				// Try to get AssemblyModel from project list
				IProjectService projectService = SD.GetRequiredService<IProjectService>();
				if (projectService.CurrentSolution != null) {
					var projectOfAssembly = projectService.CurrentSolution.Projects.FirstOrDefault(p => p.AssemblyModel.Location == assemblyFileName);
					if (projectOfAssembly != null) {
						// We automatically have an AssemblyModel from project
						assemblyModel = projectOfAssembly.AssemblyModel;
					}
				}
				
				var assemblyParserService = SD.GetService<IAssemblyParserService>();
				if (assemblyModel == null) {
					if (assemblyParserService != null) {
						if (assemblyFileName != null) {
							assemblyModel = assemblyParserService.GetAssemblyModel(assemblyFileName);
						}
					}
				}
				
				if (assemblyModel != null) {
					// Nothing in projects, load from assembly file
					resolveTypeDefModel = assemblyModel.TopLevelTypeDefinitions[baseTypeDefinition.FullTypeName];
					if (resolveTypeDefModel != null) {
						return resolveTypeDefModel;
					}
					
					// Look at referenced assemblies
					if ((assemblyModel.References != null) && (assemblyParserService != null)) {
						foreach (var referencedAssemblyName in assemblyModel.References.AssemblyNames) {
							DefaultAssemblySearcher searcher = new DefaultAssemblySearcher(assemblyModel.Location);
							var resolvedFile = searcher.FindAssembly(referencedAssemblyName.AssemblyName);
							var referenceAssemblyModel = assemblyParserService.GetAssemblyModel(resolvedFile);
							resolveTypeDefModel = referenceAssemblyModel.TopLevelTypeDefinitions[baseTypeDefinition.FullTypeName];
							if (resolveTypeDefModel != null) {
								return resolveTypeDefModel;
							}
						}
					}
				}
			} catch (Exception) {
				// TODO Can't load the type, what to do?
			}
			
			return resolveTypeDefModel;
		}
		
		protected override System.Collections.Generic.IComparer<ICSharpCode.TreeView.SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		public override object Text {
			get {
				return text;
			}
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("Icons.16x16.OpenFolderBitmap");
			}
		}
	}
}
