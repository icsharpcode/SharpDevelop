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
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardModuleMethodGroupResolveResult : MethodGroupResolveResult
	{
		public PythonStandardModuleMethodGroupResolveResult(IReturnType containingType, string methodName, MethodGroup[] methodGroups)
			: base(null, null, containingType, methodName, methodGroups)
		{
		}
		
		public static PythonStandardModuleMethodGroupResolveResult Create(PythonStandardModuleType type, string methodName)
		{
			PythonModuleCompletionItems completionItems = PythonModuleCompletionItemsFactory.Create(type);
			MethodGroup methods = completionItems.GetMethods(methodName);
			if (methods.Count > 0) {
				return Create(methods);
			}
			return null;
		}
		
		static PythonStandardModuleMethodGroupResolveResult Create(MethodGroup methods)
		{
			MethodGroup[] methodGroups = new MethodGroup[] { methods };
			IMethod method = methods[0];
			IReturnType returnType = new DefaultReturnType(method.DeclaringType);
			return new PythonStandardModuleMethodGroupResolveResult(returnType, method.Name, methodGroups);
		}
	}
}
