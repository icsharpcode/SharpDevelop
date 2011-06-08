// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class TextEditorFontSizePropertyTests
	{
		TextEditorFontSizeProperty property;
		FakeTextEditorOptions textEditorOptions;
		
		void CreateProperty()
		{
			textEditorOptions = new FakeTextEditorOptions();
			property = new TextEditorFontSizeProperty(textEditorOptions);
		}
		
		[Test]
		public void Value_GetValueWhenTextEditorFontSizeIs12_Returns9()
		{
			CreateProperty();
			textEditorOptions.FontSize = 12;
			
			Int16 value = (Int16)property.Value;
			
			Assert.AreEqual(9, value);
		}
		
		[Test]
		public void Value_SetValueTo10WhenTextEditorFontSizeIs12_TextEditorOptionsAreUpdated()
		{
			CreateProperty();
			textEditorOptions.FontSize = 12;
			
			Int16 fontSize = 10;
			property.Value = fontSize;
			
			Assert.AreEqual(13, textEditorOptions.FontSize);
		}
		
		[Test]
		public void Value_SetValueToInt32_InvalidCastExceptionIsNotThrownAndTextEditorOptionsAreUpdated()
		{
			CreateProperty();
			textEditorOptions.FontSize = 12;
			
			int fontSize = 10;
			property.Value = fontSize;
			
			Assert.AreEqual(13, textEditorOptions.FontSize);
		}
	}
}
