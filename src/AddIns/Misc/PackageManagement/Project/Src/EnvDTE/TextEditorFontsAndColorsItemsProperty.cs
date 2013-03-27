// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextEditorFontsAndColorsItemsProperty : Property
	{
		FontsAndColorsItems fontsAndColorsItems;
		
		public TextEditorFontsAndColorsItemsProperty()
			: base("FontsAndColorsItems")
		{
			fontsAndColorsItems = new FontsAndColorsItems();
		}
		
		protected override object GetObject()
		{
			return fontsAndColorsItems;
		}
	}
}
