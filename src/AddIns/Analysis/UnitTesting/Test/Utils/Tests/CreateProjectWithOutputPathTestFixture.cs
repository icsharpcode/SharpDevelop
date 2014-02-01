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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using NUnit.Framework;

namespace UnitTesting.Tests.Utils.Tests
{
	[TestFixture]
	public class CreateProjectWithOutputPathTestFixture
	{
		MockCSharpProject project;
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
		}
		
		[Test]
		public void ProjectOutputTypeIsLibrary()
		{
			Assert.AreEqual(OutputType.Library, project.OutputType);
		}
		
		[Test]
		public void ProjectFileNameIsProjectsMyTestsMyTestsCsproj()
		{
			string expectedFileName = @"c:\projects\MyTests\MyTests.csproj";
			Assert.AreEqual(expectedFileName, project.FileName);
		}
		
		[Test]
		public void ProjectAssemblyNameIsMyTests()
		{
			Assert.AreEqual("MyTests", project.AssemblyName);
		}
		
		[Test]
		public void ProjectOutputPathIsBinDebug()
		{
			Assert.AreEqual(@"bin\Debug\", project.GetProperty(null, null, "OutputPath"));
		}
		
		[Test]
		public void OutputAssemblyFullPathIsProjectsMyTestsMyTestsBinDebugMyTestsDll()
		{
			string expectedFileName = @"c:\projects\MyTests\bin\Debug\MyTests.dll";
			Assert.AreEqual(expectedFileName, project.OutputAssemblyFullPath);
		}
		
		[Test]
		public void TargetFrameworkVersionIsVersion40()
		{
			Assert.AreEqual("v4.0", project.TargetFrameworkVersion);
		}
		
		[Test]
		public void PlatformTargetIs32Bit()
		{
			Assert.AreEqual("x86", project.GetEvaluatedProperty("PlatformTarget"));
		}
	}
}
