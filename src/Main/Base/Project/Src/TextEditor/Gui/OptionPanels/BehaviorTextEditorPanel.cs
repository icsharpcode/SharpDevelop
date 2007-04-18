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
	/// Summary description for Form8.
	/// </summary>
	public class BehaviorTextEditorPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.BehaviorTextEditorPanel.xfrm"));
			
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			
			((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked = properties.AutoInsertCurlyBracket;
			((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked      = properties.HideMouseCursor;
			((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked       = properties.AllowCaretBeyondEOL;
			((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked  = properties.AutoInsertTemplates;
			((CheckBox)ControlDictionary["cutCopyWholeLine"]).Checked             = properties.CutCopyWholeLine;
			
			((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked  = properties.ConvertTabsToSpaces;
			
			ControlDictionary["tabSizeTextBox"].Text    = properties.TabIndent.ToString();
			ControlDictionary["indentSizeTextBox"].Text = properties.IndentationSize.ToString();
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}"));
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex = (int)properties.IndentStyle;
			
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.NormalMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ReverseMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex = properties.MouseWheelScrollDown ? 0 : 1;
		}
		
		public override bool StorePanelContents()
		{
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			
			properties.ConvertTabsToSpaces = ((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked;
			properties.MouseWheelScrollDown = ((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex == 0;
			
			properties.AutoInsertCurlyBracket = ((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked;
			properties.HideMouseCursor = ((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked;
			properties.AllowCaretBeyondEOL = ((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked;
			properties.AutoInsertTemplates = ((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked;
			properties.CutCopyWholeLine = ((CheckBox)ControlDictionary["cutCopyWholeLine"]).Checked;
			
			properties.IndentStyle = (IndentStyle)((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex;
			
			try {
				int tabSize = Int32.Parse(ControlDictionary["tabSizeTextBox"].Text);
				
				// FIX: don't allow to set tab size to zero as this will cause divide by zero exceptions in the text control.
				// Zero isn't a setting that makes sense, anyway.
				if (tabSize > 0) {
					properties.TabIndent = tabSize;
				}
			} catch (Exception) {
			}
			
			try {
				properties.IndentationSize = Int32.Parse(ControlDictionary["indentSizeTextBox"].Text);
			} catch (Exception) {
			}
			
			IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (activeViewContent is ITextEditorControlProvider) {
				TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			
			return true;
		}
	}
}
