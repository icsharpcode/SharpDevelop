// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// General texteditor options panel.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{
		int encoding = Encoding.UTF8.CodePage;
		
		ComboBox fontListComboBox, fontSizeComboBox;
		FontSelectionPanelHelper helper;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GeneralTextEditorPanel.xfrm"));
			
			fontListComboBox = ((ComboBox)ControlDictionary["fontListComboBox"]);
			fontSizeComboBox = ((ComboBox)ControlDictionary["fontSizeComboBox"]);
			
			((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked = ((Properties)CustomizationObject).Get("DoubleBuffer", true);
			//((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked  = ((Properties)CustomizationObject).Get("EnableCodeCompletion", true);
			((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked         = ((Properties)CustomizationObject).Get("EnableFolding", true);
			((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked = ((Properties)CustomizationObject).Get("ShowQuickClassBrowserPanel", true);
			
			((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked = ((Properties)CustomizationObject).Get("UseAntiAliasFont", false);
			((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked = ((Properties)CustomizationObject).Get("MouseWheelTextZoom", true);
			
			foreach (String name in CharacterEncodings.Names) {
				((ComboBox)ControlDictionary["textEncodingComboBox"]).Items.Add(name);
			}
			int encodingIndex = 0;
			try {
				encodingIndex = CharacterEncodings.GetEncodingIndex((Int32)((Properties)CustomizationObject).Get("Encoding", encoding));
			} catch {
				encodingIndex = CharacterEncodings.GetEncodingIndex(encoding);
			}
			((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex = encodingIndex;
			encoding = CharacterEncodings.GetEncodingByIndex(encodingIndex).CodePage;
			
			for (int i = 6; i <= 24; ++i) {
				fontSizeComboBox.Items.Add(i);
			}
			
			fontSizeComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
			fontSizeComboBox.Enabled = false;
			
			fontListComboBox.Enabled = false;
			fontListComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
			fontListComboBox.SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
			
			Font currentFont = FontSelectionPanel.ParseFont(((Properties)CustomizationObject).Get("DefaultFont", ResourceService.DefaultMonospacedFont.ToString()).ToString());
			helper = new FontSelectionPanelHelper(fontSizeComboBox, fontListComboBox, currentFont);
			
			fontListComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(helper.MeasureComboBoxItem);
			fontListComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(helper.ComboBoxDrawItem);
			
			UpdateFontPreviewLabel(null, null);
			helper.StartThread();
		}
		
		Font CurrentFont {
			get {
				return helper.GetSelectedFont();
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			helper.UpdateFontPreviewLabel(ControlDictionary["fontPreviewLabel"]);
		}
		
		public override bool StorePanelContents()
		{
			((Properties)CustomizationObject).Set("DoubleBuffer",         ((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("UseAntiAliasFont",     ((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("MouseWheelTextZoom",   ((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked);
			//((Properties)CustomizationObject).Set("EnableCodeCompletion", ((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("EnableFolding",        ((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked);
			Font currentFont = CurrentFont;
			if (currentFont != null) {
				((Properties)CustomizationObject).Set("DefaultFont", currentFont.ToString());
			}
			((Properties)CustomizationObject).Set("Encoding",             CharacterEncodings.GetCodePageByIndex(((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex));
			((Properties)CustomizationObject).Set("ShowQuickClassBrowserPanel", ((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked);
			
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window != null && (window.ViewContent is ITextEditorControlProvider)) {
				TextEditorControl textarea = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			return true;
		}
		
		static Font ParseFont(string font)
		{
			try {
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			} catch (Exception ex) {
				LoggingService.Warn(ex);
				return ResourceService.DefaultMonospacedFont;
			}
		}
	}
}
