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
