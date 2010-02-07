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
			winFormsReferenceProjectContent.AddExistingNamespaceContents("System.Windows.Forms", new ArrayList());
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
