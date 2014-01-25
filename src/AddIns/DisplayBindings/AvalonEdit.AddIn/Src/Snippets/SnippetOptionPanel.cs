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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// Interaction logic for Snippets.xaml
	/// </summary>
	public partial class SnippetOptionPanel : UserControl, IOptionPanel
	{
		ObservableCollection<CodeSnippetGroup> groups;
		
		public SnippetOptionPanel()
		{
			InitializeComponent();
		}
		
		public object Owner { get; set; }
		
		public object Control {
			get { return this; }
		}
		
		public void LoadOptions()
		{
			groups = new ObservableCollection<CodeSnippetGroup>(SnippetManager.Instance.LoadGroups().OrderBy(g => g.Extensions));
			extensionComboBox.ItemsSource = groups;
			extensionComboBox.SelectedItem = groups.FirstOrDefault();
		}
		
		public bool SaveOptions()
		{
			CodeSnippet emptySnippet = new CodeSnippet();
			foreach (CodeSnippetGroup g in groups) {
				for (int i = 0; i < g.Snippets.Count; i++) {
					if (emptySnippet.Equals(g.Snippets[i]))
						g.Snippets.RemoveAt(i--);
				}
			}
			SnippetManager.Instance.SaveGroups(groups);
			return true;
		}
		
		void AddGroupButton_Click(object sender, RoutedEventArgs e)
		{
			string result = MessageService.ShowInputBox(
				"${res:Dialog.Options.CodeTemplate.AddGroupLabel}",
				"${res:Dialog.Options.CodeTemplate.EditGroupDialog.Text}",
				"");
			if (!string.IsNullOrEmpty(result)) {
				CodeSnippetGroup g = new CodeSnippetGroup();
				g.Extensions = result;
				groups.Add(g);
				extensionComboBox.SelectedItem = g;
			}
		}
		
		void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
		{
			if (extensionComboBox.SelectedIndex >= 0)
				groups.RemoveAt(extensionComboBox.SelectedIndex);
		}
		
		void EditGroupButton_Click(object sender, RoutedEventArgs e)
		{
			CodeSnippetGroup g = (CodeSnippetGroup)extensionComboBox.SelectedItem;
			if (g != null) {
				string result = MessageService.ShowInputBox(
					"${res:Dialog.Options.CodeTemplate.EditGroupLabel}",
					"${res:Dialog.Options.CodeTemplate.EditGroupDialog.Text}",
					g.Extensions);
				if (!string.IsNullOrEmpty(result))
					g.Extensions = result;
			}
		}
		
		void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CodeSnippet snippet = dataGrid.SelectedItem as CodeSnippet;
			if (snippet != null) {
				snippetTextBox.Text = snippet.Text;
				snippetTextBox.IsReadOnly = false;
				snippetTextBox.Background = SystemColors.WindowBrush;
			} else {
				snippetTextBox.Text = null;
				snippetTextBox.IsReadOnly = true;
				snippetTextBox.Background = SystemColors.ControlBrush;
			}
		}
		
		void SnippetTextBox_TextChanged(object sender, EventArgs e)
		{
			CodeSnippet snippet = dataGrid.SelectedItem as CodeSnippet;
			if (snippet != null)
				snippet.Text = snippetTextBox.Text;
		}
	}
}
