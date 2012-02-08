// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQuality.Gui
{
	public static class NodeIconService
	{
		static readonly BitmapSource NamespaceNode = GetImage("Icons.16x16.NameSpace");
		static readonly BitmapSource Assembly = GetImage("Icons.16x16.Reference");

		static readonly BitmapSource Class = GetImage("Icons.16x16.Class");
		static readonly BitmapSource InternalClass = GetImage("Icons.16x16.InternalClass");
		static readonly BitmapSource ProtectedClass = GetImage("Icons.16x16.ProtectedClass");
		static readonly BitmapSource PrivateClass = GetImage("Icons.16x16.PrivateClass");

		static readonly BitmapSource Struct = GetImage("Icons.16x16.Struct");
		static readonly BitmapSource InternalStruct = GetImage("Icons.16x16.InternalStruct");
		static readonly BitmapSource ProtectedStruct = GetImage("Icons.16x16.ProtectedStruct");
		static readonly BitmapSource PrivateStruct = GetImage("Icons.16x16.PrivateStruct");

		static readonly BitmapSource Interface = GetImage("Icons.16x16.Interface");
		static readonly BitmapSource InternalInterface = GetImage("Icons.16x16.InternalInterface");
		static readonly BitmapSource ProtectedInterface = GetImage("Icons.16x16.ProtectedInterface");
		static readonly BitmapSource PrivateInterface = GetImage("Icons.16x16.PrivateInterface");

		static readonly BitmapSource Enum = GetImage("Icons.16x16.Enum");
		static readonly BitmapSource InternalEnum = GetImage("Icons.16x16.InternalEnum");
		static readonly BitmapSource ProtectedEnum = GetImage("Icons.16x16.ProtectedEnum");
		static readonly BitmapSource PrivateEnum = GetImage("Icons.16x16.PrivateEnum");

		static readonly BitmapSource Delegate = GetImage("Icons.16x16.Delegate");
		static readonly BitmapSource InternalDelegate = GetImage("Icons.16x16.InternalDelegate");
		static readonly BitmapSource ProtectedDelegate = GetImage("Icons.16x16.ProtectedDelegate");
		static readonly BitmapSource PrivateDelegate = GetImage("Icons.16x16.PrivateDelegate");

		static readonly BitmapSource MethodNode = GetImage("Icons.16x16.Method");
		static readonly BitmapSource ProtectedMethod = GetImage("Icons.16x16.ProtectedMethod");
		static readonly BitmapSource PrivateMethod = GetImage("Icons.16x16.PrivateMethod");
		
		static readonly BitmapSource Property = GetImage("Icons.16x16.Property");
		static readonly BitmapSource ProtectedProperty = GetImage("Icons.16x16.ProtectedProperty");
		static readonly BitmapSource PrivateProperty = GetImage("Icons.16x16.PrivateProperty");
		
		static readonly BitmapSource Field = GetImage("Icons.16x16.Field");
		static readonly BitmapSource ProtectedField = GetImage("Icons.16x16.ProtectedField");
		static readonly BitmapSource PrivateField = GetImage("Icons.16x16.PrivateField");
		
		static readonly BitmapSource Event = GetImage("Icons.16x16.Event");
		static readonly BitmapSource ProtectedEvent = GetImage("Icons.16x16.ProtectedEvent");
		static readonly BitmapSource PrivateEvent = GetImage("Icons.16x16.PrivateEvent");
		
		static readonly BitmapSource ConstantField = GetImage("Icons.16x16.Literal");

		static BitmapSource GetImage(string name)
		{
			try {
				return PresentationResourceService.GetBitmapSource(name);
			} catch (Exception) {
				return null; // image isn't needed necessarily
			}
		}

		public static BitmapSource GetIcon(FieldNode field)
		{
//			if (field.Field.IsPrivate)
//				return PrivateField;
//			if (field.Field.IsProtected)
//				return ProtectedField;
//			if (field.Field.IsConst)
//				return ConstantField;

			return Field;
		}
		
		public static BitmapSource GetIcon(NodeBase node)
		{
			if (node is AssemblyNode) 
				return GetIcon((AssemblyNode)node);
			if (node is NamespaceNode)
				return GetIcon((NamespaceNode)node);
			if (node is TypeNode)
				return GetIcon((TypeNode)node);
			if (node is MethodNode)
				return GetIcon((MethodNode)node);
			if (node is FieldNode)
				return GetIcon((FieldNode)node);
			if (node is PropertyNode)
				return GetIcon((PropertyNode)node);
			if (node is EventNode)
				return GetIcon((EventNode)node);
			
			return null;
		}
		
		public static BitmapSource GetIcon(EventNode node)
		{
//			if (node.Event.IsPrivate)
//				return PrivateEventField;
//			if (node.Event.IsProtected)
//				return ProtectedEventField;
			
			return Event;
		}

		public static BitmapSource GetIcon(MethodNode method)
		{
//			if (method.Method.IsPrivate)
//				return PrivateMethod;
//			if (method.Method.IsProtected)
//				return ProtectedMethod;

//			if (method.IsGetter || method.IsSetter)
//			{
//				if (method.IsPublic)
//					return PropertyMethod;
//				if (method.IsPrivate)
//					return PrivatePropertyMethod;
//				if (method.IsProtected)
//					return ProtectedPropertyMethod;
//			}

			return MethodNode;
		}
		
		public static BitmapSource GetIcon(PropertyNode property)
		{
			return Property;
		}

		public static BitmapSource GetIcon(AssemblyNode module)
		{
			return Assembly;
		}

		public static BitmapSource GetIcon(NamespaceNode ns)
		{
			return NamespaceNode;
		}

		public static BitmapSource GetIcon(TypeNode type)
		{
			switch (type.TypeDefinition.Kind) {
				case TypeKind.Enum:
					if (type.TypeDefinition.IsPublic)
						return Enum;
					if (type.TypeDefinition.IsProtected)
						return ProtectedEnum;
					if (type.TypeDefinition.IsInternal)
						return InternalEnum;
					return PrivateEnum;
				case TypeKind.Struct:
					if (type.TypeDefinition.IsPublic)
						return Struct;
					if (type.TypeDefinition.IsProtected)
						return ProtectedStruct;
					if (type.TypeDefinition.IsInternal)
						return InternalStruct;
					return PrivateStruct;
				case TypeKind.Interface:
					if (type.TypeDefinition.IsPublic)
						return Interface;
					if (type.TypeDefinition.IsProtected)
						return ProtectedInterface;
					if (type.TypeDefinition.IsInternal)
						return InternalInterface;
					return PrivateInterface;
				case TypeKind.Delegate:
					if (type.TypeDefinition.IsPublic)
						return Delegate;
					if (type.TypeDefinition.IsProtected)
						return ProtectedDelegate;
					if (type.TypeDefinition.IsInternal)
						return InternalDelegate;
					return PrivateDelegate;
				default:
					if (type.TypeDefinition.IsPublic)
						return Class;
					if (type.TypeDefinition.IsProtected)
						return ProtectedClass;
					if (type.TypeDefinition.IsInternal)
						return InternalClass;
					return PrivateClass;
			}
		}
	}
}
