
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using Microsoft.Win32;

namespace ReflectorAddIn.Windows
{
	public partial class SetReflectorPath : Window
	{
		public SetReflectorPath(string oldPath, string reason)
		{
			InitializeComponent();
			
			this.Title = ResourceService.GetString("ReflectorAddIn.SetReflectorPathDialogTitle");
			
			if (reason != null)
				this.txtReason.Text = reason;
			else
				this.txtReason.Visibility = Visibility.Collapsed;
			if (!String.IsNullOrEmpty(oldPath)) {
				this.slePath.Text = oldPath;
			}
		}
		
		public string SelectedFile {
			get {
				return slePath.Text;
			}
		}
		
		void OpenReflectorPageButton_Click(object sender, RoutedEventArgs e)
		{
			try {
				using(System.Diagnostics.Process.Start("http://reflector.red-gate.com/Download.aspx")) {
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
		}
		
		void OkButton_Click(object sender, RoutedEventArgs e)
		{
		 	this.DialogResult = true;
		 	Close();
		}
		
		void CancelButton_Click(object sender, RoutedEventArgs e)
		{
		 	this.DialogResult = false;
		 	Close();		
		}
		
		void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
				
			dialog.Title = this.Title;
			dialog.DefaultExt = "exe";
			dialog.RestoreDirectory = true;
			dialog.Filter = "Reflector.exe|Reflector.exe";
			
			if (!String.IsNullOrEmpty(this.slePath.Text)) {
				dialog.FileName = this.slePath.Text;
			}
			
			bool? result = dialog.ShowDialog(this);
			
			if (result.HasValue && result.Value) {
				this.slePath.Text = dialog.FileName;
			}
		}
	}
}