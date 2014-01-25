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

using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Interaction logic for StringListEditorXaml.xaml
	/// </summary>
	public partial class StringListEditorXaml : UserControl
	{
		bool browseForDirectory;
		public event EventHandler ListChanged;
		
		public StringListEditorXaml()
		{
			InitializeComponent();
			moveUpButton.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.ArrowUp") };
			moveDownButton.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.ArrowDown")};
			deleteButton.Content = new Image { Height = 16, Source = PresentationResourceService.GetBitmapSource("Icons.16x16.DeleteIcon")};
			DataContext = this;
		}
		
		public string TitleText {get;set;}
		
		public string ListCaption {get;set;}
		
		
		public bool BrowseForDirectory {
			get {
				return browseForDirectory;
			}
			set {
				browseForDirectory = value;
				if (browseForDirectory) {
					browseButton.Visibility = Visibility.Visible;
				} else {
					browseButton.Visibility = Visibility.Hidden;
				}
			}
		}
		
		
		public bool AutoAddAfterBrowse {get;set;}
		
		
		void AddButton_Click(object sender, RoutedEventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (AnyTextInTextBox()) {
				int index = listBox.Items.IndexOf(editTextBox.Text);
				if (index < 0) {
					index = listBox.Items.Add(editTextBox.Text);
					OnListChanged(EventArgs.Empty);
				}
				listBox.SelectedIndex = index;
			}
		}
		
		bool AnyTextInTextBox()
		{
			return editTextBox.Text.Length > 0;
		}
		
		#region Load/Save List
		
		public void LoadList(IEnumerable<string> list)
		{
			listBox.Items.Clear();
			foreach (string str in list) {
				listBox.Items.Add(str);
			}
			CheckEnableState();
		}
		
		
		public string[] GetList()
		{
			string[] list = new string[listBox.Items.Count];
			for (int i = 0; i < list.Length; i++) {
				list[i] = listBox.Items[i].ToString();
			}
			return list;
		}
		
		
		#endregion
		
		
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			editTextBox.Text = editTextBox.Text.Trim();
			if (AnyTextInTextBox()) {
				listBox.Items[listBox.SelectedIndex] = editTextBox.Text;
				OnListChanged(EventArgs.Empty);
			}
		}
		
		
		private void BrowseButtonClick (object sender, EventArgs e)
		{
			string path = SD.FileService.BrowseForFolder("${res:Dialog.ProjectOptions.SelectFolderTitle}");
			if (!String.IsNullOrEmpty(path)) {
				if (!path.EndsWith("\\") && !path.EndsWith("/"))
					path += "\\";
				editTextBox.Text = path;
				
				if (AutoAddAfterBrowse) {
					AddButton_Click(null, null);
				}
			}
		}
		
		
		private void EditTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			addButton.IsEnabled = AnyTextInTextBox();
			CheckEnableState();
		}
		
		
		protected virtual void OnListChanged(EventArgs e)
		{
			if (ListChanged != null) {
				ListChanged(this, e);
			}
			CheckEnableState();
		}

		
		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (listBox.SelectedIndex >= 0) {
				editTextBox.Text = listBox.Items[listBox.SelectedIndex].ToString();
			}
			CheckEnableState();
		}

		
		private void CheckEnableState()
		{
			moveUpButton.IsEnabled = (listBox.SelectedIndex > 0) && MultipleItemsInList();
			moveDownButton.IsEnabled = IsItemSelected() && MultipleItemsInList() && (listBox.SelectedIndex < listBox.Items.Count - 1);
			removeButton.IsEnabled = IsItemSelected();
			
			deleteButton.IsEnabled = IsItemSelected();
			updateButton.IsEnabled = IsItemSelected() && AnyTextInTextBox();
		}
		
		bool MultipleItemsInList()
		{
			return listBox.Items.Count > 1;
		}
		
		bool IsItemSelected()
		{
			return listBox.SelectedIndex > -1;
		}
		
		#region MoveUp-MoveDow-DeleteButton
		
		private void MoveUpButtonClick(object sender, RoutedEventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index - 1];
			listBox.Items[index - 1] = tmp;
			listBox.SelectedIndex = index - 1;
			OnListChanged(EventArgs.Empty);
		}
		
		private void MoveDownButtonClick(object sender, RoutedEventArgs e)
		{
			int index = listBox.SelectedIndex;
			object tmp = listBox.Items[index];
			listBox.Items[index] = listBox.Items[index + 1];
			listBox.Items[index + 1] = tmp;
			listBox.SelectedIndex = index + 1;
			OnListChanged(EventArgs.Empty);
		}
		
		
		private void RemoveButtonClick(object sender, RoutedEventArgs e)
		{
			if (listBox.SelectedIndex >= 0) {
				listBox.Items.RemoveAt(listBox.SelectedIndex);
				OnListChanged(EventArgs.Empty);
			}
		}
		
		#endregion
		
		
	}
}
