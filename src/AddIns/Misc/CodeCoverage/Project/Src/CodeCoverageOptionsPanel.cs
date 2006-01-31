// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageOptionsPanel : AbstractOptionPanel
	{
		static readonly string foregroundCustomColourButtonName = "foregroundCustomColourButton";
		static readonly string backgroundCustomColourButtonName = "backgroundCustomColourButton";
		static readonly string foregroundColourComboBoxName = "foregroundColorPickerComboBox";
		static readonly string backgroundColourComboBoxName = "backgroundColorPickerComboBox";
		static readonly string displayItemsListBoxName = "displayItemsListBox";
		static readonly string sampleTextLabelName = "sampleTextLabel";
		static readonly string browseButtonName = "browseButton";
		static readonly string commandTextBoxName = "commandTextBox";

		ColorPickerComboBox foregroundColorPickerComboBox;
		ColorPickerComboBox backgroundColorPickerComboBox;
		ListBox displayItemsListBox;
		Label sampleTextLabel;
		TextBox commandTextBox;

		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageOptionsPanel.xfrm"));
						
			ControlDictionary[foregroundCustomColourButtonName].Click += new EventHandler(ForegroundCustomColourButtonClick);
			ControlDictionary[backgroundCustomColourButtonName].Click += new EventHandler(BackgroundCustomColourButtonClick);
			ControlDictionary[browseButtonName].Click += new EventHandler(BrowseButtonClick);
			
			foregroundColorPickerComboBox = (ColorPickerComboBox)ControlDictionary[foregroundColourComboBoxName];
			foregroundColorPickerComboBox.SelectedIndexChanged += ForegroundColorPickerComboBoxSelectedIndexChanged;
			
			backgroundColorPickerComboBox = (ColorPickerComboBox)ControlDictionary[backgroundColourComboBoxName];
			backgroundColorPickerComboBox.SelectedIndexChanged += BackgroundColorPickerComboBoxSelectedIndexChanged;

			sampleTextLabel = (Label)ControlDictionary[sampleTextLabelName];
			
			commandTextBox = (TextBox)ControlDictionary[commandTextBoxName];
			commandTextBox.Text = CodeCoverageOptions.NCoverFileName;
			
			displayItemsListBox = (ListBox)ControlDictionary[displayItemsListBoxName];
			displayItemsListBox.Items.Add(new CodeCoverageDisplayItem("Code Covered", CodeCoverageOptions.VisitedColorProperty, CodeCoverageOptions.VisitedColor, CodeCoverageOptions.VisitedForeColorProperty, CodeCoverageOptions.VisitedForeColor));
			displayItemsListBox.Items.Add(new CodeCoverageDisplayItem("Code Not Covered", CodeCoverageOptions.NotVisitedColorProperty, CodeCoverageOptions.NotVisitedColor, CodeCoverageOptions.NotVisitedForeColorProperty, CodeCoverageOptions.NotVisitedForeColor));
			displayItemsListBox.SelectedIndexChanged += DisplayItemsListBoxSelectedIndexChanged;
			displayItemsListBox.SelectedIndex = 0;
		}

		public override bool StorePanelContents()
		{
			bool codeCoverageColorsChanged = false;
		
			foreach (CodeCoverageDisplayItem item in displayItemsListBox.Items) {
				CodeCoverageOptions.Properties.Set<Color>(item.ForeColorPropertyName, item.ForeColor);
				CodeCoverageOptions.Properties.Set<Color>(item.BackColorPropertyName, item.BackColor);
				if (item.HasChanged) {
					codeCoverageColorsChanged = true;
				}
			}

			if (codeCoverageColorsChanged) {
				CodeCoverageService.RefreshCodeCoverageHighlights();
			}
			
			CodeCoverageOptions.NCoverFileName = commandTextBox.Text;

			return true;
		}
		
		void ForegroundCustomColourButtonClick(object sender, EventArgs e)
		{
			SelectCustomColour(foregroundColorPickerComboBox);
		}
		
		void BackgroundCustomColourButtonClick(object sender, EventArgs e)
		{
			SelectCustomColour(backgroundColorPickerComboBox);
		}
		
		void SelectCustomColour(ColorPickerComboBox comboBox)
		{
			using (ColorDialog colorDialog = new ColorDialog()) {
				colorDialog.FullOpen = true;
				if (colorDialog.ShowDialog() == DialogResult.OK) {
					comboBox.SelectedColor = colorDialog.Color;	
				}
			}
		}
		
		void DisplayItemsListBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			CodeCoverageDisplayItem item = (CodeCoverageDisplayItem)displayItemsListBox.SelectedItem;
			sampleTextLabel.BackColor = item.BackColor;
			sampleTextLabel.ForeColor = item.ForeColor;
			foregroundColorPickerComboBox.SelectedColor = item.ForeColor;
			backgroundColorPickerComboBox.SelectedColor = item.BackColor;
		}
		
		void ForegroundColorPickerComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			CodeCoverageDisplayItem item = (CodeCoverageDisplayItem)displayItemsListBox.SelectedItem;
			sampleTextLabel.ForeColor = foregroundColorPickerComboBox.SelectedColor;
			item.ForeColor = foregroundColorPickerComboBox.SelectedColor;
		}
		
		void BackgroundColorPickerComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			CodeCoverageDisplayItem item = (CodeCoverageDisplayItem)displayItemsListBox.SelectedItem;
			sampleTextLabel.BackColor = backgroundColorPickerComboBox.SelectedColor;
			item.BackColor = backgroundColorPickerComboBox.SelectedColor;
		}
		
		void BrowseButtonClick(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog  = new OpenFileDialog()) {
				openFileDialog.CheckFileExists = true;
				openFileDialog.Filter = StringParser.Parse("${res:SharpDevelop.FileFilter.ExecutableFiles}|*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
				if (openFileDialog.ShowDialog() == DialogResult.OK) {
					ControlDictionary[commandTextBoxName].Text = openFileDialog.FileName;
				}
			}
		}
	}
}
