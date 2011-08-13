// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class MvcTextTemplateFileName
	{
		string textTemplatesRootFolder;
		MvcTextTemplateCriteria templateCriteria;
		
		public MvcTextTemplateFileName(
			string textTemplatesRootFolder,
			MvcTextTemplateCriteria templateCriteria)
		{
			this.textTemplatesRootFolder = textTemplatesRootFolder;
			this.templateCriteria = templateCriteria;
		}
		
		public string GetPath()
		{
			string fileName = GetTemplateFileName();
			string directory = GetTemplateFolder();
			return Path.Combine(directory, fileName);
		}
		
		string GetTemplateFileName()
		{
			return templateCriteria.TemplateName + ".tt";
		}
		
		string GetTemplateFolder()
		{
			string codeTemplatesDirectory = GetCodeTemplatesFolder();
			string subFolder = GetCodeTemplatesSubFolder();
			return Path.Combine(codeTemplatesDirectory, subFolder);
		}
		
		string GetCodeTemplatesFolder()
		{
			MvcTextTemplateLanguage language = templateCriteria.TemplateLanguage;
			string subFolder = String.Format("{0}\\CodeTemplates", language.ToString());
			return Path.Combine(textTemplatesRootFolder, subFolder);
		}
		
		protected abstract string GetCodeTemplatesSubFolder();
	}
}
