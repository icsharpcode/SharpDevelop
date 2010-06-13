// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Gallio.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioTestFrameworkIsTestMethodTests
	{
		GallioTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new GallioTestFramework();
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
			MockAttribute testAttribute = new MockAttribute("MbUnit.Framework.TestAttribute");
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
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			MockClass mockClass = new MockClass(mockProjectContent);
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
		public void IsTestMethodReturnsFalseWhenMethodHasNullDeclaringType()
		{
			MockMethod mockMethod = new MockMethod(new MockClass());
			
			Assert.IsFalse(testFramework.IsTestMethod(mockMethod));
		}
		
		[Test]
		public void IsTestMethodReturnsFalseWhenMethodHasNullLanguage()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
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
