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
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertToPythonProjectCommandTestFixture
	{
		DerivedConvertProjectToPythonProjectCommand convertProjectCommand;
		FileProjectItem source;
		FileProjectItem target;
		MockProject sourceProject;
		PythonProject targetProject;
		FileProjectItem textFileSource;
		FileProjectItem textFileTarget;
		ReferenceProjectItem ironPythonReference;
		string sourceCode = 
			"class Foo\r\n" +
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
			
			convertProjectCommand = new DerivedConvertProjectToPythonProjectCommand();
			convertProjectCommand.FileServiceDefaultEncoding = Encoding.Unicode;
			
			sourceProject = new MockProject();
			sourceProject.Directory = @"d:\projects\test";
			source = new FileProjectItem(sourceProject, ItemType.Compile, @"src\Program.cs");
			targetProject = (PythonProject)convertProjectCommand.CallCreateProject(@"d:\projects\test\converted", sourceProject);
			target = new FileProjectItem(targetProject, source.ItemType, source.Include);
			source.CopyMetadataTo(target);
			
			textFileSource = new FileProjectItem(sourceProject, ItemType.None, @"src\readme.txt");
			textFileTarget = new FileProjectItem(targetProject, textFileSource.ItemType, textFileSource.Include);
			textFileSource.CopyMetadataTo(textFileTarget);
			
			foreach (ProjectItem item in targetProject.Items) {
				ReferenceProjectItem reference = item as ReferenceProjectItem;
				if ((reference != null) && (reference.Name == "IronPython")) {
					ironPythonReference = reference;
					break;
				}
			}
			
			convertProjectCommand.AddParseableFileContent(source.FileName, sourceCode);
			
			convertProjectCommand.CallConvertFile(source, target);
			convertProjectCommand.CallConvertFile(textFileSource, textFileTarget);
		}
		
		[Test]
		public void CommandHasPythonTargetLanguage()
		{
			Assert.AreEqual(PythonProjectBinding.LanguageName, convertProjectCommand.TargetLanguageName);
		}
		
		[Test]
		public void TargetFileExtensionChanged()
		{
			Assert.AreEqual(@"src\Program.py", target.Include);
		}
		
		[Test]
		public void TextFileTargetFileExtensionUnchanged()
		{
			Assert.AreEqual(@"src\readme.txt", textFileTarget.Include);
		}
		
		[Test]
		public void FilesPassedToBaseClassForConversion()
		{
			List<SourceAndTargetFile> expectedFiles = new List<SourceAndTargetFile>();
			expectedFiles.Add(new SourceAndTargetFile(textFileSource, textFileTarget));
			Assert.AreEqual(expectedFiles, convertProjectCommand.SourceAndTargetFilesPassedToBaseClass);
		}

		[Test]
		public void ExpectedCodeWrittenToFile()
		{
			NRefactoryToPythonConverter converter = new NRefactoryToPythonConverter();
			string expectedCode = converter.Convert(sourceCode, SupportedLanguage.CSharp) + 
				"\r\n" +
				"\r\n" + 
				converter.GenerateMainMethodCall(converter.EntryPointMethods[0]);
			
			List<ConvertedFile> expectedSavedFiles = new List<ConvertedFile>();
			expectedSavedFiles.Add(new ConvertedFile(target.FileName, expectedCode, Encoding.Unicode));
			Assert.AreEqual(expectedSavedFiles, convertProjectCommand.SavedFiles);
		}
		
		[Test]
		public void IronPythonReferenceAddedToProject()
		{
			Assert.IsNotNull(ironPythonReference, "No IronPython reference added to converted project.");
		}
		
		[Test]
		public void IronPythonReferenceHintPath()
		{
			Assert.AreEqual(@"$(PythonBinPath)\IronPython.dll", ironPythonReference.GetMetadata("HintPath"));
		}
		
		[Test]
		public void MainFileIsProgramPyFile()
		{
			PropertyStorageLocations location;
			Assert.AreEqual(@"src\Program.py", targetProject.GetProperty(null, null, "MainFile", out location));
		}
		
		[Test]
		public void PropertyStorageLocationForMainFilePropertyIsGlobal()
		{
			PropertyStorageLocations location;
			targetProject.GetProperty(null, null, "MainFile", out location);
			Assert.AreEqual(PropertyStorageLocations.Base, location);
		}
	}
}
