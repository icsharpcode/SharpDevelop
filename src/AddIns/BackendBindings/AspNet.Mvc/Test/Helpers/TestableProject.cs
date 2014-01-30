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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;
using Dom = ICSharpCode.SharpDevelop.Dom;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableProject : CompilableProject
	{
		string language = "C#";
		string outputAssemblyFullPath = String.Empty;
		
		public TestableProject(ProjectCreateInformation createInfo)
			: base(createInfo)
		{
		}
		
		public static TestableProject CreateProject()
		{
			return CreateProject(@"d:\projects\MyProject\MyProject.csproj", "MyProject");
		}
		
		public static TestableProject CreateProject(string fileName, string projectName)
		{
			var createInfo = new ProjectCreateInformation(FakeSolution.Create(), new FileName(fileName));
			createInfo.ProjectName = projectName;
			return new TestableProject(createInfo);
		}
		
		public override string Language {
			get { return language; }
		}
		
		public void SetLanguage(string language)
		{
			this.language = language;
		}
		
		public bool IsSaved;
		
		public override void Save(string fileName)
		{
			IsSaved = true;
		}
		
		public override bool IsReadOnly {
			get { return false; }
		}
		
		public FileProjectItem AddFileToProject(string fileName)
		{
			var projectItem = new FileProjectItem(this, ItemType.Compile);
			projectItem.FileName = FileName.Create(fileName);
			ProjectService.AddProjectItem(this, projectItem);
			return projectItem;
		}
		
		public override FileName OutputAssemblyFullPath {
			get { return FileName.Create(outputAssemblyFullPath); }
		}
		
		public void SetOutputAssemblyFullPath(string path)
		{
			outputAssemblyFullPath = path;
		}
	}
}
