// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;

//using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ShowOutputFromComboBox : AbstractComboBoxCommand
	{
		ComboBox comboBox;
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			comboBox = (ComboBox)base.ComboBox;
			SetItems();
			CompilerMessageView.Instance.MessageCategoryAdded         += new EventHandler(CompilerMessageViewMessageCategoryAdded);
			CompilerMessageView.Instance.SelectedCategoryIndexChanged += new EventHandler(CompilerMessageViewSelectedCategoryIndexChanged);
			comboBox.SelectedIndex = 0;
			comboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBoxSelectionChanged);
		}

		void CompilerMessageViewSelectedCategoryIndexChanged(object sender, EventArgs e)
		{
			if (comboBox.SelectedIndex != CompilerMessageView.Instance.SelectedCategoryIndex) {
				comboBox.SelectedIndex = CompilerMessageView.Instance.SelectedCategoryIndex;
			}
		}
		
		void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (comboBox.SelectedIndex != CompilerMessageView.Instance.SelectedCategoryIndex) {
				CompilerMessageView.Instance.SelectedCategoryIndex = comboBox.SelectedIndex;
			}
		}
		
		void CompilerMessageViewMessageCategoryAdded(object sender, EventArgs e)
		{
			SetItems();
		}
		
		void SetItems()
		{
			comboBox.Items.Clear();
			foreach (MessageViewCategory category in CompilerMessageView.Instance.MessageCategories) {
				comboBox.Items.Add(StringParser.Parse(category.DisplayCategory));
			}
			comboBox.SelectedIndex = 0;
		}
		
		public override void Run()
		{
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
