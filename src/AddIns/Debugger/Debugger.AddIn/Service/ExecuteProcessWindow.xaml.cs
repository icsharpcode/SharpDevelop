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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Services
{
	/// <summary>
	/// Interaction logic for ExecuteProcessWindow.xaml
	/// </summary>
	public partial class ExecuteProcessWindow : Window
	{
		public ExecuteProcessWindow()
		{
			InitializeComponent();
		}
		
		public string SelectedExecutable {
			get {
				return pathTextBox.Text;
			}
			set {
				pathTextBox.Text = value;
				workingDirectoryTextBox.Text = Path.GetDirectoryName(value);
			}
		}
		
		public string WorkingDirectory {
			get {
				return workingDirectoryTextBox.Text;
			}
			set {
				workingDirectoryTextBox.Text = value;
			}
		}
		
		public string Arguments {
			get {
				return argumentsTextBox.Text;
			}
		}
		
		void pathButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog() {
				Filter = ".NET Executable (*.exe) | *.exe",
				InitialDirectory = workingDirectoryTextBox.Text,
				RestoreDirectory = true,
				DefaultExt = "exe"
			};
			
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				SelectedExecutable = dialog.FileName;
			}
		}
		
		void ExecuteButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(SelectedExecutable))
				return;
			this.DialogResult = true;
		}
		
		void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		
		void workingDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog() {
				SelectedPath = workingDirectoryTextBox.Text
			};
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				workingDirectoryTextBox.Text = dialog.SelectedPath;
			}
		}
	}
}