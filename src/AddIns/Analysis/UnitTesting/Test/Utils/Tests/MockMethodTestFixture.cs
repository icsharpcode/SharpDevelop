// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
