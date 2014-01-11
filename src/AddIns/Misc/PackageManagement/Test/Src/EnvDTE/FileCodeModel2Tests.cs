// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
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
	public class FileCodeModel2Tests : CodeModelTestBase
	{
		FileCodeModel2 fileCodeModel;
		
		void CreateProjectWithOneFile(string code)
		{
			CreateProjectWithOneFile(code, @"d:\project\MyProject\MyFile.cs");
		}
		
		void CreateProjectWithOneFile(string code, string fileName)
		{
			codeModelContext = codeModelContext.WithFilteredFileName(fileName);
			CreateCodeModel();
			AddCodeFile(fileName, code);
		}
		
		void CreateFileCodeModel()
		{
			fileCodeModel = new FileCodeModel2(codeModelContext, dteProject);
		}
		
		CodeImport GetFirstCodeImportFromCodeElements()
		{
			return fileCodeModel.CodeElements.FirstCodeImportOrDefault();
		}
		
		global::EnvDTE.CodeNamespace GetFirstCodeNamespaceFromCodeElements()
		{
			return fileCodeModel
				.CodeElements
				.OfType<global::EnvDTE.CodeNamespace>()
				.FirstOrDefault();
		}
		
		global::EnvDTE.CodeNamespace GetLastCodeNamespaceFromCodeElements()
		{
			return fileCodeModel
				.CodeElements
				.OfType<global::EnvDTE.CodeNamespace>()
				.LastOrDefault();
		}
		
		[Test]
		public void CodeElements_OneNamespaceInFile_ReturnsOneCodeImport()
		{
			string code = "using System.Security;";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			CodeImport import = GetFirstCodeImportFromCodeElements();
			
			Assert.AreEqual("System.Security", import.Namespace);
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementImportStmt, import.Kind);
		}
		
		[Test]
		public void CodeElements_UnknownNamespaceInFile_ReturnsOneCodeImport()
		{
			string code = "using Tests.CodeModel;";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			CodeImport import = GetFirstCodeImportFromCodeElements();
			
			Assert.AreEqual("Tests.CodeModel", import.Namespace);
		}
		
		[Test]
		public void CodeElements_OneNamespaceAliasInFile_ReturnsOneCodeImport()
		{
			string code = "using TCM = Tests.CodeModel;";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			CodeImport import = GetFirstCodeImportFromCodeElements();
			
			Assert.AreEqual("Tests.CodeModel", import.Namespace);
		}
		
		[Test]
		[Ignore("TODO - Not implemented")]
		public void AddImport_AddSystemXmlNamespace_NamespaceAndcompilationUnitPassedToNamespaceCreator()
		{
			CreateProjectWithOneFile("");
			CreateFileCodeModel();
			
			fileCodeModel.AddImport("System.Xml");
			
//			namespaceCreator.AssertWasCalled(creator => creator.AddNamespace(compilationUnitHelper.CompilationUnit, "System.Xml"));
		}
		
		[Test]
		public void CodeElements_OneClassInFileWithNamespace_ReturnsOneClassInsideCodeNamespace()
		{
			string code =
				"namespace Test.CodeModel {\r\n" +
				"    public class Class1 {}\r\n" +
				"}";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			global::EnvDTE.CodeNamespace ns = GetFirstCodeNamespaceFromCodeElements();
			CodeClass2 codeClass = ns.Members.FirstCodeClass2OrDefault();
			
			Assert.AreEqual("Test.CodeModel.Class1", codeClass.FullName);
		}
		
		[Test]
		public void CodeElements_TwoClassesInFileWithNamespace_ReturnsTwoClassesInsideCodeNamespace()
		{
			string code =
				"namespace Test.CodeModel {\r\n" +
				"    public class Class1 {}\r\n" +
				"    public class Class2 {}\r\n" +
				"}";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			global::EnvDTE.CodeNamespace ns = GetFirstCodeNamespaceFromCodeElements();
			CodeClass2 codeClass1 = ns.Members.FirstCodeClass2OrDefault();
			CodeClass2 codeClass2 = ns.Members.LastCodeClass2OrDefault();
			
			Assert.AreEqual("Test.CodeModel.Class1", codeClass1.FullName);
			Assert.AreEqual("Test.CodeModel.Class2", codeClass2.FullName);
		}
		
		[Test]
		public void CodeElements_TwoClassesInFileEachWithDifferentNamespace_ReturnsTwoCodeNamespaces()
		{
			string code =
				"namespace Test.CodeModel1 {\r\n" +
				"    public class Class1 {}\r\n" +
				"}\r\n" +
				"namespace Test.CodeModel2 {\r\n" +
				"    public class Class2 {}\r\n" +
				"}";
			CreateProjectWithOneFile(code);
			CreateFileCodeModel();
			
			global::EnvDTE.CodeNamespace ns1 = GetFirstCodeNamespaceFromCodeElements();
			global::EnvDTE.CodeNamespace ns2 = GetLastCodeNamespaceFromCodeElements();
			
			Assert.AreEqual("Test.CodeModel1", ns1.Name);
			Assert.AreEqual("Test.CodeModel2", ns2.Name);
		}
	}
}
