// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public class NamespaceNode : AbstractProjectBrowserTreeNode
	{
		public NamespaceNode()
		{
			SetIcon("Icons.16x16.NameSpace");
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			throw new NotImplementedException();
		}
	}
}