// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.PythonBinding
{
	public class PythonInsightWindowHandler : IInsightWindowHandler
	{
		ITextEditor editor;
		IInsightWindow insightWindow;
		
		public void InitializeOpenedInsightWindow(ITextEditor editor, IInsightWindow insightWindow)
		{
			this.editor = editor;
			this.insightWindow = insightWindow;
			int offset = insightWindow.StartOffset;
			insightWindow.DocumentChanged += DocumentChanged;
		}

		void DocumentChanged(object sender, TextChangeEventArgs e)
		{
			if (IsOutsideMethodCall()) {
				insightWindow.Close();
			}
		}
		
		bool IsOutsideMethodCall()
		{
			string text = GetTextInsideMethodCallUpToCursor();
			return TextContainsClosingBracketForMethod(text);
		}
		
		string GetTextInsideMethodCallUpToCursor()
		{
			int insightStartOffset = insightWindow.StartOffset;
			int currentOffset = editor.Caret.Offset;
			int length = currentOffset - insightStartOffset;
			if (length < 0) {
				// Force completion window to close by returning the close bracket.
				return ")";
			}
			return editor.Document.GetText(insightStartOffset, length);	
		}
		
		bool TextContainsClosingBracketForMethod(string text)
		{
			int bracketCount = 1;
			foreach (char ch in text) {
				switch (ch) {
					case '(':
						bracketCount++;
						break;
					case ')':
						bracketCount--;
						if (bracketCount == 0) {
							return true;
						}
						break;
				}
			}
			return false;
		}
		
		public bool InsightRefreshOnComma(ITextEditor editor, char ch, out IInsightWindow insightWindow)
		{
			insightWindow = null;
			return false;
		}
		
		public void HighlightParameter(IInsightWindow window, int index)
		{
		}
	}
}
