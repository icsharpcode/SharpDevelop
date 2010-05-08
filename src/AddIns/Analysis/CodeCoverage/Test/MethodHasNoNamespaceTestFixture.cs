// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;

namespace ICSharpCode.CodeCoverage.Tests
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
