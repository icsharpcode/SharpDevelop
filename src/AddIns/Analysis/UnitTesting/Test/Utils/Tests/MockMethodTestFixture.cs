// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockMethodTestFixture
	{
		[Test]
		public void DeclaringTypeReturnsExpectedClassWhenMethodCreated()
		{
			MockClass declaringType = new MockClass();
			MockMethod method = new MockMethod(declaringType);
			Assert.AreEqual(declaringType, method.DeclaringType);
		}
		
		[Test]
		public void NameReturnsExpectedMethodNameWhenMethodCreated()
		{
			MockClass declaringType = new MockClass();
			MockMethod method = new MockMethod(declaringType, "MyMethod");
			Assert.AreEqual("MyMethod", method.Name);
		}
		
		[Test]
		public void FullyQualifiedNameReturnsFullyQualifiedMethodName()
		{
			MockClass declaringType = new MockClass("MyNamespace.MyClass");
			MockMethod method = new MockMethod(declaringType, "MyMethod");
			string expectedName = "MyNamespace.MyClass.MyMethod";
			Assert.AreEqual(expectedName, method.FullyQualifiedName);
		}
	}
}
