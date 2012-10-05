// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.Bookmarks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.ILSpyAddIn.ViewContent
{
	/// <summary>
	/// Equivalent to AE.AddIn CodeEditor, but without editing capabilities.
	/// </summary>
	class CodeView : Grid, IDisposable, IPositionable
	{
		readonly SharpDevelopTextEditor textEditor;
		readonly IconBarManager iconBarManager;
		readonly IconBarMargin iconMargin;
		readonly TextMarkerService textMarkerService;
		readonly AvalonEditTextEditorAdapter adapter;
		
		public CodeView()
		{
			textEditor = new SharpDevelopTextEditor();
			textEditor.IsReadOnly = true;
			this.Children.Add(textEditor);
			adapter = new AvalonEditTextEditorAdapter(textEditor);
			
			textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
			
			// add margin
			this.iconMargin = new IconBarMargin(iconBarManager = new IconBarManager());
			textEditor.TextArea.LeftMargins.Insert(0, iconMargin);
			textEditor.TextArea.TextView.VisualLinesChanged += delegate { iconMargin.InvalidateVisual(); };
			
			// add marker service
			this.textMarkerService = new TextMarkerService(textEditor.Document);
			textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
			var documentServiceContainer = textEditor.Document.GetRequiredService<ServiceContainer>();
			documentServiceContainer.AddService(typeof(ITextMarkerService), textMarkerService);
			documentServiceContainer.AddService(typeof(IBookmarkMargin), iconBarManager);
			
			textEditor.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new SearchInputHandler(textEditor.TextArea));
		}

		public TextDocument Document {
			get { return textEditor.Document; }
		}
		
		public IconBarManager IconBarManager {
			get { return iconBarManager; }
		}
		
		public void Dispose()
		{
		}
		
		public int Line {
			get {
				return textEditor.TextArea.Caret.Line;
			}
		}
		
		public int Column {
			get {
				return textEditor.TextArea.Caret.Column;
			}
		}
		
		public void JumpTo(int line, int column)
		{
			adapter.JumpTo(line, column);
		}
	}
}
