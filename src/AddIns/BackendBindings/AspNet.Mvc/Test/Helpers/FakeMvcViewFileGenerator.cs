// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcViewFileGenerator : IMvcViewFileGenerator
	{
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public MvcTextTemplateType TemplateType { get; set; }
		public IMvcProject Project { get; set; }
		public string ModelClassName { get; set; }
		
		public bool IsGenerateFileCalled;
		public MvcViewFileName FileNamePassedToGenerateFile;
		
		public void GenerateFile(MvcViewFileName fileName)
		{
			FileNamePassedToGenerateFile = fileName;
			IsGenerateFileCalled = true;
		}
	}
}
