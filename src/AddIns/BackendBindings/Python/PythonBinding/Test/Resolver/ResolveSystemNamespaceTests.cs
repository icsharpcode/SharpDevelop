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
