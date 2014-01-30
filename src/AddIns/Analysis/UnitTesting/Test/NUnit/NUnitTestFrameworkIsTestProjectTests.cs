// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
