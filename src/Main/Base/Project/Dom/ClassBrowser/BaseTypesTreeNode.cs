// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Description of BaseTypesTreeNode.
	/// </summary>
	public class BaseTypesTreeNode : ModelCollectionTreeNode
	{
		ITypeDefinitionModel definition;
		string text;
		SimpleModelCollection<ITypeDefinitionModel> baseTypes;
		
		public BaseTypesTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.definition.Updated += (sender, e) => UpdateBaseTypes();
			this.text = SD.ResourceService.GetString("MainWindow.Windows.ClassBrowser.BaseTypes");
			baseTypes = new SimpleModelCollection<ITypeDefinitionModel>();
			UpdateBaseTypes();
		}

		protected override IModelCollection<object> ModelChildren {
			get {
				return baseTypes;
			}
		}
		
		public bool HasBaseTypes()
		{
			return baseTypes.Count > 0;
		}
		
		void UpdateBaseTypes()
		{
			baseTypes.Clear();
			ITypeDefinition currentTypeDef = definition.Resolve();
			if (currentTypeDef != null) {
				foreach (var baseType in currentTypeDef.DirectBaseTypes) {
					ITypeDefinition baseTypeDef = baseType.GetDefinition();
					if ((baseTypeDef != null) && (baseTypeDef.FullName != "System.Object")) {
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
			var assemblyParserService = SD.GetService<IAssemblyParserService>();
			if (assemblyParserService != null) {
				var assemblyFileName = mainTypeDefinition.ParentAssembly.GetRuntimeAssemblyLocation();
				if (assemblyFileName != null) {
					try {
						// Look in the type's AssemblyModel
						var assemblyModel = assemblyParserService.GetAssemblyModel(assemblyFileName);
						resolveTypeDefModel = assemblyModel.TopLevelTypeDefinitions[baseTypeDefinition.FullTypeName];
						if (resolveTypeDefModel != null) {
							return resolveTypeDefModel;
						}
						
						if (assemblyModel.References != null) {
							foreach (var referencedAssemblyName in assemblyModel.References) {
								DefaultAssemblySearcher searcher = new DefaultAssemblySearcher(assemblyModel.Location);
								var resolvedFile = searcher.FindAssembly(referencedAssemblyName);
								var referenceAssemblyModel = assemblyParserService.GetAssemblyModel(resolvedFile);
								resolveTypeDefModel = referenceAssemblyModel.TopLevelTypeDefinitions[baseTypeDefinition.FullTypeName];
								if (resolveTypeDefModel != null) {
									return resolveTypeDefModel;
								}
							}
						}
					} catch (Exception) {
						// TODO Can't load the type, what to do?
					}
				}
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
