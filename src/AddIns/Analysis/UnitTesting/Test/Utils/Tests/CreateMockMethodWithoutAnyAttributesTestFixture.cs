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
	public class CreateMockMethodWithoutAnyAttributesTestFixture
	{
		MockMethod mockMethod;
		
		[SetUp]
		public void Init()
		{
			mockMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
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
		public void MethodHasNoAttributes()
		{
			Assert.AreEqual(0, mockMethod.Attributes.Count);
		}
	}
}
