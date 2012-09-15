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
	public class NUnitTestFrameworkIsTestMemberTests
	{
		NUnitTestFramework testFramework;
	
		void CreateTestFramework()
		{
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void IsTestMember_MethodHasNoAttributes_ReturnsFalse()
		{
			CreateTestFramework();
			MockMethod mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeWithoutAttributePart_ReturnsTrue()
		{
			CreateTestFramework();
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasTestAttributeAttribute_ReturnsTrue()
		{
			CreateTestFramework();
			MockAttribute testAttribute = new MockAttribute("TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsTrue(result);
		}

		[Test]
		public void IsTestMember_MethodHasFullyQualifiedNUnitTestAttribute_ReturnsTrue()
		{
			CreateTestFramework();
			MockAttribute testAttribute = new MockAttribute("NUnit.Framework.TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodIsNull_ReturnsFalse()
		{
			CreateTestFramework();
			bool result = testFramework.IsTestMember(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_ProjectContentLanguageHasNullNameComparer_ReturnsFalse()
		{
			CreateTestFramework();
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockClass.MockProjectContent.Language = new LanguageProperties(null);
			var mockMethod = new MockMethod(mockClass);
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsFalse(result);
		}
		
		/// <summary>
		/// Even if the project is null the method should be
		/// flagged as a TestMethod.
		/// </summary>
		[Test]
		public void IsTestMember_ProjectIsNull_ReturnsTrue()
		{
			CreateTestFramework();
			var testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			MockProjectContent mockProjectContent = (MockProjectContent)mockMethod.DeclaringType.ProjectContent;
			mockProjectContent.Project = null;
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasNullLanguage_ReturnsFalse()
		{
			CreateTestFramework();
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			mockClass.MockProjectContent.Language = null;
			var mockMethod = new MockMethod(mockClass);
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_MethodHasParameters_ReturnsFalse()
		{
			CreateTestFramework();
			var testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			var mockParameter = new MockParameter();
			mockMethod.Parameters.Add(mockParameter);
			
			bool result = testFramework.IsTestMember(mockMethod);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void IsTestMember_FieldHasOneAttribute_ReturnsFalseAndDoesNotThrowInvalidCastException()
		{
			CreateTestFramework();
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			var field = new DefaultField(mockClass, "MyField");
			var testAttribute = new MockAttribute("Test");
			field.Attributes.Add(testAttribute);
			
			bool result = testFramework.IsTestMember(field);
			
			Assert.IsFalse(result);
		}
	}
}
