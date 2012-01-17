// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.CodeQualityAnalysis
{
	public static class NodeIconService
	{
		private static readonly BitmapSource NamespaceNode = GetImage("Icons.16x16.NameSpace");
		private static readonly BitmapSource Assembly = GetImage("Icons.16x16.Assembly");

		private static readonly BitmapSource Class = GetImage("Icons.16x16.Class");
		private static readonly BitmapSource InternalClass = GetImage("Icons.16x16.InternalClass");
		private static readonly BitmapSource ProtectedClass = GetImage("Icons.16x16.ProtectedClass");
		private static readonly BitmapSource PrivateClass = GetImage("Icons.16x16.PrivateClass");

		private static readonly BitmapSource Struct = GetImage("Icons.16x16.Struct");
		private static readonly BitmapSource InternalStruct = GetImage("Icons.16x16.InternalStruct");
		private static readonly BitmapSource ProtectedStruct = GetImage("Icons.16x16.ProtectedStruct");
		private static readonly BitmapSource PrivateStruct = GetImage("Icons.16x16.PrivateStruct");

		private static readonly BitmapSource Interface = GetImage("Icons.16x16.Interface");
		private static readonly BitmapSource InternalInterface = GetImage("Icons.16x16.InternalInterface");
		private static readonly BitmapSource ProtectedInterface = GetImage("Icons.16x16.ProtectedInterface");
		private static readonly BitmapSource PrivateInterface = GetImage("Icons.16x16.PrivateInterface");

		private static readonly BitmapSource Enum = GetImage("Icons.16x16.Enum");
		private static readonly BitmapSource InternalEnum = GetImage("Icons.16x16.InternalEnum");
		private static readonly BitmapSource ProtectedEnum = GetImage("Icons.16x16.ProtectedEnum");
		private static readonly BitmapSource PrivateEnum = GetImage("Icons.16x16.PrivateEnum");

		private static readonly BitmapSource Delegate = GetImage("Icons.16x16.Delegate");
		private static readonly BitmapSource InternalDelegate = GetImage("Icons.16x16.InternalDelegate");
		private static readonly BitmapSource ProtectedDelegate = GetImage("Icons.16x16.ProtectedDelegate");
		private static readonly BitmapSource PrivateDelegate = GetImage("Icons.16x16.PrivateDelegate");

		private static readonly BitmapSource MethodNode = GetImage("Icons.16x16.Method");
		private static readonly BitmapSource ProtectedMethod = GetImage("Icons.16x16.ProtectedMethod");
		private static readonly BitmapSource PrivateMethod = GetImage("Icons.16x16.PrivateMethod");
		private static readonly BitmapSource PropertyMethod = GetImage("Icons.16x16.Property");
		private static readonly BitmapSource ProtectedPropertyMethod = GetImage("Icons.16x16.ProtectedProperty");
		private static readonly BitmapSource PrivatePropertyMethod = GetImage("Icons.16x16.PrivateProperty");
		
		private static readonly BitmapSource Field = GetImage("Icons.16x16.Field");
		private static readonly BitmapSource ProtectedField = GetImage("Icons.16x16.ProtectedField");
		private static readonly BitmapSource PrivateField = GetImage("Icons.16x16.PrivateField");
		private static readonly BitmapSource EventField = GetImage("Icons.16x16.Event");
		private static readonly BitmapSource ProtectedEventField = GetImage("Icons.16x16.ProtectedEvent");
		private static readonly BitmapSource PrivateEventField = GetImage("Icons.16x16.PrivateEvent");
		private static readonly BitmapSource ConstantField = GetImage("Icons.16x16.Literal");

		private static BitmapSource GetImage(string name)
		{
			try
			{
				return PresentationResourceService.GetBitmapSource(name);
			}
			catch (Exception)
			{
				return null; // image isn't needed necessarily
			}
			
		}

		public static BitmapSource GetIcon(FieldNode field)
		{
			if (field.Field.IsPrivate)
				return PrivateField;
			if (field.Field.IsProtected)
				return ProtectedField;
			if (field.Field.IsConst)
				return ConstantField;

			return Field;
		}
		
		public static BitmapSource GetIcon(EventNode node)
		{
			if (node.Event.IsPrivate)
				return PrivateEventField;
			if (node.Event.IsProtected)
				return ProtectedEventField;
			
			return EventField;
		}

		public static BitmapSource GetIcon(MethodNode method)
		{
			if (method.Method.IsPrivate)
				return PrivateMethod;
			if (method.Method.IsProtected)
				return ProtectedMethod;

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
