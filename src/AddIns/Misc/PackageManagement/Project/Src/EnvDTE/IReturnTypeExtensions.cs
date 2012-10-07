// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	internal static class IReturnTypeExtensions
	{
		public static string GetFullName(this IReturnType returnType)
		{
			return returnType
				.DotNetName
				.Replace('{', '<')
				.Replace('}', '>');
		}
		
		public static string AsCSharpString(this IReturnType returnType)
		{
			string name = String.Empty;
			if (TypeReference.PrimitiveTypesCSharpReverse.TryGetValue(returnType.FullyQualifiedName, out name)) {
				return name;
			}
			return returnType.GetFullName();
		}
		
		public static string AsVisualBasicString(this IReturnType returnType)
		{
			string name = String.Empty;
			if (TypeReference.PrimitiveTypesVBReverse.TryGetValue(returnType.FullyQualifiedName, out name)) {
				return name;
			}
			return returnType.GetFullName();
		}
		
		public static global::EnvDTE.vsCMTypeRef GetTypeKind(this IReturnType returnType)
		{
			global::EnvDTE.vsCMTypeRef typeRef = GetSystemTypeKind(returnType.FullyQualifiedName);
			if (typeRef != global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther) {
				return typeRef;
			}
			
			if (returnType.IsReferenceType.GetValueOrDefault()) {
				return global::EnvDTE.vsCMTypeRef.vsCMTypeRefCodeType;
			}
			return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
		}
		
		static global::EnvDTE.vsCMTypeRef GetSystemTypeKind(string fullyQualifiedTypeName)
		{
			switch (fullyQualifiedTypeName) {
				case "System.String":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefString;
				case "System.Void":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid;
				case "System.Boolean":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefBool;
				case "System.Int16":
				case "System.UInt16":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort;
				case "System.Int32":
				case "System.UInt32":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt;
				case "System.Int64":
				case "System.UInt64":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong;
				case "System.Decimal":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDecimal;
				case "System.Char":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefChar;
				case "System.Byte":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefByte;
				case "System.Object":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject;
				case "System.Double":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDouble;
				case "System.Single":
					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefFloat;
			}
			return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
		}
	}
}
