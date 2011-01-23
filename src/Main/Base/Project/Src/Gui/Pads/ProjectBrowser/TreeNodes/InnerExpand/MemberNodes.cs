// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public abstract class MemberNode : AbstractProjectBrowserTreeNode
	{
		protected readonly MemberReference member;
		protected readonly TypeDefinition type;
		
		public MemberNode(string name, MemberReference member, TypeDefinition type)
		{
			this.member = member;
			this.type = type;
			Text = name;			
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	#region Field nodes
	public class PublicFieldNode : MemberNode
	{
		public PublicFieldNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.Field");
		}
	}
	
	public class InternalFieldNode : MemberNode
	{
		public InternalFieldNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.InteralField");
		}
	}
	
	public class ProtectedFieldNode : MemberNode
	{
		public ProtectedFieldNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.ProtectedField");
		}
	}
	
	public class PrivateFieldNode : MemberNode
	{
		public PrivateFieldNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.PrivateField");
		}
	}
	#endregion
	
	#region Properties nodes
	public class PublicPropertyNode : MemberNode
	{
		public PublicPropertyNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.Property");
		}
	}
	
	public class InternalPropertyNode : MemberNode
	{
		public InternalPropertyNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.InteralProperty");
		}
	}
	
	public class ProtectedPropertyNode : MemberNode
	{
		public ProtectedPropertyNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.ProtectedProperty");
		}
	}
	
	public class PrivatePropertyNode : MemberNode
	{
		public PrivatePropertyNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.PrivateProperty");
		}
	}
	#endregion
	
	#region Method nodes
	
	public class PublicMethodNode : MemberNode
	{	
		public PublicMethodNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.Method");
		}
	}
		
	public class InternalMethodNode : MemberNode
	{	
		public InternalMethodNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.InternalMethod");
		}
	}
	
	public class ProtectedMethodNode : MemberNode
	{	
		public ProtectedMethodNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.ProtectedMethod");
		}
	}
	
	public class PrivateMethodNode : MemberNode
	{	
		public PrivateMethodNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.PrivateMethod");
		}
	}
	
	#endregion
	
	#region Event node
	public class PublicEventNode : MemberNode
	{	
		public PublicEventNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.Event");
		}
	}
	
	public class InternalEventNode : MemberNode
	{	
		public InternalEventNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.InternalEvent");
		}
	}
	
	public class ProtectedEventNode : MemberNode
	{	
		public ProtectedEventNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.ProtectedEvent");
		}
	}
	
	public class PrivateEventNode : MemberNode
	{	
		public PrivateEventNode(string name, MemberReference member, TypeDefinition type) : base(name, member, type)
		{
			SetIcon("Icons.16x16.PrivateEvent");
		}
	}
	#endregion
}
