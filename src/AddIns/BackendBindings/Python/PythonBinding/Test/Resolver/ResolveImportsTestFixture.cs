// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		List<ICompletionEntry> results;
		PythonResolver resolver;
		MockProjectContent mockProjectContent;		
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.NamespacesToAdd.Add("Test");

			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(mockProjectContent));
			
			results = resolver.CtrlSpace(0, "import".Length, parseInfo, "import", ExpressionContext.Importable);
		}
				
		[Test]
		public void ProjectContentAddNamespaceContentsCalled()
		{
			Assert.IsTrue(mockProjectContent.AddNamespaceContentsCalled);
		}
		
		[Test]
		public void NamespaceName()
		{
			Assert.AreEqual(String.Empty, mockProjectContent.NamespaceAddedName);
		}
		
		[Test]
		public void LookInReferencesIsTrue()
		{
			Assert.IsTrue(mockProjectContent.LookInReferences);
		}
		
		[Test]
		public void ProjectContentLanguagePassedToAddNamespaceContents()
		{
			Assert.AreSame(mockProjectContent.Language, mockProjectContent.LanguagePassedToAddNamespaceContents);
		}
		
		[Test]
		public void TestNamespaceAdded()
		{
			Assert.Contains(new NamespaceEntry("Test"), results);
		}
				
		/// <summary>
		/// Tests that the resolver handles the parse info being null
		/// </summary>
		[Test]
		public void NullParseInfo()
		{
			PythonResolver resolver = new PythonResolver();
			List<ICompletionEntry> results = resolver.CtrlSpace(0, 0, null, "abc", ExpressionContext.Importable);			
			Assert.AreEqual(0, results.Count);
		}
		
		[Test]
		public void ContainsSysModule()
		{
			Assert.Contains(new NamespaceEntry("sys"), results);
		}
	}
}
