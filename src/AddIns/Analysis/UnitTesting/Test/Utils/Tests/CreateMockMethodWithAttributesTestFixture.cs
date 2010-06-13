// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateMockMethodWithAttributesTestFixture
	{
		MockMethod mockMethod;
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
			
			mockMethod = MockMethod.CreateMockMethodWithAttributes(attributes);
		}
		
		[Test]
		public void DeclaringTypeProjectContentLanguageIsCSharp()
		{
			Assert.AreEqual(LanguageProperties.CSharp, mockMethod.DeclaringType.ProjectContent.Language);
		}
		
		[Test]
		public void ProjectContentProjectIsMockCSharpProject()
		{
			Assert.IsNotNull(mockMethod.DeclaringType.ProjectContent.Project as MockCSharpProject);
		}
		
		[Test]
		public void MethodHasTwoAttributes()
		{
			Assert.AreEqual(2, mockMethod.Attributes.Count);
		}
		
		[Test]
		public void FirstClassAttributeHasAttributeTypeWithFullyQualifiedNameOfFirst()
		{
			Assert.AreEqual("first", mockMethod.Attributes[0].AttributeType.FullyQualifiedName);
		}
		
		[Test]
		public void SecondClassAttributeHasAttributeTypeWithFullyQualifiedNameOfSeocnd()
		{
			Assert.AreEqual("second", mockMethod.Attributes[1].AttributeType.FullyQualifiedName);
		}
		
		[Test]
		public void MethodDeclaringTypeHasCompilationUnit()
		{
			Assert.IsNotNull(mockMethod.DeclaringType.CompilationUnit);
		}
	}
}
