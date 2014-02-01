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
