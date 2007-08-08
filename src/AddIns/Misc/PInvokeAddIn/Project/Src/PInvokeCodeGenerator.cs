// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PInvokeAddIn
{
	/// <summary>
	/// Generates PInvoke signature code in the SharpDevelop text editor.
	/// </summary>
	public class PInvokeCodeGenerator
	{
		public PInvokeCodeGenerator()
		{
		}
		
		/// <summary>
		/// Inserts the PInvoke signature at the current cursor position.
		/// </summary>
		/// <param name="textArea">The text editor.</param>
		/// <param name="signature">A PInvoke signature string.</param>
		public void Generate(TextArea textArea, string signature)
		{
			IndentStyle oldIndentStyle = textArea.TextEditorProperties.IndentStyle;
			bool oldEnableEndConstructs = PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true);

			try {

				textArea.BeginUpdate();
				textArea.Document.UndoStack.StartUndoGroup();
				textArea.TextEditorProperties.IndentStyle = IndentStyle.Smart;
				PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", false);

				string[] lines = signature.Replace("\r\n", "\n").Split('\n');
				
				for (int i = 0; i < lines.Length; ++i) {
					
					textArea.InsertString(lines[i]);
					
					// Insert new line if not the last line.
					if ( i < (lines.Length - 1))
					{
						Return(textArea);
					}
				}
				
			} finally {
				textArea.Document.UndoStack.EndUndoGroup();
				textArea.TextEditorProperties.IndentStyle = oldIndentStyle;
				PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", oldEnableEndConstructs);
				textArea.EndUpdate();
				textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				textArea.Document.CommitUpdate();	
			}
		}
		
		void Return(TextArea textArea)
		{
			IndentLine(textArea);
			new Return().Execute(textArea);
		}
		
		void IndentLine(TextArea textArea)
		{
			int delta = textArea.Document.FormattingStrategy.IndentLine(textArea, textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset));
			if (delta != 0) {
				LineSegment caretLine = textArea.Document.GetLineSegmentForOffset(textArea.Caret.Offset);
				textArea.Caret.Position = textArea.Document.OffsetToPosition(Math.Min(textArea.Caret.Offset + delta, caretLine.Offset + caretLine.Length));
			}
		}		
	}
}
