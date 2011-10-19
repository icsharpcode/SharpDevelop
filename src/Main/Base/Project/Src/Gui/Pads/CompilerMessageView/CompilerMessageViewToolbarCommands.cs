// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ShowOutputFromComboBox : AbstractComboBoxCommand
	{
		ComboBox comboBox;
		
		protected override void OnOwnerChanged(EventArgs e)
		{
			base.OnOwnerChanged(e);
			ToolBarComboBox toolbarItem = (ToolBarComboBox)base.ComboBox;
			comboBox = toolbarItem.ComboBox;
			SetItems();
			CompilerMessageView.Instance.MessageCategoryAdded         += new EventHandler(CompilerMessageViewMessageCategoryAdded);
			CompilerMessageView.Instance.SelectedCategoryIndexChanged += new EventHandler(CompilerMessageViewSelectedCategoryIndexChanged);
			comboBox.SelectedIndex = 0;
			comboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChanged);
		}
		void CompilerMessageViewSelectedCategoryIndexChanged(object sender, EventArgs e)
		{
			if (comboBox.SelectedIndex != CompilerMessageView.Instance.SelectedCategoryIndex) {
				comboBox.SelectedIndex = CompilerMessageView.Instance.SelectedCategoryIndex;
			}
		}
		void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
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
		ToolBarCheckBox checkBox;
		
		public override bool IsChecked {
			get {
				return CompilerMessageView.Instance.WordWrap;
			}
			set {
				CompilerMessageView.Instance.WordWrap = value;
			}
		}
		
		public override object Owner {
			set {
				base.Owner = value;
				checkBox = (ToolBarCheckBox)Owner;
			}
		}
	}
}
