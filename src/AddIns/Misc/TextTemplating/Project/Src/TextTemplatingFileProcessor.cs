// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
