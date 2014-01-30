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

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceFolder : CustomFolderNode
	{
		IProject project;
		
		public ReferenceFolder(IProject project)
		{
			sortOrder = 0;
			
			this.project = project;

			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceFolderNode";
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.ReferencesNodeText}");
			OpenedImage = "ProjectBrowser.ReferenceFolder.Open";
			ClosedImage = "ProjectBrowser.ReferenceFolder.Closed";
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					new CustomNode().AddTo(this);
					break;
				}
			}
		}
		
		public void ShowReferences()
		{
			Nodes.Clear();
			
			foreach (ProjectItem item in project.Items) {
				if (item is ReferenceProjectItem) {
					ReferenceNode referenceNode = new ReferenceNode((ReferenceProjectItem)item);
					referenceNode.InsertSorted(this);
				}
			}
			UpdateIcon();
		}
		
		protected override void Initialize()
		{
			ShowReferences();
			base.Initialize();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
