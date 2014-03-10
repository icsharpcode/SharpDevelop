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
			this.IsChecked = TaskListPad.Instance.DisplayedTokens[token];
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.CheckBoxStyleKey);
		}
		
		bool ICheckableMenuCommand.IsChecked(object parameter)
		{
			return TaskListPad.Instance.DisplayedTokens[token];
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
		
		public void Execute(object parameter)
		{
			var pad = TaskListPad.Instance;
			pad.DisplayedTokens[token] = IsChecked == true;
			if (pad.IsInitialized)
				pad.UpdateItems();
		}
	}
}
