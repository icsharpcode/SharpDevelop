// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockProjectSaveTestFixture
	{
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			project.FileName = @"e:\projects\myproj.csproj";
		}
		
		[Test]
		public void IsSavedReturnsFalseByDefault()
		{
			Assert.IsFalse(project.IsSaved);
		}
		
		[Test]
		public void IsSavedReturnsTrueAfterSaveMethodCalled()
		{
			project.Save();
			Assert.IsTrue(project.IsSaved);
		}
	}
}
