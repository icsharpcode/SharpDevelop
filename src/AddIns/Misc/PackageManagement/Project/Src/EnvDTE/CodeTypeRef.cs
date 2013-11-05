// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeRef : MarshalByRefObject, global::EnvDTE.CodeTypeRef
	{
		protected readonly CodeModelContext context;
		protected readonly CodeElement parent;
		protected readonly IType type;
		
		public CodeTypeRef()
		{
		}
		
		public CodeTypeRef(CodeModelContext context, CodeElement parent, IType type)
		{
			this.context = context;
			this.parent = parent;
			this.type = type;
		}
		
		public virtual string AsFullName {
			get { return type.ReflectionName; }
		}
		
		public virtual string AsString {
			get {
				return new CSharpAmbience().ConvertType(type);
			}
		}
		
		public virtual global::EnvDTE.CodeElement Parent {
			get { return parent; }
		}
		
		public virtual global::EnvDTE.CodeType CodeType {
			get {
				return EnvDTE.CodeType.Create(context, type);
			}
		}
		
		public virtual global::EnvDTE.vsCMTypeRef TypeKind {
			get {
				switch (type.Kind) {
					case NRefactory.TypeSystem.TypeKind.Class:
						if (type.IsKnownType(KnownTypeCode.String))
							return global::EnvDTE.vsCMTypeRef.vsCMTypeRefString;
						else
							return global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject;
					case NRefactory.TypeSystem.TypeKind.Struct:
						var typeDef = type.GetDefinition();
						if (typeDef != null)
							return GetStructTypeKind(typeDef.KnownTypeCode);
						else
							return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
					case NRefactory.TypeSystem.TypeKind.Delegate:
					case NRefactory.TypeSystem.TypeKind.Interface:
					case NRefactory.TypeSystem.TypeKind.Module:
						return global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject;
					case NRefactory.TypeSystem.TypeKind.Void:
						return global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid;
					case NRefactory.TypeSystem.TypeKind.Array:
						return global::EnvDTE.vsCMTypeRef.vsCMTypeRefArray;
					case NRefactory.TypeSystem.TypeKind.Pointer:
						return global::EnvDTE.vsCMTypeRef.vsCMTypeRefPointer;
					default:
						if (type.IsReferenceType == true)
							return global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject;
						else
							return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
				}
			}
		}
		
		global::EnvDTE.vsCMTypeRef GetStructTypeKind(KnownTypeCode knownTypeCode)
		{
			switch (knownTypeCode) {
				case KnownTypeCode.Boolean:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefBool;
				case KnownTypeCode.Char:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefChar;
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefByte;
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort;
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt;
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong;
				case KnownTypeCode.Single:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefFloat;
				case KnownTypeCode.Double:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDouble;
				case KnownTypeCode.Decimal:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDecimal;
				case KnownTypeCode.Void:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid;
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.UIntPtr:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefPointer;
				default:
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
			}
		}
	}
}
