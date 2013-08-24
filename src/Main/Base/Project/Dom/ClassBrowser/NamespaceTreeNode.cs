// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class NamespaceTreeNode : ModelCollectionTreeNode
	{
		static readonly IComparer<SharpTreeNode> TypeNodeComparer =  new TypeDefinitionNodeComparer();
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
			get {
				return TypeNodeComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return model.Types;
			}
		}
		
		public override object Icon {
			get {
				return ClassBrowserIconService.Namespace.ImageSource;
			}
		}
		
		public override object Text {
			get {
				return model.FullName;
			}
		}
		
		class TypeDefinitionNodeComparer : IComparer<SharpTreeNode>
		{
			public int Compare(SharpTreeNode x, SharpTreeNode y)
			{
				var a = x.Model as ITypeDefinitionModel;
				var b = y.Model as ITypeDefinitionModel;
				
				if (a == null && b == null)
					return NodeTextComparer.Compare(x, y);
				if (a == null)
					return -1;
				if (b == null)
					return 1;
				
				int typeNameComparison = StringComparer.OrdinalIgnoreCase.Compare(a.Name, b.Name);
				if (typeNameComparison == 0) {
					// Compare length of complete string (type parameters...)
					if (x.Text.ToString().Length < y.Text.ToString().Length)
						return -1;
					if (x.Text.ToString().Length < y.Text.ToString().Length)
						return 1;
				}
				
				return typeNameComparison;
			}
		}
	}
}


