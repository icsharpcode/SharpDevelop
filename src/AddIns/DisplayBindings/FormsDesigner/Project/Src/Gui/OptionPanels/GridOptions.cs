// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	public class GridOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.WindowsFormsGridOptions.xfrm"));
			
			bool snapToGridOn = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGridMode", false);
			
			((RadioButton)ControlDictionary["snapToGridRadioButton"]).Checked        = snapToGridOn;
			((RadioButton)ControlDictionary["snapLinesRadioButton"]).Checked         = !snapToGridOn;
			ControlDictionary["widthTextBox"].Text                                   = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8).ToString();
			ControlDictionary["heightTextBox"].Text                                  = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8).ToString();
			((CheckBox)ControlDictionary["showGridCheckBox"]).Checked                = PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
			((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked              = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
			
			((RadioButton)ControlDictionary["snapToGridRadioButton"]).CheckedChanged += SnapToGridChanged;
			EnableGridOptions(snapToGridOn);
		}
		
		public override bool StorePanelContents()
		{
			int width = 0;
			try {
				width = Int32.Parse(ControlDictionary["widthTextBox"].Text);
			} catch {
				MessageService.ShowError("Forms Designer grid width is invalid");
				return false;
			}
			
			int height = 0;
			try {
				height = Int32.Parse(ControlDictionary["heightTextBox"].Text);
			} catch {
				MessageService.ShowError("Forms Designer grid height is invalid");
				return false;
			}
			
			PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGridMode", ((RadioButton)ControlDictionary["snapToGridRadioButton"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth", width);
			PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", height);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseSnapLines", ((RadioButton)ControlDictionary["snapLinesRadioButton"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", ((CheckBox)ControlDictionary["showGridCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGrid", ((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked);

			return true;
		}
		
		void EnableGridOptions(bool snapToGridOn)
		{
			ControlDictionary["widthTextBox"].Enabled                   = snapToGridOn;
			ControlDictionary["heightTextBox"].Enabled                  = snapToGridOn;
			((CheckBox)ControlDictionary["showGridCheckBox"]).Enabled   = snapToGridOn;
			((CheckBox)ControlDictionary["snapToGridCheckBox"]).Enabled = snapToGridOn;
		}
		
		void SnapToGridChanged(object source, EventArgs e)
		{
			EnableGridOptions(((RadioButton)ControlDictionary["snapToGridRadioButton"]).Checked);
		}
	}
}
