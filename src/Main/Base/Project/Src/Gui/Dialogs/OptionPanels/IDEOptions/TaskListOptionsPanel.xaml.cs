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
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for TaskListOptionsPanel.xaml
	/// </summary>
	public partial class TaskListOptionsPanel : OptionPanel
	{
		public TaskListOptionsPanel()
		{
			InitializeComponent();
			foreach (var token in SD.ParserService.TaskListTokens) {
				listView.Items.Add(token);
			};
		}
		
		
		void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listView.SelectedItem != null) {
				taskText.Text = listView.SelectedItem.ToString();
				changeBtn.IsEnabled = true;
				removeBtn.IsEnabled = true;
			}
		}
		
		
		void ChangeBtn_Click(object sender, RoutedEventArgs e)
		{
			if ((listView.SelectedItem != null) && (!String.IsNullOrWhiteSpace(taskText.Text))) {
				var i = listView.Items.IndexOf(listView.SelectedItem);
				listView.Items[i] =  taskText.Text;
			}
		}
		
		
		void RemoveBtn_Click(object sender, RoutedEventArgs e)
		{
			if (listView.SelectedItem != null) {
				listView.Items.Remove(listView.SelectedItem);
				taskText.Text = String.Empty;
			}
		}
		
		
		void AddButton_Click(object sender, RoutedEventArgs e)
		{
			if (!String.IsNullOrWhiteSpace(taskText.Text)) {
				var i = listView.Items.IndexOf(taskText.Text);
				if (i < 0) {
					
					listView.Items.Add(taskText.Text);
				}
			}
		}
		
		
		public override bool SaveOptions()
		{
			List<string> tokens = new List<string>();
			
			foreach (var item in listView.Items) {
				string text = item.ToString().Trim();
				if (text.Length > 0) {
					tokens.Add(text);
				}
			}
			SD.ParserService.TaskListTokens = tokens.ToArray();
			return true;
		}
	}
}
