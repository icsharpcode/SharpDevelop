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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : AbstractProjectBrowserTreeNode
	{
		ReferenceProjectItem referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return referenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			this.referenceProjectItem = referenceProjectItem;
			Tag = referenceProjectItem;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
			SetIcon("Icons.16x16.Reference");
			Text = referenceProjectItem.ShortName;
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return !Project.IsReadOnly;
			}
		}
		
		public override void Delete()
		{
			IProject project = Project;
			ProjectService.RemoveProjectItem(referenceProjectItem.Project, referenceProjectItem);
			Debug.Assert(Parent != null);
			Debug.Assert(Parent is ReferenceFolder);
			((ReferenceFolder)Parent).ShowReferences();
			project.Save();
		}
		#endregion
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
