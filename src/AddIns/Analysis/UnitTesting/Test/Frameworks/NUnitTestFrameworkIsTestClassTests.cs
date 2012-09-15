// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Frameworks
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestClassTests
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void IsTestClassReturnsFalseHasClassHasNoAttributes()
		{
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttributeMissingAttributePart()
		{
			MockAttribute testAttribute = new MockAttribute("TestFixture");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassHasTestFixtureAttributeAndIsAbstract() {
			MockAttribute testAttribute = new MockAttribute("TestFixture");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testAttribute);
			mockClass.Modifiers = mockClass.Modifiers | ModifierEnum.Abstract;
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttribute()
		{
			MockAttribute testFixtureAttribute = new MockAttribute("TestFixtureAttribute");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testFixtureAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasFullyQualifiedNUnitTestFixtureAttribute()
		{
			MockAttribute testFixtureAttribute = new MockAttribute("NUnit.Framework.TestFixtureAttribute");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testFixtureAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassIsNull()
		{
			Assert.IsFalse(testFramework.IsTestClass(null));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenProjectContentLanguageIsNull()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
			
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenProjectContentLanguageNameComparerIsNull()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			MockClass mockClass = new MockClass(mockProjectContent);
			mockClass.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
	}
}
