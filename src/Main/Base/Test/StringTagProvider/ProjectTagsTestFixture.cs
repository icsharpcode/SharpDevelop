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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests.StringTagProvider
{
	/// <summary>
	/// Tests the SharpDevelopStringTagProvider when there is an active project.
	/// </summary>
	[TestFixture]
	public class ProjectTagsTestFixture : SDTestFixtureBase
	{
		SharpDevelopStringTagProvider tagProvider;
		IProject project;
		
		public override void FixtureSetUp()
		{
			base.FixtureSetUp();
			project = MockRepository.GenerateStrictMock<IProject>();
			project.Stub(p => p.FileName).Return(FileName.Create(@"C:\Projects\MyProject\MyProject.csproj"));
			project.Stub(p => p.Directory).Return(DirectoryName.Create(@"C:\Projects\MyProject"));
			project.Stub(p => p.OutputAssemblyFullPath).Return(FileName.Create(@"C:\Projects\MyProject\bin\Debug\MyProject.exe"));
			project.Stub(p => p.Name).Return("MyProject");
			
			var projectService = MockRepository.GenerateStrictMock<IProjectService>();
			projectService.Stub(p => p.CurrentProject).Return(project);
			SD.Services.AddService(typeof(IProjectService), projectService);
			
			tagProvider = new SharpDevelopStringTagProvider();
		}
		
		[Test]
		public void ConvertCurrentProjectName()
		{
			Assert.AreEqual(project.Name, tagProvider.ProvideString("CurrentProjectName"));
		}
		
		[Test]
		public void ConvertTargetPath()
		{
			Assert.AreEqual(project.OutputAssemblyFullPath.ToString(), tagProvider.ProvideString("TargetPath"));
		}		

		[Test]
		public void ConvertTargetDir()
		{
			Assert.AreEqual(Path.GetDirectoryName(project.OutputAssemblyFullPath), tagProvider.ProvideString("TargetDir"));
		}		

		[Test]
		public void ConvertTargetName()
		{
			Assert.AreEqual("MyProject.exe", tagProvider.ProvideString("TargetName"));
		}		
		
		[Test]
		public void ConvertTargetExt()
		{
			Assert.AreEqual(".exe", tagProvider.ProvideString("TargetExt"));
		}		

		[Test]
		public void ConvertProjectDir()
		{
			Assert.AreEqual(project.Directory.ToString(), tagProvider.ProvideString("ProjectDir"));
		}		
		
		[Test]
		public void ConvertProjectFileName()
		{
			Assert.AreEqual(Path.GetFileName(project.FileName), tagProvider.ProvideString("ProjectFileName"));
		}		
	}
}
