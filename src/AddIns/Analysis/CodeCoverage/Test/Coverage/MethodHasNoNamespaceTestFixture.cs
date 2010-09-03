// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Checks that the CodeCoverageMethod class can handle a class name
	/// that has no namespace.
	/// </summary>
	[TestFixture]
	public class MethodHasNoNamespaceTestFixture
	{
		CodeCoverageMethod method;
		
		[SetUp]
		public void Init()
		{
			string methodName = "Foo";
			string className = "Foo";
			
			method = new CodeCoverageMethod(methodName, className);
		}
		
		[Test]
		public void ClassName()
		{
			Assert.AreEqual("Foo", method.ClassName);
		}
		
		[Test]
		public void MethodNamespace()
		{
			Assert.AreEqual(String.Empty, method.ClassNamespace);
		}
	}
}
