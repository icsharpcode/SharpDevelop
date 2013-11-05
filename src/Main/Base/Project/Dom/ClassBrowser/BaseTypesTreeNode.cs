// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

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
					if (baseTypeDef != null) {
						ITypeDefinitionModel baseTypeModel = GetTypeDefinitionModel(baseTypeDef);
						if (baseTypeModel != null)
							baseTypes.Add(baseTypeModel);
					}
				}
			}
		}
		
		ITypeDefinitionModel GetTypeDefinitionModel(ITypeDefinition definition)
		{
			ITypeDefinitionModel model = definition.GetModel();
			if (model == null) {
				// Try to get model from ClassBrowser's assembly list
				var classBrowser = SD.GetService<IClassBrowser>();
				if (classBrowser != null) {
					foreach (var assemblyModel in classBrowser.MainAssemblyList.Assemblies) {
						model = assemblyModel.TopLevelTypeDefinitions[definition.FullTypeName];
						if (model != null) {
							return model;
						}
					}
				}
			}
			
			return model;
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
