// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of the pad content
	/// </summary>
	public class DefinitionViewPad : AbstractPadContent
	{
		TextEditorControl ctl;
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control {
			get {
				return ctl;
			}
		}
		
		/// <summary>
		/// Creates a new DefinitionViewPad object
		/// </summary>
		public DefinitionViewPad()
		{
			ctl = new TextEditorControl();
			ctl.Document.ReadOnly = true;
			ctl.TextEditorProperties = SharpDevelopTextEditorProperties.Instance;
			ctl.ActiveTextAreaControl.TextArea.DoubleClick += OnDoubleClick;
			ParserService.ParserUpdateStepFinished += OnParserUpdateStep;
			ctl.VisibleChanged += delegate { UpdateTick(null); };
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= OnParserUpdateStep;
			ctl.Dispose();
			base.Dispose();
		}
		
		void OnDoubleClick(object sender, EventArgs e)
		{
			string fileName = ctl.FileName;
			if (fileName != null) {
				Caret caret = ctl.ActiveTextAreaControl.Caret;
				FileService.JumpToFilePosition(fileName, caret.Line + 1, caret.Column + 1);
				
				// refresh DefinitionView to show the definition of the expression that was double-clicked
				UpdateTick(null);
			}
		}
		
		void OnParserUpdateStep(object sender, ParserUpdateStepEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateTick, e);
		}
		
		void UpdateTick(ParserUpdateStepEventArgs e)
		{
			if (!this.IsVisible) return;
			LoggingService.Debug("DefinitionViewPad.Update");
			
			ResolveResult res = ResolveAtCaret(e);
			if (res == null) return;
			FilePosition pos = res.GetDefinitionPosition();
			if (pos.IsEmpty) return;
			OpenFile(pos);
		}
		
		bool disableDefinitionView;
		
		ResolveResult ResolveAtCaret(ParserUpdateStepEventArgs e)
		{
			if (disableDefinitionView)
				return null;
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) return null;
			ITextEditorProvider provider = window.ActiveViewContent as ITextEditorProvider;
			if (provider == null) return null;
			ITextEditor editor = provider.TextEditor;
			
			// e might be null when this is a manually triggered update
			// don't resolve when an unrelated file was changed
			if (e != null && editor.FileName != e.FileName) return null;
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null) return null;
			ITextEditorCaret caret = editor.Caret;
			string content = (e == null) ? editor.Document.Text : e.Content.Text;
			if (caret.Offset > content.Length) {
				LoggingService.Debug("caret.Offset = " + caret.Offset + ", content.Length=" + content.Length);
				return null;
			}
			try {
				ExpressionResult expr = expressionFinder.FindFullExpression(content, caret.Offset);
				if (expr.Expression == null) return null;
				return ParserService.Resolve(expr, caret.Line, caret.Column, editor.FileName, content);
			} catch (Exception ex) {
				disableDefinitionView = true;
				ctl.Visible = false;
				MessageService.ShowException(ex, "Error resolving at " + caret.Line + "/" + caret.Column
				                         + ". DefinitionViewPad is disabled until you restart SharpDevelop.");
				return null;
			}
		}
		
		FilePosition oldPosition;
		
		void OpenFile(FilePosition pos)
		{
			if (pos.Equals(oldPosition)) return;
			oldPosition = pos;
			if (pos.FileName != ctl.FileName)
				LoadFile(pos.FileName);
			ctl.ActiveTextAreaControl.ScrollTo(int.MaxValue); // scroll completely down
			ctl.ActiveTextAreaControl.Caret.Line = pos.Line - 1;
			ctl.ActiveTextAreaControl.ScrollToCaret(); // scroll up to search position
		}
		
		/// <summary>
		/// Loads the file from the corresponding text editor window if it is
		/// open otherwise the file is loaded from the file system.
		/// </summary>
		void LoadFile(string fileName)
		{
			// Get currently open text editor that matches the filename.
			ITextEditor openTextEditor = null;
			ITextEditorProvider provider = FileService.GetOpenFile(fileName) as ITextEditorProvider;
			if (provider != null) {
				openTextEditor = provider.TextEditor;
			}
			
			// Load the text into the definition view's text editor.
			if (openTextEditor != null) {
				ctl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
				ctl.Text = openTextEditor.Document.Text;
				ctl.FileName = fileName;
			} else {
				ctl.LoadFile(fileName, true, true); // TODO: get AutoDetectEncoding from settings
			}
		}
	}
}
