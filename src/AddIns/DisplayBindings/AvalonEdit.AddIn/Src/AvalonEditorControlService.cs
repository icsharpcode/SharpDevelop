// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Implementation of IEditorControlService, allows other addins to create editors or access the options without
	/// requiring a reference to AvalonEdit.AddIn.
	/// </summary>
	public class AvalonEditorControlService : IEditorControlService
	{
		public ITextEditorOptions GlobalOptions {
			get { return CodeEditorOptions.Instance; }
		}
		
		public ITextEditor CreateEditor(out object control)
		{
			SharpDevelopTextEditor editor = new SharpDevelopTextEditor();
			control = editor;
			return new CodeCompletionEditorAdapter(editor);
		}
		
		public ISyntaxHighlighter CreateHighlighter(IDocument document, string fileName)
		{
			var def = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
			var doc = document.GetService(typeof(TextDocument)) as TextDocument;
			if (def == null || doc == null)
				return null;
			var baseHighlighter = new DocumentHighlighter(doc, def.MainRuleSet);
			var highlighter = new CustomizableHighlightingColorizer.CustomizingHighlighter(CustomizedHighlightingColor.FetchCustomizations(def.Name), baseHighlighter);
			return new DocumentSyntaxHighlighter(document, highlighter, def.Name);
		}
	}
}
