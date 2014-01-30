// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.Highlighting;
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
