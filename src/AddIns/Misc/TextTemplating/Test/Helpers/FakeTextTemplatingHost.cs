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
