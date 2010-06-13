// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateMockClassWithoutAnyAttributesTestFixture
	{
		MockClass mockClass;
		
		[SetUp]
		public void Init()
		{
			mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
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
		public void ClassHasNoAttributes()
		{
			Assert.AreEqual(0, mockClass.Attributes.Count);
		}
		
		[Test]
		public void ProjectIsMockCSharpProject()
		{
			Assert.IsNotNull(mockClass.Project);
		}
		
		[Test]
		public void ProjectContentContainsMockClass()
		{
			Assert.IsTrue(mockClass.ProjectContent.Classes.Contains(mockClass));
		}
	}
}
