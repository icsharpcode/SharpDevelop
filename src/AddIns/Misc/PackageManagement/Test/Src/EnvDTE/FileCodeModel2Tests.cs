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
using System.Linq;
using ICSharpCode.Core;
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
		public void AddImport_AddSystemXmlNamespace_NamespaceAndcompilationUnitPassedToNamespaceCreator()
		{
			CreateProjectWithOneFile("", @"d:\project\MyProject\MyFile.cs");
			CreateFileCodeModel();
			
			fileCodeModel.AddImport("System.Xml");
			
			codeGenerator.AssertWasCalled(generator => generator.AddImport(
				new FileName(@"d:\project\MyProject\MyFile.cs"),
				"System.Xml"));
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
