// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public abstract class MemberNode : AbstractProjectBrowserTreeNode
	{
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			throw new NotImplementedException();
		}
	}
	
	#region Field nodes
	public class PublicFieldNode : MemberNode
	{
		public PublicFieldNode()
		{
			SetIcon("Icons.16x16.Field");
		}
	}
	
	public class InternalFieldNode : MemberNode
	{
		public InternalFieldNode()
		{
			SetIcon("Icons.16x16.InteralField");
		}
	}
	
	public class ProtectedFieldNode : MemberNode
	{
		public ProtectedFieldNode()
		{
			SetIcon("Icons.16x16.ProtectedField");
		}
	}
	
	public class PrivateFieldNode : MemberNode
	{
		public PrivateFieldNode()
		{
			SetIcon("Icons.16x16.PrivateField");
		}
	}
	#endregion
	
	#region Properties nodes
	public class PublicPropertyNode : MemberNode
	{
		public PublicPropertyNode()
		{
			SetIcon("Icons.16x16.Property");
		}
	}
	
	public class InternalPropertyNode : MemberNode
	{
		public InternalPropertyNode()
		{
			SetIcon("Icons.16x16.InteralProperty");
		}
	}
	
	public class ProtectedPropertyNode : MemberNode
	{
		public ProtectedPropertyNode()
		{
			SetIcon("Icons.16x16.ProtectedProperty");
		}
	}
	
	public class PrivatePropertyNode : MemberNode
	{
		public PrivatePropertyNode()
		{
			SetIcon("Icons.16x16.PrivateProperty");
		}
	}
	#endregion
	
	#region Method nodes
	
	public class PublicMethodNode : MemberNode
	{	
		public PublicMethodNode()
		{
			SetIcon("Icons.16x16.Method");
		}
	}
		
	public class InternalMethodNode : MemberNode
	{	
		public InternalMethodNode()
		{
			SetIcon("Icons.16x16.InternalMethod");
		}
	}
	
	public class ProtectedMethodNode : MemberNode
	{	
		public ProtectedMethodNode()
		{
			SetIcon("Icons.16x16.ProtectedMethod");
		}
	}
	
	public class PrivateMethodNode : MemberNode
	{	
		public PrivateMethodNode()
		{
			SetIcon("Icons.16x16.PrivateMethod");
		}
	}
	
	#endregion
	
	#region Event node
	public class PublicEventNode : MemberNode
	{	
		public PublicEventNode()
		{
			SetIcon("Icons.16x16.Event");
		}
	}
	
	public class InternalEventNode : MemberNode
	{	
		public InternalEventNode()
		{
			SetIcon("Icons.16x16.InternalEvent");
		}
	}
	
	public class ProtectedEventNode : MemberNode
	{	
		public ProtectedEventNode()
		{
			SetIcon("Icons.16x16.ProtectedEvent");
		}
	}
	
	public class PrivateEventNode : MemberNode
	{	
		public PrivateEventNode()
		{
			SetIcon("Icons.16x16.PrivateEvent");
		}
	}
	#endregion
}
