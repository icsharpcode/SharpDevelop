// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Text;
using System.Windows;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Interaction logic for ChooseEncodingDialog.xaml
	/// </summary>
	public partial class ChooseEncodingDialog : Window
	{
		public ChooseEncodingDialog()
		{
			InitializeComponent();
			encodingComboBox.ItemsSource = FileService.AllEncodings;
			encodingComboBox.SelectedItem = FileService.DefaultFileEncoding;
		}
		
		public Encoding Encoding {
			get { return ((EncodingInfo)encodingComboBox.SelectedItem).GetEncoding(); }
			set { encodingComboBox.SelectedItem = FileService.AllEncodings.Single(e => e.CodePage == value.CodePage); }
		}
		
		void okButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
