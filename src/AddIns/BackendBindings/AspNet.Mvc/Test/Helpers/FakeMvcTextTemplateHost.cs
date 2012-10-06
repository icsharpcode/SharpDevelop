// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
