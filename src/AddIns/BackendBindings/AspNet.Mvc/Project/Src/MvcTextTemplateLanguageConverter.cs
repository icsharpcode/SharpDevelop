// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public static class MvcTextTemplateLanguageConverter
	{
		/// <summary>
		/// Converts from an IProject.Language string to an MvcTemplateLanguage. Only C# and VB.NET languages
		/// are supported.
		/// </summary>
		public static MvcTextTemplateLanguage Convert(string language)
		{
			if (language == "VBNet") {
				return MvcTextTemplateLanguage.VisualBasic;
			}
			return MvcTextTemplateLanguage.CSharp;
		}
	}
}
