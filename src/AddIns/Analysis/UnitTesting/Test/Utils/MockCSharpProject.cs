// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockCSharpProject : CompilableProject
	{
		bool saved;
		IProjectContent projectContent;
		
		public MockCSharpProject()
			: this(new Solution(new MockProjectChangeWatcher()), "MyTests")
		{
		}
		
		public MockCSharpProject(Solution solution, string name)
			: base(new ProjectCreateInformation {
			       	Solution = solution,
			       	ProjectName = name,
			       	Platform = "x86",
			       	TargetFramework = TargetFramework.Net40Client,
			       	OutputProjectFileName = "c:\\projects\\" + name + "\\" + name + ".csproj"
			       })
		{
			OutputType = OutputType.Library;
			projectContent = new CSharpProjectContent().SetAssemblyName(name).SetProjectFileName(this.FileName);
			projectContent = projectContent.AddAssemblyReferences(AssemblyLoader.Corlib);
		}
		
		public override string Language {
			get { return "C#"; }
		}
		
//		public override LanguageProperties LanguageProperties {
//			get { return LanguageProperties.CSharp; }
//		}
//
		public override IProjectContent ProjectContent {
			get {
				return projectContent;
			}
		}
		
		public void AddAssemblyReference(IAssemblyReference reference)
		{
			projectContent = projectContent.AddAssemblyReferences(reference);
		}
		
		public bool IsSaved {
			get { return saved; }
		}
		
		public override void Save(string fileName)
		{
			saved = true;
		}
	}
}
