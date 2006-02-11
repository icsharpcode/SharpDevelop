// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

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
			
			((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked = ((Properties)CustomizationObject).Get("AutoInsertCurlyBracket", true);
			((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked      = ((Properties)CustomizationObject).Get("HideMouseCursor", true);
			((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked       = ((Properties)CustomizationObject).Get("CursorBehindEOL", false);
			((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked  = ((Properties)CustomizationObject).Get("AutoInsertTemplates", true);
			((CheckBox)ControlDictionary["cutCopyWholeLine"]).Checked             = ((Properties)CustomizationObject).Get("CutCopyWholeLine", true);
			
			((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked  = ((Properties)CustomizationObject).Get("TabsToSpaces", false);
			
			ControlDictionary["tabSizeTextBox"].Text    = ((Properties)CustomizationObject).Get("TabIndent", 4).ToString();
			ControlDictionary["indentSizeTextBox"].Text = ((Properties)CustomizationObject).Get("IndentationSize", 4).ToString();
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}"));
			((ComboBox)ControlDictionary["indentStyleComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}"));
			
			((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex = (int)(IndentStyle)((Properties)CustomizationObject).Get("IndentStyle", IndentStyle.Smart);
		
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.NormalMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.ReverseMouseDirectionRadioButton}"));
			((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex = ((Properties)CustomizationObject).Get("MouseWheelScrollDown", true) ? 0 : 1;
		}
		
		public override bool StorePanelContents()
		{
			((Properties)CustomizationObject).Set("TabsToSpaces",         ((CheckBox)ControlDictionary["convertTabsToSpacesCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("MouseWheelScrollDown", ((ComboBox)ControlDictionary["mouseWhellDirectionComboBox"]).SelectedIndex == 0);
			
			((Properties)CustomizationObject).Set("AutoInsertCurlyBracket", ((CheckBox)ControlDictionary["autoinsertCurlyBraceCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("HideMouseCursor",        ((CheckBox)ControlDictionary["hideMouseCursorCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("CursorBehindEOL",        ((CheckBox)ControlDictionary["caretBehindEOLCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("AutoInsertTemplates",    ((CheckBox)ControlDictionary["auotInsertTemplatesCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("CutCopyWholeLine",       ((CheckBox)ControlDictionary["cutCopyWholeLine"]).Checked);
			
			((Properties)CustomizationObject).Set("IndentStyle", (IndentStyle)((ComboBox)ControlDictionary["indentStyleComboBox"]).SelectedIndex);
			
			try {
				int tabSize = Int32.Parse(ControlDictionary["tabSizeTextBox"].Text);
				
				// FIX: don't allow to set tab size to zero as this will cause divide by zero exceptions in the text control.
				// Zero isn't a setting that makes sense, anyway.
				if (tabSize > 0) {
					((Properties)CustomizationObject).Set("TabIndent", tabSize);
				}
			} catch (Exception) {
			}
			
			try {
				((Properties)CustomizationObject).Set("IndentationSize", Int32.Parse(ControlDictionary["indentSizeTextBox"].Text));
			} catch (Exception) {
			}
			
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
				TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			
			return true;
		}
	}
}
