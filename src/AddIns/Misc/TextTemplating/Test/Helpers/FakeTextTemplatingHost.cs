// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.Text;

using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingHost : ITextTemplatingHost
	{
		public string InputFilePassedToProcessTemplate;
		public string OutputFilePassedToProcessTemplate;
		public bool ProcessTemplateReturnValue = true;
		
		public string InputFilePassedToPreprocessTemplate;
		public Encoding EncodingPassedToPreprocessTemplate;
		public string ClassNamePassedToPreprocessTemplate;
		public string ClassNamespacePassedToPreprocessTemplate;
		public string OutputFilePassedToPreprocessTemplate;
		public bool PreprocessTemplateReturnValue = true;
		public string PreprocessTemplateLanguageOutParameter;
		public string[] PreprocessTemplateReferencesOutParameter;
		
		public bool IsDisposeCalled;
		public Exception ExceptionToThrowWhenProcessTemplateCalled;
		
		public Exception ExceptionToThrowWhenPreprocessTemplateCalled;
		
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
		
		public bool PreprocessTemplate (
			string inputFile,
			string className,
			string classNamespace,
			string outputFile,
			Encoding encoding,
			out string language,
			out string[] references)
		{
			InputFilePassedToPreprocessTemplate = inputFile;
			ClassNamePassedToPreprocessTemplate = className;
			ClassNamespacePassedToPreprocessTemplate = classNamespace;
			OutputFilePassedToPreprocessTemplate = outputFile;
			EncodingPassedToPreprocessTemplate = encoding;
			
			language = PreprocessTemplateLanguageOutParameter;
			references = PreprocessTemplateReferencesOutParameter;
			
			if (ExceptionToThrowWhenPreprocessTemplateCalled != null) {
				throw ExceptionToThrowWhenPreprocessTemplateCalled;
			}
			
			return PreprocessTemplateReturnValue;
		}
	}
}
