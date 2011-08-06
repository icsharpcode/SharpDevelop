// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using Dom = ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableProject : CompilableProject
	{
		string language = "C#";
		
		public TestableProject(ProjectCreateInformation createInfo)
			: base(createInfo)
		{
		}
		
		public static TestableProject CreateProject()
		{
			var createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution();
			createInfo.OutputProjectFileName = @"d:\projects\MyProject\MyProject.csproj";
			return new TestableProject(createInfo);
		}
		
		public override LanguageProperties LanguageProperties {
			get { return Dom.LanguageProperties.CSharp; }
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
	}
}
