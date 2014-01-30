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
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.CSharp;

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
			if (TemplateFile.Project.Language == "VB") {
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
			return CreateCodeDomProvider().CreateValidIdentifier(className);
		}
		
		CodeDomProvider CreateCodeDomProvider()
		{
			CodeDomProvider provider = TemplateFile.Project.CreateCodeDomProvider();
			if (provider != null) {
				return provider;
			}
			AddMissingCodeDomProviderTask();
			return new CSharpCodeProvider();
		}
		
		void AddMissingCodeDomProviderTask()
		{
			string message = "Project does not provide a CodeDomProvider. Using C# provider by default";
			Context.AddTask(new SDTask(null, message, 0, 0, TaskType.Warning));
		}
	}
}
