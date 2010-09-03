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
	/// Tests the TestMethod.IsTestMethod method.
	/// </summary>
	[TestFixture]
	public class IsTestMethodTests
	{
		[Test]
		public void HasNoAttributes()
		{
			MockMethod mockMethod = CreateMockMethod();
			Assert.IsFalse(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void HasTestAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("Test"));
			MockMethod mockMethod = CreateMockMethod(attributes);
			Assert.IsTrue(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void HasTestAttributeAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("TestAttribute"));
			MockMethod mockMethod = CreateMockMethod(attributes);
			Assert.IsTrue(TestMethod.IsTestMethod(mockMethod));
		}

		[Test]
		public void HasNUnitTestAttribute()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("NUnit.Framework.TestAttribute"));
			MockMethod mockMethod = CreateMockMethod(attributes);
			Assert.IsTrue(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void NullMethod()
		{
			Assert.IsFalse(TestMethod.IsTestMethod(null));
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
			MockMethod mockMethod = new MockMethod();
			mockMethod.DeclaringType = mockClass;
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(TestMethod.IsTestMethod(mockMethod));
		}
		
		/// <summary>
		/// Even if the project is null the method should be
		/// flagged as a TestMethod.
		/// </summary>
		[Test]
		public void NullProject()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("Test"));
			MockMethod mockMethod = CreateMockMethod(attributes);
			MockProjectContent mockProjectContent = (MockProjectContent)mockMethod.DeclaringType.ProjectContent;
			mockProjectContent.Project = null;

			Assert.IsTrue(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void NullDeclaringType()
		{
			MockMethod mockMethod = new MockMethod();
			
			Assert.IsFalse(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void NullLanguage()
		{
			IProject project = new MockCSharpProject();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			MockMethod mockMethod = new MockMethod();
			mockMethod.DeclaringType = mockClass;
			
			Assert.IsFalse(TestMethod.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void MethodHasParameters()
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(new MockAttribute("Test"));
			MockMethod mockMethod = CreateMockMethod(attributes);
			MockParameter mockParameter = new MockParameter();
			mockMethod.Parameters.Add(mockParameter);
			
			Assert.IsFalse(TestMethod.IsTestMethod(mockMethod));
		}
		
		static MockMethod CreateMockMethod()
		{
			return CreateMockMethod(new MockAttribute[0]);
		}

		static MockMethod CreateMockMethod(IList<MockAttribute> attributes)
		{
			MockMethod mockMethod = new MockMethod();
			MockClass mockClass = new MockClass();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Language = LanguageProperties.None;
			IProject project = new MockCSharpProject();
			mockProjectContent.Project = project;
			mockClass.ProjectContent = mockProjectContent;
			mockMethod.DeclaringType = mockClass;
			
			foreach (MockAttribute attribute in attributes) {
				mockMethod.Attributes.Add(attribute);
			}
			
			return mockMethod;
		}
	}
}
