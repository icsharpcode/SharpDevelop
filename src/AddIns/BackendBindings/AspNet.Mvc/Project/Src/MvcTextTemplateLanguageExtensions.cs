// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public static class MvcTextTemplateLanguageExtensions
	{
		public static bool IsCSharp(this MvcTextTemplateLanguage templateLanguage)
		{
			return templateLanguage == MvcTextTemplateLanguage.CSharp;
		}
		
		public static bool IsVisualBasic(this MvcTextTemplateLanguage templateLanguage)
		{
			return templateLanguage == MvcTextTemplateLanguage.VisualBasic;
		}
	}
}
