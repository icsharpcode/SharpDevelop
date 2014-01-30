// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
