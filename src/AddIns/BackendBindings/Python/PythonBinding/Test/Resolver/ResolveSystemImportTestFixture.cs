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
	/// <summary>
	/// Tests that the PythonResolver resolves "import System" to 
	/// a namespace.
	/// </summary>
	[TestFixture]
	public class ResolveSystemImportTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		PythonImportModuleResolveResult resolveResult;
		ExpressionResult expressionResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.SetNamespaceExistsReturnValue(true);
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			cu.ErrorsDuringCompile = true;
			cu.FileName = @"C:\Projects\Test\test.py";
			parseInfo.SetCompilationUnit(cu);
			
			string code = "import System";
			PythonExpressionFinder finder = new PythonExpressionFinder();
			expressionResult = finder.FindExpression(code, code.Length);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, code) as PythonImportModuleResolveResult;
		}
		
		[Test]
		public void NamespaceResolveResultFound()
		{
			Assert.IsNotNull(resolveResult);
		}
		
		[Test]
		public void NamespaceName()
		{
			Assert.AreEqual("System", resolveResult.Name);
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsTrueForString()
		{
			Assert.IsTrue(expressionResult.Context.ShowEntry("abc"));
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsFalseForProjectContent()
		{
			MockProjectContent projectContent = new MockProjectContent();
			Assert.IsFalse(expressionResult.Context.ShowEntry(projectContent));
		}
	}
}
