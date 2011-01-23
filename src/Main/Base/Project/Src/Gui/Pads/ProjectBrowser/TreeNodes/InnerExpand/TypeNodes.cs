// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Text;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Project.InnerExpand
{
	public abstract class TypeNode : AbstractProjectBrowserTreeNode
	{
		protected readonly TypeDefinition type;
		
		public TypeNode(string name, TypeDefinition type)
		{
			Text = name;
			this.type = type;
			
			this.PerformInitialization();
		}
		
		public virtual void ShowMembers(bool forceRefresh = false)
		{
			if (Nodes.Count > 0 && !forceRefresh)
				return;
			
			Nodes.Clear();
			
			foreach (var ev in type.Events) {
				if (ev.AddMethod == null && ev.RemoveMethod == null ) continue;
				
				if (ev.AddMethod != null && !ev.AddMethod.IsPublic &&
				    ev.RemoveMethod != null && !ev.RemoveMethod.IsPublic) continue;
				
				new PublicEventNode(ev.Name, ev, type).InsertSorted(this);
			}

			foreach (var property in type.Properties) {
				if (property.GetMethod == null && property.SetMethod == null ) continue;
				
				if (property.GetMethod != null && !property.GetMethod.IsPublic && 
				    property.SetMethod != null && !property.SetMethod.IsPublic) continue;
				new PublicPropertyNode(property.Name, property, type).InsertSorted(this);
			}

			foreach (var method in type.Methods) {
				if (!method.IsPublic) continue;
				if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")) continue;
				
				StringBuilder sb = new StringBuilder();

				if (!method.IsConstructor) {
					sb.Append(method.Name);
				} else {
					sb.Append(method.DeclaringType.Name);
				}

				sb.Append(DecompilerService.GetParameters(method));
				
				new PublicMethodNode(sb.ToString(), method, type).InsertSorted(this);
			}
		}
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	#region Classes
	
	public abstract class ClassNode : TypeNode
	{
		public ClassNode(string name, TypeDefinition type) : base(name, type) { }
	}
	
	public class PublicClassNode : ClassNode
	{
		public PublicClassNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.Class");
		}
	}
	
	public class InternalClassNode : ClassNode
	{
		public InternalClassNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.InternalClass");
		}
	}
	
	public class ProtectedClassNode : ClassNode
	{
		public ProtectedClassNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.ProtectedClass");
		}
	}
	
	public class PrivateClassNode : ClassNode
	{
		public PrivateClassNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.PrivateClass");
		}
	}
	
	#endregion
	
	#region Interfaces
	public abstract class InterfaceNode : TypeNode
	{
		public InterfaceNode(string name, TypeDefinition type) : base(name, type) { }
		
	}
	
	public class PublicInterfaceNode : InterfaceNode
	{
		public PublicInterfaceNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.Interface");
		}
	}
	
	public class InternalInterfaceNode : InterfaceNode
	{
		public InternalInterfaceNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.InternalInterface");
		}
	}
	
	public class ProtectedInterfaceNode : InterfaceNode
	{
		public ProtectedInterfaceNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.ProtectedInterface");
		}
	}
	
	public class PrivateInterfaceNode : InterfaceNode
	{
		public PrivateInterfaceNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.PrivateInterface");
		}
	}
	
	#endregion
	
	#region Structs
	
	public abstract class StructNode : TypeNode
	{
		public StructNode(string name, TypeDefinition type) : base(name, type)
		{
		}
	}
	
	public class PublicStructNode : StructNode
	{
		public PublicStructNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.Struct");
		}
	}
	
	public class InternalStructNode : StructNode
	{
		public InternalStructNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.InternalStruct");
		}
	}
	
	public class ProtectedStructNode : StructNode
	{
		public ProtectedStructNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.ProtectedStruct");
		}
	}
	
	public class PrivateStructNode : StructNode
	{
		public PrivateStructNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.PrivateStruct");
		}
	}
	#endregion
	
	#region Enums
	public abstract class EnumNode : TypeNode
	{
		public EnumNode(string name, TypeDefinition type) : base(name, type)
		{
		}
	}
	
	public class PublicEnumNode : EnumNode
	{
		public PublicEnumNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.Enum");
		}
	}
	
	public class IntenalEnumNode : EnumNode
	{
		public IntenalEnumNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.InternalEnum");
		}
	}
	
	public class ProtectedEnumNode : EnumNode
	{
		public ProtectedEnumNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.ProtectedEnum");
		}
	}
	
	public class PrivateEnumNode : EnumNode
	{
		public PrivateEnumNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.PrivateEnum");
		}
	}
	#endregion
	
	#region Delegates
	public abstract class DelegateNode : TypeNode
	{
		public DelegateNode(string name, TypeDefinition type) : base(name, type)
		{
		}
		
		public override void ShowMembers(bool forceRefresh)
		{
			// do nothing
		}
	}
	
	public class PublicDelegateNode : DelegateNode
	{
		public PublicDelegateNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.Delegate");
		}
	}
	
	public class InternalDelegateNode : DelegateNode
	{
		public InternalDelegateNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.InternalDelegate");
		}
	}
	
	public class ProtectedDelegateNode : DelegateNode
	{
		public ProtectedDelegateNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.ProtectedDelegate");
		}
	}
	
	public class PrivateDelegateNode : DelegateNode
	{
		public PrivateDelegateNode(string name, TypeDefinition type) : base(name, type)
		{
			SetIcon("Icons.16x16.PrivateDelegate");
		}
	}
	#endregion
}
