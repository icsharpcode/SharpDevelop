// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingHost : ITextTemplatingHost
	{
		public string InputFilePassedToProcessTemplate;
		public string OutputFilePassedToProcessTemplate;
		public bool ProcessTemplateReturnValue = true;
		public bool IsDisposeCalled;
		public Exception ExceptionToThrowWhenProcessTemplateCalled;
		
		public CompilerErrorCollection ErrorsCollection = new CompilerErrorCollection();
		
		public CompilerErrorCollection Errors {
			get { return ErrorsCollection; }
		}
		
		public bool ProcessTemplate(string inputFile, string outputFile)
		{
			InputFilePassedToProcessTemplate = inputFile;
			OutputFilePassedToProcessTemplate = outputFile;
			
			if (ExceptionToThrowWhenProcessTemplateCalled != null) {
				throw ExceptionToThrowWhenProcessTemplateCalled;
			}
			
			return ProcessTemplateReturnValue;
		}
		
		public void Dispose()
		{
			IsDisposeCalled = true;
		}
		
		public string OutputFile { get; set; }
	}
}
