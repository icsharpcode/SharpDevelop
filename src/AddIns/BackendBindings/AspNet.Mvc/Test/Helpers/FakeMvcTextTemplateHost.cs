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
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcTextTemplateHost : IMvcTextTemplateHost
	{
		public bool ProcessTemplateReturnValue = true;
		public string InputFilePassedToProcessTemplate;
		public string OutputFilePassedToProcessTemplate;
		
		public FakeMvcTextTemplateHost()
		{
			this.Errors = new CompilerErrorCollection();
		}
		
		public bool ProcessTemplate(string inputFile, string outputFile)
		{
			InputFilePassedToProcessTemplate = inputFile;
			OutputFilePassedToProcessTemplate = outputFile;
			return ProcessTemplateReturnValue;
		}
		
		public bool IsDisposed;
		
		public void Dispose()
		{
			IsDisposed = true;
		}
		
		public string ViewName { get; set; }
		public string ControllerName { get; set; }
		public string ControllerRootName { get; set; }
		public string Namespace { get; set; }
		public bool AddActionMethods { get; set; }
		public bool IsPartialView { get; set; }
		public string ViewDataTypeName { get; set; }
		public string ViewDataTypeAssemblyLocation { get; set; }
		public bool IsContentPage { get; set; }
		public string MasterPageFile { get; set; }
		public string PrimaryContentPlaceHolderID { get; set; }
		
		public CompilerErrorCollection Errors { get; set; }
		
		public CompilerError AddCompilerError()
		{
			var error = new CompilerError();
			Errors.Add(error);
			return error;
		}
	}
}
