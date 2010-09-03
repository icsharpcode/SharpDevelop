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
	public class CreateMockMethodWithSingleAttributeTestFixture
	{
		MockMethod mockMethod;
		MockAttribute firstAttribute;
		
		[SetUp]
		public void Init()
		{
			firstAttribute = new MockAttribute("first");
			mockMethod = MockMethod.CreateMockMethodWithAttribute(firstAttribute);
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
		public void MethodHasOneAttribute()
		{
			Assert.AreEqual(1, mockMethod.Attributes.Count);
		}
		
		[Test]
		public void FirstClassAttributeHasAttributeTypeWithFullyQualifiedNameOfFirst()
		{
			Assert.AreEqual("first", mockMethod.Attributes[0].AttributeType.FullyQualifiedName);
		}
	}
}
