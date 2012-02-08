// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateTypeFolder
	{
		string name;
		
		public MvcTextTemplateTypeFolder(MvcTextTemplateCriteria criteria)
			: this(criteria.TemplateType, criteria.TemplateLanguage)
		{
		}
		
		public MvcTextTemplateTypeFolder(
			MvcTextTemplateType type,
			MvcTextTemplateLanguage language)
		{
			name = GetFolderName(type, language);
		}
		
		string GetFolderName(MvcTextTemplateType type, MvcTextTemplateLanguage language)
		{
			if (type.IsRazor()) {
				return GetRazorFolderName(language);
			}
			return GetAspxFolderName(language);
		}
		
		string GetRazorFolderName(MvcTextTemplateLanguage language)
		{
			if (language.IsVisualBasic()) {
				return "VBHTML";
			}
			return "CSHTML";
		}
		
		string GetAspxFolderName(MvcTextTemplateLanguage language)
		{
			return "Aspx" + language;
		}
		
		public string Name {
			get { return name; }
		}
	}
}
