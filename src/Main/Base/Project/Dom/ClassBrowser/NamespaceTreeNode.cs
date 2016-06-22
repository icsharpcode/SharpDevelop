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
