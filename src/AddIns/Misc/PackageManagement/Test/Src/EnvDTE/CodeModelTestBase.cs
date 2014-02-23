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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	public class CodeModelTestBase : CSharpModelTestBase
	{
		protected CodeGenerator codeGenerator;
		protected CodeModelContext codeModelContext;
		protected string projectLanguage = "C#";
		protected ILanguageBinding languageBinding;
		
		public override void SetUp()
		{
			base.SetUp();
			project.Stub(p => p.Language).Return(null).WhenCalled(mi => mi.ReturnValue = projectLanguage);
			
			codeGenerator = MockRepository.GenerateMock<CodeGenerator>();
			languageBinding = MockRepository.GenerateMock<ILanguageBinding>();
			languageBinding.Stub(binding => binding.CodeGenerator).Return(codeGenerator);
			project.Stub(p => p.LanguageBinding).Return(languageBinding);
			
			codeModelContext = new CodeModelContext {
				CurrentProject = project
			};
		}
		
		protected CodeModel codeModel;
		protected Project dteProject;
		protected IPackageManagementProjectService projectService;
		protected IPackageManagementFileService fileService;
		protected TestableProject msbuildProject;
		
		protected void CreateCodeModel()
		{
			ISolution solution = ProjectHelper.CreateSolutionWithoutInitializingServicesForUnitTests();
			msbuildProject = ProjectHelper.CreateTestProject(solution, "MyProject");
			
			projectService = MockRepository.GenerateStub<IPackageManagementProjectService>();
			fileService = MockRepository.GenerateStub<IPackageManagementFileService>();
			dteProject = new Project(msbuildProject, projectService, fileService);
			codeModelContext.DteProject = dteProject;
			
			codeModel = new CodeModel(codeModelContext, dteProject);
			
			msbuildProject.SetAssemblyModel(assemblyModel);
			project.Stub(p => p.AssemblyModel).Return(assemblyModel);
			
			fileService
				.Stub(fs => fs.GetCompilationUnit(msbuildProject))
				.WhenCalled(compilation => compilation.ReturnValue = CreateCompilation());
		}
		
		ICompilation CreateCompilation()
		{
			var solutionSnapshot = new TestableSolutionSnapshot(msbuildProject);
			msbuildProject.SetProjectContent(projectContent);
			ICompilation compilation = new SimpleCompilation(solutionSnapshot, projectContent, projectContent.AssemblyReferences);
			solutionSnapshot.AddCompilation(projectContent, compilation);
			return compilation;
		}
		
		protected void CreateCompilationForUpdatedCodeFile(string fileName, string code)
		{
			fileService
				.Stub(fs => fs.GetCompilationUnit(fileName))
				.WhenCalled(compilation => {
					UpdateCodeFile(fileName, code);
					compilation.ReturnValue = CreateCompilation();
				})
				.Return(null); // HACK: Not returning null here but Rhino Mocks fails to work otherwise.
		}
	}
}
