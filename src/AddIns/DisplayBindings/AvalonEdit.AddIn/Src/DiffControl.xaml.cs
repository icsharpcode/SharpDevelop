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
			
			revertButton.Content = PresentationResourceService.GetImage("Icons.16x16.UndoIcon");
			copyButton.Content = PresentationResourceService.GetImage("Icons.16x16.CopyIcon");
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
		
		public void CopyEditorSettings(TextEditor source)
		{
			string language = source.SyntaxHighlighting != null ? source.SyntaxHighlighting.Name : null;
			editor.TextArea.TextView.LineTransformers.RemoveWhere(x => x is HighlightingColorizer);
			if (source.SyntaxHighlighting != null)
				editor.TextArea.TextView.LineTransformers.Insert(0, new CustomizableHighlightingColorizer(source.SyntaxHighlighting.MainRuleSet, CustomizedHighlightingColor.FetchCustomizations(language)));
			CustomizableHighlightingColorizer.ApplyCustomizationsToDefaultElements(editor, CustomizedHighlightingColor.FetchCustomizations(language));
			HighlightingOptions.ApplyToRendering(editor, CustomizedHighlightingColor.FetchCustomizations(language));
			editor.TextArea.TextView.Redraw(); // manually redraw if default elements didn't change but customized highlightings did
		}
	}
}