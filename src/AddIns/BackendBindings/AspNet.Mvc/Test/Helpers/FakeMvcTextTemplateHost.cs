// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcTextTemplateHost : IMvcTextTemplateHost
	{
		public bool ProcessTemplateReturnValue = true;
		public string InputFilePassedToProcessTemplate;
		public string OutputFilePassedToProcessTemplate;
		
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
	}
}
