// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class TypeDefinitionTreeNode : ModelCollectionTreeNode
	{
		static readonly IComparer<SharpTreeNode> TypeMemberNodeComparer =  new TypeDefinitionMemberNodeComparer();
		ITypeDefinitionModel definition;
		
		public TypeDefinitionTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.definition.Updated += (sender, e) => {
				UpdateBaseTypesNode();
				UpdateDerivedTypesNode();
			};
		}
		
		protected override object GetModel()
		{
			return definition;
		}
		
		public override object Icon {
			// TODO why do I have to resolve this?
			get {
				return ClassBrowserIconService.GetIcon(definition.Resolve()).ImageSource;
			}
		}
		
		public override object Text {
			get {
				return GenerateNodeText();
			}
		}
		
		protected override IComparer<SharpTreeNode> NodeComparer {
			get {
				return TypeMemberNodeComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return definition.NestedTypes.Concat<object>(definition.Members);
			}
		}
		
		protected override bool IsSpecialNode()
		{
			return true;
		}
		
		protected override void InsertSpecialNodes()
		{
			// Since following both methods set their entries to the top, "Base types" must come last to get them on top
			UpdateDerivedTypesNode();
			UpdateBaseTypesNode();
		}
		
		static readonly FullTypeName SystemObjectName = new FullTypeName("System.Object");
		
		void UpdateBaseTypesNode()
		{
			this.Children.RemoveAll(n => n is BaseTypesTreeNode);
			if (definition.FullTypeName != SystemObjectName) {
				Children.Insert(0, new BaseTypesTreeNode(definition));
			}
		}
		
		void UpdateDerivedTypesNode()
		{
			this.Children.RemoveAll(n => n is DerivedTypesTreeNode);
			if (!definition.IsSealed) {
				Children.Insert(0, new DerivedTypesTreeNode(definition));
			}
		}
		
		public override void ActivateItem(System.Windows.RoutedEventArgs e)
		{
			var target = definition.Resolve();
			if (target != null)
				NavigationService.NavigateTo(target);
		}
		
		public override void ShowContextMenu()
		{
			var entityModel = this.Model as IEntityModel;
			if (entityModel != null) {
				var ctx = MenuService.ShowContextMenu(null, entityModel, "/SharpDevelop/EntityContextMenu");
			}
		}
		
		string GenerateNodeText()
		{
			var target = definition.Resolve();
			if (target != null) {
				return GetTypeName(target);
			}
			return definition.Name;
		}
		
		string GetTypeName(IType type)
		{
			StringBuilder typeName = new StringBuilder(type.Name);
			if (type.TypeParameterCount > 0) {
				typeName.Append("<");
				bool isFirst = true;
				foreach (var typeParameter in type.TypeArguments) {
					if (!isFirst)
						typeName.Append(",");
					typeName.Append(GetTypeName(typeParameter));
					isFirst = false;
				}
				typeName.Append(">");
			}
			return typeName.ToString();
		}
		
		class TypeDefinitionMemberNodeComparer : IComparer<SharpTreeNode>
		{
			public int Compare(SharpTreeNode x, SharpTreeNode y)
			{
				var a = x.Model as IMemberModel;
				var b = y.Model as IMemberModel;
				
				if (a == null && b == null)
					return NodeTextComparer.Compare(x, y);
				if (a == null)
					return -1;
				if (b == null)
					return 1;
				
				if (a.SymbolKind < b.SymbolKind)
					return -1;
				if (a.SymbolKind > b.SymbolKind)
					return 1;
				
				return NodeTextComparer.Compare(x, y);
			}
		}
	}
}


