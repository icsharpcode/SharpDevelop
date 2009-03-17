// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Specifies the mode for getting the next caret position.
	/// </summary>
	public enum CaretPositioningMode
	{
		/// <summary>
		/// Normal positioning (stop at every caret position)
		/// </summary>
		Normal,
		/// <summary>
		/// Stop only on word borders. This is used for word-selection using the mouse.
		/// </summary>
		WordBorder,
		/// <summary>
		/// Stop only at the beginning of words. This is used for Ctrl+Left/Ctrl+Right.
		/// </summary>
		WordStart
	}
	
	static class CaretNavigationCommandHandler
	{
		public static readonly CommandBindingCollection CommandBindings = new CommandBindingCollection();
		public static readonly InputBindingCollection InputBindings = new InputBindingCollection();
		
		static void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
		{
			CommandBindings.Add(new CommandBinding(command, handler));
			InputBindings.Add(new KeyBinding(command, key, modifiers));
		}
		
		static CaretNavigationCommandHandler()
		{
			const ModifierKeys None = ModifierKeys.None;
			const ModifierKeys Ctrl = ModifierKeys.Control;
			const ModifierKeys Shift = ModifierKeys.Shift;
			
			AddBinding(EditingCommands.MoveLeftByCharacter, None, Key.Left, OnMoveCaret(CaretMovementType.CharLeft));
			AddBinding(EditingCommands.SelectLeftByCharacter, Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.CharLeft));
			AddBinding(EditingCommands.MoveRightByCharacter, None, Key.Right, OnMoveCaret(CaretMovementType.CharRight));
			AddBinding(EditingCommands.SelectRightByCharacter, Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.CharRight));
			
			AddBinding(EditingCommands.MoveLeftByWord, Ctrl, Key.Left, OnMoveCaret(CaretMovementType.WordLeft));
			AddBinding(EditingCommands.SelectLeftByWord, Ctrl | Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.WordLeft));
			AddBinding(EditingCommands.MoveRightByWord, Ctrl, Key.Right, OnMoveCaret(CaretMovementType.WordRight));
			AddBinding(EditingCommands.SelectRightByWord, Ctrl | Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.WordRight));
			
			AddBinding(EditingCommands.MoveUpByLine, None, Key.Up, OnMoveCaret(CaretMovementType.LineUp));
			AddBinding(EditingCommands.SelectUpByLine, Shift, Key.Up, OnMoveCaretExtendSelection(CaretMovementType.LineUp));
			AddBinding(EditingCommands.MoveDownByLine, None, Key.Down, OnMoveCaret(CaretMovementType.LineDown));
			AddBinding(EditingCommands.SelectDownByLine, Shift, Key.Down, OnMoveCaretExtendSelection(CaretMovementType.LineDown));
			
			AddBinding(EditingCommands.MoveDownByPage, None, Key.PageDown, OnMoveCaret(CaretMovementType.PageDown));
			AddBinding(EditingCommands.SelectDownByPage, Shift, Key.PageDown, OnMoveCaretExtendSelection(CaretMovementType.PageDown));
			AddBinding(EditingCommands.MoveUpByPage, None, Key.PageUp, OnMoveCaret(CaretMovementType.PageUp));
			AddBinding(EditingCommands.SelectUpByPage, Shift, Key.PageUp, OnMoveCaretExtendSelection(CaretMovementType.PageUp));
			
			AddBinding(EditingCommands.MoveToLineStart, None, Key.Home, OnMoveCaret(CaretMovementType.LineStart));
			AddBinding(EditingCommands.SelectToLineStart, Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.LineStart));
			AddBinding(EditingCommands.MoveToLineEnd, None, Key.End, OnMoveCaret(CaretMovementType.LineEnd));
			AddBinding(EditingCommands.SelectToLineEnd, Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.LineEnd));
			
			AddBinding(EditingCommands.MoveToDocumentStart, Ctrl, Key.Home, OnMoveCaret(CaretMovementType.DocumentStart));
			AddBinding(EditingCommands.SelectToDocumentStart, Ctrl | Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.DocumentStart));
			AddBinding(EditingCommands.MoveToDocumentEnd, Ctrl, Key.End, OnMoveCaret(CaretMovementType.DocumentEnd));
			AddBinding(EditingCommands.SelectToDocumentEnd, Ctrl | Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.DocumentEnd));
			
			CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, OnSelectAll));
		}
		
		static void OnSelectAll(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				textArea.Caret.Offset = textArea.Document.TextLength;
				textArea.Selection = new SimpleSelection(0, textArea.Document.TextLength);
				textArea.Caret.BringCaretToView();
			}
		}
		
		static TextArea GetTextArea(object target)
		{
			return target as TextArea;
		}
		
		enum CaretMovementType
		{
			CharLeft,
			CharRight,
			WordLeft,
			WordRight,
			LineUp,
			LineDown,
			PageUp,
			PageDown,
			LineStart,
			LineEnd,
			DocumentStart,
			DocumentEnd
		}
		
		static ExecutedRoutedEventHandler OnMoveCaret(CaretMovementType direction)
		{
			return (target, args) => {
				TextArea textArea = GetTextArea(target);
				if (textArea != null && textArea.Document != null) {
					args.Handled = true;
					textArea.Selection = Selection.Empty;
					MoveCaret(textArea, direction);
					textArea.Caret.BringCaretToView();
				}
			};
		}
		
		static ExecutedRoutedEventHandler OnMoveCaretExtendSelection(CaretMovementType direction)
		{
			return (target, args) => {
				TextArea textArea = GetTextArea(target);
				if (textArea != null && textArea.Document != null) {
					args.Handled = true;
					int oldOffset = textArea.Caret.Offset;
					MoveCaret(textArea, direction);
					textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(oldOffset, textArea.Caret.Offset);
					textArea.Caret.BringCaretToView();
				}
			};
		}
		
		#region Caret movement
		static void MoveCaret(TextArea textArea, CaretMovementType direction)
		{
			DocumentLine caretLine = textArea.Document.GetLineByNumber(textArea.Caret.Position.Line);
			VisualLine visualLine = textArea.TextView.GetOrConstructVisualLine(caretLine);
			textArea.Caret.ValidateVisualColumn(visualLine);
			TextViewPosition caretPosition = textArea.Caret.Position;
			TextLine textLine = visualLine.GetTextLine(caretPosition.VisualColumn);
			switch (direction) {
				case CaretMovementType.CharLeft:
					MoveCaretLeft(textArea, caretPosition, visualLine, CaretPositioningMode.Normal);
					break;
				case CaretMovementType.CharRight:
					MoveCaretRight(textArea, caretPosition, visualLine, CaretPositioningMode.Normal);
					break;
				case CaretMovementType.WordLeft:
					MoveCaretLeft(textArea, caretPosition, visualLine, CaretPositioningMode.WordStart);
					break;
				case CaretMovementType.WordRight:
					MoveCaretRight(textArea, caretPosition, visualLine, CaretPositioningMode.WordStart);
					break;
				case CaretMovementType.LineUp:
				case CaretMovementType.LineDown:
				case CaretMovementType.PageUp:
				case CaretMovementType.PageDown:
					MoveCaretUpDown(textArea, direction, visualLine, textLine, caretPosition.VisualColumn);
					break;
				case CaretMovementType.DocumentStart:
					SetCaretPosition(textArea, 0, 0);
					break;
				case CaretMovementType.DocumentEnd:
					SetCaretPosition(textArea, -1, textArea.Document.TextLength);
					break;
				case CaretMovementType.LineStart:
					MoveCaretToStartOfLine(textArea, visualLine);
					break;
				case CaretMovementType.LineEnd:
					MoveCaretToEndOfLine(textArea, visualLine);
					break;
				default:
					throw new NotSupportedException(direction.ToString());
			}
		}
		#endregion
		
		#region Home/End
		static void MoveCaretToStartOfLine(TextArea textArea, VisualLine visualLine)
		{
			int newVC = visualLine.GetNextCaretPosition(-1, false, CaretPositioningMode.WordStart);
			// in empty lines (whitespace only), jump to the end
			if (newVC < 0)
				newVC = visualLine.VisualLength;
			// when the caret is already at the start of the text, jump to start before whitespace
			if (newVC == textArea.Caret.Position.VisualColumn)
				newVC = 0;
			int offset = visualLine.FirstDocumentLine.Offset + visualLine.GetRelativeOffset(newVC);
			SetCaretPosition(textArea, newVC, offset);
		}
		
		static void MoveCaretToEndOfLine(TextArea textArea, VisualLine visualLine)
		{
			int newVC = visualLine.VisualLength;
			int offset = visualLine.FirstDocumentLine.Offset + visualLine.GetRelativeOffset(newVC);
			SetCaretPosition(textArea, newVC, offset);
		}
		#endregion
		
		#region By-character / By-word movement
		static void MoveCaretRight(TextArea textArea, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode)
		{
			int pos = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, false, mode);
			if (pos >= 0) {
				SetCaretPosition(textArea, pos, visualLine.GetRelativeOffset(pos) + visualLine.FirstDocumentLine.Offset);
			} else {
				// move to start of next line
				SetCaretPosition(textArea, 0, visualLine.LastDocumentLine.Offset + visualLine.LastDocumentLine.TotalLength);
			}
		}

		static void MoveCaretLeft(TextArea textArea, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode)
		{
			int pos = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, true, mode);
			if (pos >= 0) {
				SetCaretPosition(textArea, pos, visualLine.GetRelativeOffset(pos) + visualLine.FirstDocumentLine.Offset);
			} else if (caretPosition.Line > 1) {
				DocumentLine prevLine = textArea.Document.GetLineByNumber(caretPosition.Line - 1);
				SetCaretPosition(textArea, -1, prevLine.Offset + prevLine.Length);
			}
		}
		#endregion

		#region Line+Page up/down
		static void MoveCaretUpDown(TextArea textArea, CaretMovementType direction, VisualLine visualLine, TextLine textLine, int caretVisualColumn)
		{
			// moving up/down happens using the desired visual X position
			double xPos = textArea.Caret.DesiredXPos;
			if (double.IsNaN(xPos))
				xPos = textLine.GetDistanceFromCharacterHit(new CharacterHit(caretVisualColumn, 0));
			// now find the TextLine+VisualLine where the caret will end up in
			VisualLine targetVisualLine = visualLine;
			TextLine targetLine;
			int textLineIndex = visualLine.TextLines.IndexOf(textLine);
			switch (direction) {
				case CaretMovementType.LineUp:
					{
						// Move up: move to the previous TextLine in the same visual line
						// or move to the last TextLine of the previous visual line
						int prevLineNumber = visualLine.FirstDocumentLine.LineNumber - 1;
						if (textLineIndex > 0) {
							targetLine = visualLine.TextLines[textLineIndex - 1];
						} else if (prevLineNumber >= 1) {
							DocumentLine prevLine = textArea.Document.GetLineByNumber(prevLineNumber);
							targetVisualLine = textArea.TextView.GetOrConstructVisualLine(prevLine);
							targetLine = targetVisualLine.TextLines[targetVisualLine.TextLines.Count - 1];
						} else {
							targetLine = null;
						}
						break;
					}
				case CaretMovementType.LineDown:
					{
						// Move down: move to the next TextLine in the same visual line
						// or move to the first TextLine of the next visual line
						int nextLineNumber = visualLine.LastDocumentLine.LineNumber + 1;
						if (textLineIndex < visualLine.TextLines.Count - 1) {
							targetLine = visualLine.TextLines[textLineIndex + 1];
						} else if (nextLineNumber <= textArea.Document.LineCount) {
							DocumentLine nextLine = textArea.Document.GetLineByNumber(nextLineNumber);
							targetVisualLine = textArea.TextView.GetOrConstructVisualLine(nextLine);
							targetLine = targetVisualLine.TextLines[0];
						} else {
							targetLine = null;
						}
						break;
					}
				case CaretMovementType.PageUp:
				case CaretMovementType.PageDown:
					{
						// Page up/down: find the target line using its visual position
						double yPos = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineMiddle);
						if (direction == CaretMovementType.PageUp)
							yPos -= textArea.TextView.RenderSize.Height;
						else
							yPos += textArea.TextView.RenderSize.Height;
						DocumentLine newLine = textArea.TextView.GetDocumentLineByVisualTop(yPos);
						targetVisualLine = textArea.TextView.GetOrConstructVisualLine(newLine);
						targetLine = targetVisualLine.GetTextLineByVisualYPosition(yPos);
						break;
					}
				default:
					throw new NotSupportedException(direction.ToString());
			}
			if (targetLine != null) {
				CharacterHit ch = targetLine.GetCharacterHitFromDistance(xPos);
				SetCaretPosition(textArea, targetVisualLine, targetLine, ch, false);
				textArea.Caret.DesiredXPos = xPos;
			}
		}
		#endregion
		
		#region SetCaretPosition
		static void SetCaretPosition(TextArea textArea, VisualLine targetVisualLine, TextLine targetLine,
		                             CharacterHit ch, bool allowWrapToNextLine)
		{
			int newVisualColumn = ch.FirstCharacterIndex + ch.TrailingLength;
			int targetLineStartCol = targetVisualLine.GetTextLineVisualStartColumn(targetLine);
			if (!allowWrapToNextLine && newVisualColumn >= targetLineStartCol + targetLine.Length)
				newVisualColumn = targetLineStartCol + targetLine.Length - 1;
			int newOffset = targetVisualLine.GetRelativeOffset(newVisualColumn) + targetVisualLine.FirstDocumentLine.Offset;
			SetCaretPosition(textArea, newVisualColumn, newOffset);
		}
		
		static void SetCaretPosition(TextArea textArea, int newVisualColumn, int newOffset)
		{
			textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(newOffset), newVisualColumn);
			textArea.Caret.DesiredXPos = double.NaN;
		}
		#endregion
	}
}
