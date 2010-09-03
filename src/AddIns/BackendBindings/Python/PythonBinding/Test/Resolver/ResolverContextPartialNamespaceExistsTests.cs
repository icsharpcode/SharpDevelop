// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
