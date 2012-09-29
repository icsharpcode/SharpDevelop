// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcControllerFileGenerator : IMvcControllerFileGenerator
	{
		public FakeMvcControllerFileGenerator()
		{
			this.Errors = new CompilerErrorCollection();
		}
		
		public IMvcProject Project { get; set; }
		public MvcControllerTextTemplate Template { get; set; }
		
		public bool IsGenerateFileCalled;
		public MvcControllerFileName FileNamePassedToGenerateController;
		
		public void GenerateFile(MvcControllerFileName fileName)
		{
			FileNamePassedToGenerateController = fileName;
			IsGenerateFileCalled = true;
		}
		
		public CompilerErrorCollection Errors { get; set; }
		
		public CompilerError AddCompilerError()
		{
			var error = new CompilerError();
			Errors.Add(error);
			return error;
		}
		
		public bool HasErrors {
			get { return Errors.Count > 0; }
		}
	}
}
