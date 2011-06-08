// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class TextEditorFontsAndColorsItemsPropertyTests
	{
		TextEditorFontsAndColorsItemsProperty property;
		
		void CreateProperty()
		{
			property = new TextEditorFontsAndColorsItemsProperty();
		}
		
		[Test]
		public void Object_PropertyAccessed_ReturnsFontsAndColorsItems()
		{
			CreateProperty();
			
			var fontsAndColorsItemsObject = property.Object as FontsAndColorsItems;
			
			Assert.IsNotNull(fontsAndColorsItemsObject);
		}
	}
}
