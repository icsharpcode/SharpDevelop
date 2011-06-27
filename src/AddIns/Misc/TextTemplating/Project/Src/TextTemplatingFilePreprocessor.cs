// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFilePreprocessor : TextTemplatingFileProcessor, ITextTemplatingFilePreprocessor
	{
		public TextTemplatingFilePreprocessor(
			ITextTemplatingHost host,
			FileProjectItem templateFile,
			ITextTemplatingCustomToolContext context)
			: base(host, templateFile, context)
		{
		}
		
		public void PreprocessTemplate()
		{
			ClearTasksExceptCommentTasks();
			
			string outputFileName = GetOutputFileName();
			string classNamespace = GetClassNamespace();
			
			SetNamespaceHint(classNamespace);
			if (TryGenerateOutputFileForTemplate(outputFileName, classNamespace)) {
				AddOutputFileToProjectIfRequired(outputFileName);
			}
			AddAnyErrorsToTaskList();
			BringErrorsToFrontIfRequired();
		}
		
		string GetOutputFileName()
		{
			string extension = GetFileExtensionForProject();
			return Path.ChangeExtension(TemplateFile.FileName, extension);
		}
		
		string GetFileExtensionForProject()
		{
			if (TemplateFile.Project.Language == "VBNet") {
				return ".vb";
			}
			return ".cs";
		}
		
		string GetClassNamespace()
		{
			var hint = new NamespaceHint(TemplateFile);
			return hint.ToString();
		}
		
		bool TryGenerateOutputFileForTemplate(string outputFileName, string classNamespace)
		{
			string language = null;
			string[] references = null;
			
			string className = GetClassName();
			
			string inputFileName = TemplateFile.FileName;
			
			try {
				return Host.PreprocessTemplate(
					inputFileName,
					className,
					classNamespace,
					outputFileName,
					Encoding.UTF8,
					out language,
					out references);			
			} catch (Exception ex) {
				AddCompilerErrorToTemplatingHost(ex, inputFileName);
				DebugLogException(ex, inputFileName);
			}
			return false;
		}
		
		string GetClassName()
		{
			string className = Path.GetFileNameWithoutExtension(TemplateFile.FileName);
			return CreateValidClassName(className);
		}
		
		string CreateValidClassName(string className)
		{
			return TemplateFile
				.Project
				.LanguageProperties
				.CodeDomProvider
				.CreateValidIdentifier(className);
		}
	}
}
