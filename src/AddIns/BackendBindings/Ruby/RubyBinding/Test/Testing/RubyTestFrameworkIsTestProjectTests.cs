// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using RubyBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Testing
{
	[TestFixture]
	public class RubyTestFrameworkIsTestProjectTests
	{
		RubyTestFramework testFramework;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			RubyMSBuildEngineHelper.InitMSBuildEngine();
		}
		
		[SetUp]
		public void Init()
		{
			testFramework = new RubyTestFramework();
		}
		
		[Test]
		public void IsTestProjectWhenPassedNullProjectReturnsFalse()
		{
			Assert.IsFalse(testFramework.IsTestProject(null));
		}
		
		[Test]
		public void IsTestProjectWhenPassedRubyPythonProjectReturnsTrue()
		{
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
			createInfo.OutputProjectFileName = @"C:\projects\test.rbproj";
			RubyProject project = new RubyProject(createInfo);
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectWhenPassedNonPythonProjectReturnsFalse()
		{
			MockProject project = new MockProject();
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectWhenPassedNullRubyProjectReturnsFalse()
		{
			RubyProject project = null;
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
	}
}
