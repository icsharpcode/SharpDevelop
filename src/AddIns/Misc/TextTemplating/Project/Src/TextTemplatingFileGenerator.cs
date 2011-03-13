// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFileGenerator : TextTemplatingFileProcessor, ITextTemplatingFileGenerator
	{
		public TextTemplatingFileGenerator(
			ITextTemplatingHost host,
			FileProjectItem templateFile,
			ITextTemplatingCustomToolContext context)
			: base(host, templateFile, context)
		{
		}
		
		public void Dispose()
		{
			Host.Dispose();
		}
		
		public void ProcessTemplate()
		{
			ClearTasksExceptCommentTasks();
			SetNamespaceHint();
			if (TryGenerateOutputFileForTemplate()) {
				AddOutputFileToProjectIfRequired();
			}
			AddAnyErrorsToTaskList();
			BringErrorsToFrontIfRequired();
		}
		
		void SetNamespaceHint()
		{
			var namespaceHint = new NamespaceHint(TemplateFile);
			SetNamespaceHint(namespaceHint.ToString());
		}

		bool TryGenerateOutputFileForTemplate()
		{
			string inputFileName = TemplateFile.FileName;
			string outputFileName = GetOutputFileName(inputFileName);
			return TryProcessingTemplate(inputFileName, outputFileName);
		}
		
		void AddOutputFileToProjectIfRequired()
		{
			AddOutputFileToProjectIfRequired(Host.OutputFile);
		}
		
		string GetOutputFileName(string inputFileName)
		{
			return Path.ChangeExtension(inputFileName, ".cs");
		}

		bool TryProcessingTemplate(string inputFileName, string outputFileName)
		{
			try {
				return Host.ProcessTemplate(inputFileName, outputFileName);
			} catch (Exception ex) {
				AddCompilerErrorToTemplatingHost(ex, inputFileName);
				DebugLogException(ex, inputFileName);
			}
			return false;
		}
	}
}