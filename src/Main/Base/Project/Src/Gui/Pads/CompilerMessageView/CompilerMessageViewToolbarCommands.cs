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
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

//using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ShowOutputFromComboBox : ComboBox
	{
		public ShowOutputFromComboBox()
		{
			SetItems();
			CompilerMessageView.Instance.MessageCategoryAdded         += CompilerMessageViewMessageCategoryAdded;
			CompilerMessageView.Instance.SelectedCategoryIndexChanged += CompilerMessageViewSelectedCategoryIndexChanged;
			this.SelectedIndex = 0;
		}

		void CompilerMessageViewSelectedCategoryIndexChanged(object sender, EventArgs e)
		{
			if (this.SelectedIndex != CompilerMessageView.Instance.SelectedCategoryIndex) {
				this.SelectedIndex = CompilerMessageView.Instance.SelectedCategoryIndex;
			}
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			if (this.SelectedIndex != CompilerMessageView.Instance.SelectedCategoryIndex) {
				CompilerMessageView.Instance.SelectedCategoryIndex = this.SelectedIndex;
			}
		}
		
		void CompilerMessageViewMessageCategoryAdded(object sender, EventArgs e)
		{
			SetItems();
		}
		
		void SetItems()
		{
			this.Items.Clear();
			foreach (MessageViewCategory category in CompilerMessageView.Instance.MessageCategories) {
				this.Items.Add(StringParser.Parse(category.DisplayCategory));
			}
			this.SelectedIndex = 0;
		}
	}
	
	public class ClearOutputWindow : AbstractCommand
	{
		public override void Run()
		{
			MessageViewCategory selectedMessageViewCategory = CompilerMessageView.Instance.SelectedMessageViewCategory;
			if (selectedMessageViewCategory != null) {
				selectedMessageViewCategory.ClearText();
			}
		}
	}
	
	public class ToggleMessageViewWordWrap : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return CompilerMessageView.Instance.WordWrap;
			}
			set {
				CompilerMessageView.Instance.WordWrap = value;
			}
		}
	}
}
