// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SelectScopeComboBox : ToolStripComboBox
	{
		private static string[] viewTypes = new string[] {"Solution", "Project", "All open documents", "Document", "Namespace", "Class/Module"};
		
		public SelectScopeComboBox()
		{
			SetItems();
			this.SelectedIndex = 0;
		}
		
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (this.SelectedIndex != TaskListPad.Instance.SelectedScopeIndex) {
				TaskListPad.Instance.SelectedScopeIndex = this.SelectedIndex;
			}
		}
		
		void SetItems()
		{
			this.Items.Clear();
			this.Items.AddRange(viewTypes);
		}
	}
	
	sealed class TaskListTokensToolbarCheckBox : CheckBox, ICheckableMenuCommand
	{
		public event EventHandler IsCheckedChanged = delegate {};
		
		public event EventHandler CanExecuteChanged { add {} remove {} }

		readonly string token;
		
		public TaskListTokensToolbarCheckBox(string token)
		{
			this.token = token;
			this.Text = token;
		}
		
		public bool IsChecked(object parameter)
		{
			var pad = (TaskListPad)parameter;
			return pad.DisplayedTokens[token];
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
		
		public void Execute(object parameter)
		{
			var pad = (TaskListPad)parameter;
			pad.DisplayedTokens[token] = this.Checked;
			if (pad.IsInitialized)
				pad.UpdateItems();
		}
	}
}
