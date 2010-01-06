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
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveFromMathImportedMathModuleCompletionItemsTestFixture
	{
		ResolveResult resolveResult;
		ArrayList results;
		PythonResolver resolver;
		ExpressionResult result;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			MockProjectContent projectContent = new MockProjectContent();
			parseInfo.SetCompilationUnit(new DefaultCompilationUnit(projectContent));
			
			string code = "from math import";
			PythonExpressionFinder finder = new PythonExpressionFinder();
			result = finder.FindExpression(code, code.Length);
			resolveResult = resolver.Resolve(result, parseInfo, code);
			results = resolveResult.GetCompletionData(projectContent);
		}
				
		[Test]
		public void CompletionResultsContainCosMethodFromMathModule()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromArray("cos", results);
			Assert.IsNotNull(method);
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsTrueForIMethod()
		{
			MockProjectContent projectContent = new MockProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "MyClass");
			DefaultMethod method = new DefaultMethod(c, "Test");
			
			Assert.IsTrue(result.Context.ShowEntry(method));
		}
	}
}
