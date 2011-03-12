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
		FileProjectItem templateFile;
		ITextTemplatingCustomToolContext context;
		
		public TextTemplatingFileGenerator(
			ITextTemplatingHost host,
			FileProjectItem projectFile,
			ITextTemplatingCustomToolContext context)
		{
			this.host = host;
			this.templateFile = projectFile;
			this.context = context;
		}
		
		public void Dispose()
		{
			host.Dispose();
		}
		
		public void ProcessTemplate()
		{
			context.ClearTasksExceptCommentTasks();
			SetLogicalCallContextData();
			if (TryGenerateOutputFileForTemplate()) {
				AddOutputFileToProjectIfRequired();
			}
			AddAnyErrorsToTaskList();
			BringErrorsToFrontIfRequired();
		}
		
		void SetLogicalCallContextData()
		{
			var namespaceHint = new NamespaceHint(templateFile);
			context.SetLogicalCallContextData("NamespaceHint", namespaceHint.ToString());
		}

		bool TryGenerateOutputFileForTemplate()
		{
			string inputFileName = templateFile.FileName;
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
		
		void BringErrorsToFrontIfRequired()
		{
			if (host.Errors.HasErrors) { 
				context.BringErrorsPadToFront();
			}
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
			context.EnsureOutputFileIsInProject(templateFile, host.OutputFile);
		}
	}
}