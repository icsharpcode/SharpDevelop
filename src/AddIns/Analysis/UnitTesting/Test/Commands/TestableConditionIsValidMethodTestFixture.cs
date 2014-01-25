// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Commands
{
	[TestFixture, Ignore("Class browser is not implemented")]
	public class TestableConditionIsValidForMemberNodeTestFixture
	{
		/*
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
		}*/
	}
}
