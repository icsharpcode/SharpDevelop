// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
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
