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
	/// Tests that the contents of the imported namespaces are added
	/// to the completion data list when the user presses Ctrl+Space
	/// in a class.
	/// </summary>
	[TestFixture]
	public class NamespaceContentsAddedToCtrlSpaceTestFixture
	{
		List<ICompletionEntry> results;
		PythonResolver resolver;
		MockProjectContent mockProjectContent;
		MockClass myTestClass;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new MockProjectContent();
			mockProjectContent.NamespacesToAdd.Add("Test");
			myTestClass = new MockClass(mockProjectContent, "MyTestClass");
			mockProjectContent.NamespaceContentsToReturn.Add(myTestClass);

			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent) { ErrorsDuringCompile = true };
			
			// Add usings.
			DefaultUsing newUsing = new DefaultUsing(cu.ProjectContent);
			newUsing.Usings.Add("MyNamespace");
			cu.UsingScope.Usings.Add(newUsing);
			
			ParseInformation parseInfo = new ParseInformation(cu);

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
			Assert.AreEqual("MyNamespace", mockProjectContent.NamespaceContentsSearched);
		}
		
		[Test]
		public void TwoResultsReturned()
		{
			Assert.AreEqual(2, results.Count);
		}

		[Test]
		public void TestNamespaceReturnedInResults()
		{
			Assert.Contains(new NamespaceEntry("Test"), results);
		}
	}
}
