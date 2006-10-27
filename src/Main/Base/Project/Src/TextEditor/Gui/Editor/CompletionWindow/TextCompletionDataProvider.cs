// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Data provider for code completion.
	/// </summary>
	public class TextCompletionDataProvider : AbstractCompletionDataProvider
	{
		string[] texts;
		
		public TextCompletionDataProvider(params string[] texts)
		{
			this.texts = texts;
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			ICompletionData[] data = new ICompletionData[texts.Length];
			for (int i = 0; i < data.Length; i++) {
				data[i] = new DefaultCompletionData(texts[i], null, ClassBrowserIconService.GotoArrowIndex);
			}
			return data;
		}
	}
}
