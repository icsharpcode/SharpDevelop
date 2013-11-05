// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for TaskListXaml.xaml
	/// </summary>
	public partial class TaskListXaml : OptionPanel
	{
		public TaskListXaml()
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
