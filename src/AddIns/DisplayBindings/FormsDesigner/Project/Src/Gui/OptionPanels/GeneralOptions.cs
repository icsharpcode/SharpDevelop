// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	public class GeneralOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.WindowsFormsGeneralOptions.xfrm"));
			
			((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked        = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
			((CheckBox)ControlDictionary["optimizedCodeGenerationCheckBox"]).Checked = PropertyService.Get("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
			((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked        = PropertyService.Get("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", true);
			((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked             = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked            = PropertyService.Get("FormsDesigner.DesignerOptions.UseSmartTags", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", ((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", ((CheckBox)ControlDictionary["optimizedCodeGenerationCheckBox"]).Checked); 
			PropertyService.Set("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", ((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.EnableInSituEditing", ((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseSmartTags", ((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked);
			
			return true;
		}
	}
}
