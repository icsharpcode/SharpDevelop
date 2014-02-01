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
		IModelCollection<object> combinedModelChildren;
		
		public TypeDefinitionTreeNode(ITypeDefinitionModel definition)
		{
			if (definition == null)
				throw new ArgumentNullException("definition");
			this.definition = definition;
			this.definition.Updated += (sender, e) => {
				UpdateBaseTypesNode();
				UpdateDerivedTypesNode();
			};
			this.combinedModelChildren = definition.NestedTypes.Concat<object>(definition.Members);
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
				return combinedModelChildren;
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
				Children.OrderedInsert(new BaseTypesTreeNode(definition), TypeMemberNodeComparer);
			}
		}
		
		void UpdateDerivedTypesNode()
		{
			this.Children.RemoveAll(n => n is DerivedTypesTreeNode);
			if (!definition.IsSealed) {
				Children.OrderedInsert(new DerivedTypesTreeNode(definition), TypeMemberNodeComparer);
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
				// "Base types" and "Derive types" nodes have precedence over other nodes
				if ((x is BaseTypesTreeNode) && !(y is BaseTypesTreeNode))
					return -1;
				if (!(x is BaseTypesTreeNode) && (y is BaseTypesTreeNode))
					return 1;
				if ((x is DerivedTypesTreeNode) && !(y is DerivedTypesTreeNode))
					return -1;
				if (!(x is DerivedTypesTreeNode) && (y is DerivedTypesTreeNode))
					return 1;
				
				var a = x.Model as IMemberModel;
				var b = y.Model as IMemberModel;
				
				if ((a == null) && (b != null))
					return -1;
				if ((a != null) && (b == null))
					return 1;
				if ((a != null) && (b != null)) {
					if (a.SymbolKind < b.SymbolKind)
						return -1;
					if (a.SymbolKind > b.SymbolKind)
						return 1;
				}
				
				return NodeTextComparer.Compare(x, y);
			}
		}
	}
}
