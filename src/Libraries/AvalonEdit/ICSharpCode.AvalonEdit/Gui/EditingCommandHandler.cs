// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Gui
{
	static class EditingCommandHandler
	{
		public static readonly CommandBindingCollection CommandBindings = new CommandBindingCollection();
		public static readonly InputBindingCollection InputBindings = new InputBindingCollection();
		
		static void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
		{
			CommandBindings.Add(new CommandBinding(command, handler));
			InputBindings.Add(new KeyBinding(command, key, modifiers));
		}
		
		static EditingCommandHandler()
		{
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete(ApplicationCommands.NotACommand), HasSomethingSelected));
			AddBinding(EditingCommands.Delete, ModifierKeys.None, Key.Delete, OnDelete(EditingCommands.SelectRightByCharacter));
			AddBinding(EditingCommands.DeleteNextWord, ModifierKeys.Control, Key.Delete, OnDelete(EditingCommands.SelectRightByWord));
			AddBinding(EditingCommands.Backspace, ModifierKeys.None, Key.Back, OnDelete(EditingCommands.SelectLeftByCharacter));
			InputBindings.Add(new KeyBinding(EditingCommands.Backspace, Key.Back, ModifierKeys.Shift)); // make Shift-Backspace do the same as plain backspace
			AddBinding(EditingCommands.DeletePreviousWord, ModifierKeys.Control, Key.Back, OnDelete(EditingCommands.SelectLeftByWord));
			AddBinding(EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Enter, OnEnter);
			AddBinding(EditingCommands.EnterLineBreak, ModifierKeys.Shift, Key.Enter, OnEnter);
			AddBinding(EditingCommands.TabForward, ModifierKeys.None, Key.Tab, OnTab);
			AddBinding(EditingCommands.TabBackward, ModifierKeys.Shift, Key.Tab, OnShiftTab);
			
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, HasSomethingSelected));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, HasSomethingSelected));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
		}
		
		static TextArea GetTextArea(object target)
		{
			return target as TextArea;
		}
		
		#region EnterLineBreak
		static void OnEnter(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				string newLine = NewLineFinder.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
				textArea.ReplaceSelectionWithText(newLine);
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		#endregion
		
		#region Tab
		// TODO: make these per-textarea options
		const string indentationString = "\t";
		const int tabSize = 4;
		
		static void OnTab(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				textArea.Document.BeginUpdate();
				try {
					if (textArea.Selection.GetIsMultiline(textArea.Document)) {
						DocumentLine start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset);
						DocumentLine end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.GetEndOffset());
						while (true) {
							int offset = start.Offset;
							if (textArea.ReadOnlySectionProvider.CanInsert(offset))
								textArea.Document.Insert(offset, indentationString);
							if (start == end)
								break;
							start = textArea.Document.GetLineByNumber(start.LineNumber + 1);
						}
					} else {
						textArea.ReplaceSelectionWithText(indentationString);
					}
				} finally {
					textArea.Document.EndUpdate();
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				textArea.Document.BeginUpdate();
				try {
					DocumentLine start, end;
					if (textArea.Selection.IsEmpty) {
						start = end = textArea.Document.GetLineByNumber(textArea.Caret.Line);
					} else {
						start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset);
						end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.GetEndOffset());
					}
					while (true) {
						int offset = start.Offset;
						ISegment s = GetFirstIndentationSegment(textArea.Document, offset);
						if (s.Length > 0) {
							s = textArea.ReadOnlySectionProvider.GetDeletableSegments(s).FirstOrDefault();
							if (s != null && s.Length > 0) {
								textArea.Document.Remove(s.Offset, s.Length);
							}
						}
						if (start == end)
							break;
						start = textArea.Document.GetLineByNumber(start.LineNumber + 1);
					}
				} finally {
					textArea.Document.EndUpdate();
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		static ISegment GetFirstIndentationSegment(TextDocument document, int offset)
		{
			int pos = offset;
			while (pos < document.TextLength) {
				char c = document.GetCharAt(pos);
				if (c == '\t') {
					if (pos == offset)
						return new SimpleSegment(offset, 1);
					else
						break;
				} else if (c == ' ') {
					if (pos - offset >= tabSize)
						break;
				} else {
					break;
				}
				// continue only if c==' ' and (pos-offset)<tabSize
				pos++;
			}
			return new SimpleSegment(offset, pos - offset);
		}
		#endregion
		
		#region Delete
		static ExecutedRoutedEventHandler OnDelete(RoutedUICommand selectingCommand)
		{
			return (target, args) => {
				TextArea textArea = GetTextArea(target);
				if (textArea != null && textArea.Document != null) {
					if (textArea.Selection.IsEmpty)
						selectingCommand.Execute(args.Parameter, textArea);
					textArea.RemoveSelectedText();
					textArea.Caret.BringCaretToView();
					args.Handled = true;
				}
			};
		}
		#endregion
		
		#region Clipboard commands
		static void HasSomethingSelected(object target, CanExecuteRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				args.CanExecute = !textArea.Selection.IsEmpty;
				args.Handled = true;
			}
		}
		
		static void OnCopy(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				CopySelectedText(textArea);
				args.Handled = true;
			}
		}
		
		static void OnCut(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				CopySelectedText(textArea);
				textArea.RemoveSelectedText();
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		static void CopySelectedText(TextArea textArea)
		{
			string text = textArea.Selection.GetText(textArea.Document);
			// ensure we use the appropriate newline sequence for the OS
			Clipboard.SetText(NewLineFinder.NormalizeNewLines(text, Environment.NewLine));
		}
		
		static void CanPaste(object target, CanExecuteRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				args.CanExecute = textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset)
					&& Clipboard.ContainsText();
				args.Handled = true;
			}
		}
		
		static void OnPaste(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				// convert text back to correct newlines for this document
				string newLine = NewLineFinder.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
				string text = NewLineFinder.NormalizeNewLines(Clipboard.GetText(), newLine);
				textArea.ReplaceSelectionWithText(text);
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		#endregion
	}
}
