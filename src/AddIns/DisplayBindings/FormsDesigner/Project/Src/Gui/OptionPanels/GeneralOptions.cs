// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	public class GeneralOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.FormsDesigner.Resources.WindowsFormsGeneralOptions.xfrm"));
			
			((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked        = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
			((CheckBox)ControlDictionary["optimizedCodeGenerationCheckBox"]).Checked = PropertyService.Get("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
			((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked        = SmartTagAutoShow;
			((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked             = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked            = UseSmartTags;
			((CheckBox)ControlDictionary["insertTodoCommentCheckBox"]).Checked       = InsertTodoComment;
			((CheckBox)ControlDictionary["generateVSStyleHandlersCheckBox"]).Checked = GenerateVisualStudioStyleEventHandlers;
		}
		
		public static bool UseSmartTags {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.UseSmartTags", true);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.UseSmartTags", value);
			}
		}
		
		public static bool SmartTagAutoShow {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", true);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", value);
			}
		}
		
		public static bool InsertTodoComment {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.InsertTodoComment", false);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.InsertTodoComment", value);
			}
		}
		
		public static bool GenerateVisualStudioStyleEventHandlers {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.GenerateVisualStudioStyleEventHandlers", false);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.GenerateVisualStudioStyleEventHandlers", value);
			}
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", ((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", ((CheckBox)ControlDictionary["optimizedCodeGenerationCheckBox"]).Checked);
			SmartTagAutoShow = ((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked;
			PropertyService.Set("FormsDesigner.DesignerOptions.EnableInSituEditing", ((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked);
			UseSmartTags = ((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked;
			InsertTodoComment = ((CheckBox)ControlDictionary["insertTodoCommentCheckBox"]).Checked;
			GenerateVisualStudioStyleEventHandlers = ((CheckBox)ControlDictionary["generateVSStyleHandlersCheckBox"]).Checked;
			
			return true;
		}
	}
}
