// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.PythonBinding;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;

namespace PythonBinding.Tests.Utils
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
	/// Used to test the ConvertProjectToPythonProjectCommand class.
	/// </summary>
	public class DerivedConvertProjectToPythonProjectCommand : ConvertProjectToPythonProjectCommand
	{
		List<SourceAndTargetFile> sourceAndTargetFilesPassedToBaseClass = new List<SourceAndTargetFile>();
		List<ConvertedFile> savedFiles = new List<ConvertedFile>();
		List<ConvertedFile> parseableFileContent = new List<ConvertedFile>();
		IProjectContent projectContent;
		
		public DerivedConvertProjectToPythonProjectCommand(ITextEditorProperties textEditorProperties) 
			: base(textEditorProperties)
		{
		}
					
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
		
		protected override IProjectContent GetProjectContent(IProject project)
		{
			return projectContent;
		}
	}
}
