// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateRepository
	{
		string textTemplatesRootDirectory;
		
		public MvcTextTemplateRepository(string mvcAddInPath)
		{
			GetTextTemplatesRootDirectory(mvcAddInPath);
		}
		
		public MvcTextTemplateRepository()
			: this(StringParser.GetValue("addinpath:ICSharpCode.AspNet.Mvc"))
		{
		}
		
		void GetTextTemplatesRootDirectory(string mvcAddInPath)
		{
			this.textTemplatesRootDirectory = Path.Combine(mvcAddInPath, "ItemTemplates");
		}
		
		public string GetMvcViewTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcViewTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
		
		public string GetMvcControllerTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcControllerTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
	}
}
