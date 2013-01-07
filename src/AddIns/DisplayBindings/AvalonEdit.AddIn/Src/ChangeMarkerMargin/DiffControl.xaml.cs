// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Interaction logic for DiffControl.xaml
	/// </summary>
	public partial class DiffControl : UserControl
	{
		public DiffControl()
		{
			InitializeComponent();
			
			revertButton.Content = SD.ResourceService.GetImage("Icons.16x16.UndoIcon").CreateImage();
			copyButton.Content = SD.ResourceService.GetImage("Icons.16x16.CopyIcon").CreateImage();
		}
		
		void CopyButtonClick(object sender, RoutedEventArgs e)
		{
			if (editor.SelectionLength == 0) {
				int offset = editor.CaretOffset;
				editor.SelectAll();
				editor.Copy();
				editor.Select(offset, 0);
			} else {
				editor.Copy();
			}
		}
		
		public void CopyEditorSettingsAndHighlighting(TextEditor source)
		{
			editor.CopySettingsFrom(source);
			string language = source.SyntaxHighlighting != null ? source.SyntaxHighlighting.Name : null;
			editor.TextArea.TextView.LineTransformers.RemoveAll(x => x is HighlightingColorizer);
			var customizedHighlighter = new CustomizingHighlighter(
				new DocumentHighlighter(editor.Document, source.SyntaxHighlighting),
				CustomizedHighlightingColor.FetchCustomizations(language)
			);
			editor.TextArea.TextView.LineTransformers.Insert(0, new HighlightingColorizer(customizedHighlighter));
		}
	}
}