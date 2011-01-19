// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public abstract class TypeNode : AbstractProjectBrowserTreeNode
	{		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			throw new NotImplementedException();
		}
	}
	
	#region Classes
	public class PublicClassNode : TypeNode
	{		
		public PublicClassNode()
		{
			SetIcon("Icons.16x16.Class");
		}
	}
	
	public class InternalClassNode : TypeNode
	{		
		public InternalClassNode()
		{
			SetIcon("Icons.16x16.InternalClass");
		}
	}
	
	public class ProtectedClassNode : TypeNode
	{		
		public ProtectedClassNode()
		{
			SetIcon("Icons.16x16.ProtectedClass");
		}
	}
	
	public class PrivateClassNode : TypeNode
	{		
		public PrivateClassNode()
		{
			SetIcon("Icons.16x16.PrivateClass");
		}
	}
	
	#endregion
	
	#region Interfaces
	public class PublicInterfaceNode : TypeNode
	{		
		public PublicInterfaceNode()
		{
			SetIcon("Icons.16x16.Interface");
		}
	}
	
	public class InternalInterfaceNode : TypeNode
	{		
		public InternalInterfaceNode()
		{
			SetIcon("Icons.16x16.InternalInterface");
		}
	}
	
	public class ProtectedInterfaceNode : TypeNode
	{		
		public ProtectedInterfaceNode()
		{
			SetIcon("Icons.16x16.ProtectedInterface");
		}
	}
	
	public class PrivateInterfaceNode : TypeNode
	{		
		public PrivateInterfaceNode()
		{
			SetIcon("Icons.16x16.PrivateInterface");
		}
	}
	
	#endregion
	
	#region Structs
	public class PublicStructNode : TypeNode
	{		
		public PublicStructNode()
		{
			SetIcon("Icons.16x16.Struct");
		}
	}
	
	public class InternalStructNode : TypeNode
	{		
		public InternalStructNode()
		{
			SetIcon("Icons.16x16.InternalStruct");
		}
	}
	
	public class ProtectedStructNode : TypeNode
	{		
		public ProtectedStructNode()
		{
			SetIcon("Icons.16x16.ProtectedStruct");
		}
	}
	
	public class PrivateStructNode : TypeNode
	{		
		public PrivateStructNode()
		{
			SetIcon("Icons.16x16.PrivateStruct");
		}
	}
	#endregion
	
	#region Enums
	public class PublicEnumNode : TypeNode
	{		
		public PublicEnumNode()
		{
			SetIcon("Icons.16x16.Enum");
		}
	}
	
	public class IntenalEnumNode : TypeNode
	{		
		public IntenalEnumNode()
		{
			SetIcon("Icons.16x16.InternalEnum");
		}
	}
	
	public class ProtectedEnumNode : TypeNode
	{		
		public ProtectedEnumNode()
		{
			SetIcon("Icons.16x16.ProtectedEnum");
		}
	}
	
	public class PrivateEnumNode : TypeNode
	{		
		public PrivateEnumNode()
		{
			SetIcon("Icons.16x16.PrivateEnum");
		}
	}
	#endregion
	
	#region Delegates
	public class PublicDelegateNode : TypeNode
	{		
		public PublicDelegateNode()
		{
			SetIcon("Icons.16x16.Delegate");
		}
	}
	
	public class InternalDelegateNode : TypeNode
	{		
		public InternalDelegateNode()
		{
			SetIcon("Icons.16x16.InternalDelegate");
		}
	}
	
	public class ProtectedDelegateNode : TypeNode
	{		
		public ProtectedDelegateNode()
		{
			SetIcon("Icons.16x16.ProtectedDelegate");
		}
	}
	
	public class PrivateDelegateNode : TypeNode
	{		
		public PrivateDelegateNode()
		{
			SetIcon("Icons.16x16.PrivateDelegate");
		}
	}
	#endregion
}
