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
	}
}
