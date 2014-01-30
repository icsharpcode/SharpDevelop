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

//using System;
//using System.Collections.Generic;
//using ICSharpCode.NRefactory.Ast;
//using ICSharpCode.SharpDevelop.Dom;
//
//namespace ICSharpCode.PackageManagement.EnvDTE
//{
//	internal static class IReturnTypeExtensions
//	{
//		public static string GetFullName(this IReturnType returnType)
//		{
//			return returnType
//				.DotNetName
//				.Replace('{', '<')
//				.Replace('}', '>');
//		}
//		
//		public static string AsCSharpString(this IReturnType returnType)
//		{
//			string name = String.Empty;
//			if (TypeReference.PrimitiveTypesCSharpReverse.TryGetValue(returnType.FullyQualifiedName, out name)) {
//				return name;
//			}
//			return returnType.GetFullName();
//		}
//		
//		public static string AsVisualBasicString(this IReturnType returnType)
//		{
//			string name = String.Empty;
//			if (TypeReference.PrimitiveTypesVBReverse.TryGetValue(returnType.FullyQualifiedName, out name)) {
//				return name;
//			}
//			return returnType.GetFullName();
//		}
//		
//		public static global::EnvDTE.vsCMTypeRef GetTypeKind(this IReturnType returnType)
//		{
//			global::EnvDTE.vsCMTypeRef typeRef = GetSystemTypeKind(returnType.FullyQualifiedName);
//			if (typeRef != global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther) {
//				return typeRef;
//			}
//			
//			if (returnType.IsReferenceType.GetValueOrDefault()) {
//				return global::EnvDTE.vsCMTypeRef.vsCMTypeRefCodeType;
//			}
//			return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
//		}
//		
//		static global::EnvDTE.vsCMTypeRef GetSystemTypeKind(string fullyQualifiedTypeName)
//		{
//			switch (fullyQualifiedTypeName) {
//				case "System.String":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefString;
//				case "System.Void":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefVoid;
//				case "System.Boolean":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefBool;
//				case "System.Int16":
//				case "System.UInt16":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefShort;
//				case "System.Int32":
//				case "System.UInt32":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefInt;
//				case "System.Int64":
//				case "System.UInt64":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefLong;
//				case "System.Decimal":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDecimal;
//				case "System.Char":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefChar;
//				case "System.Byte":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefByte;
//				case "System.Object":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefObject;
//				case "System.Double":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefDouble;
//				case "System.Single":
//					return global::EnvDTE.vsCMTypeRef.vsCMTypeRefFloat;
//			}
//			return global::EnvDTE.vsCMTypeRef.vsCMTypeRefOther;
//		}
//	}
//}
