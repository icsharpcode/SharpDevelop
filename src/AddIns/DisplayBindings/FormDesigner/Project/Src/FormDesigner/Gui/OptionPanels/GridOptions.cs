// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;


namespace ICSharpCode.FormDesigner.Gui.OptionPanels
{
	public class GridOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.WindowsFormsGridOptions.xfrm"));
			
			ControlDictionary["widthTextBox"].Text                      = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeWidth", 8).ToString();
			ControlDictionary["heightTextBox"].Text                     = PropertyService.Get("FormsDesigner.DesignerOptions.GridSizeHeight", 8).ToString();
			((CheckBox)ControlDictionary["showGridCheckBox"]).Checked   = PropertyService.Get("FormsDesigner.DesignerOptions.ShowGrid", true);
			((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked = PropertyService.Get("FormsDesigner.DesignerOptions.SnapToGrid", true);
			((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
		}
		
		public override bool StorePanelContents()
		{
			int width = 0;
			try {
				width = Int32.Parse(ControlDictionary["widthTextBox"].Text);
			} catch (Exception e) {
				Console.WriteLine(e);
				MessageService.ShowError("Forms Designer grid with is invalid");
				return false;
			}
			
			int height = 0;
			try {
				height = Int32.Parse(ControlDictionary["heightTextBox"].Text);
			} catch (Exception e) {
				Console.WriteLine(e);
				MessageService.ShowError("Forms Designer height with is invalid");
				return false;
			}
			
			PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth", width);
			PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", height);
			PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", ((CheckBox)ControlDictionary["showGridCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGrid", ((CheckBox)ControlDictionary["snapToGridCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", ((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked);
			
			return true;
		}
	}
}
