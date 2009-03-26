// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public class SharpDevelopCompletionWindow : CompletionWindow
	{
		readonly ICompletionItemList itemList;
		
		public SharpDevelopCompletionWindow(TextArea textArea, ICompletionItemList itemList) : base(textArea)
		{
			if (itemList == null)
				throw new ArgumentNullException("itemList");
			this.itemList = itemList;
			ICompletionItem suggestedItem = itemList.SuggestedItem;
			foreach (ICompletionItem item in itemList.Items) {
				ICompletionData adapter = new CodeCompletionDataAdapter(item);
				this.CompletionList.CompletionData.Add(adapter);
				if (item == suggestedItem)
					this.CompletionList.SelectedItem = adapter;
			}
			this.StartOffset -= itemList.PreselectionLength;
		}
		
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			if (itemList.PreselectionLength > 0 && itemList.SuggestedItem == null) {
				string preselection = this.TextArea.Document.GetText(this.StartOffset, this.EndOffset - this.StartOffset);
				this.CompletionList.SelectItemWithStart(preselection);
			}
		}
		
		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			if (!e.Handled) {
				foreach (char c in e.Text) {
					switch (itemList.ProcessInput(c)) {
						case CompletionItemListKeyResult.BeforeStartKey:
							this.ExpectInsertionBeforeStart = true;
							break;
						case CompletionItemListKeyResult.NormalKey:
							break;
						case CompletionItemListKeyResult.InsertionKey:
							this.CompletionList.RequestInsertion(e);
							return;
					}
				}
			}
		}
	}
	
	sealed class CodeCompletionDataAdapter : ICompletionData
	{
		readonly ICompletionItem item;
		
		public CodeCompletionDataAdapter(ICompletionItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this.item = item;
		}
		
		public string Text {
			get { return item.Text; }
		}
		
		public object Content {
			get { return item.Text; }
		}
		
		public object Description {
			get {
				return item.Description;
			}
		}
		
		public ImageSource Image {
			get { return null; }
		}
		
		public void Complete(TextArea textArea, ISegment completionSegment)
		{
			textArea.Document.Replace(completionSegment, item.Text);
		}
	}
}
