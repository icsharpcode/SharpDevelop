// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(new NamespaceEntry("Test"));
			mockProjectContent.AddExistingNamespaceContents(String.Empty, namespaceItems);

			ParseInformation parseInfo = new ParseInformation(new DefaultCompilationUnit(mockProjectContent));
			
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
			Assert.Contains(new NamespaceEntry("Test"), results);
		}
		
		[Test]
		public void ContainsSysModule()
		{
			Assert.Contains(new NamespaceEntry("sys"), results);
		}
	}
}
