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
	public class NUnitTestFrameworkIsTestMethodTests
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenMethodHasNoAttributes()
		{
			MockMethod mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsTrueWhenMethodHasTestAttributeWithoutAttributePart()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsTrueWhenMethodHasTestAttributeAttribute()
		{
			MockAttribute testAttribute = new MockAttribute("TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMethod(mockMethod));
		}

		[Test]
		public void IsTestMethodReturnsTrueWhenMethodHasFullyQualifiedNUnitTestAttribute()
		{
			MockAttribute testAttribute = new MockAttribute("NUnit.Framework.TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenMethodIsNull()
		{
			Assert.IsFalse(testFramework.IsTestMethod(null));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenProjectContentLanguageHasNullNameComparer()
		{
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockClass.MockProjectContent.Language = new LanguageProperties(null);
			MockMethod mockMethod = new MockMethod(mockClass);
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(testFramework.IsTestMethod(mockMethod));
		}
		
		/// <summary>
		/// Even if the project is null the method should be
		/// flagged as a TestMethod.
		/// </summary>
		[Test]
		public void IsTestMethodReturnsTrueWhenProjectIsNull()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			MockProjectContent mockProjectContent = (MockProjectContent)mockMethod.DeclaringType.ProjectContent;
			mockProjectContent.Project = null;

			Assert.IsTrue(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenMethodHasNullLanguage()
		{
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockClass.MockProjectContent.Language = null;
			MockMethod mockMethod = new MockMethod(mockClass);
			
			Assert.IsFalse(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenMethodHasHasParameters()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			MockParameter mockParameter = new MockParameter();
			mockMethod.Parameters.Add(mockParameter);
			
			Assert.IsFalse(testFramework.IsTestMethod(mockMethod));
		}
	}
}
