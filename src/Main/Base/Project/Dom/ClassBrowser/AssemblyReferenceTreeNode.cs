// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// Tree node representing one single referenced assembly.
	/// </summary>
	public class AssemblyReferenceTreeNode : ModelCollectionTreeNode
	{
		private IAssemblyReferenceModel model;
		
		public AssemblyReferenceTreeNode(IAssemblyReferenceModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.model = model;
			
			// To avoid the "+" sign in front of node...
			this.LazyLoading = false;
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		protected override IModelCollection<object> ModelChildren {
			get {
				// TODO Show assemblies referenced by this assembly?
				return ImmutableModelCollection<object>.Empty;
			}
		}

		protected override System.Collections.Generic.IComparer<ICSharpCode.TreeView.SharpTreeNode> NodeComparer {
			get {
				return NodeTextComparer;
			}
		}
		
		public override object Text {
			get {
				return model.AssemblyName.ShortName;
			}
		}
		
		public override object Icon {
			get {
				return SD.ResourceService.GetImageSource("Icons.16x16.Reference");
			}
		}
		
		public override void ShowContextMenu()
		{
			var assemblyReferenceModel = this.Model as IAssemblyReferenceModel;
			if (assemblyReferenceModel != null) {
				var ctx = MenuService.ShowContextMenu(null, assemblyReferenceModel, "/SharpDevelop/Pads/ClassBrowser/AssemblyReferenceContextMenu");
			}
		}
	}
}
