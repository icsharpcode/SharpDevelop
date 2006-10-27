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
	/// The parent node that contains WixLibrary project items.
	/// </summary>
	public class WixLibraryFolderNode : CustomFolderNode
	{
		IProject project;
		
		public WixLibraryFolderNode(IProject project)
		{
			this.project = project;
			Text = StringParser.Parse("${res:ICSharpCode.WixBinding.WixLibraryFolderNode.Text}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/WixLibraryFolderNode";
			
			foreach (ProjectItem item in project.Items) {
				if (item is WixLibraryProjectItem) {
					CustomNode node = new CustomNode();
					node.AddTo(this);
					break;
				}
			}
		}
		
		public override void Refresh()
		{
			AddLibraryNodes();
			base.Refresh();
		}
		
		protected override void Initialize()
		{
			AddLibraryNodes();
			base.Initialize();
		}
		
		void AddLibraryNodes()
		{
			Nodes.Clear();

			foreach (ProjectItem item in project.Items) {
				WixLibraryProjectItem wixLibraryItem = item as WixLibraryProjectItem;
				if (wixLibraryItem != null) {
					WixLibraryNode node = new WixLibraryNode(wixLibraryItem);
					node.AddTo(this);
				}
			}
		}
	}
}
