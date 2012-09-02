// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Allows creating new text editor instances and accessing the default text editor options.
	/// </summary>
	public interface IEditorControlService
	{
		ITextEditor CreateEditor(out object control);
		ITextEditorOptions GlobalOptions { get; }
		ISyntaxHighlighter CreateHighlighter(IDocument document, string fileName);
	}
	
	/// <summary>
	/// Allows creating new text editor instances and accessing the default text editor options.
	/// </summary>
	public static class EditorControlService
	{
		static readonly Lazy<IEditorControlService> instance = new Lazy<IEditorControlService>(
			delegate {
				// fetch IEditorControlService that's normally implemented in AvalonEdit.AddIn
				var node = Core.AddInTree.GetTreeNode("/SharpDevelop/ViewContent/TextEditor/EditorControlService", false);
				IEditorControlService ecs = null;
				if (node != null && node.Codons.Count > 0) {
					ecs = (IEditorControlService)node.BuildChildItem(node.Codons[0], null);
				}
				return ecs ?? new DummyService();
			}
		);
		
		public static IEditorControlService Instance {
			get { return instance.Value; }
		}
		
		public static ITextEditor CreateEditor(out object control)
		{
			return Instance.CreateEditor(out control);
		}
		
		public static ITextEditorOptions GlobalOptions {
			get { return Instance.GlobalOptions; }
		}
		
		// Fallback if AvalonEdit.AddIn is not available (e.g. some unit tests)
		sealed class DummyService : IEditorControlService, ITextEditorOptions
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
			
			public ISyntaxHighlighter CreateHighlighter(IDocument document, string fileName)
			{
				return null;
			}
		}
	}
}
