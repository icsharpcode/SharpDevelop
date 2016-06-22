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

namespace ICSharpCode.SharpDevelop.Project
{
	public class CustomFolderNode : AbstractProjectBrowserTreeNode
	{
		string closedImage = null;
		string openedImage = null;
		
		public string ClosedImage {
			get {
				return closedImage;
			}
			set {
				closedImage = value;
				if (!IsExpanded) {
					SetIcon(closedImage);
				}
				
			}
		}
		
		public string OpenedImage {
			get {
				return openedImage;
			}
			set {
				openedImage = value;
				if (IsExpanded) {
					SetIcon(openedImage);
				}
			}
		}

		public CustomFolderNode()
		{
		}
		
		protected void UpdateIcon()
		{
			if (Nodes.Count == 0) {
				SetIcon(ClosedImage);
			} else if (IsExpanded) {
				SetIcon(openedImage);
			}
		}
		
		public override void Refresh() 
		{
			base.Refresh();
			UpdateIcon();
		}
		
		public override void Expanding()
		{
			if (openedImage != null) {
				SetIcon(openedImage);
			}
			base.Expanding();
			if (Nodes.Count == 0) {
				SetIcon(ClosedImage);
			}
		}
		
		public override void Collapsing()
		{
			if (closedImage != null) {
				SetIcon(closedImage);
			}
			base.Collapsing();
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
