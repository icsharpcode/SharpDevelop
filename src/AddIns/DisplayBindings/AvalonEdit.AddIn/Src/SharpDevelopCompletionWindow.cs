// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
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
			foreach (ICompletionItem item in itemList.Items) {
				this.CompletionList.CompletionData.Add(new CodeCompletionDataAdapter(item));
			}
		}
		
		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			if (!e.Handled) {
				foreach (char c in e.Text) {
					switch (itemList.ProcessInput(c)) {
						case CompletionItemListKeyResult.BeforeStartKey:
							if (StartOffset == EndOffset) {
								StartOffset++;
								EndOffset++;
							}
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
			get { return item.Description; }
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
