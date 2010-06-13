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
	public class GallioTestFrameworkIsTestClassTests
	{
		GallioTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			testFramework = new GallioTestFramework();
		}
		
		[Test]
		public void IsTestClassReturnsFalseHasClassHasNoAttributes()
		{
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttributeMissingAttributePart()
		{
			MockAttribute testAttribute = new MockAttribute("TestFixture");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasTestFixtureAttribute()
		{
			MockAttribute testFixtureAttribute = new MockAttribute("TestFixtureAttribute");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testFixtureAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsTrueHasClassHasFullyQualifiedNUnitTestFixtureAttribute()
		{
			MockAttribute testFixtureAttribute = new MockAttribute("MbUnit.Framework.TestFixtureAttribute");
			MockClass mockClass = MockClass.CreateMockClassWithAttribute(testFixtureAttribute);
			Assert.IsTrue(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenClassIsNull()
		{
			Assert.IsFalse(testFramework.IsTestClass(null));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenProjectContentLanguageIsNull()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			MockClass mockClass = new MockClass(mockProjectContent);
			
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
		
		[Test]
		public void IsTestClassReturnsFalseWhenProjectContentLanguageNameComparerIsNull()
		{
			IProject project = new MockCSharpProject();
			MockProjectContent mockProjectContent = new MockProjectContent();
			mockProjectContent.Project = project;
			mockProjectContent.Language = new LanguageProperties(null);
			MockClass mockClass = new MockClass(mockProjectContent);
			mockClass.Attributes.Add(new MockAttribute("Test"));
			
			Assert.IsFalse(testFramework.IsTestClass(mockClass));
		}
	}
}
