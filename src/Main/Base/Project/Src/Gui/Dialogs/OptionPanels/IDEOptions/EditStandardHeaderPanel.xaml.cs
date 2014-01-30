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
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class EditStandardHeaderPanel : OptionPanel
	{
		public EditStandardHeaderPanel()
		{
			InitializeComponent();
		}
		
		public override void LoadOptions()
		{
			headerTextBox.FontFamily = new FontFamily("Consolas, Courier New");
		
			foreach (StandardHeader header in StandardHeader.StandardHeaders) {
				headerChooser.Items.Add(header);
			}
			headerChooser.SelectionChanged += HeaderChooser_SelectionChanged;
			headerChooser.SelectedIndex = 0;
			headerTextBox.TextChanged += HeaderTextBox_TextChanged;
		}
		
		void HeaderChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			headerTextBox.TextChanged -= HeaderTextBox_TextChanged;
			int idx = headerChooser.SelectedIndex;
			if (idx >= 0) {
				headerTextBox.Text = SelectedHeader.Header;
				headerTextBox.IsEnabled = true;
			} else {
				headerTextBox.Text = "";
				headerTextBox.IsEnabled = false;
			}
			headerTextBox.TextChanged += HeaderTextBox_TextChanged;
		}
		
		StandardHeader SelectedHeader {
			get { return (StandardHeader)headerChooser.SelectedItem; }
		}
		
		void HeaderTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			SelectedHeader.Header = headerTextBox.Text;
		}
		
		public override bool SaveOptions()
		{
			StandardHeader.StoreHeaders();
			return true;
		}
	}
}
