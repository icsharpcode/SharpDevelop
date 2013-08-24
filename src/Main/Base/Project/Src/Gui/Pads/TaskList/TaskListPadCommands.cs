// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class SelectScopeComboBox : ComboBox
	{
		// TODO Translate!
		static readonly string[] viewTypes = new string[] {"Solution", "Project", "All open files", "File", "Namespace", "Class/Module"};
		
		public SelectScopeComboBox()
		{
			this.ItemsSource = viewTypes;
			this.SelectedIndex = 0;
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			if (this.SelectedIndex != TaskListPad.Instance.SelectedScopeIndex) {
				TaskListPad.Instance.SelectedScopeIndex = this.SelectedIndex;
			}
		}
	}
	
	sealed class TaskListTokensToolbarCheckBox : CheckBox, ICheckableMenuCommand
	{
		event EventHandler ICheckableMenuCommand.IsCheckedChanged { add {} remove {} }
		event EventHandler System.Windows.Input.ICommand.CanExecuteChanged { add {} remove {} }
		readonly string token;
		
		public TaskListTokensToolbarCheckBox(string token)
		{
			this.token = token;
			this.Content = token;
			this.Command = this;
			this.CommandParameter = TaskListPad.Instance;
			this.IsChecked = TaskListPad.Instance.DisplayedTokens[token];
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.CheckBoxStyleKey);
		}
		
		bool ICheckableMenuCommand.IsChecked(object parameter)
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
			pad.DisplayedTokens[token] = IsChecked == true;
			if (pad.IsInitialized)
				pad.UpdateItems();
		}
	}
}
