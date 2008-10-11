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
			
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked         = properties.ShowLineNumbers;
			((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked       = properties.ShowInvalidLines;
			((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked = properties.ShowMatchingBracket;
			((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked             = properties.UnderlineErrors;
			((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked             = properties.ShowHorizontalRuler;
			((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked         = properties.ShowEOLMarker;
			((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked             = properties.ShowVerticalRuler;
			((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked           = properties.ShowTabs;
			((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked         = properties.ShowSpaces;
			((CheckBox)ControlDictionary["showCaretLineCheckBox"]).Checked          = properties.CaretLine;
			
			ControlDictionary["vRulerRowTextBox"].Text = properties.VerticalRulerRow.ToString();
			
			
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.None"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.LineViewerStyle.FullRow"));
			((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex = (int)properties.LineViewerStyle;
			
			
			
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.BeforeCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).Items.Add(ResourceService.GetString("Dialog.Options.IDEOptions.TextEditor.Markers.BracketMatchingStyle.AfterCaret"));
			((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex = (int)properties.BracketMatchingStyle;
		}
		
		public override bool StorePanelContents()
		{
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			properties.ShowInvalidLines = ((CheckBox)ControlDictionary["showInvalidLinesCheckBox"]).Checked;
			properties.ShowLineNumbers = ((CheckBox)ControlDictionary["showLineNumberCheckBox"]).Checked;
			properties.ShowMatchingBracket = ((CheckBox)ControlDictionary["showBracketHighlighterCheckBox"]).Checked;
			properties.UnderlineErrors = ((CheckBox)ControlDictionary["showErrorsCheckBox"]).Checked;
			properties.ShowHorizontalRuler = ((CheckBox)ControlDictionary["showHRulerCheckBox"]).Checked;
			properties.ShowEOLMarker = ((CheckBox)ControlDictionary["showEOLMarkersCheckBox"]).Checked;
			properties.ShowVerticalRuler = ((CheckBox)ControlDictionary["showVRulerCheckBox"]).Checked;
			properties.ShowTabs = ((CheckBox)ControlDictionary["showTabCharsCheckBox"]).Checked;
			properties.ShowSpaces = ((CheckBox)ControlDictionary["showSpaceCharsCheckBox"]).Checked;
			properties.CaretLine = ((CheckBox)ControlDictionary["showCaretLineCheckBox"]).Checked;
			
			try {
				properties.VerticalRulerRow = Int32.Parse(ControlDictionary["vRulerRowTextBox"].Text);
			} catch (Exception) {
			}
			
			properties.LineViewerStyle = (LineViewerStyle)((ComboBox)ControlDictionary["lineMarkerStyleComboBox"]).SelectedIndex;
			properties.BracketMatchingStyle = (BracketMatchingStyle)((ComboBox)ControlDictionary["bracketMatchingStyleComboBox"]).SelectedIndex;
			
			IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (activeViewContent is ITextEditorControlProvider) {
				TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			
			return true;
		}
	}
}
