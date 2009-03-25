// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Integrates AvalonEdit with SharpDevelop.
	/// </summary>
	public class CodeEditor : TextEditor
	{
		readonly CodeEditorAdapter textEditorAdapter;
		internal string FileName;
		
		public CodeEditor()
		{
			textEditorAdapter = new CodeEditorAdapter(this);
			this.Background = Brushes.White;
			this.FontFamily = new FontFamily("Consolas");
			this.FontSize = 13;
			this.TextArea.TextEntered += TextArea_TextInput;
			this.MouseHover += CodeEditor_MouseHover;
			this.MouseHoverStopped += CodeEditor_MouseHoverStopped;
		}
		
		ToolTip toolTip;

		void CodeEditor_MouseHover(object sender, MouseEventArgs e)
		{
			ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(textEditorAdapter);
			var pos = GetPositionFromPoint(e.GetPosition(this));
			args.InDocument = pos.HasValue;
			if (pos.HasValue) {
				args.LogicalPosition = AvalonEditDocumentAdapter.ToLocation(pos.Value);
			}
			ToolTipRequestService.RequestToolTip(args);
			if (args.ContentToShow != null) {
				if (toolTip == null) {
					toolTip = new ToolTip();
					toolTip.Closed += toolTip_Closed;
				}
				toolTip.Content = args.ContentToShow;
				toolTip.IsOpen = true;
				e.Handled = true;
			}
		}
		
		void CodeEditor_MouseHoverStopped(object sender, MouseEventArgs e)
		{
			if (toolTip != null) {
				toolTip.IsOpen = false;
				e.Handled = true;
			}
		}

		void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			toolTip = null;
		}
		
		volatile static ReadOnlyCollection<ICodeCompletionBinding> codeCompletionBindings;
		
		public static ReadOnlyCollection<ICodeCompletionBinding> CodeCompletionBindings {
			get {
				if (codeCompletionBindings == null) {
					codeCompletionBindings = AddInTree.BuildItems<ICodeCompletionBinding>("/AddIns/DefaultTextEditor/CodeCompletion", null, false).AsReadOnly();
				}
				return codeCompletionBindings;
			}
		}
		
		void TextArea_TextInput(object sender, TextCompositionEventArgs e)
		{
			foreach (char c in e.Text) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CompletionWindow oldCompletionWindow = lastCompletionWindow;
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(textEditorAdapter, c);
					if (result == CodeCompletionKeyPressResult.Completed) {
						if (lastCompletionWindow != null && lastCompletionWindow != oldCompletionWindow) {
							// a new CompletionWindow was shown, but does not eat the input
							// tell it to expect the text insertion
							lastCompletionWindow.ExpectInsertionBeforeStart = true;
						}
						return;
					} else if (result == CodeCompletionKeyPressResult.EatKey) {
						e.Handled = true;
						return;
					}
				}
			}
		}
		
		CompletionWindow lastCompletionWindow;
		
		internal void NotifyCompletionWindowOpened(CompletionWindow window)
		{
			lastCompletionWindow = window;
			window.Closed += delegate { lastCompletionWindow = null; };
		}
	}
}
