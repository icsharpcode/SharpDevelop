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
	/// Tests that the PythonResolver resolves "from System" to 
	/// a namespace.
	/// </summary>
	[TestFixture]
	public class ResolveFromImportTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		PythonImportModuleResolveResult resolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			
			mockProjectContent = new MockProjectContent();
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			cu.ErrorsDuringCompile = true;
			cu.FileName = @"C:\Projects\Test\test.py";
			parseInfo.SetCompilationUnit(cu);
					
			string python = "from System";
			PythonExpressionFinder finder = new PythonExpressionFinder();
			ExpressionResult expressionResult = finder.FindExpression(python, python.Length);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python) as PythonImportModuleResolveResult;
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
	}
}
