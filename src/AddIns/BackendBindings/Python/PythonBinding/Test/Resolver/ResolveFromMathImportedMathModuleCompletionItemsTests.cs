// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
