// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// The parent node that contains WixExtension project items.
	/// </summary>
	public class WixExtensionFolderNode : CustomFolderNode
	{
		IProject project;
		
		public WixExtensionFolderNode(IProject project)
		{
			this.project = project;
			Text = StringParser.Parse("${res:ICSharpCode.WixBinding.WixExtensionsFolderNode.Text}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixExtensionFolderNode";
			
			foreach (ProjectItem item in project.Items) {
				if (item is WixExtensionProjectItem) {
					CustomNode node = new CustomNode();
					node.AddTo(this);
					break;
				}
			}
		}
		
		public override void Refresh()
		{
			AddExtensionNodes();
			base.Refresh();
		}
		
		protected override void Initialize()
		{
			AddExtensionNodes();
			base.Initialize();
		}
		
		void AddExtensionNodes()
		{
			Nodes.Clear();

			foreach (ProjectItem item in project.Items) {
				WixExtensionProjectItem extensionItem = item as WixExtensionProjectItem;
				if (extensionItem != null) {
					WixExtensionNode node = new WixExtensionNode(extensionItem);
					node.AddTo(this);
				}
			}
		}
	}
}
