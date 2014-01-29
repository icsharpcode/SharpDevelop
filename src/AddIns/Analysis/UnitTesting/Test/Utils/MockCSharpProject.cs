// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop.Project;
using Rhino.Mocks;

namespace UnitTesting.Tests.Utils
{
	public class MockCSharpProject : CompilableProject
	{
		bool saved;
		IProjectContent projectContent;
		
		public MockCSharpProject()
			: this(MockSolution.Create(), "MyTests")
		{
		}
		
		public MockCSharpProject(ISolution solution, string name)
			: base(new ProjectCreateInformation(solution, FileName.Create("c:\\projects\\" + name + "\\" + name + ".csproj")) {
			       	ProjectName = name,
			       	ActiveProjectConfiguration = new ConfigurationAndPlatform("Debug", "x86"),
			       	TargetFramework = TargetFramework.Net40Client,
			       })
		{
			OutputType = OutputType.Library;
			projectContent = new CSharpProjectContent().SetAssemblyName(name).SetProjectFileName(this.FileName);
			projectContent = projectContent.AddAssemblyReferences(NRefactoryHelper.Corlib);
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
