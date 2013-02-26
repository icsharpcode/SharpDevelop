// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
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
			var createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution(null);
			createInfo.ProjectName = projectName;
			createInfo.OutputProjectFileName = new FileName(fileName);
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
			projectItem.FileName = fileName;
			ProjectService.AddProjectItem(this, projectItem);
			return projectItem;
		}
		
		public override string OutputAssemblyFullPath {
			get { return outputAssemblyFullPath; }
		}
		
		public void SetOutputAssemblyFullPath(string path)
		{
			outputAssemblyFullPath = path;
		}
	}
}
