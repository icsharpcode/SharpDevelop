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
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Codons;
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
		MockTextEditorProperties mockTextEditorProperties;
		ParseInformation parseInfo;
		
		string sourceCode = "class Foo\r\n" +
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
			using (TextReader reader = RubyBindingAddInFile.ReadAddInFile()) {
				AddIn addin = AddIn.Load(reader, String.Empty);
				bindings.Add(new LanguageBindingDescriptor(AddInHelper.GetCodon(addin, "/SharpDevelop/Workbench/LanguageBindings", "Ruby")));
			}
			LanguageBindingService.SetBindings(bindings);
			
			mockTextEditorProperties = new MockTextEditorProperties();
			convertProjectCommand = new DerivedConvertProjectToRubyProjectCommand(mockTextEditorProperties);
			parseInfo = new ParseInformation();
			convertProjectCommand.ParseInfo = parseInfo;
			mockTextEditorProperties.Encoding = Encoding.Unicode;
			
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
			Assert.AreEqual(RubyLanguageBinding.LanguageName, convertProjectCommand.TargetLanguageName);
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
			expectedSavedFiles.Add(new ConvertedFile(target.FileName, expectedCode, mockTextEditorProperties.Encoding));
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
