// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

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
					if (cc.HandleKeyPress(textEditorAdapter, e.Text[0])) {
						e.Handled = true;
						return;
					}
				}
			}
		}
	}
}
