// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
