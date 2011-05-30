// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting
{
	public static class TextEditorFactory
	{
		public static TextEditor CreateTextEditor()
		{
			object control;
			EditorControlService.CreateEditor(out control);
			var textEditor = (TextEditor)control;
			
			textEditor.Options = new TextEditorOptions();
			textEditor.Options.AllowScrollBelowDocument = false;
			textEditor.FontFamily = new FontFamily(WinFormsResourceService.DefaultMonospacedFont.Name);
			textEditor.FontSize = 13.0;
			
			return textEditor;
		}
	}
}
