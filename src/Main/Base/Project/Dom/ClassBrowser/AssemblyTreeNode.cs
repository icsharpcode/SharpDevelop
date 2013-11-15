// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core.Presentation;
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
		
		protected override void LoadChildren()
		{
			Children.Clear();
			if (model.Context.IsValid) {
				base.LoadChildren();
			} else {
				// This assembly could not be loaded correctly, add sub-node with error text
				Children.Add(new AssemblyLoadErrorTreeNode());
			}
		}
		
		public override object Text {
			get {
				return model.AssemblyName;
			}
		}
		
		public override object Icon {
			get {
				if (model.Context.IsValid) {
					if (model.IsUnpinned()) {
						return SD.ResourceService.GetImageSource("Icons.16x16.AssemblyUnpinned");
					} else {
						return SD.ResourceService.GetImageSource("Icons.16x16.Assembly");
					}
				} else {
					return SD.ResourceService.GetImageSource("Icons.16x16.AssemblyError");
				}
			}
		}
		
		public override void ShowContextMenu()
		{
			var assemblyModel = this.Model as IAssemblyModel;
			if (assemblyModel != null) {
				var ctx = MenuService.ShowContextMenu(null, assemblyModel, "/SharpDevelop/Pads/ClassBrowser/AssemblyContextMenu");
			}
		}
	}
}


