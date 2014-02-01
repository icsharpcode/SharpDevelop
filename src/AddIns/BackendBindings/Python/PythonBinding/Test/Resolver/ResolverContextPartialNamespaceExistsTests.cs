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
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolverContextPartialNamespaceExistsTests
	{
		ParseInformation parseInfo;
		PythonResolverContext resolverContext;
		
		[SetUp]
		public void Init()
		{
			MockProjectContent projectContent = new MockProjectContent();
			MockProjectContent winFormsReferenceProjectContent = new MockProjectContent();
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			winFormsReferenceProjectContent.AddExistingNamespaceContents("System.Windows.Forms", namespaceItems);
			projectContent.ReferencedContents.Add(winFormsReferenceProjectContent);
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			parseInfo = new ParseInformation(unit);
			
			resolverContext = new PythonResolverContext(parseInfo);
		}
		
		[Test]
		public void PartialNamespaceExistsReturnsFalseForUnknownNamespace()
		{
			string ns = "unknown";
			Assert.IsFalse(resolverContext.PartialNamespaceExistsInProjectReferences(ns));
		}
		
		[Test]
		public void PartialNamespaceExistsReturnsTrueForCompleteSystemWinFormsNamespaceMatch()
		{
			string ns = "System.Windows.Forms";
			Assert.IsTrue(resolverContext.PartialNamespaceExistsInProjectReferences(ns));
		}
		
		[Test]
		public void PartialNamespaceExistsReturnsTrueForSystemWindowsNamespace()
		{
			string ns = "System.Windows";
			Assert.IsTrue(resolverContext.PartialNamespaceExistsInProjectReferences(ns));
		}
	}
}
