// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// Summary description for Form9.
	/// </summary>
	public class MarkersTextEditorPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.MarkersTextEditorPanel.xfrm"));
			
			((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked         = ((Properties)CustomizationObject).Get("ShowLineNumbers", true);
			((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked       = ((Properties)CustomizationObject).Get("ShowInvalidLines", true);
			((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked = ((Properties)CustomizationObject).Get("ShowBracketHighlight", true);
			((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked             = ((Properties)CustomizationObject).Get("ShowErrors", true);
			((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked             = ((Properties)CustomizationObject).Get("ShowHRuler", false);
			((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked         = ((Properties)CustomizationObject).Get("ShowEOLMarkers", false);
			((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked             = ((Properties)CustomizationObject).Get("ShowVRuler", false);
			((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked           = ((Properties)CustomizationObject).Get("ShowTabs", false);
			((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked         = ((Properties)CustomizationObject).Get("ShowSpaces", false);
			
			ControlDictionary["vRulerRowTextBox"].Text = ((Properties)CustomizationObject).Get("VRulerRow", 80).ToString();
			
			
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.None"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.FullRow"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex = (int)(LineViewerStyle)((Properties)CustomizationObject).Get("LineViewerStyle", LineViewerStyle.None);
			
			
			
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.BeforeCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.AfterCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex = (int)(BracketMatchingStyle)((Properties)CustomizationObject).Get("BracketMatchingStyle", BracketMatchingStyle.After);
		}
		
		public override bool StorePanelContents()
		{
			((Properties)CustomizationObject).Set("ShowInvalidLines",     ((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowLineNumbers",      ((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowBracketHighlight", ((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowErrors",           ((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowHRuler",           ((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowEOLMarkers",       ((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowVRuler",           ((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowTabs",             ((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("ShowSpaces",           ((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked);
			
			try {
				((Properties)CustomizationObject).Set("VRulerRow", Int32.Parse(ControlDictionary["vRulerRowTextBox"].Text));
			} catch (Exception) {
			}
			
			((Properties)CustomizationObject).Set("LineViewerStyle", (LineViewerStyle)((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex);
			((Properties)CustomizationObject).Set("BracketMatchingStyle", (BracketMatchingStyle)((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex);
			
			IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (activeViewContent is ITextEditorControlProvider) {
				TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			
			return true;
		}
	}
}
