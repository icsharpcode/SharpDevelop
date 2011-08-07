// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerTextTemplateFileName : MvcTextTemplateFileName
	{
		public MvcControllerTextTemplateFileName(
			string textTemplatesRootDirectory,
			MvcTextTemplateLanguage language,
			string templateName)
			: base(textTemplatesRootDirectory, language, templateName)
		{
		}
		
		protected override string LanguageSubdirectoryFormatString {
			get { return "{0}\\CodeTemplates\\AddController"; }
		}
	}
}
