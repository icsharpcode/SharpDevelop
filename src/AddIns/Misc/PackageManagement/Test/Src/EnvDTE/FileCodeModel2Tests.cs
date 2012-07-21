// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.EasyCodeDom;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class FileCodeModel2Tests
	{
		FileCodeModel2 fileCodeModel;
		TestableDTEProject project;
		TestableProject msbuildProject;
		FakeFileService fakeFileService;
		FileProjectItem fileProjectItem;
		CompilationUnitHelper compilationUnitHelper;
		IDocumentNamespaceCreator namespaceCreator;
		
		void CreateProjectWithOneFile()
		{
			CreateProjectWithOneFile(@"d:\project\MyProject\MyFile.cs");
		}
		
		void CreateProjectWithOneFile(string fileName)
		{
			project = new TestableDTEProject();
			msbuildProject = project.TestableProject;
			fakeFileService = project.FakeFileService;
			
			fileProjectItem = new FileProjectItem(msbuildProject, ItemType.Compile) {
				FileName = fileName
			};
		}
		
		void CreateCompilationUnitForFileProjectItem()
		{
			compilationUnitHelper = new CompilationUnitHelper();
			fakeFileService.CompilationUnitToReturnFromGetCompilationUnit = compilationUnitHelper.CompilationUnit;
		}
		
		void CreateFileCodeModel()
		{
			namespaceCreator = MockRepository.GenerateStub<IDocumentNamespaceCreator>();
			fileCodeModel = new FileCodeModel2(project, fileProjectItem, namespaceCreator);
		}
		
		CodeImport GetFirstCodeImportFromCodeElements()
		{
			return fileCodeModel.CodeElements.FirstCodeImportOrDefault();
		}
		
		void AddNamespaceToCompilationUnit(string namespaceName)
		{
			compilationUnitHelper.AddNamespace(namespaceName);
		}
		
		void AddNamespaceAliasToCompilationUnit(string alias, string namespaceName)
		{
			compilationUnitHelper.AddNamespaceAlias(alias, namespaceName);
		}

		[Test]
		public void CodeElements_OneNamespaceInFile_FileNameUsedToGetCompilationUnit()
		{
			CreateProjectWithOneFile(@"d:\projects\MyProject\MyFile.cs");
			CreateCompilationUnitForFileProjectItem();
			CreateFileCodeModel();
			AddNamespaceToCompilationUnit("Test.CodeModel");
			
			CodeImport import = GetFirstCodeImportFromCodeElements();

			Assert.AreEqual(@"d:\projects\MyProject\MyFile.cs", fakeFileService.FileNamePassedToGetCompilationUnit);
		}
		
		[Test]
		public void CodeElements_OneNamespaceInFile_ReturnsOneCodeImport()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			CreateFileCodeModel();
			AddNamespaceToCompilationUnit("Test.CodeModel");
						
			CodeImport import = GetFirstCodeImportFromCodeElements();
			
			Assert.AreEqual("Test.CodeModel", import.Namespace);
		}
		
		[Test]
		public void CodeElements_OneNamespaceAliasInFile_ReturnsOneCodeImport()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			CreateFileCodeModel();
			AddNamespaceAliasToCompilationUnit("TCM", "Test.CodeModel");
						
			CodeImport import = GetFirstCodeImportFromCodeElements();
			
			Assert.AreEqual("Test.CodeModel", import.Namespace);
		}
		
		[Test]
		public void AddImport_AddSystemXmlNamespace_NamespaceAndcompilationUnitPassedToNamespaceCreator()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			CreateFileCodeModel();
			
			fileCodeModel.AddImport("System.Xml");
			
			namespaceCreator.AssertWasCalled(creator => creator.AddNamespace(compilationUnitHelper.CompilationUnit, "System.Xml"));
		}
	}
}
