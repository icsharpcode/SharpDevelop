// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	[TestFixture]
	public class ClassWithTwoChildNamespacesTestsFixture
	{
		TestClass c;
		MockTestFrameworksWithNUnitFrameworkSupport testFrameworks;
		
		[SetUp]
		public void Init()
		{
			testFrameworks = new MockTestFrameworksWithNUnitFrameworkSupport();
			MockClass mockClass = new MockClass("UnitTesting.Tests.MyTestFixture");
			c = new TestClass(mockClass, testFrameworks);
		}
		
		[Test]
		public void FullNamespace()
		{
			Assert.AreEqual("UnitTesting.Tests", c.GetChildNamespace(String.Empty));
		}
		
		[Test]
		public void FirstChildNamespace()
		{
			Assert.AreEqual("Tests", c.GetChildNamespace("UnitTesting"));
		}
		
		[Test]
		public void EmptyChildNamespace()
		{
			Assert.AreEqual(String.Empty, c.GetChildNamespace("UnitTesting.Tests"));
		}
		
		[Test]
		public void UnknownChildNamespace()
		{
			Assert.AreEqual(String.Empty, c.GetChildNamespace("NotKnown"));
		}
	}
}
