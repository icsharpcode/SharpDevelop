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
	public class GallioTestFrameworkIsTestMemberTests
	{
		GallioTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new GallioTestFramework();
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenMethodHasNoAttributes()
		{
			MockMethod mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsTrueWhenMethodHasTestAttributeWithoutAttributePart()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsTrueWhenMethodHasTestAttributeAttribute()
		{
			MockAttribute testAttribute = new MockAttribute("TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMember(mockMethod));
		}

		[Test]
		public void IsTestMemberReturnsTrueWhenMethodHasFullyQualifiedNUnitTestAttribute()
		{
			MockAttribute testAttribute = new MockAttribute("MbUnit.Framework.TestAttribute");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenMethodIsNull()
		{
			Assert.IsFalse(testFramework.IsTestMember(null));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenProjectContentLanguageHasNullNameComparer()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			MockClass mockClass = new MockClass(mockProjectContent);
			MockMethod mockMethod = new MockMethod(mockClass);
			mockMethod.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(testFramework.IsTestMember(mockMethod));
		}
		
		/// <summary>
		/// Even if the project is null the method should be
		/// flagged as a TestMethod.
		/// </summary>
		[Test]
		public void IsTestMemberReturnsTrueWhenProjectIsNull()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			MockProjectContent mockProjectContent = (MockProjectContent)mockMethod.DeclaringType.ProjectContent;
			mockProjectContent.Project = null;

			Assert.IsTrue(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenMethodHasNullDeclaringType()
		{
			MockMethod mockMethod = new MockMethod(new MockClass());
			
			Assert.IsFalse(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenMethodHasNullLanguage()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
			MockMethod mockMethod = new MockMethod(mockClass);
			
			Assert.IsFalse(testFramework.IsTestMember(mockMethod));
		}
		
		[Test]
		public void IsTestMemberReturnsFalseWhenMethodHasHasParameters()
		{
			MockAttribute testAttribute = new MockAttribute("Test");
			MockMethod mockMethod = MockMethod.CreateMockMethodWithAttribute(testAttribute);
			MockParameter mockParameter = new MockParameter();
			mockMethod.Parameters.Add(mockParameter);
			
			Assert.IsFalse(testFramework.IsTestMember(mockMethod));
		}
	}
}
