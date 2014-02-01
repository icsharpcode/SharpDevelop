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
