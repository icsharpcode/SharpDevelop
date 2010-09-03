// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateMockClassWithAttributesTestFixture
	{
		MockClass mockClass;
		MockAttribute firstAttribute;
		MockAttribute secondAttribute;
		
		[SetUp]
		public void Init()
		{
			firstAttribute = new MockAttribute("first");
			secondAttribute = new MockAttribute("second");
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(firstAttribute);
			attributes.Add(secondAttribute);
			
			mockClass = MockClass.CreateMockClassWithAttributes(attributes);
		}
		
		[Test]
		public void ProjectContentLanguageIsCSharp()
		{
			Assert.AreEqual(LanguageProperties.CSharp, mockClass.ProjectContent.Language);
		}
		
		[Test]
		public void ProjectContentProjectIsMockCSharpProject()
		{
			Assert.IsNotNull(mockClass.ProjectContent.Project as MockCSharpProject);
		}
		
		[Test]
		public void ClassHasTwoAttributes()
		{
			Assert.AreEqual(2, mockClass.Attributes.Count);
		}
		
		[Test]
		public void FirstClassAttributeHasAttributeTypeWithFullyQualifiedNameOfFirst()
		{
			Assert.AreEqual("first", mockClass.Attributes[0].AttributeType.FullyQualifiedName);
		}
		
		[Test]
		public void SecondClassAttributeHasAttributeTypeWithFullyQualifiedNameOfSeocnd()
		{
			Assert.AreEqual("second", mockClass.Attributes[1].AttributeType.FullyQualifiedName);
		}
		
		[Test]
		public void ProjectContentContainsMockClass()
		{
			Assert.IsTrue(mockClass.ProjectContent.Classes.Contains(mockClass));
		}
		
		[Test]
		public void CompoundClassIsSameAsMockClass()
		{
			Assert.AreEqual(mockClass, mockClass.GetCompoundClass());
		}
		
		[Test]
		public void MockClassHasDefaultReturnType()
		{
			Assert.AreEqual(mockClass, mockClass.DefaultReturnType.GetUnderlyingClass());
		}
		
		[Test]
		public void ClassHasCompilationUnit()
		{
			Assert.IsNotNull(mockClass.CompilationUnit);
		}
		
		[Test]
		public void ClassCompilationUnitHasSameProjectContentAsClass()
		{
			Assert.AreEqual(mockClass.ProjectContent, mockClass.CompilationUnit.ProjectContent);
		}
	}
}
