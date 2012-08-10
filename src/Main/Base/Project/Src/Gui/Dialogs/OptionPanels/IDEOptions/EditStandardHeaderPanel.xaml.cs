/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.03.2012
 * Time: 20:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Controls;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for EditStandardHeaderPanelXaml.xaml
	/// </summary>
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
			int idx =headerChooser.SelectedIndex;
			if (idx >= 0) {
				headerTextBox.Text = ((StandardHeader)((ComboBox)headerChooser).SelectedItem).Header;
				headerTextBox.IsEnabled = true;
			} else {
				headerTextBox.Text = "";
				headerTextBox.IsEnabled = false;
			}
			headerTextBox.TextChanged += HeaderTextBox_TextChanged;
		}
		
		
		void HeaderTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			((StandardHeader)((ComboBox)headerChooser).SelectedItem).Header = headerTextBox.Text;
		}
	}
}