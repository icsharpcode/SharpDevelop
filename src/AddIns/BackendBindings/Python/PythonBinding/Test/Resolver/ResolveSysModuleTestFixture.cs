// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSysModuleTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("sys", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import sys\r\n" +
				"sys\r\n" +
				"\r\n";
		}
		
		[Test]
		public void CompilationUnitHasSysModuleInUsingsCollection()
		{
			Assert.AreEqual("sys", compilationUnit.UsingScope.Usings[0].Usings[0]);
		}
		
		[Test]
		public void ResolveResultContainsExitMethod()
		{
			List<ICompletionEntry> items = GetCompletionItems();
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", items);
			Assert.IsNotNull(method);
		}
		
		List<ICompletionEntry> GetCompletionItems()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
		
		[Test]
		public void MathModuleExpressionShouldNotHaveAnyCompletionItemsSinceMathModuleIsNotImported()
		{
			ExpressionResult result = new ExpressionResult("math", ExpressionContext.Default);
			resolveResult = resolver.Resolve(result, parseInfo, GetPythonScript());
			
			Assert.IsNull(resolveResult);
		}
	}
}
