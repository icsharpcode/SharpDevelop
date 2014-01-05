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
					if (baseTypeDef != null) {
						ITypeDefinitionModel baseTypeModel = baseTypeDef.GetModel();
						if (baseTypeModel != null)
							baseTypes.Add(baseTypeModel);
					}
				}
			}
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
