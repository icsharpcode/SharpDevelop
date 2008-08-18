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
		ResolveResult resolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.SearchNamespaceToReturn = "System";
			
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			cu.ErrorsDuringCompile = true;
			cu.FileName = @"C:\Projects\Test\test.py";
			parseInfo.SetCompilationUnit(cu);
			
			string python = "import System\r\n" +
							"class Test:\r\n" +
							"\tdef __init__(self):\r\n" +
							"\t\tSystem.\r\n";
			ExpressionResult expressionResult = new ExpressionResult("System", new DomRegion(3, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python);
		}
		
		[Test]
		public void ResolveResultExists()
		{
			Assert.IsNotNull(resolveResult);
		}
		
		[Test]
		public void IsNamespaceResolveResult()
		{
			Assert.IsInstanceOfType(typeof(NamespaceResolveResult), resolveResult);
		}
		
		[Test]
		public void SearchNamespaceCalled()
		{
			Assert.IsTrue(mockProjectContent.SearchNamespaceCalled);
		}
		
		[Test]
		public void NamespaceSearchedFor()
		{
			Assert.AreEqual("System", mockProjectContent.NamespaceSearchedFor);
		}
				
		[Test]
		public void NamespaceResolveResultHasSystemNamespace()
		{
			NamespaceResolveResult nsResult = (NamespaceResolveResult)resolveResult;
			Assert.AreEqual("System", nsResult.Name);
		}
	}
}
