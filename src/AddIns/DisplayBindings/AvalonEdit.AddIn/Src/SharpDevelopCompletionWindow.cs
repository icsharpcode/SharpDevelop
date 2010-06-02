// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public class SharpDevelopCompletionWindow : CompletionWindow, ICompletionListWindow
	{
		/*static SharpDevelopCompletionWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SharpDevelopCompletionWindow),
			                                         new FrameworkPropertyMetadata(typeof(SharpDevelopCompletionWindow)));
		}*/
		
		public ICompletionItem SelectedItem {
			get {
				return ((CodeCompletionDataAdapter)this.CompletionList.SelectedItem).Item;
			}
			set {
				var itemAdapters = this.CompletionList.CompletionData.Cast<CodeCompletionDataAdapter>();
				this.CompletionList.SelectedItem = itemAdapters.FirstOrDefault(a => a.Item == value);
			}
		}
		
		double ICompletionWindow.Width {
			get { return this.Width; }
			set {
				// Disable virtualization if we use automatic width - this prevents the window from resizing
				// when the user scrolls.
				VirtualizingStackPanel.SetIsVirtualizing(this.CompletionList.ListBox, !double.IsNaN(value));
				this.Width = value;
				if (double.IsNaN(value)) {
					// enable size-to-width:
					if (this.SizeToContent == SizeToContent.Manual)
						this.SizeToContent = SizeToContent.Width;
					else if (this.SizeToContent == SizeToContent.Height)
						this.SizeToContent = SizeToContent.WidthAndHeight;
				} else {
					// disable size-to-width:
					if (this.SizeToContent == SizeToContent.Width)
						this.SizeToContent = SizeToContent.Manual;
					else if (this.SizeToContent == SizeToContent.WidthAndHeight)
						this.SizeToContent = SizeToContent.Height;
				}
			}
		}
		
		readonly ICompletionItemList itemList;
		
		public ICompletionItemList ItemList {
			get { return itemList; }
		}
		
		public ITextEditor Editor { get; private set; }
		
		public SharpDevelopCompletionWindow(ITextEditor editor, TextArea textArea, ICompletionItemList itemList) : base(textArea)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (itemList == null)
				throw new ArgumentNullException("itemList");
			this.Style = ICSharpCode.Core.Presentation.GlobalStyles.WindowStyle;
			this.Editor = editor;
			this.itemList = itemList;
			ICompletionItem suggestedItem = itemList.SuggestedItem;
			foreach (ICompletionItem item in itemList.Items) {
				ICompletionData adapter = new CodeCompletionDataAdapter(this, item);
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
				this.CompletionList.SelectItem(preselection);
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
	
	/// <summary>
	/// Completion item that supports complex content and description.
	/// </summary>
	public interface IFancyCompletionItem : ICompletionItem
	{
		object Content { get; }
		new object Description { get; }
	}
	
	sealed class CodeCompletionDataAdapter : ICompletionData
	{
		readonly SharpDevelopCompletionWindow window;
		readonly ICompletionItem item;
		readonly IFancyCompletionItem fancyCompletionItem;
		
		public CodeCompletionDataAdapter(SharpDevelopCompletionWindow window, ICompletionItem item)
		{
			if (window == null)
				throw new ArgumentNullException("window");
			if (item == null)
				throw new ArgumentNullException("item");
			this.window = window;
			this.item = item;
			this.fancyCompletionItem = item as IFancyCompletionItem;
		}
		
		public ICompletionItem Item {
			get { return item; }
		}
		
		public string Text {
			get { return item.Text; }
		}
		
		public object Content {
			get {
				return (fancyCompletionItem != null) ? fancyCompletionItem.Content : item.Text;
			}
		}
		
		public object Description {
			get {
				return (fancyCompletionItem != null) ? fancyCompletionItem.Description : item.Description;
			}
		}
		
		public ImageSource Image {
			get {
				IImage image = item.Image;
				return image != null ? image.ImageSource : null;
			}
		}
		
		public double Priority {
			get { return item.Priority; }
		}
		
		public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			CompletionContext context = new CompletionContext {
				Editor = window.Editor,
				StartOffset = window.StartOffset,
				EndOffset = window.EndOffset
			};
			TextCompositionEventArgs txea = insertionRequestEventArgs as TextCompositionEventArgs;
			KeyEventArgs kea = insertionRequestEventArgs as KeyEventArgs;
			if (txea != null && txea.Text.Length > 0)
				context.CompletionChar = txea.Text[0];
			else if (kea != null && kea.Key == Key.Tab)
				context.CompletionChar = '\t';
			window.ItemList.Complete(context, item);
			if (context.CompletionCharHandled && txea != null)
				txea.Handled = true;
		}
	}
}
