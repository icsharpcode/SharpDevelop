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
	public class TextTemplatingFileGenerator : ITextTemplatingFileGenerator
	{
		ITextTemplatingHost host;
		FileProjectItem projectFile;
		ITextTemplatingCustomToolContext context;
		
		public TextTemplatingFileGenerator(
			ITextTemplatingHost host,
			FileProjectItem projectFile,
			ITextTemplatingCustomToolContext context)
		{
			this.host = host;
			this.projectFile = projectFile;
			this.context = context;
		}
		
		public void Dispose()
		{
			host.Dispose();
		}
		
		public void ProcessTemplate()
		{
			context.ClearTasksExceptCommentTasks();
			if (TryGenerateOutputFileForTemplate()) {
				AddOutputFileToProjectIfRequired();
			}
			AddAnyErrorsToTaskList();
			BringErrorsToFrontIfRequired();
		}
		
		void BringErrorsToFrontIfRequired()
		{
			if (host.Errors.HasErrors) { 
				context.BringErrorsPadToFront();
			}
		}

		bool TryGenerateOutputFileForTemplate()
		{
			string inputFileName = projectFile.FileName;
			string outputFileName = GetOutputFileName(inputFileName);
			return TryProcessingTemplate(inputFileName, outputFileName);
		}
		
		string GetOutputFileName(string inputFileName)
		{
			return Path.ChangeExtension(inputFileName, ".cs");
		}

		bool TryProcessingTemplate(string inputFileName, string outputFileName)
		{
			try {
				return host.ProcessTemplate(inputFileName, outputFileName);
			} catch (Exception ex) {
				AddCompilerErrorToTemplatingHost(ex, inputFileName);
				DebugLogException(ex, inputFileName);
			}
			return false;
		}
		
		void AddCompilerErrorToTemplatingHost(Exception ex, string fileName)
		{
			var error = new TemplatingHostProcessTemplateError(ex, fileName);
			host.Errors.Add(error);
		}
		
		void DebugLogException(Exception ex, string fileName)
		{
			string message = String.Format("Exception thrown when processing template '{0}'.", fileName);
			context.DebugLog(message, ex);
		}
				
		void AddAnyErrorsToTaskList()
		{
			foreach (CompilerError error in host.Errors) {
				AddErrorToTaskList(error);
			}
		}
		
		void AddErrorToTaskList(CompilerError error)
		{
			var task = new CompilerErrorTask(error);
			context.AddTask(task);
		}

		void AddOutputFileToProjectIfRequired()
		{
			context.EnsureOutputFileIsInProject(projectFile, host.OutputFile);
		}
	}
}