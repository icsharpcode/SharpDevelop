// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
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
			((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked        = SmartTagAutoShow;
			((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked             = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked            = UseSmartTags;
			((CheckBox)ControlDictionary["insertTodoCommentCheckBox"]).Checked       = InsertTodoComment;
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
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", ((CheckBox)ControlDictionary["sortAlphabeticalCheckBox"]).Checked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", ((CheckBox)ControlDictionary["optimizedCodeGenerationCheckBox"]).Checked);
			SmartTagAutoShow = ((CheckBox)ControlDictionary["smartTagAutoShowCheckBox"]).Checked;
			PropertyService.Set("FormsDesigner.DesignerOptions.EnableInSituEditing", ((CheckBox)ControlDictionary["inPlaceEditCheckBox"]).Checked);
			UseSmartTags = ((CheckBox)ControlDictionary["useSmartTagsCheckBox"]).Checked;
			InsertTodoComment = ((CheckBox)ControlDictionary["insertTodoCommentCheckBox"]).Checked;
			
			return true;
		}
	}
}
