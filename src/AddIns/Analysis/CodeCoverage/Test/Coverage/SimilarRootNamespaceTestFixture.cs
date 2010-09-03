// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// If there are two namespaces that start the same and match up
	/// to at least the start of the dot character then the 
	/// CodeCoverageMethod.GetChildNamespaces fails to correctly identify
	/// child namespaces.
	/// 
	/// For example:
	/// 
	/// Root.Tests
	/// RootBar
	/// 
	/// If we look for child namespaces using the string "Root" the
	/// code should only return "Tests", but it will also return
	/// "Bar" due to a bug matching only the start of the class namespace
	/// without taking into account the dot character.
	/// </summary>
	[TestFixture]
	public class SimilarRootNamespaceTestFixture
	{
		List<string> childNamespaces = new List<string>();
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			CodeCoverageModule module = new CodeCoverageModule("Root.Tests");
		
			// Add two methods in namespaces that start with the
			// same initial characters.
			CodeCoverageMethod rootTestsMethod = new CodeCoverageMethod("RunThis", "Root.Tests.MyTestFixture");
			module.Methods.Add(rootTestsMethod);
			CodeCoverageMethod rootBarMethod = new CodeCoverageMethod("RunThis", "RootBar.MyTestFixture");
			module.Methods.Add(rootBarMethod);
			
			childNamespaces = CodeCoverageMethod.GetChildNamespaces(module.Methods, "Root");	
		}
		
		[Test]
		public void RootNamespaceHasOneChildNamespace()
		{
			Assert.AreEqual(1, childNamespaces.Count);
		}
		
		[Test]
		public void RootNamespaceChildNamespaceIsTests()
		{
			Assert.AreEqual("Tests", childNamespaces[0]);
		}
	}
}
