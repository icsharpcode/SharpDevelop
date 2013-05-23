// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
