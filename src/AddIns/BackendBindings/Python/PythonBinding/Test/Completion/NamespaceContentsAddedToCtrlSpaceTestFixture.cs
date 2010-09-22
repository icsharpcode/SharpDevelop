// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

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
		ICSharpCode.Scripting.Tests.Utils.MockProjectContent mockProjectContent;
		MockClass myTestClass;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resolver = new PythonResolver();
			mockProjectContent = new ICSharpCode.Scripting.Tests.Utils.MockProjectContent();
			mockProjectContent.NamespacesToAdd.Add("Test");
			myTestClass = new MockClass(mockProjectContent, "MyTestClass");
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(myTestClass);
			mockProjectContent.AddExistingNamespaceContents("MyNamespace", namespaceItems);
			
			DefaultCompilationUnit cu = new DefaultCompilationUnit(mockProjectContent);
			
			// Add usings.
			DefaultUsing newUsing = new DefaultUsing(cu.ProjectContent);
			newUsing.Usings.Add("MyNamespace");
			DefaultUsingScope usingScope = new DefaultUsingScope();
			usingScope.Usings.Add(newUsing);
			cu.UsingScope = usingScope;
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
			Assert.Contains(new NamespaceEntry("Test"), results);
		}
	}
}
