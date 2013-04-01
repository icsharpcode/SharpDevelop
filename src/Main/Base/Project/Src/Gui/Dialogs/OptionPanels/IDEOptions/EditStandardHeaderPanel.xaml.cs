// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Internal.Templates;

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