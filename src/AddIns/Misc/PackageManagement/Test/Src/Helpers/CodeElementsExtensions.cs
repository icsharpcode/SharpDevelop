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
		
		public static CodeElement FindFirstOrDefault(this global::EnvDTE.CodeElements codeElements, Func<CodeElement, bool> predicate)
		{
			return ToList(codeElements).FirstOrDefault(predicate);
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
		
		public static CodeClass2 FindFirstCodeClass2OrDefault(this global::EnvDTE.CodeElements codeElements, Func<CodeClass2, bool> predicate)
		{
			return codeElements.OfType<CodeClass2>().FirstOrDefault(predicate);
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
		
		public static CodeNamespace FindFirstCodeNamespaceOrDefault(this global::EnvDTE.CodeElements codeElements, Func<CodeNamespace, bool> predicate)
		{
			return codeElements.OfType<CodeNamespace>().FirstOrDefault(predicate);
		}
	}
}
