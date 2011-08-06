// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcViewFileGenerator : IMvcViewFileGenerator
	{
		public MvcTextTemplateLanguage Language { get; set; }
		public IProject Project { get; set; }
		
		public bool IsGenerateViewCalled;
		public MvcViewFileName FileNamePassedToGenerateView;
		
		public void GenerateView(MvcViewFileName fileName)
		{
			FileNamePassedToGenerateView = fileName;
			IsGenerateViewCalled = true;
		}
	}
}
