// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

using ICSharpCode.Core.Presentation;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// We re-use the CommandBinding and InputBinding instances between multiple text areas,
	/// so this class is static.
	/// </summary>
	static class EditingCommandHandler
	{
		static BindingGroup bindingGroup;
		
		/// <summary>
		/// Creates a new <see cref="TextAreaInputHandler"/> for the text area.
		/// </summary>
		public static TextAreaInputHandler Create(TextArea textArea)
		{
			TextAreaInputHandler handler = new TextAreaInputHandler(textArea);
			handler.BindingGroup = bindingGroup;
			
			// TODO: REMOVE
			
			// var groups = new BindingGroupCollection { bindingGroup };
			
			// var commandBindings = SDCommandManager.FindCommandBindings(null, null, null, groups);
			// handler.CommandBindings.AddRange(commandBindings.Cast<CommandBinding>()); // Todo: fix later
			
			// var inputBindings = SDCommandManager.FindInputBindings(null, null, null, groups);
			// handler.InputBindings.AddRange(inputBindings.Cast<InputBinding>()); // Todo: fix later
			
			
			
			return handler;
		}
		
		static readonly List<CommandBinding> CommandBindings = new List<CommandBinding>();
		static readonly List<InputBinding> InputBindings = new List<InputBinding>();
		
		static void AddBinding(string routedCommandName, string gesturesString, CanExecuteRoutedEventHandler canExecuteHandler, ExecutedRoutedEventHandler executedHandler)
		{
			AddCommandBinding(routedCommandName, canExecuteHandler, executedHandler);
			AddInputBinding(routedCommandName, gesturesString);
		}
		
		static void AddInputBinding(string routedCommandName, string gesturesString)
		{
			var inputBinding = new InputBindingInfo();
			inputBinding.OwnerTypeName = typeof(TextArea).GetShortAssemblyQualifiedName();
			inputBinding.DefaultGestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFrom(gesturesString);
			inputBinding.Groups.Add(bindingGroup);
			inputBinding.Categories.AddRange(SDCommandManager.GetInputBindingCategoryCollection("/MainMenu/Edit", true));
			inputBinding.RoutedCommandName = routedCommandName;
			SDCommandManager.RegisterInputBinding(inputBinding);
		}
		
		static void AddCommandBinding(string routedCommandName, CanExecuteRoutedEventHandler canExecuteHandler, ExecutedRoutedEventHandler executedHandler)
		{
			var commandBinding = new CommandBindingInfo();
			commandBinding.OwnerTypeName = typeof(TextArea).GetShortAssemblyQualifiedName();
			commandBinding.ExecutedEventHandler = executedHandler;
			commandBinding.CanExecuteEventHandler = canExecuteHandler;
			commandBinding.IsLazy = false;
			commandBinding.Groups.Add(bindingGroup);
			commandBinding.RoutedCommandName = routedCommandName;
			SDCommandManager.RegisterCommandBinding(commandBinding);
		}
		
		static EditingCommandHandler()
		{
			bindingGroup = new BindingGroup();
			
			AddCommandBinding("ApplicationCommands.Delete", CanDelete, OnDelete(ApplicationCommands.NotACommand));
			AddBinding("EditingCommands.Delete", "Delete", null, OnDelete(EditingCommands.SelectRightByCharacter));
			AddBinding("EditingCommands.DeleteNextWord", "Ctrl+Delete", null, OnDelete(EditingCommands.SelectRightByWord));
			AddBinding("EditingCommands.Backspace", "Back", null, OnDelete(EditingCommands.SelectLeftByCharacter));
			AddInputBinding("EditingCommands.Backspace", "Shift+Back"); // make Shift-Backspace do the same as plain backspace
			AddBinding("EditingCommands.DeletePreviousWord", "Ctrl+Back", null, OnDelete(EditingCommands.SelectLeftByWord));
			AddBinding("EditingCommands.EnterParagraphBreak", "Return", null, OnEnter);
			AddBinding("EditingCommands.EnterLineBreak", "Shift+Return", null, OnEnter);
			AddBinding("EditingCommands.TabForward", "Tab", null, OnTab);
			AddBinding("EditingCommands.TabBackward", "Shift+Tab", null, OnShiftTab);
			AddBinding("ApplicationCommands.Copy", "Ctrl+C", CanCutOrCopy, OnCopy);
			AddBinding("ApplicationCommands.Cut", "Ctrl+X", CanCutOrCopy, OnCut);
			AddBinding("ApplicationCommands.Paste", "Ctrl+V", CanPaste, OnPaste);
			
			AddBinding("AvalonEditCommands.DeleteLine", "Ctrl+D", null, OnDeleteLine);
			
			AddBinding("AvalonEditCommands.RemoveLeadingWhitespace", "", null, OnRemoveLeadingWhitespace);
			AddBinding("AvalonEditCommands.RemoveTrailingWhitespace", "", null, OnRemoveTrailingWhitespace);
			AddBinding("AvalonEditCommands.ConvertToUppercase", "", null, OnConvertToUpperCase);
			AddBinding("AvalonEditCommands.ConvertToLowercase", "", null, OnConvertToLowerCase);
			AddBinding("AvalonEditCommands.ConvertToTitleCase", "", null, OnConvertToTitleCase);
			AddBinding("AvalonEditCommands.InvertCase", "", null, OnInvertCase);
			AddBinding("AvalonEditCommands.ConvertTabsToSpaces", "", null, OnConvertTabsToSpaces);
			AddBinding("AvalonEditCommands.ConvertSpacesToTabs", "", null, OnConvertSpacesToTabs);
			AddBinding("AvalonEditCommands.ConvertLeadingTabsToSpaces", "", null, OnConvertLeadingTabsToSpaces);
			AddBinding("AvalonEditCommands.ConvertLeadingSpacesToTabs", "", null, OnConvertLeadingSpacesToTabs);
			AddBinding("AvalonEditCommands.IndentSelection", "", null, OnIndentSelection);
		}
		
		static TextArea GetTextArea(object target)
		{
			return target as TextArea;
		}
		
		#region Text Transformation Helpers
		enum DefaultSegmentType
		{
			None,
			WholeDocument,
			CurrentLine
		}
		
		/// <summary>
		/// Calls transformLine on all lines in the selected range.
		/// transformLine needs to handle read-only segments!
		/// </summary>
		static void TransformSelectedLines(Action<TextArea, DocumentLine> transformLine, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				using (textArea.Document.RunUpdate()) {
					DocumentLine start, end;
					if (textArea.Selection.IsEmpty) {
						if (defaultSegmentType == DefaultSegmentType.CurrentLine) {
							start = end = textArea.Document.GetLineByNumber(textArea.Caret.Line);
						} else if (defaultSegmentType == DefaultSegmentType.WholeDocument) {
							start = textArea.Document.Lines.First();
							end = textArea.Document.Lines.Last();
						} else {
							start = end = null;
						}
					} else {
						start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset);
						end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset);
					}
					if (start != null) {
						transformLine(textArea, start);
						while (start != end) {
							start = start.NextLine;
							transformLine(textArea, start);
						}
					}
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		/// <summary>
		/// Calls transformLine on all writable segment in the selected range.
		/// </summary>
		static void TransformSelectedSegments(Action<TextArea, ISegment> transformSegment, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				using (textArea.Document.RunUpdate()) {
					IEnumerable<ISegment> segments;
					if (textArea.Selection.IsEmpty) {
						if (defaultSegmentType == DefaultSegmentType.CurrentLine) {
							segments = new ISegment[] { textArea.Document.GetLineByNumber(textArea.Caret.Line) };
						} else if (defaultSegmentType == DefaultSegmentType.WholeDocument) {
							segments = textArea.Document.Lines.Cast<ISegment>();
						} else {
							segments = null;
						}
					} else {
						segments = textArea.Selection.Segments;
					}
					if (segments != null) {
						foreach (ISegment segment in segments.Reverse()) {
							foreach (ISegment writableSegment in textArea.ReadOnlySectionProvider.GetDeletableSegments(segment).Reverse()) {
								transformSegment(textArea, writableSegment);
							}
						}
					}
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		#endregion
		
		#region EnterLineBreak
		static void OnEnter(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null) {
				TextComposition textComposition = new TextComposition(InputManager.Current, textArea, "\n");
				TextCompositionEventArgs e = new TextCompositionEventArgs(Keyboard.PrimaryDevice, textComposition);
				e.RoutedEvent = TextArea.TextInputEvent;
				textArea.PerformTextInput(e);
				args.Handled = true;
			}
		}
		#endregion
		
		#region Tab
		static void OnTab(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				using (textArea.Document.RunUpdate()) {
					if (textArea.Selection.IsMultiline(textArea.Document)) {
						DocumentLine start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset);
						DocumentLine end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset);
						while (true) {
							int offset = start.Offset;
							if (textArea.ReadOnlySectionProvider.CanInsert(offset))
								textArea.Document.Insert(offset, textArea.Options.IndentationString);
							if (start == end)
								break;
							start = start.NextLine;
						}
					} else {
						string indentationString = textArea.Options.GetIndentationString(textArea.Caret.Column);
						textArea.ReplaceSelectionWithText(indentationString);
					}
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedLines(
				delegate (TextArea textArea, DocumentLine line) {
					int offset = line.Offset;
					ISegment s = TextUtilities.GetSingleIndentationSegment(line.Document, offset, textArea.Options.IndentationSize);
					if (s.Length > 0) {
						s = textArea.ReadOnlySectionProvider.GetDeletableSegments(s).FirstOrDefault();
						if (s != null && s.Length > 0) {
							textArea.Document.Remove(s.Offset, s.Length);
						}
					}
				}, target, args, DefaultSegmentType.CurrentLine);
		}
		#endregion
		
		#region Delete
		static ExecutedRoutedEventHandler OnDelete(RoutedUICommand selectingCommand)
		{
			return (target, args) => {
				TextArea textArea = GetTextArea(target);
				if (textArea != null && textArea.Document != null) {
					// call BeginUpdate before running the 'selectingCommand'
					// so that undoing the delete does not select the deleted character
					using (textArea.Document.RunUpdate()) {
						if (textArea.Selection.IsEmpty)
							selectingCommand.Execute(args.Parameter, textArea);
						textArea.RemoveSelectedText();
					}
					textArea.Caret.BringCaretToView();
					args.Handled = true;
				}
			};
		}
		
		static void CanDelete(object target, CanExecuteRoutedEventArgs args)
		{
			// HasSomethingSelected for delete command
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				args.CanExecute = !textArea.Selection.IsEmpty;
				args.Handled = true;
			}
		}
		#endregion
		
		#region Clipboard commands
		static void CanCutOrCopy(object target, CanExecuteRoutedEventArgs args)
		{
			// HasSomethingSelected for copy and cut commands
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				args.CanExecute = textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty;
				args.Handled = true;
			}
		}
		
		static void OnCopy(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine) {
					DocumentLine currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
					CopyWholeLine(textArea, currentLine);
				} else {
					CopySelectedText(textArea);
				}
				args.Handled = true;
			}
		}
		
		static void OnCut(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine) {
					DocumentLine currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
					CopyWholeLine(textArea, currentLine);
					textArea.Document.Remove(currentLine.Offset, currentLine.TotalLength);
				} else {
					CopySelectedText(textArea);
					textArea.RemoveSelectedText();
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		
		static void CopySelectedText(TextArea textArea)
		{
			string text = textArea.Selection.GetText(textArea.Document);
			// Ensure we use the appropriate newline sequence for the OS
			DataObject data = new DataObject(NewLineFinder.NormalizeNewLines(text, Environment.NewLine));
			// Also copy text in HTML format to clipboard - good for pasting text into Word
			// or to the SharpDevelop forums.
			HtmlClipboard.SetHtml(data, HtmlClipboard.CreateHtmlFragmentForSelection(textArea, new HtmlOptions(textArea.Options)));
			Clipboard.SetDataObject(data, true);
		}
		
		const string LineSelectedType = "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy
		
		static void CopyWholeLine(TextArea textArea, DocumentLine line)
		{
			ISegment wholeLine = new SimpleSegment(line.Offset, line.TotalLength);
			string text = textArea.Document.GetText(wholeLine);
			// Ensure we use the appropriate newline sequence for the OS
			DataObject data = new DataObject(NewLineFinder.NormalizeNewLines(text, Environment.NewLine));
			
			// Also copy text in HTML format to clipboard - good for pasting text into Word
			// or to the SharpDevelop forums.
			DocumentHighlighter highlighter = textArea.GetService(typeof(DocumentHighlighter)) as DocumentHighlighter;
			HtmlClipboard.SetHtml(data, HtmlClipboard.CreateHtmlFragment(textArea.Document, highlighter, wholeLine, new HtmlOptions(textArea.Options)));
			
			MemoryStream lineSelected = new MemoryStream(1);
			lineSelected.WriteByte(1);
			data.SetData(LineSelectedType, lineSelected, false);
			
			Clipboard.SetDataObject(data, true);
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
				Debug.WriteLine( Clipboard.GetText(TextDataFormat.Html) );
				
				// convert text back to correct newlines for this document
				string newLine = NewLineFinder.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
				string text = NewLineFinder.NormalizeNewLines(Clipboard.GetText(), newLine);
				
				if (!string.IsNullOrEmpty(text)) {
					bool fullLine = textArea.Options.CutCopyWholeLine && Clipboard.ContainsData(LineSelectedType);
					if (fullLine) {
						DocumentLine currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
						textArea.Document.Insert(currentLine.Offset, text);
					} else {
						textArea.ReplaceSelectionWithText(text);
					}
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
		#endregion
		
		#region DeleteLine
		static void OnDeleteLine(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				DocumentLine currentLine = textArea.Document.GetLineByNumber(textArea.Caret.Line);
				textArea.Document.Remove(currentLine.Offset, currentLine.TotalLength);
				args.Handled = true;
			}
		}
		#endregion
		
		#region Remove..Whitespace / Convert Tabs-Spaces
		static void OnRemoveLeadingWhitespace(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedLines(
				delegate (TextArea textArea, DocumentLine line) {
					line.Document.Remove(TextUtilities.GetLeadingWhitespace(line));
				}, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void OnRemoveTrailingWhitespace(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedLines(
				delegate (TextArea textArea, DocumentLine line) {
					line.Document.Remove(TextUtilities.GetTrailingWhitespace(line));
				}, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void OnConvertTabsToSpaces(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedSegments(ConvertTabsToSpaces, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void OnConvertLeadingTabsToSpaces(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedLines(
				delegate (TextArea textArea, DocumentLine line) {
					ConvertTabsToSpaces(textArea, TextUtilities.GetLeadingWhitespace(line));
				}, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void ConvertTabsToSpaces(TextArea textArea, ISegment segment)
		{
			TextDocument document = textArea.Document;
			int endOffset = segment.EndOffset;
			string indentationString = new string(' ', textArea.Options.IndentationSize);
			for (int offset = segment.Offset; offset < endOffset; offset++) {
				if (document.GetCharAt(offset) == '\t') {
					document.Replace(offset, 1, indentationString, OffsetChangeMappingType.CharacterReplace);
					endOffset += indentationString.Length - 1;
				}
			}
		}
		
		static void OnConvertSpacesToTabs(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedSegments(ConvertSpacesToTabs, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void OnConvertLeadingSpacesToTabs(object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedLines(
				delegate (TextArea textArea, DocumentLine line) {
					ConvertSpacesToTabs(textArea, TextUtilities.GetLeadingWhitespace(line));
				}, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void ConvertSpacesToTabs(TextArea textArea, ISegment segment)
		{
			TextDocument document = textArea.Document;
			int endOffset = segment.EndOffset;
			int indentationSize = textArea.Options.IndentationSize;
			int spacesCount = 0;
			for (int offset = segment.Offset; offset < endOffset; offset++) {
				if (document.GetCharAt(offset) == ' ') {
					spacesCount++;
					if (spacesCount == indentationSize) {
						document.Replace(offset, indentationSize, "\t", OffsetChangeMappingType.CharacterReplace);
						spacesCount = 0;
						offset -= indentationSize - 1;
						endOffset -= indentationSize - 1;
					}
				} else {
					spacesCount = 0;
				}
			}
		}
		#endregion
		
		#region Convert...Case
		static void ConvertCase(Func<string, string> transformText, object target, ExecutedRoutedEventArgs args)
		{
			TransformSelectedSegments(
				delegate (TextArea textArea, ISegment segment) {
					string oldText = textArea.Document.GetText(segment);
					string newText = transformText(oldText);
					textArea.Document.Replace(segment.Offset, segment.Length, newText, OffsetChangeMappingType.CharacterReplace);
				}, target, args, DefaultSegmentType.WholeDocument);
		}
		
		static void OnConvertToUpperCase(object target, ExecutedRoutedEventArgs args)
		{
			ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToUpper, target, args);
		}
		
		static void OnConvertToLowerCase(object target, ExecutedRoutedEventArgs args)
		{
			ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToLower, target, args);
		}
		
		static void OnConvertToTitleCase(object target, ExecutedRoutedEventArgs args)
		{
			ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToTitleCase, target, args);
		}
		
		static void OnInvertCase(object target, ExecutedRoutedEventArgs args)
		{
			ConvertCase(InvertCase, target, args);
		}
		
		static string InvertCase(string text)
		{
			CultureInfo culture = CultureInfo.CurrentCulture;
			char[] buffer = text.ToCharArray();
			for (int i = 0; i < buffer.Length; ++i) {
				char c = buffer[i];
				buffer[i] = char.IsUpper(c) ? char.ToLower(c, culture) : char.ToUpper(c, culture);
			}
			return new string(buffer);
		}
		#endregion
		
		static void OnIndentSelection(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null) {
				using (textArea.Document.RunUpdate()) {
					int start, end;
					if (textArea.Selection.IsEmpty) {
						start = 1;
						end = textArea.Document.LineCount;
					} else {
						start = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset).LineNumber;
						end = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset).LineNumber;
					}
					textArea.IndentationStrategy.IndentLines(start, end);
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		}
	}
}
