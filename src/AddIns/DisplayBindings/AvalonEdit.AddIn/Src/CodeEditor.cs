// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core;
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
		
		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			base.OnPreviewTextInput(e);
			if (!e.Handled && e.Text.Length == 1) {
				foreach (ICodeCompletionBinding cc in CodeCompletionBindings) {
					CompletionWindow oldCompletionWindow = lastCompletionWindow;
					CodeCompletionKeyPressResult result = cc.HandleKeyPress(textEditorAdapter, e.Text[0]);
					if (result == CodeCompletionKeyPressResult.Completed) {
						if (lastCompletionWindow != null && lastCompletionWindow != oldCompletionWindow) {
							// a new CompletionWindow was shown, but does not eat the input
							// increment the offsets so that they are correct after the text insertion
							lastCompletionWindow.StartOffset++;
							lastCompletionWindow.EndOffset++;
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
