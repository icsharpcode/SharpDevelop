// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcViewFileGenerator : IMvcViewFileGenerator
	{
		public FakeMvcViewFileGenerator()
		{
			this.Errors = new CompilerErrorCollection();
		}
		
		public IMvcProject Project { get; set; }
		public string ModelClassName { get; set; }
		public string ModelClassAssemblyLocation { get; set; }
		public bool IsContentPage { get; set; }
		public string MasterPageFile { get; set; }
		public string PrimaryContentPlaceHolderId { get; set; }
		public MvcViewTextTemplate Template { get; set; }
		
		public bool IsGenerateFileCalled;
		public MvcViewFileName FileNamePassedToGenerateFile;
		
		public void GenerateFile(MvcViewFileName fileName)
		{
			FileNamePassedToGenerateFile = fileName;
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
