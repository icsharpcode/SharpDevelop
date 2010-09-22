// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the PythonResolver correctly resolves the expression:
	/// "System"
	/// </summary>
	[TestFixture]
	public class ResolveSystemNamespaceTestFixture
	{
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		NamespaceResolveResult resolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.AddExistingNamespaceContents("System", new List<ICompletionEntry>());
			
			string python =
				"import System\r\n" +
				"class Test:\r\n" +
				"    def __init__(self):\r\n" +
				"        System.\r\n";
			
			PythonParser parser = new PythonParser();
			string fileName = @"C:\Projects\Test\test.py";
			DefaultCompilationUnit cu = parser.Parse(mockProjectContent, fileName, python) as DefaultCompilationUnit;
			ParseInformation parseInfo = new ParseInformation(cu);
			
			ExpressionResult expressionResult = new ExpressionResult("System", new DomRegion(4, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python) as NamespaceResolveResult;
		}
		
		[Test]
		public void NamespaceExistsCalled()
		{
			Assert.IsTrue(mockProjectContent.NamespaceExistsCalled);
		}
		
		[Test]
		public void NamespaceSearchedFor()
		{
			Assert.AreEqual("System", mockProjectContent.NamespacePassedToNamespaceExistsMethod);
		}
				
		[Test]
		public void NamespaceResolveResultHasSystemNamespace()
		{
			Assert.AreEqual("System", resolveResult.Name);
		}
	}
}
