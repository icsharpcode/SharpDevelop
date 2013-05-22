// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

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
	
	sealed class TaskListTokensToolbarCheckBox : ToolStripButton
	{
		readonly string token;
		
		public TaskListTokensToolbarCheckBox(string token)
			: base(token)
		{
			this.token = token;
			this.CheckOnClick = true;
			this.Checked = true;
		}
		
		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			TaskListPad.Instance.DisplayedTokens[token] = this.Checked;
			if (TaskListPad.Instance.IsInitialized)
				TaskListPad.Instance.UpdateItems();
		}
	}
}
