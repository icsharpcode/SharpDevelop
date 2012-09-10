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
		
		public static vsCMTypeRef GetTypeKind(this IReturnType returnType)
		{
			vsCMTypeRef typeRef = GetSystemTypeKind(returnType.FullyQualifiedName);
			if (typeRef != vsCMTypeRef.vsCMTypeRefOther) {
				return typeRef;
			}
			
			if (returnType.IsReferenceType.GetValueOrDefault()) {
				return vsCMTypeRef.vsCMTypeRefCodeType;
			}
			return vsCMTypeRef.vsCMTypeRefOther;
		}
		
		static vsCMTypeRef GetSystemTypeKind(string fullyQualifiedTypeName)
		{
			switch (fullyQualifiedTypeName) {
				case "System.String":
					return vsCMTypeRef.vsCMTypeRefString;
				case "System.Void":
					return vsCMTypeRef.vsCMTypeRefVoid;
				case "System.Boolean":
					return vsCMTypeRef.vsCMTypeRefBool;
				case "System.Int16":
				case "System.UInt16":
					return vsCMTypeRef.vsCMTypeRefShort;
				case "System.Int32":
				case "System.UInt32":
					return vsCMTypeRef.vsCMTypeRefInt;
				case "System.Int64":
				case "System.UInt64":
					return vsCMTypeRef.vsCMTypeRefLong;
				case "System.Decimal":
					return vsCMTypeRef.vsCMTypeRefDecimal;
				case "System.Char":
					return vsCMTypeRef.vsCMTypeRefChar;
				case "System.Byte":
					return vsCMTypeRef.vsCMTypeRefByte;
				case "System.Object":
					return vsCMTypeRef.vsCMTypeRefObject;
				case "System.Double":
					return vsCMTypeRef.vsCMTypeRefDouble;
				case "System.Single":
					return vsCMTypeRef.vsCMTypeRefFloat;
			}
			return vsCMTypeRef.vsCMTypeRefOther;
		}
	}
}
