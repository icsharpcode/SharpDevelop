// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop
{
	public interface ICompletionItemList
	{
		IEnumerable<ICompletionItem> Items { get; }
		
		ICompletionItem SuggestedItem { get; }
		
		/// <summary>
		/// Processes the specified key press.
		/// </summary>
		CompletionItemListKeyResult ProcessInput(char key);
	}
	
	public enum CompletionItemListKeyResult
	{
		/// <summary>
		/// Normal key, used to choose an entry from the completion list
		/// </summary>
		NormalKey,
		/// <summary>
		/// This key triggers insertion of the completed expression
		/// </summary>
		InsertionKey,
		/// <summary>
		/// Increment both start and end offset of completion region when inserting this
		/// key. Can be used to insert whitespace (or other characters) in front of the expression
		/// while the completion window is open.
		/// </summary>
		BeforeStartKey
	}
	
	public class DefaultCompletionItemList : ICompletionItemList
	{
		List<ICompletionItem> items = new List<ICompletionItem>();
		
		public List<ICompletionItem> Items {
			get { return items; }
		}
		
		public void SortItems()
		{
			items.Sort((a,b) => string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase));
		}
		
		public ICompletionItem SuggestedItem { get; set; }
		
		IEnumerable<ICompletionItem> ICompletionItemList.Items {
			get { return items; }
		}
		
		/// <summary>
		/// Allows the insertion of a single space in front of the completed text.
		/// </summary>
		public bool InsertSpace { get; set; }
		
		public virtual CompletionItemListKeyResult ProcessInput(char key)
		{
			if (key == ' ' && this.InsertSpace) {
				this.InsertSpace = false; // insert space only once
				return CompletionItemListKeyResult.BeforeStartKey;
			} else if (char.IsLetterOrDigit(key) || key == '_') {
				this.InsertSpace = false; // don't insert space if user types normally
				return CompletionItemListKeyResult.NormalKey;
			} else {
				// do not reset insertSpace when doing an insertion!
				return CompletionItemListKeyResult.InsertionKey;
			}
		}
	}
}
