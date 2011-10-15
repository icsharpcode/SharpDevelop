// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextEditorFontsAndColorsPropertyFactory : IPropertyFactory
	{
		public Property CreateProperty(string name)
		{
			if (IsFontsAndColorItems(name)) {
				return new TextEditorFontsAndColorsItemsProperty();
			} else if (IsFontSize(name)) {
				return new TextEditorFontSizeProperty();
			}
			return null;
		}
		
		bool IsFontsAndColorItems(string name)
		{
			return IsCaseInsensitiveMatch(name, "FontsAndColorsItems");
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
		
		bool IsFontSize(string name)
		{
			return IsCaseInsensitiveMatch(name, "FontSize");
		}
		
		public IEnumerator<Property> GetEnumerator()
		{
			List<Property> properties = GetProperties().ToList();
			return properties.GetEnumerator();
		}
		
		IEnumerable<Property> GetProperties()
		{
			yield return new TextEditorFontSizeProperty();
			yield return new TextEditorFontsAndColorsItemsProperty();
		}
	}
}
