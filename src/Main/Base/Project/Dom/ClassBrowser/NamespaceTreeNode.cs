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
				return NodeTextComparer;
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
	}
}


