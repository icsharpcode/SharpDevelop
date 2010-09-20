// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProjectContentTestFixture
	{
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			projectContent = new MockProjectContent();
		}
		
		[Test]
		public void LanguageIsCSharpLanguage()
		{
			Assert.AreEqual(LanguageProperties.CSharp, projectContent.Language);
		}
		
		[Test]
		public void SystemTypesIsNotNull()
		{
			Assert.IsNotNull(projectContent.SystemTypes);
		}
		
		[Test]
		public void SystemObjectReturnTypeReturnedFromSystemTypesHasSystemObjectFullyQualifiedName()
		{
			string expectedName = "System.Object";
			IReturnType returnType = projectContent.SystemTypes.Object;
			Assert.AreEqual(expectedName, returnType.FullyQualifiedName);
		}
		
		[Test]
		public void GetClassWithTypeParameterCountReturnsMockClassWithSpecifiedName()
		{
			string expectedName = "MyNamespace.MyTestClass";
			MockClass c = projectContent.GetClass(expectedName, 0) as MockClass;
			Assert.AreEqual(expectedName, c.FullyQualifiedName);
		}
	}
}
