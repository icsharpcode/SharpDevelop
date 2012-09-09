// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Project;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestMemberTests : ProjectTestFixtureBase
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		IMethod SimpleMethod {
			get {
				ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
				return myClass.Methods.Single(m => m.Name == "SimpleMethod");
			}
		}
		
		[Test]
		public void IsTestMember_MethodHasNoAttributes_ReturnsFalse()
		{
			CreateProject(testFramework, Parse("class MyClass { public void SimpleMethod() {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeWithoutAttributePart_ReturnsTrue()
		{
			CreateProject(testFramework, Parse("using NUnit.Framework; class MyClass { [Test] public void SimpleMethod() {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeAttribute_ReturnsTrue()
		{
			CreateProject(testFramework, Parse("using NUnit.Framework; class MyClass { [TestAttribute] public void SimpleMethod() {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsTrue(result);
		}

		[Test]
		public void IsTestMember_MethodHasFullyQualifiedNUnitTestAttribute_ReturnsTrue()
		{
			CreateProject(testFramework, Parse("class MyClass { [NUnit.Framework.Test] public void SimpleMethod() {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodIsNull_ReturnsFalse()
		{
			bool result = testFramework.IsTestMember(null);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasParameters_ReturnsFalse()
		{
			CreateProject(testFramework, Parse("class MyClass { [NUnit.Framework.Test] public void SimpleMethod(int a) {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_TestCase_With_Parameters()
		{
			CreateProject(testFramework, Parse("class MyClass { [NUnit.Framework.TestCase(10)] public void SimpleMethod(int a) {} }"));
			bool result = testFramework.IsTestMember(SimpleMethod);
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_FieldHasOneAttribute_ReturnsFalseAndDoesNotThrowInvalidCastException()
		{
			CreateProject(testFramework, Parse("class MyClass { [NUnit.Framework.Test] public int MyField; }"));
			ITypeDefinition myClass = projectContent.CreateCompilation().MainAssembly.GetTypeDefinition("", "MyClass", 0);
			bool result = testFramework.IsTestMember(myClass.Fields.Single());
			Assert.IsFalse(result);
		}
	}
}
