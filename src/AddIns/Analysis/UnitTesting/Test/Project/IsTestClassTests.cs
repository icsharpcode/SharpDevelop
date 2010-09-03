// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Project
{
	/// <summary>
	/// Tests the TestClass.IsTestClass method.
	/// </summary>
	[TestFixture]
	public class IsTestClassTests
	{
		[Test]
		public void HasNoAttributes()
		{
			MockClass mockClass = CreateMockClass();
			Assert.IsFalse(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void HasTestFixtureAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("TestFixture"));
			MockClass mockClass = CreateMockClass(attributes);
			Assert.IsTrue(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void HasTestFixtureAttributeAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("TestFixtureAttribute"));
			MockClass mockClass = CreateMockClass(attributes);
			Assert.IsTrue(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void HasNUnitTestFixtureAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("NUnit.Framework.TestFixtureAttribute"));
			MockClass mockClass = CreateMockClass(attributes);
			Assert.IsTrue(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void NullClass()
		{
			Assert.IsFalse(TestClass.IsTestClass(null));
		}
		
		[Test]
		public void NullLanguage()
		{
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			
			Assert.IsFalse(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void NullNameComparer()
		{
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			mockClass.ProjectContent = mockProjectContent;
			mockClass.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(TestClass.IsTestClass(mockClass));
		}
		
		[Test]
		public void GetNameComparerWithNullClass()
		{
			Assert.IsNull(TestClass.GetNameComparer(null));
		}
		
		static MockClass CreateMockClass()
		{
			return CreateMockClass(new MockAttribute[0]);
		}
		
		static MockClass CreateMockClass(IList<MockAttribute> attributes)
		{
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Language = LanguageProperties.None;
			mockProjectContent.Project = new MockCSharpProject();
			mockClass.ProjectContent = mockProjectContent;
			
			foreach (MockAttribute attribute in attributes) {
				mockClass.Attributes.Add(attribute);
			}
		
			return mockClass;
		}
	}
}
