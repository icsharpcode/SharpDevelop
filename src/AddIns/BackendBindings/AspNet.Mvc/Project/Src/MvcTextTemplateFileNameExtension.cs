// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public static class MvcTextTemplateFileNameExtension
	{
		public static string GetControllerFileExtension(MvcTextTemplateLanguage language)
		{
			if (language.IsVisualBasic()) {
				return ".vb";
			}
			return ".cs";
		}
		
		public static string GetViewFileExtension(
			MvcTextTemplateType type,
			MvcTextTemplateLanguage language)
		{
			if (type.IsAspx()) {
				return ".aspx";
			}
			return GetRazorFileExtension(language);
		}
		
		public static string GetRazorFileExtension(MvcTextTemplateLanguage language)
		{
			if (language.IsVisualBasic()) {
				return ".vbhtml";
			}
			return ".cshtml";
		}
	}
}
