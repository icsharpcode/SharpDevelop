// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Provides code completion for the Python Console window.
	/// </summary>
	public class PythonConsoleCompletionDataProvider : AbstractCompletionDataProvider
	{
		IMemberProvider memberProvider;
		
		public PythonConsoleCompletionDataProvider(IMemberProvider memberProvider)
		{
			this.memberProvider = memberProvider;
			DefaultIndex = 0;
		}
				
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			return GenerateCompletionData(GetLineText(textArea));
		}
		
		/// <summary>
		/// Generates completion data for the specified text. The text should be everything before
		/// the dot character that triggered the completion. The text can contain the command line prompt
		/// '>>>' as this will be ignored.
		/// </summary>
		public ICompletionData[] GenerateCompletionData(string line)
		{
			List<DefaultCompletionData> items = new List<DefaultCompletionData>();

			string name = GetName(line);
			if (!String.IsNullOrEmpty(name)) {
				try {
					foreach (string member in memberProvider.GetMemberNames(name)) {
						items.Add(new DefaultCompletionData(member, String.Empty, ClassBrowserIconService.MethodIndex));
					}
				} catch { 
					// Do nothing.
				}
			}
			return items.ToArray();
		}
		
		string GetName(string text)
		{
			int startIndex = text.LastIndexOf(' ');
			return text.Substring(startIndex + 1);
		}
		
		/// <summary>
		/// Gets the line of text up to the cursor position.
		/// </summary>
		string GetLineText(TextArea textArea)
		{
			LineSegment lineSegment = textArea.Document.GetLineSegmentForOffset(textArea.Caret.Offset);
			return textArea.Document.GetText(lineSegment);
		}
	}
}
