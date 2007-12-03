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
	/// Tests the PythonResolver does not return a namespace resolve result for
	/// an unknown namespace.
	/// </summary>
	[TestFixture]
	public class ResolveUnknownNamespaceTestFixture
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
			mockProjectContent.NamespacesToAdd.Add("Test");

			parseInfo.DirtyCompilationUnit = new DefaultCompilationUnit(mockProjectContent);
						
			string python = "import System\r\n" +
							"class Test:\r\n" +
							"\tdef __init__(self):\r\n" +
							"\t\tUnknown\r\n";
			ExpressionResult expressionResult = new ExpressionResult("Unknown", new DomRegion(3, 2), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python);
		}
		
		[Test]
		public void ResolveResultDoesNotExist()
		{
			Assert.IsNull(resolveResult);
		}
		
		[Test]
		public void SearchNamespaceCalled()
		{
			Assert.IsTrue(mockProjectContent.SearchNamespaceCalled);
		}	
	}
}
