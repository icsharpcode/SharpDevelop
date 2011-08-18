// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcControllerFileGenerator : IMvcControllerFileGenerator
	{
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public IMvcProject Project { get; set; }
		public bool AddActionMethods { get; set; }
		
		public bool IsGenerateFileCalled;
		public MvcControllerFileName FileNamePassedToGenerateController;
		
		public void GenerateFile(MvcControllerFileName fileName)
		{
			FileNamePassedToGenerateController = fileName;
			IsGenerateFileCalled = true;
		}
	}
}
