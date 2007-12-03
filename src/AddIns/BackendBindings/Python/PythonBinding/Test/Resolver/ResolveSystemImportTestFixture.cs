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
		NamespaceResolveResult resolveResult;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.SearchNamespaceToReturn = "System";
			parseInfo.DirtyCompilationUnit = new DefaultCompilationUnit(mockProjectContent);
					
			string python = "import System.";
			ExpressionResult expressionResult = new ExpressionResult("import System", new DomRegion(1, 14), null, null);
			resolveResult = resolver.Resolve(expressionResult, parseInfo, python) as NamespaceResolveResult;
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
		public void NamespaceSearchedFor()
		{
			Assert.AreEqual("System", mockProjectContent.NamespaceSearchedFor);
		}		
	}
}
