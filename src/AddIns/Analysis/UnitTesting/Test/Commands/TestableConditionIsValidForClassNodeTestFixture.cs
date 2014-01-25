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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Commands
{
	[TestFixture, Ignore("Class browser is not implemented")]
	public class TestableConditionIsValidForClassNodeTestFixture
	{
		/*
		TestableCondition testableCondition;
		MockClass classWithTestAttribute;
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
			classWithTestAttribute = MockClass.CreateMockClassWithAttribute(testAttribute);
			IProject project = classWithTestAttribute.MockProjectContent.ProjectAsIProject;
			classNodeForClassWithTestAttribute = new ClassNode(project, classWithTestAttribute);
			
			testFrameworks = new MockRegisteredTestFrameworks();
			testFrameworks.AddTestClass(classWithTestAttribute);
			
			testableCondition = new TestableCondition(testFrameworks);
		}
		
		[Test]
		public void IsValidReturnsTrueForClassWithTestAttribute()
		{
			Assert.IsTrue(testableCondition.IsValid(classNodeForClassWithTestAttribute, null));
		}
		
		[Test]
		public void IsValidReturnsFalseForClassWithoutAnyAttributes()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			IProject project = c.MockProjectContent.ProjectAsIProject;
			ClassNode classNode = new ClassNode(project, c);
			Assert.IsFalse(testableCondition.IsValid(classNode, null));
		}
		
		[Test]
		public void TestClassPassedToRegisteredTestFrameworksIsTestClass()
		{
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			IProject project = c.MockProjectContent.ProjectAsIProject;
			ClassNode classNode = new ClassNode(project, c);
			testableCondition.IsValid(classNode, null);
			Assert.AreEqual(c, testFrameworks.IsTestClassParameterUsed);
		}
		
		/// <summary>
		/// When class.ProjectContent.Project == null the 
		/// TestableCondition.IsValid should return false.
		/// </summary>
		[Test]
		public void IsValidReturnFalseWhenProjectIsNull()
		{
			classWithTestAttribute.MockProjectContent.Project = null;
			Assert.IsFalse(testableCondition.IsValid(classNodeForClassWithTestAttribute, null));
		}*/
	}
}
