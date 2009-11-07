// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	[Obsolete]
	public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
	{
		public virtual ImageList ImageList {
			get {
				return ClassBrowserIconService.ImageList;
			}
		}
		
		int defaultIndex = -1;
		
		/// <summary>
		/// Gets the index of the element in the list that is chosen by default.
		/// </summary>
		public int DefaultIndex {
			get {
				return defaultIndex;
			}
			set {
				defaultIndex = value;
			}
		}
		
		protected string preSelection = null;
		
		public string PreSelection {
			get {
				return preSelection;
			}
		}
		
		bool insertSpace;
		
		/// <summary>
		/// Gets/Sets if a space should be inserted in front of the completed expression.
		/// </summary>
		public bool InsertSpace {
			get {
				return insertSpace;
			}
			set {
				insertSpace = value;
			}
		}
		
		/// <summary>
		/// Gets if pressing 'key' should trigger the insertion of the currently selected element.
		/// </summary>
		public virtual CompletionDataProviderKeyResult ProcessKey(char key)
		{
			CompletionDataProviderKeyResult res;
			if (key == ' ' && insertSpace) {
				insertSpace = false; // insert space only once
				res = CompletionDataProviderKeyResult.BeforeStartKey;
			} else if (char.IsLetterOrDigit(key) || key == '_') {
				insertSpace = false; // don't insert space if user types normally
				res = CompletionDataProviderKeyResult.NormalKey;
			} else {
				// do not reset insertSpace when doing an insertion!
				res = CompletionDataProviderKeyResult.InsertionKey;
			}
			return res;
		}
		
		public virtual bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			if (InsertSpace) {
				textArea.Document.Insert(insertionOffset++, " ");
			}
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			
			return data.InsertAction(textArea, key);
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// This method may return null.
		/// </summary>
		public abstract ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);
	}
}
