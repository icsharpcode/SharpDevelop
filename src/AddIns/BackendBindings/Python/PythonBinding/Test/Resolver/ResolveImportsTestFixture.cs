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
	/// Tests the PythonResolver returns the correct namespaces for
	/// an import statement when the user presses Ctrl+Space.
	/// </summary>
	[TestFixture]
	public class ResolveImportsTestFixture
	{
		ArrayList results;
		PythonResolver resolver;
		MockProjectContent mockProjectContent;		
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add("Test");
			mockProjectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);

			// Set the dirty compilation unit and the valid compilation unit
			// so we make sure that the most recent compilation unit 
			// (i.e the dirty compilation unit) is being taken.
			parseInfo.SetCompilationUnit(new DefaultCompilationUnit(new MockProjectContent()));
			parseInfo.SetCompilationUnit(new DefaultCompilationUnit(mockProjectContent));
			
			results = resolver.CtrlSpace(0, "import".Length, parseInfo, "import", ExpressionContext.Namespace);
		}
		
		[Test]
		public void NamespaceName()
		{
			Assert.AreEqual(String.Empty, mockProjectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
				
		[Test]
		public void TestNamespaceAdded()
		{
			Assert.Contains("Test", results);
		}
		
		[Test]
		public void ContainsSysModule()
		{
			Assert.Contains("sys", results);
		}
	}
}
