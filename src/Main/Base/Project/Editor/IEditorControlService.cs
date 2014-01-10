// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Allows creating new text editor instances and accessing the default text editor options.
	/// </summary>
	[SDService("SD.EditorControlService", FallbackImplementation = typeof(EditorControlServiceFallback))]
	public interface IEditorControlService
	{
		ITextEditor CreateEditor(out object control);
		ITextEditorOptions GlobalOptions { get; }
		
		/// <summary>
		/// Creates a highlighter for the specified document.
		/// This method never returns null.
		/// </summary>
		IHighlighter CreateHighlighter(IDocument document);
	}
	
	// Fallback if AvalonEdit.AddIn is not available (e.g. some unit tests)
	sealed class EditorControlServiceFallback : IEditorControlService, ITextEditorOptions
	{
		public ITextEditorOptions GlobalOptions {
			get { return this; }
		}
		
		public ITextEditor CreateEditor(out object control)
		{
			TextEditor avalonedit = new TextEditor();
			control = avalonedit;
			return new AvalonEditTextEditorAdapter(avalonedit);
		}
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged { add {} remove {} }
		
		public string IndentationString {
			get { return "\t"; }
		}
		
		public bool AutoInsertBlockEnd {
			get { return true; }
		}
		
		public bool ConvertTabsToSpaces {
			get { return false; }
		}
		
		public int IndentationSize {
			get { return 4; }
		}
		
		public int VerticalRulerColumn {
			get { return 120; }
		}
		
		public bool UnderlineErrors {
			get { return true; }
		}
		
		public string FontFamily {
			get {
				return "Consolas";
			}
		}
		
		public double FontSize {
			get {
				return 10.0;
			}
		}
		
		public IHighlighter CreateHighlighter(IDocument document)
		{
			return null;
		}
	}
}
