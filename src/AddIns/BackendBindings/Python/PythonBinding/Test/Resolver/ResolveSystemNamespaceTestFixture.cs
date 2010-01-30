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
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.AddExistingNamespaceContents("System", new ArrayList());
			
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			cu.ErrorsDuringCompile = true;
			cu.FileName = @"C:\Projects\Test\test.py";
			parseInfo.SetCompilationUnit(cu);
			
			string python =
				"import System\r\n" +
				"class Test:\r\n" +
				"    def __init__(self):\r\n" +
				"        System.\r\n";
			
			ExpressionResult expressionResult = new ExpressionResult("System", new DomRegion(3, 2), null, null);
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
