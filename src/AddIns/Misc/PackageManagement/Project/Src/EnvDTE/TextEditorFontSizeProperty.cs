// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextEditorFontSizeProperty : Property
	{
		ITextEditorOptions textEditorOptions;
		
		public TextEditorFontSizeProperty()
			: this(new TextEditorOptions())
		{
		}
		
		public TextEditorFontSizeProperty(ITextEditorOptions textEditorOptions)
			: base("FontSize")
		{
			this.textEditorOptions = textEditorOptions;
		}
		
		protected override object GetValue()
		{
			double fontSize = Math.Round(textEditorOptions.FontSize * 72.0 / 96.0);
			return Convert.ToInt16(fontSize);
		}
		
		protected override void SetValue(object value)
		{
			int fontSize = Convert.ToInt32(value);
			textEditorOptions.FontSize = Math.Round(fontSize * 96.0 / 72.0);
		}
	}
}
