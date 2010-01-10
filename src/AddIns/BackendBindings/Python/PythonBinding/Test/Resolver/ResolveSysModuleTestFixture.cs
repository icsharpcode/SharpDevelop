// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSysModuleTestFixture : ResolveTestFixtureBase
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
			ArrayList items = GetCompletionItems();
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", items);
			Assert.IsNotNull(method);
		}
		
		ArrayList GetCompletionItems()
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
