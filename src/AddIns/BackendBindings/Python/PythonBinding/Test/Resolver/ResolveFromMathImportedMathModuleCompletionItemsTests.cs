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
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveFromMathImportedMathModuleCompletionItemsTestFixture : ResolveTestsBase
	{
		List<ICompletionEntry> GetCompletionResults()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
		
		protected override ExpressionResult GetExpressionResult()
		{
			string code = GetPythonScript();
			PythonExpressionFinder finder = new PythonExpressionFinder();
			return finder.FindExpression(code, code.Length);
		}
		
		protected override string GetPythonScript()
		{
			return "from math import";
		}
		
		[Test]
		public void CompletionResultsContainCosMethodFromMathModule()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("cos", GetCompletionResults());
			Assert.IsNotNull(method);
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsTrueForIMethod()
		{
			MockProjectContent projectContent = new MockProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "MyClass");
			DefaultMethod method = new DefaultMethod(c, "Test");
			
			Assert.IsTrue(expressionResult.Context.ShowEntry(method));
		}
	}
}
