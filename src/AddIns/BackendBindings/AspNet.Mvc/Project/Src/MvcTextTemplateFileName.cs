// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class MvcTextTemplateFileName
	{
		string textTemplatesRootDirectory;
		MvcTextTemplateLanguage language;
		string templateName;
		
		public MvcTextTemplateFileName(
			string textTemplatesRootDirectory,
			MvcTextTemplateLanguage language,
			string templateName)
		{
			this.textTemplatesRootDirectory = textTemplatesRootDirectory;
			this.language = language;
			this.templateName = templateName;
		}
		
		public string GetPath()
		{
			string fileName = GetTemplateFileName();
			string directory = GetTemplateDirectory(language);
			return Path.Combine(directory, fileName);
		}
		
		string GetTemplateFileName()
		{
			return templateName + ".tt";
		}
		
		string GetTemplateDirectory(MvcTextTemplateLanguage language)
		{
			string languageSubdirectory = GetLanguageSubdirectory(language);
			return Path.Combine(textTemplatesRootDirectory, languageSubdirectory);
		}
		
		string GetLanguageSubdirectory(MvcTextTemplateLanguage language)
		{
			return String.Format(LanguageSubdirectoryFormatString, language.ToString());
		}
		
		protected abstract string LanguageSubdirectoryFormatString { get; }
	}
}
