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
		public static List<CodeElement> ToList(this CodeElements codeElements)
		{
			var list = new List<CodeElement>();
			foreach (CodeElement codeElement in codeElements) {
				list.Add(codeElement);
			}
			return list;
		}
		
		public static CodeElement FirstOrDefault(this CodeElements codeElements)
		{
			return ToList(codeElements).FirstOrDefault();
		}
		
		public static CodeFunction FirstCodeFunctionOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeFunction;
		}
		
		public static CodeClass2 FirstCodeClass2OrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeClass2;
		}
		
		public static CodeInterface FirstCodeInterfaceOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeInterface;
		}
		
		public static CodeAttributeArgument FirstCodeAttributeArgumentOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeAttributeArgument;
		}
		
		public static CodeNamespace FirstCodeNamespaceOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeNamespace;
		}
		
		public static CodeNamespace LastCodeNamespaceOrDefault(this CodeElements codeElements)
		{
			return codeElements.LastOrDefault() as CodeNamespace;
		}
		
		public static CodeElement LastOrDefault(this CodeElements codeElements)
		{
			return codeElements.ToList().LastOrDefault();
		}
		
		public static CodeAttribute2 FirstCodeAttribute2OrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeAttribute2;
		}
		
		public static CodeProperty2 FirstCodeProperty2OrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeProperty2;
		}
		
		public static CodeVariable FirstCodeVariableOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeVariable;
		}
		
		public static CodeParameter FirstCodeParameterOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeParameter;
		}
		
		public static CodeImport FirstCodeImportOrDefault(this CodeElements codeElements)
		{
			return codeElements.FirstOrDefault() as CodeImport;
		}
	}
}
