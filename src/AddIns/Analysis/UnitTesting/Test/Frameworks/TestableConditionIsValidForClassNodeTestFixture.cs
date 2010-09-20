// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class TestableConditionIsValidForClassNodeTestFixture
	{
		TestableCondition testableCondition;
		MockClass classWithTestAttribute;
		ClassNode classNodeForClassWithTestAttribute;
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
		}
	}
}
