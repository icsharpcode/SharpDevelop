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
using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;
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
		MockProject targetProject;
		FileProjectItem textFileSource;
		FileProjectItem textFileTarget;
		MockTextEditorProperties mockTextEditorProperties;
		string sourceCode = "class Foo\r\n" +
							"{\r\n" +
							"}";
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			mockTextEditorProperties = new MockTextEditorProperties();
			convertProjectCommand = new DerivedConvertProjectToPythonProjectCommand(mockTextEditorProperties);
			mockTextEditorProperties.Encoding = Encoding.Unicode;
			
			sourceProject = new MockProject();
			sourceProject.Directory = @"d:\projects\test";
			source = new FileProjectItem(sourceProject, ItemType.Compile, @"src\Program.cs");
			targetProject = new MockProject();
			targetProject.Directory = @"d:\projects\test\converted";
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
		public void CommandHasPythonTargetLanguage()
		{
			Assert.AreEqual(PythonLanguageBinding.LanguageName, convertProjectCommand.TargetLanguageName);
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
			string expectedCode = converter.Convert(sourceCode, SupportedLanguage.CSharp);
			
			List<ConvertedFile> expectedSavedFiles = new List<ConvertedFile>();
			expectedSavedFiles.Add(new ConvertedFile(target.FileName, expectedCode, mockTextEditorProperties.Encoding));
			Assert.AreEqual(expectedSavedFiles, convertProjectCommand.SavedFiles);
		}
	}
}
