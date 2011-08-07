// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public static class MvcTextTemplateLanguageFileNameExtension
	{
		public static string GetFileExtension(MvcTextTemplateLanguage language)
		{
			if (language == MvcTextTemplateLanguage.VisualBasic) {
				return ".vb";
			}
			return ".cs";
		}
	}
}
