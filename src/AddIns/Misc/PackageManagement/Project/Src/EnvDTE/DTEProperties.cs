// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DTEProperties
	{
		public Properties GetProperties(string category, string page)
		{
			if (IsTextEditorFontsAndColors(category, page)) {
				return CreateTextEditorFontsAndColorsProperties();
			}
			return null;
		}
		
		bool IsTextEditorFontsAndColors(string category, string page)
		{
			return
				IsCaseInsensitiveMatch(category, "FontsAndColors") &&
				IsCaseInsensitiveMatch(page, "TextEditor");
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
		
		Properties CreateTextEditorFontsAndColorsProperties()
		{
			return new Properties(new TextEditorFontsAndColorsPropertyFactory());
		}
	}
}
