// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestableConditionIsValidForMemberNodeTestFixture
	{
		TestableCondition testableCondition;
		MockMethod methodWithTestAttribute;
		MockMemberNode memberNodeForMethodWithTestAttribute;
		MockRegisteredTestFrameworks testFrameworks;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager.Initialize();
		}

		[SetUp]
		public void Init()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			methodWithTestAttribute = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			memberNodeForMethodWithTestAttribute = new MockMemberNode(methodWithTestAttribute);
			
			testFrameworks = new MockRegisteredTestFrameworks();
			testFrameworks.AddTestMethod(methodWithTestAttribute);
			
			testableCondition = new TestableCondition(testFrameworks);
		}
		
		[Test]
		public void IsValidReturnsTrueForMethodWithTestAttribute()
		{
			Assert.IsTrue(testableCondition.IsValid(memberNodeForMethodWithTestAttribute, null));
		}
		
		[Test]
		public void IsValidReturnsFalseForMethodWithoutAnyAttributes()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			MockMemberNode memberNode = new MockMemberNode(method);
			Assert.IsFalse(testableCondition.IsValid(memberNode, null));
		}
		
		[Test]
		public void TestMethodPassedToRegisteredTestFrameworksIsTestMethod()
		{
			MockMethod method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			MockMemberNode memberNode = new MockMemberNode(method);
			testableCondition.IsValid(memberNode, null);
			Assert.AreEqual(method, testFrameworks.IsTestMethodMemberParameterUsed);
		}
	}
}
