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
		
		CodeNamespace GetFirstCodeNamespaceFromCodeElements()
		{
			return fileCodeModel.CodeElements.FirstCodeNamespaceOrDefault();
		}
		
		void AddNamespaceToCompilationUnit(string namespaceName)
		{
			compilationUnitHelper.AddNamespace(namespaceName);
		}
		
		void AddNamespaceAliasToCompilationUnit(string alias, string namespaceName)
		{
			compilationUnitHelper.AddNamespaceAlias(alias, namespaceName);
		}
		
		void AddClassToCompilationUnit(string namespaceName, string className)
		{
			var classHelper = new ClassHelper();
			classHelper.CreatePublicClass(className);
			classHelper.AddNamespaceUsingScopeToClass(namespaceName);
			classHelper.AddClassNamespace(namespaceName);
			compilationUnitHelper.AddClass(classHelper.Class);
		}
		
		CodeNamespace GetLastCodeNamespaceFromCodeElements()
		{
			return fileCodeModel.CodeElements.LastCodeNamespaceOrDefault();
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
		
		[Test]
		public void CodeElements_OneClassInFileWithNamespace_ReturnsOneCodeNamespace()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			AddClassToCompilationUnit("Test.CodeModel", "Test.CodeModel.Class1");
			CreateFileCodeModel();
			
			CodeNamespace ns = GetFirstCodeNamespaceFromCodeElements();
			
			Assert.AreEqual("Test.CodeModel", ns.Name);
		}
		
		[Test]
		public void CodeElements_OneClassInFileWithNamespace_ReturnsOneClassInsideCodeNamespace()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			AddClassToCompilationUnit("Test.CodeModel", "Test.CodeModel.Class1");
			CreateFileCodeModel();
			
			CodeNamespace ns = GetFirstCodeNamespaceFromCodeElements();
			CodeClass2 codeClass = ns.Members.FirstCodeClass2OrDefault();
			
			Assert.AreEqual("Test.CodeModel.Class1", codeClass.FullName);
		}
		
		[Test]
		public void CodeElements_TwoClassesInFileWithNamespace_ReturnsTwoClassesInsideCodeNamespace()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			AddClassToCompilationUnit("Test.CodeModel", "Test.CodeModel.Class1");
			AddClassToCompilationUnit("Test.CodeModel", "Test.CodeModel.Class2");
			CreateFileCodeModel();
			
			CodeNamespace ns = GetFirstCodeNamespaceFromCodeElements();
			CodeClass2 codeClass1 = ns.Members.FirstCodeClass2OrDefault();
			CodeClass2 codeClass2 = ns.Members.LastCodeClass2OrDefault();
			
			Assert.AreEqual("Test.CodeModel.Class1", codeClass1.FullName);
			Assert.AreEqual("Test.CodeModel.Class2", codeClass2.FullName);
		}
		
		[Test]
		public void CodeElements_TwoClassesInFileEachWithDifferentNamespace_ReturnsTwoCodeNamespaces()
		{
			CreateProjectWithOneFile();
			CreateCompilationUnitForFileProjectItem();
			AddClassToCompilationUnit("Test.CodeModel1", "Test.CodeModel.Class1");
			AddClassToCompilationUnit("Test.CodeModel2", "Test.CodeModel.Class2");
			CreateFileCodeModel();
			
			CodeNamespace ns1 = GetFirstCodeNamespaceFromCodeElements();
			CodeNamespace ns2 = GetLastCodeNamespaceFromCodeElements();
			
			Assert.AreEqual("Test.CodeModel1", ns1.Name);
			Assert.AreEqual("Test.CodeModel2", ns2.Name);
		}
	}
}
