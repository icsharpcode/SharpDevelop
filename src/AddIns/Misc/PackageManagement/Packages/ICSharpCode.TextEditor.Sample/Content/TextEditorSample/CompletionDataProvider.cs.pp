
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace $rootnamespace$.TextEditorSample
{
	public class CompletionDataProvider : ICompletionDataProvider
	{
		ImageList imageList = new ImageList();
		
		List<DefaultCompletionData> completionData = new List<DefaultCompletionData>();
		
		public CompletionDataProvider()
		{
			completionData.Add(new DefaultCompletionData("Item1", 0));
			completionData.Add(new DefaultCompletionData("Item2", 0));
			completionData.Add(new DefaultCompletionData("Item3", 0));
			completionData.Add(new DefaultCompletionData("Another item", 0));
		}
		
		public ImageList ImageList {
			get { return imageList; }
		}
		
		public string PreSelection {
			get { return null; }
		}
		
		public int DefaultIndex {
			get { return 0; }
		}
		
		public CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (char.IsLetterOrDigit(key)) {
				return CompletionDataProviderKeyResult.NormalKey;
			}
			return CompletionDataProviderKeyResult.InsertionKey;
		}
		
		public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			return data.InsertAction(textArea, key);
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			return completionData.ToArray();
		}
	}
}
