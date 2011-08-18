// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.SharpDevelop.Project;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeSelectedMvcFolder : ISelectedMvcFolder
	{
		public string Path { get; set; }
		
		public FakeMvcProject FakeMvcProject = new FakeMvcProject();
		
		public IMvcProject Project {
			get { return FakeMvcProject; }
		}
		
		public string FileNamePassedToAddFile;
		
		public void AddFileToProject(string fileName)
		{
			FileNamePassedToAddFile = fileName;
		}
		
		public MvcTextTemplateLanguage TemplateLanguage = MvcTextTemplateLanguage.CSharp;
		
		public MvcTextTemplateLanguage GetTemplateLanguage()
		{
			return TemplateLanguage;
		}
		
		public void SetCSharpAsTemplateLanguage()
		{
			TemplateLanguage = MvcTextTemplateLanguage.CSharp;
		}
		
		public void SetVisualBasicAsTemplateLanguage()
		{
			TemplateLanguage = MvcTextTemplateLanguage.VisualBasic;
		}
	}
}
