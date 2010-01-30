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
	/// Tests that the contents of the imported namespaces are added
	/// to the completion data list when the user presses Ctrl+Space
	/// in a class.
	/// </summary>
	[TestFixture]
	public class NamespaceContentsAddedToCtrlSpaceTestFixture
	{
		ArrayList results;
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		MockClass myTestClass;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			ParseInformation parseInfo = new ParseInformation();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.NamespacesToAdd.Add("Test");
			myTestClass = new MockClass(mockProjectContent, "MyTestClass");
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add(myTestClass);
			mockProjectContent.AddExistingNamespaceContents("MyNamespace", namespaceItems);

			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent) { ErrorsDuringCompile = true };
			parseInfo.SetCompilationUnit(cu);
			
			// Add usings.
			DefaultUsing newUsing = new DefaultUsing(cu.ProjectContent);
			newUsing.Usings.Add("MyNamespace");
			cu.UsingScope.Usings.Add(newUsing);

			results = resolver.CtrlSpace(0, "".Length, parseInfo, "", ExpressionContext.Default);
		}

		[Test]
		public void MyTestClassReturnedInResults()
		{
			Assert.Contains(myTestClass, results);
		}
		
		[Test]
		public void NamespaceSearchedForContents()
		{
			Assert.AreEqual("MyNamespace", mockProjectContent.NamespacePassedToGetNamespaceContentsMethod);
		}
		
		[Test]
		public void TwoResultsReturned()
		{
			Assert.AreEqual(2, results.Count);
		}

		[Test]
		public void TestNamespaceReturnedInResults()
		{
			Assert.Contains("Test", results);
		}
	}
}
