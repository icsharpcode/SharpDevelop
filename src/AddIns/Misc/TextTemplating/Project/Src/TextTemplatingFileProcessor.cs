// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public abstract class TextTemplatingFileProcessor
	{
		ITextTemplatingHost host;
		FileProjectItem templateFile;
		ITextTemplatingCustomToolContext context;

		public TextTemplatingFileProcessor(
			ITextTemplatingHost host,
			FileProjectItem templateFile,
			ITextTemplatingCustomToolContext context)
		{
			this.host = host;
			this.templateFile = templateFile;
			this.context = context;
		}

		protected ITextTemplatingHost Host {
			get { return host; }
		}
		
		protected FileProjectItem TemplateFile {
			get { return templateFile; }
		}
		
		protected ITextTemplatingCustomToolContext Context {
			get { return context; }
		}
		
		protected void ClearTasksExceptCommentTasks()
		{
			context.ClearTasksExceptCommentTasks();
		}
		
		protected void SetNamespaceHint(string namespaceHint)
		{
			context.SetLogicalCallContextData("NamespaceHint", namespaceHint);
		}

		protected void BringErrorsToFrontIfRequired()
		{
			if (host.Errors.HasErrors) { 
				context.BringErrorsPadToFront();
			}
		}
		
		protected void AddCompilerErrorToTemplatingHost(Exception ex, string fileName)
		{
			var error = new TemplatingHostProcessTemplateError(ex, fileName);
			host.Errors.Add(error);
		}
		
		protected void DebugLogException(Exception ex, string fileName)
		{
			string message = String.Format("Exception thrown when processing template '{0}'.", fileName);
			context.DebugLog(message, ex);
		}
				
		protected void AddAnyErrorsToTaskList()
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

		protected void AddOutputFileToProjectIfRequired(string outputFileName)
		{
			context.EnsureOutputFileIsInProject(templateFile, outputFileName);
		}
	}
}
