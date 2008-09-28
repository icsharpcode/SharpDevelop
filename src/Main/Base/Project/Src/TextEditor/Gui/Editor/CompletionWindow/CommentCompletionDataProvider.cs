// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class CommentCompletionDataProvider : AbstractCompletionDataProvider
	{
		int caretLineNumber;
		int caretColumn;
		
		string[][] commentTags = new string[][] {
			new string[] {"c", "marks text as code"},
			new string[] {"code", "marks text as code"},
			new string[] {"example", "A description of the code example\n(must have a <code> tag inside)"},
			new string[] {"exception cref=\"\"", "description to an exception thrown"},
			new string[] {"list type=\"\"", "A list"},
			new string[] {"listheader", "The header from the list"},
			new string[] {"item", "A list item"},
			new string[] {"inheritdoc/", "Inherit documentation from base class"},
			new string[] {"term", "A term in a list"},
			new string[] {"description", "A description to a term in a list"},
			new string[] {"para", "A text paragraph"},
			new string[] {"param name=\"\"", "A description for a parameter"},
			new string[] {"paramref name=\"\"", "A reference to a parameter"},
			new string[] {"permission cref=\"\"", ""},
			new string[] {"remarks", "Gives description for a member"},
			new string[] {"include file=\"\" path=\"\"", "Includes comments from other files"},
			new string[] {"returns", "Gives description for a return value"},
			new string[] {"see cref=\"\"", "A reference to a member"},
			new string[] {"seealso cref=\"\"", "A reference to a member in the seealso section"},
			new string[] {"summary", "A summary of the object"},
			new string[] {"value", "A description of a property"}
		};
		
		/// <remarks>
		/// Returns true, if the given coordinates (row, column) are in the region.
		/// </remarks>
		bool IsBetween(int row, int column, DomRegion region)
		{
			return row >= region.BeginLine && (row <= region.EndLine || region.EndLine == -1);
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			caretLineNumber = textArea.Caret.Line;
			caretColumn     = textArea.Caret.Column;
			LineSegment caretLine = textArea.Document.GetLineSegment(caretLineNumber);
			string lineText = textArea.Document.GetText(caretLine.Offset, caretLine.Length);
			if (!lineText.Trim().StartsWith("///") && !lineText.Trim().StartsWith("'''")) {
				return null;
			}
			
			ArrayList completionData = new ArrayList();
			foreach (string[] tag in commentTags) {
				completionData.Add(new CommentCompletionData(tag[0], tag[1]));
			}
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class CommentCompletionData : ICompletionData
		{
			string text;
			string description;
			
			public int ImageIndex {
				get {
					return ClassBrowserIconService.MethodIndex;
				}
			}
			
			public string Text {
				get {
					return text;
				}
				set {
					text = value;
				}
			}
			
			public string Description {
				get {
					return description;
				}
			}
			
			public double Priority {
				get {
					return 0;
				}
			}
			
			public bool InsertAction(TextArea textArea, char ch)
			{
				textArea.InsertString(text);
				return false;
			}
			
			public CommentCompletionData(string text, string description)
			{
				this.text        = text;
				this.description = description;
			}
		}
	}
}
