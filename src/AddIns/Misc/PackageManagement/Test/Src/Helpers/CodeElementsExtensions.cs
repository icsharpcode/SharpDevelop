// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.EnvDTE;

namespace PackageManagement.Tests.Helpers
{
	public static class CodeElementsExtensions
	{
		public static List<CodeElement> ToList(this global::EnvDTE.CodeElements codeElements)
		{
			var list = new List<CodeElement>();
			foreach (CodeElement codeElement in codeElements) {
				list.Add(codeElement);
			}
			return list;
		}
		
		public static CodeElement FirstOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return ToList(codeElements).FirstOrDefault();
		}
		
		public static CodeFunction2 FirstCodeFunction2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeFunction2;
		}
		
		public static CodeClass2 FirstCodeClass2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeClass2;
		}
		
		public static CodeInterface FirstCodeInterfaceOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeInterface;
		}
		
		public static CodeAttributeArgument FirstCodeAttributeArgumentOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeAttributeArgument;
		}
		
		public static CodeNamespace FirstCodeNamespaceOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeNamespace;
		}
		
		public static CodeNamespace LastCodeNamespaceOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.LastOrDefault() as CodeNamespace;
		}
		
		public static CodeElement LastOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.ToList().LastOrDefault();
		}
		
		public static CodeAttribute2 FirstCodeAttribute2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeAttribute2;
		}
		
		public static CodeProperty2 FirstCodeProperty2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeProperty2;
		}
		
		public static CodeVariable FirstCodeVariableOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeVariable;
		}
		
		public static CodeParameter FirstCodeParameterOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeParameter;
		}
		
		public static CodeParameter2 FirstCodeParameter2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeParameter2;
		}
		
		public static CodeImport FirstCodeImportOrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeImport;
		}
		
		public static CodeClass2 LastCodeClass2OrDefault(this global::EnvDTE.CodeElements codeElements)
		{
			return codeElements.LastOrDefault() as CodeClass2;
		}
	}
}
