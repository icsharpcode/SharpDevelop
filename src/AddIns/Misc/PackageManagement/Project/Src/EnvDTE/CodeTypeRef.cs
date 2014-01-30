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
