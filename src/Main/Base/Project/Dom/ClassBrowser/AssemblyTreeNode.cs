// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.TreeView;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	public class AssemblyTreeNode : ModelCollectionTreeNode
	{
		IAssemblyModel model;
		
		public AssemblyTreeNode(IAssemblyModel model)
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
				return NodeTextComparer;
			}
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				return model.Namespaces;
			}
		}
	}
}


