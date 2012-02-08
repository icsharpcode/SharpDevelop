// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			testFrameworks.AddTestMember(methodWithTestAttribute);
			
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
			Assert.AreEqual(method, testFrameworks.IsTestMemberParameterUsed);
		}
	}
}
