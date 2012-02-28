/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.02.2012
 * Time: 19:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Gui;

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
			string[] tokens = ParserService.TaskListTokens;
			foreach (var token in tokens) {
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
			ParserService.TaskListTokens = tokens.ToArray();
			return true;
		}
	}
}