// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
			
			mockProjectContent = new MockProjectContent();
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			cu.FileName = @"C:\Projects\Test\test.py";
			ParseInformation parseInfo = new ParseInformation(cu);
					
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
