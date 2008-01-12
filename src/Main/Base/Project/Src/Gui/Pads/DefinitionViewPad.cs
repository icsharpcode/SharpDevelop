// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		public override Control Control {
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
				FileService.JumpToFilePosition(fileName, caret.Line, caret.Column);
				
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
			ITextEditorControlProvider provider = window.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null) return null;
			TextEditorControl ctl = provider.TextEditorControl;
			
			// e might be null when this is a manually triggered update
			string fileName = (e == null) ? ctl.FileName : e.FileName;
			if (ctl.FileName != fileName) return null;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) return null;
			Caret caret = ctl.ActiveTextAreaControl.Caret;
			string content = (e == null) ? ctl.Text : e.Content;
			if (caret.Offset > content.Length) {
				LoggingService.Debug("caret.Offset = " + caret.Offset + ", content.Length=" + content.Length);
				return null;
			}
			try {
				ExpressionResult expr = expressionFinder.FindFullExpression(content, caret.Offset);
				if (expr.Expression == null) return null;
				return ParserService.Resolve(expr, caret.Line + 1, caret.Column + 1, fileName, content);
			} catch (Exception ex) {
				disableDefinitionView = true;
				this.Control.Visible = false;
				MessageService.ShowError(ex, "Error resolving at " + (caret.Line + 1) + "/" + (caret.Column + 1)
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
			TextEditorControl openTextEditor = null;
			ITextEditorControlProvider provider = FileService.GetOpenFile(fileName) as ITextEditorControlProvider;
			if (provider != null) {
				openTextEditor = provider.TextEditorControl;
			}
			
			// Load the text into the definition view's text editor.
			if (openTextEditor != null) {
				ctl.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
				ctl.Text = openTextEditor.Text;
				ctl.FileName = fileName;
			} else {
				ctl.LoadFile(fileName, true, true); // TODO: get AutoDetectEncoding from settings
			}
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			// Refresh the whole pad control here, renew all resource strings whatever.
		}
	}
}
