// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Dom.ClassBrowser
{
	/// <summary>
	/// "References" tree node.
	/// </summary>
	public class AssemblyReferencesTreeNode : ModelCollectionTreeNode
	{
		private IAssemblyReferencesModel model;
		private string text;
		
		public AssemblyReferencesTreeNode(IAssemblyReferencesModel referencesModel)
		{
			if (referencesModel == null)
				throw new ArgumentNullException("referencesModel");
			this.model = referencesModel;
			this.text = SD.ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText");
		}
		
		protected override object GetModel()
		{
			return model;
		}
		
		protected override IModelCollection<object> ModelChildren
		{
			get {
				return model.AssemblyNames;
			}
		}
		
		protected override System.Collections.Generic.IComparer<ICSharpCode.TreeView.SharpTreeNode> NodeComparer
		{
			get {
				return NodeTextComparer;
			}
		}
		
		public override object Text
		{
			get {
				return text;
			}
		}
		
		public override object Icon
		{
			get {
				return SD.ResourceService.GetImageSource("ProjectBrowser.ReferenceFolder.Closed");
			}
		}
		
		public override void ShowContextMenu()
		{
			var assemblyReferencesModel = this.Model as IAssemblyReferencesModel;
			if (assemblyReferencesModel != null) {
				var ctx = MenuService.ShowContextMenu(null, assemblyReferencesModel, "/SharpDevelop/Pads/ClassBrowser/AssemblyReferencesContextMenu");
			}
		}
	}
}
