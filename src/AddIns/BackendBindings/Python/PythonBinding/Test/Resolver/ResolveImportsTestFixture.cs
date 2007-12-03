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
			mockProjectContent.NamespacesToAdd.Add("Test");

			// Set the dirty compilation unit and the valid compilation unit
			// so we make sure that the most recent compilation unit 
			// (i.e the dirty compilation unit) is being taken.
			parseInfo.DirtyCompilationUnit = new DefaultCompilationUnit(mockProjectContent);
			parseInfo.ValidCompilationUnit = new DefaultCompilationUnit(new MockProjectContent());
			
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
			Assert.Contains("Test", results);
		}
				
		/// <summary>
		/// Tests that the resolver handles the parse info being null
		/// </summary>
		[Test]
		public void NullParseInfo()
		{
			PythonResolver resolver = new PythonResolver();
			ArrayList results = resolver.CtrlSpace(0, 0, null, "abc", ExpressionContext.Importable);			
			Assert.AreEqual(0, results.Count);
		}
		
		/// <summary>
		/// Tests that the resolver handles the compilation units
		/// being null.
		/// </summary>
		[Test]
		public void NullCompilationUnit()
		{
			PythonResolver resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ArrayList results = resolver.CtrlSpace(0, 0, parseInfo, String.Empty, ExpressionContext.Importable);			
			Assert.AreEqual(0, results.Count);
		}
		
		[Test]
		public void ContainsSysModule()
		{
			Assert.Contains("sys", results);
		}
	}
}
