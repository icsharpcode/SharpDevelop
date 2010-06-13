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
	public class CreateMockClassWithSingleAttributeTestFixture
	{
		MockClass mockClass;
		MockAttribute firstAttribute;
		
		[SetUp]
		public void Init()
		{
			firstAttribute = new MockAttribute("first");
			mockClass = MockClass.CreateMockClassWithAttribute(firstAttribute);
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
		public void ClassHasOneAttribute()
		{
			Assert.AreEqual(1, mockClass.Attributes.Count);
		}
		
		[Test]
		public void FirstClassAttributeHasAttributeTypeWithFullyQualifiedNameOfFirst()
		{
			Assert.AreEqual("first", mockClass.Attributes[0].AttributeType.FullyQualifiedName);
		}
	}
}
