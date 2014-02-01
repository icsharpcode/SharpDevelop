// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
