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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// The code completion window.
	/// </summary>
	public partial class SharpDevelopCompletionWindow : CompletionWindow, ICompletionListWindow
	{
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
				VirtualizingPanel.SetIsVirtualizing(this.CompletionList.ListBox, !double.IsNaN(value));
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
			
			if (!itemList.ContainsAllAvailableItems) {
				// If more items are available (Ctrl+Space wasn't pressed), show this hint
				this.EmptyText = StringParser.Parse("${res:ICSharpCode.AvalonEdit.AddIn.SharpDevelopCompletionWindow.EmptyText}");
			}
			
			InitializeComponent();
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
			this.EndOffset += itemList.PostselectionLength;
		}
		
		public static readonly DependencyProperty EmptyTextProperty =
			DependencyProperty.Register("EmptyText", typeof(string), typeof(SharpDevelopCompletionWindow),
			                            new FrameworkPropertyMetadata());
		/// <summary>
		/// The text thats is displayed when the <see cref="ItemList" /> is empty.
		/// </summary>
		public string EmptyText {
			get { return (string)GetValue(EmptyTextProperty); }
			set { SetValue(EmptyTextProperty, value); }
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
						case CompletionItemListKeyResult.Cancel:
							Close();
							return;
					}
				}
			}
		}
	}
	
	sealed class CodeCompletionDataAdapter : ICompletionData, INotifyPropertyChanged
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
		
		// This is required to work around http://support.microsoft.com/kb/938416/en-us
		event System.ComponentModel.PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { }
			remove { }
		}
	}
}
