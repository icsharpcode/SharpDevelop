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
			ctl.TextEditorProperties = new SharpDevelopTextEditorProperties();
			ctl.ActiveTextAreaControl.TextArea.DoubleClick += OnDoubleClick;
			ParserService.ParserUpdateStepFinished += UpdateTick;
			ctl.VisibleChanged += delegate { UpdateTick(null, null); };
		}
		
		void OnDoubleClick(object sender, EventArgs e)
		{
			string fileName = ctl.FileName;
			if (fileName != null) {
				Caret caret = ctl.ActiveTextAreaControl.Caret;
				FileService.JumpToFilePosition(fileName, caret.Line, caret.Column);
				
				// refresh DefinitionView to show the definition of the expression that was double-clicked
				UpdateTick(null, null);
			}
		}
		
		void UpdateTick(object sender, ParserUpdateStepEventArgs e)
		{
			if (!this.IsVisible) return;
			LoggingService.Debug("DefinitionViewPad.Update");
			ResolveResult res = ResolveAtCaret(e);
			if (res == null) return;
			FilePosition pos = res.GetDefinitionPosition();
			if (pos.IsEmpty) return;
			WorkbenchSingleton.SafeThreadAsyncCall(OpenFile, pos);
		}
		
		ResolveResult ResolveAtCaret(ParserUpdateStepEventArgs e)
		{
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
			ExpressionResult expr = expressionFinder.FindFullExpression(content, caret.Offset);
			if (expr.Expression == null) return null;
			return ParserService.Resolve(expr, caret.Line + 1, caret.Column + 1, fileName, content);
		}
		
		FilePosition oldPosition;
		
		void OpenFile(FilePosition pos)
		{
			if (pos.Equals(oldPosition)) return;
			oldPosition = pos;
			if (pos.FileName != ctl.FileName)
				ctl.LoadFile(pos.FileName, true, true); // TODO: get AutoDetectEncoding from settings
			ctl.ActiveTextAreaControl.ScrollTo(int.MaxValue); // scroll completely down
			ctl.ActiveTextAreaControl.Caret.Line = pos.Line - 1;
			ctl.ActiveTextAreaControl.ScrollToCaret(); // scroll up to search position
		}
		
		/// <summary>
		/// Refreshes the pad
		/// </summary>
		public override void RedrawContent()
		{
			// Refresh the whole pad control here, renew all resource strings whatever.
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= UpdateTick;
			ctl.Dispose();
			base.Dispose();
		}
	}
}
