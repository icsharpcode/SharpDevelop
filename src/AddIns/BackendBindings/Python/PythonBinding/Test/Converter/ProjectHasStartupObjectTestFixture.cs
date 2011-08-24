// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

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
		ICSharpCode.Scripting.Tests.Utils.MockProjectContent mockProjectContent;
		
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
			PythonMSBuildEngineHelper.InitMSBuildEngine();

			List<ProjectBindingDescriptor> bindings = new List<ProjectBindingDescriptor>();
			using (TextReader reader = PythonBindingAddInFile.ReadAddInFile()) {
				AddIn addin = AddIn.Load(reader, String.Empty);
				bindings.Add(new ProjectBindingDescriptor(AddInHelper.GetCodon(addin, "/SharpDevelop/Workbench/ProjectBindings", "Python")));
			}
			ProjectBindingService.SetBindings(bindings);
			
			// Set up IProjectContent so the ConvertProjectToPythonProjectCommand can
			// locate the startup object and determine it's filename.
			
			mockProjectContent = new ICSharpCode.Scripting.Tests.Utils.MockProjectContent();
			MockClass mainClass = new MockClass(mockProjectContent, startupObject);
			mainClass.CompilationUnit.FileName = @"d:\projects\test\src\Main2.cs";
			mockProjectContent.SetClassToReturnFromGetClass(startupObject, mainClass);
			
			convertProjectCommand = new DerivedConvertProjectToPythonProjectCommand();
			convertProjectCommand.ProjectContent = mockProjectContent;
			convertProjectCommand.FileServiceDefaultEncoding = Encoding.Unicode;
			
			Solution solution = new Solution(new MockProjectChangeWatcher());
			sourceProject = new MSBuildBasedProject(
				new ProjectCreateInformation() {
					Solution = solution,
					OutputProjectFileName = @"d:\projects\test\source.csproj",
					ProjectName = "source"
				});
			sourceProject.Parent = solution;
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
