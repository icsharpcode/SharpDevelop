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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.SharpDevelop;
using ResourceEditor.ViewModels;

namespace ResourceEditor.Views
{
	/// <summary>
	/// Interaction logic for BinaryView.xaml
	/// </summary>
	public partial class BinaryView : UserControl, IResourceItemView
	{
		ResourceItem resourceItem;
		
		readonly ASCIIEncoding encoding = new ASCIIEncoding();
		readonly Regex regex = new Regex(@"\p{Cc}");
		
		public BinaryView()
		{
			InitializeComponent();
			this.DataContext = this;
			
			// Make the TextBox look like any other code window
			binaryDataTextBox.FontFamily = new FontFamily(SD.EditorControlService.GlobalOptions.FontFamily);
			binaryDataTextBox.FontSize = SD.EditorControlService.GlobalOptions.FontSize;
		}
		
		public static readonly DependencyProperty ViewHexDumpProperty =
			DependencyProperty.Register("ViewHexDump", typeof(bool), typeof(BinaryView),
				new FrameworkPropertyMetadata());
		
		public bool ViewHexDump {
			get { return (bool)GetValue(ViewHexDumpProperty); }
			set { SetValue(ViewHexDumpProperty, value); }
		}
		
		public static readonly DependencyProperty DisplayedByteDataProperty =
			DependencyProperty.Register("DisplayedByteData", typeof(string), typeof(BinaryView),
				new FrameworkPropertyMetadata());
		
		public string DisplayedByteData {
			get { return (string)GetValue(DisplayedByteDataProperty); }
			set { SetValue(DisplayedByteDataProperty, value); }
		}

		#region IResourceItemView implementation

		public ResourceItem ResourceItem {
			get {
				return resourceItem;
			}
			set {
				resourceItem = value;
				UpdateDisplayedData();
			}
		}

		public FrameworkElement UIControl {
			get {
				return this;
			}
		}

		#endregion
		
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			
			if (e.Property.Name == ViewHexDumpProperty.Name) {
				UpdateDisplayedData();
			}
		}
		
		void UpdateDisplayedData()
		{
			if (ResourceItem == null) {
				return;
			}
			
			byte[] bytes = (byte[])ResourceItem.ResourceValue;
			string regText = encoding.GetString(bytes).Replace("\x0", ".");
			
			if (ViewHexDump) {
				// Hex Dump
				StringBuilder sb = new StringBuilder();
				
				string byteString = BitConverter.ToString(bytes).Replace("-", " ");
				string stext = regex.Replace(regText, ".");
				
				DisplayedByteData = "";
				int max = bytes.Length;
				int last = max % 16;
				
				int i = 0;
				int count = 16;
				do {
					sb.Append(String.Format("{0:X8}  ", i) +
					byteString.Substring(i * 3, (count * 3) - 1) + "  " +
					stext.Substring(i, count) + "\r\n");
					i += 16;
					if (i >= (max - last)) {
						count = last;
					}
				} while (i < max);
				DisplayedByteData = sb.ToString();
			} else {
				// Regular Text
				DisplayedByteData = regText;
			}
		}
	}
}