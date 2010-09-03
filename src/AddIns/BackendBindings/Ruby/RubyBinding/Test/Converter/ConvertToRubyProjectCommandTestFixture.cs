// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertToRubyProjectCommandTestFixture
	{
		DerivedConvertProjectToRubyProjectCommand convertProjectCommand;
		FileProjectItem source;
		FileProjectItem target;
		MockProject sourceProject;
		RubyProject targetProject;
		FileProjectItem textFileSource;
		FileProjectItem textFileTarget;
		ParseInformation parseInfo;
		
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
			RubyMSBuildEngineHelper.InitMSBuildEngine();

			List<ProjectBindingDescriptor> bindings = new List<ProjectBindingDescriptor>();
			using (TextReader reader = RubyBindingAddInFile.ReadAddInFile()) {
				AddIn addin = AddIn.Load(reader, String.Empty);
				bindings.Add(new ProjectBindingDescriptor(AddInHelper.GetCodon(addin, "/SharpDevelop/Workbench/ProjectBindings", "Ruby")));
			}
			ProjectBindingService.SetBindings(bindings);
			
			convertProjectCommand = new DerivedConvertProjectToRubyProjectCommand();
			parseInfo = new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
			convertProjectCommand.ParseInfo = parseInfo;
			convertProjectCommand.FileServiceDefaultEncoding = Encoding.Unicode;
			
			sourceProject = new MockProject();
			sourceProject.Directory = @"d:\projects\test";
			source = new FileProjectItem(sourceProject, ItemType.Compile, @"src\Program.cs");
			targetProject = (RubyProject)convertProjectCommand.CallCreateProject(@"d:\projects\test\converted", sourceProject);
			target = new FileProjectItem(targetProject, source.ItemType, source.Include);
			source.CopyMetadataTo(target);
			
			textFileSource = new FileProjectItem(sourceProject, ItemType.None, @"src\readme.txt");
			textFileTarget = new FileProjectItem(targetProject, textFileSource.ItemType, textFileSource.Include);
			textFileSource.CopyMetadataTo(textFileTarget);
			
			convertProjectCommand.AddParseableFileContent(source.FileName, sourceCode);
			
			convertProjectCommand.CallConvertFile(source, target);
			convertProjectCommand.CallConvertFile(textFileSource, textFileTarget);
		}
		
		[Test]
		public void CommandHasRubyTargetLanguage()
		{
			Assert.AreEqual(RubyProjectBinding.LanguageName, convertProjectCommand.TargetLanguageName);
		}
		
		[Test]
		public void TargetFileExtensionChanged()
		{
			Assert.AreEqual(@"src\Program.rb", target.Include);
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
			NRefactoryToRubyConverter converter = new NRefactoryToRubyConverter(SupportedLanguage.CSharp, parseInfo);
			string expectedCode = converter.Convert(sourceCode, SupportedLanguage.CSharp) + 
				"\r\n" +
				"\r\n" + 
				converter.GenerateMainMethodCall(converter.EntryPointMethods[0]);
			
			List<ConvertedFile> expectedSavedFiles = new List<ConvertedFile>();
			expectedSavedFiles.Add(new ConvertedFile(target.FileName, expectedCode, Encoding.Unicode));
			Assert.AreEqual(expectedSavedFiles, convertProjectCommand.SavedFiles);
		}
		
		[Test]
		public void FileNamesPassedToGetParseInfoMethod()
		{
			List<string> expectedFileNames = new List<string>();
			expectedFileNames.Add(@"d:\projects\test\src\Program.cs");
			expectedFileNames.Add(@"d:\projects\test\src\readme.txt");
			Assert.AreEqual(expectedFileNames.ToArray(), convertProjectCommand.FilesPassedToGetParseInfo.ToArray());
		}
	}
}
