// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
