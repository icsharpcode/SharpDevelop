// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void NodeInitializer(CustomNode node);
	
	public class CustomNode : AbstractProjectBrowserTreeNode
	{
		NodeInitializer nodeInitializer = null;
		
		public NodeInitializer NodeInitializer {
			get {
				return nodeInitializer;
			}
			set {
				nodeInitializer = value;
			}
		}
		
		public CustomNode()
		{
		}
		
		protected override void Initialize()
		{
			if (nodeInitializer != null) {
				nodeInitializer(this);
			}
			base.Initialize();
		}
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
	}
}
