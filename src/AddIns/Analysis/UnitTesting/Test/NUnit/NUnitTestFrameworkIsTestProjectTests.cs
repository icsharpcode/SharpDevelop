// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;
using Rhino.Mocks;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.NUnit
{
	[TestFixture]
	public class NUnitTestFrameworkIsTestProjectTests : SDTestFixtureBase
	{
		NUnitTestFramework testFramework;
		
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			testFramework = new NUnitTestFramework();
		}
		
		[Test]
		public void NUnitTestFrameworkImplementsITestFramework()
		{
			Assert.IsNotNull(testFramework as ITestFramework);
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForNullProject()
		{
			Assert.IsFalse(testFramework.IsTestProject(null));
		}
		
		[Test]
		public void IsTestProjectReturnsTrueForProjectWithNUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			project.AddAssemblyReference(NRefactoryHelper.NUnitFramework);
			SD.ParserService.Stub(p => p.GetCompilation(project)).Return(project.ProjectContent.CreateCompilation());
			
			Assert.IsTrue(testFramework.IsTestProject(project));
		}
		
		[Test]
		public void IsTestProjectReturnsFalseForProjectWithoutNUnitFrameworkAssemblyReference()
		{
			MockCSharpProject project = new MockCSharpProject();
			SD.ParserService.Stub(p => p.GetCompilation(project)).Return(project.ProjectContent.CreateCompilation());
			Assert.IsFalse(testFramework.IsTestProject(project));
		}
	}
}
