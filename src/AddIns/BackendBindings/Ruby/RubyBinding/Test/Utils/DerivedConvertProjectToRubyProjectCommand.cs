// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace RubyBinding.Tests.Utils
{
	public struct SourceAndTargetFile
	{
		public FileProjectItem Source;
		public FileProjectItem Target;
		
		public SourceAndTargetFile(FileProjectItem source, FileProjectItem target)
		{
			this.Source = source;
			this.Target = target;
		}
	}
	
	/// <summary>
	/// Used to test the ConvertProjectToRubyProjectCommand class.
	/// </summary>
	public class DerivedConvertProjectToRubyProjectCommand : ConvertProjectToRubyProjectCommand
	{
		List<SourceAndTargetFile> sourceAndTargetFilesPassedToBaseClass = new List<SourceAndTargetFile>();
		List<ConvertedFile> savedFiles = new List<ConvertedFile>();
		List<ConvertedFile> parseableFileContent = new List<ConvertedFile>();
		IProjectContent projectContent;
		ParseInformation parseInfo;
		List<string> filesPassedToGetParseInfo = new List<string>();
		Encoding fileServiceDefaultEncoding = Encoding.ASCII;
					
		public List<SourceAndTargetFile> SourceAndTargetFilesPassedToBaseClass {
			get { return sourceAndTargetFilesPassedToBaseClass; }
		}
		
		/// <summary>
		/// Gets the files converted and saved.
		/// </summary>
		public List<ConvertedFile> SavedFiles {
			get { return savedFiles; }
		}
		
		/// <summary>
		/// Sets the project content to be returned from the GetProjectContent method.
		/// </summary>
		public IProjectContent ProjectContent {
			get { return projectContent; }
			set { projectContent = value; }
		}
		
		public ParseInformation ParseInfo {
			get { return parseInfo; }
			set { parseInfo = value; }
		}
		
		public Encoding FileServiceDefaultEncoding {
			get { return fileServiceDefaultEncoding; }
			set { fileServiceDefaultEncoding = value; }
		}
		
		public List<string> FilesPassedToGetParseInfo {
			get { return filesPassedToGetParseInfo; }
		}
		
		public void AddParseableFileContent(string fileName, string content)
		{
			parseableFileContent.Add(new ConvertedFile(fileName, content, null));
		}
		
		public void CallConvertFile(FileProjectItem source, FileProjectItem target)
		{
			ConvertFile(source, target);
		}
		
		public IProject CallCreateProject(string directory, IProject sourceProject)
		{
			return base.CreateProject(directory, sourceProject);
		}
		
		public void CallCopyProperties(IProject source, IProject target)
		{
			base.CopyProperties(source, target);
		}
		
		protected override void LanguageConverterConvertFile(FileProjectItem source, FileProjectItem target)
		{
			sourceAndTargetFilesPassedToBaseClass.Add(new SourceAndTargetFile(source, target));
		}
		
		protected override void SaveFile(string fileName, string content, Encoding encoding)
		{
			savedFiles.Add(new ConvertedFile(fileName, content, encoding));
		}
		
		protected override string GetParseableFileContent(string fileName)
		{
			foreach (ConvertedFile file in parseableFileContent) {
				if (file.FileName == fileName) {
					return file.Text;
				}
			}
			return null;
		}
		
		protected override ParseInformation GetParseInfo(string fileName)
		{
			filesPassedToGetParseInfo.Add(fileName);
			return parseInfo;
		}
			
		protected override IProjectContent GetProjectContent(IProject project)
		{
			return projectContent;
		}
		
		protected override Encoding GetDefaultFileEncoding()
		{
			return fileServiceDefaultEncoding;
		}
	}
}
