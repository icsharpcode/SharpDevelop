// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests the TestClass.RootNamespace property.
	/// </summary>
	[TestFixture]
	public class ClassRootNamespaceTests
	{
		[Test]
		public void OneParentNamespace()
		{
			MockClass mockClass = new MockClass("TestRootNamespace.MyFixture");
			mockClass.Namespace = "TestRootNamespace";
			TestClass testClass = new TestClass(mockClass);
			Assert.AreEqual("TestRootNamespace", testClass.RootNamespace);
		}
		
		[Test]
		public void TwoParentNamespaces()
		{
			MockClass mockClass = new MockClass("TestRootNamespace.Tests.MyFixture");
			mockClass.Namespace = "TestRootNamespace.Tests";
			TestClass testClass = new TestClass(mockClass);
			Assert.AreEqual("TestRootNamespace", testClass.RootNamespace);
		}
		
		[Test]
		public void NoNamespace()
		{
			MockClass mockClass = new MockClass("MyFixture");
			mockClass.Namespace = String.Empty;
			TestClass testClass = new TestClass(mockClass);
			Assert.AreEqual(String.Empty, testClass.RootNamespace);
		}
	}
}
