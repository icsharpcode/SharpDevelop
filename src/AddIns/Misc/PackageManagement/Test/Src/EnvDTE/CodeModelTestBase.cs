// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public override void SetUp()
		{
			base.SetUp();
			project.Stub(p => p.Language).Return(null).WhenCalled(mi => mi.ReturnValue = projectLanguage);
			codeGenerator = MockRepository.GenerateStrictMock<CodeGenerator>();
			codeModelContext = new CodeModelContext {
				CodeGenerator = codeGenerator,
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
			msbuildProject = ProjectHelper.CreateTestProject();
			
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
	}
}
