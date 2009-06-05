// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Codons;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ProjectHasStartupObjectTestFixture
	{
		DerivedConvertProjectToPythonProjectCommand convertProjectCommand;
		FileProjectItem mainFile;
		FileProjectItem main2File;
		FileProjectItem targetMainFile;
		FileProjectItem targetMain2File;
		MSBuildBasedProject sourceProject;
		PythonProject targetProject;
		MockTextEditorProperties mockTextEditorProperties;
		MockProjectContent mockProjectContent;
		
		string startupObject = "RootNamespace.Main";
		
		string mainSource = "class Foo\r\n" +
							"{\r\n" +
							"    static void Main()\r\n" +
							"    {\r\n" +
							"    }\r\n" +
							"}";
		
		string main2Source = "class Bar\r\n" +
							"{\r\n" +
							"    static void Main()\r\n" +
							"    {\r\n" +
							"    }\r\n" +
							"}";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MSBuildEngineHelper.InitMSBuildEngine();

			List<LanguageBindingDescriptor> bindings = new List<LanguageBindingDescriptor>();
			using (TextReader reader = PythonBindingAddInFile.ReadAddInFile()) {
				AddIn addin = AddIn.Load(reader, String.Empty);
				bindings.Add(new LanguageBindingDescriptor(AddInHelper.GetCodon(addin, "/SharpDevelop/Workbench/LanguageBindings", "Python")));
			}
			LanguageBindingService.SetBindings(bindings);
			
			// Set up IProjectContent so the ConvertProjectToPythonProjectCommand can
			// locate the startup object and determine it's filename.
			
			mockProjectContent = new MockProjectContent();
			MockClass mainClass = new MockClass(mockProjectContent, startupObject);
			mainClass.CompilationUnit.FileName = @"d:\projects\test\src\Main2.cs";
			mockProjectContent.ClassToReturnFromGetClass = mainClass;
			
			mockTextEditorProperties = new MockTextEditorProperties();
			convertProjectCommand = new DerivedConvertProjectToPythonProjectCommand(mockTextEditorProperties);
			convertProjectCommand.ProjectContent = mockProjectContent;
			mockTextEditorProperties.Encoding = Encoding.Unicode;
			
			Solution solution = new Solution();
			sourceProject = new MSBuildBasedProject(solution.BuildEngine);
			sourceProject.Parent = solution;
			sourceProject.FileName = @"d:\projects\test\source.csproj";
			sourceProject.SetProperty(null, null, "StartupObject", startupObject, PropertyStorageLocations.Base, true);
			mainFile = new FileProjectItem(sourceProject, ItemType.Compile, @"src\Main.cs");
			targetProject = (PythonProject)convertProjectCommand.CallCreateProject(@"d:\projects\test\converted", sourceProject);
			convertProjectCommand.CallCopyProperties(sourceProject, targetProject);
			targetMainFile = new FileProjectItem(targetProject, mainFile.ItemType, mainFile.Include);
			mainFile.CopyMetadataTo(targetMainFile);
			
			main2File = new FileProjectItem(sourceProject, ItemType.Compile, @"src\Main2.cs");
			targetMain2File = new FileProjectItem(targetProject, main2File.ItemType, main2File.Include);
			main2File.CopyMetadataTo(targetMain2File);
						
			convertProjectCommand.AddParseableFileContent(mainFile.FileName, mainSource);
			convertProjectCommand.AddParseableFileContent(main2File.FileName, main2Source);
			
			convertProjectCommand.CallConvertFile(mainFile, targetMainFile);
			convertProjectCommand.CallConvertFile(main2File, targetMain2File);
		}
		
		[Test]
		public void MainFileIsMain2PyFile()
		{
			PropertyStorageLocations location;
			Assert.AreEqual(@"src\Main2.py", targetProject.GetProperty(null, null, "MainFile", out location));
		}
		
		[Test]
		public void PropertyStorageLocationForMainFilePropertyIsGlobal()
		{
			PropertyStorageLocations location;
			targetProject.GetProperty(null, null, "MainFile", out location);
			Assert.AreEqual(PropertyStorageLocations.Base, location);
		}
		
		[Test]
		public void ClassSearchedFor()
		{
			Assert.AreEqual(startupObject, mockProjectContent.GetClassName);
		}
	}
}
