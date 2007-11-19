// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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

		ColorPickerComboBox foregroundColorPickerComboBox;
		ColorPickerComboBox backgroundColorPickerComboBox;
		ListBox displayItemsListBox;
		Label sampleTextLabel;

		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageOptionsPanel.xfrm"));
						
			ControlDictionary[foregroundCustomColourButtonName].Click += ForegroundCustomColourButtonClick;
			ControlDictionary[backgroundCustomColourButtonName].Click += BackgroundCustomColourButtonClick;
			
			foregroundColorPickerComboBox = (ColorPickerComboBox)ControlDictionary[foregroundColourComboBoxName];
			foregroundColorPickerComboBox.SelectedIndexChanged += ForegroundColorPickerComboBoxSelectedIndexChanged;
			
			backgroundColorPickerComboBox = (ColorPickerComboBox)ControlDictionary[backgroundColourComboBoxName];
			backgroundColorPickerComboBox.SelectedIndexChanged += BackgroundColorPickerComboBoxSelectedIndexChanged;

			sampleTextLabel = (Label)ControlDictionary[sampleTextLabelName];
						
			displayItemsListBox = (ListBox)ControlDictionary[displayItemsListBoxName];
			displayItemsListBox.Items.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeCovered}"), CodeCoverageOptions.VisitedColorProperty, CodeCoverageOptions.VisitedColor, CodeCoverageOptions.VisitedForeColorProperty, CodeCoverageOptions.VisitedForeColor));
			displayItemsListBox.Items.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeNotCovered}"), CodeCoverageOptions.NotVisitedColorProperty, CodeCoverageOptions.NotVisitedColor, CodeCoverageOptions.NotVisitedForeColorProperty, CodeCoverageOptions.NotVisitedForeColor));
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
			using (SharpDevelopColorDialog colorDialog = new SharpDevelopColorDialog()) {
				colorDialog.FullOpen = true;
				colorDialog.Color = comboBox.SelectedColor;
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
	}
}
