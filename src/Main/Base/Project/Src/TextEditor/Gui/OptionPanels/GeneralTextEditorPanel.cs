// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Drawing.Text;
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
		ComboBox fontListComboBox, fontSizeComboBox;
		FontSelectionPanelHelper helper;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GeneralTextEditorPanel.xfrm"));
			
			fontListComboBox = ((ComboBox)ControlDictionary["fontListComboBox"]);
			fontSizeComboBox = ((ComboBox)ControlDictionary["fontSizeComboBox"]);
			
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			
			((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked = properties.EnableFolding;
			((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked = properties.ShowQuickClassBrowserPanel;
			
			if (IsClearTypeEnabled) {
				// Somehow, SingleBitPerPixelGridFit still renders as Cleartype if cleartype is enabled
				// and we're using the TextRenderer for rendering.
				// So we cannot support not using antialiasing if system-wide font smoothening is enabled.
				((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked = true;
				((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Enabled = false;
			} else {
				((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked =
					(properties.TextRenderingHint == TextRenderingHint.AntiAliasGridFit || properties.TextRenderingHint == TextRenderingHint.ClearTypeGridFit);
			}
			
			((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked = properties.MouseWheelTextZoom;
			
			foreach (String name in CharacterEncodings.Names) {
				((ComboBox)ControlDictionary["textEncodingComboBox"]).Items.Add(name);
			}
			int encodingIndex = 0;
			try {
				encodingIndex = CharacterEncodings.GetEncodingIndex(properties.EncodingCodePage);
			} catch {
				encodingIndex = CharacterEncodings.GetEncodingIndex(Encoding.UTF8.CodePage);
			}
			((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex = encodingIndex;
			
			for (int i = 6; i <= 24; ++i) {
				fontSizeComboBox.Items.Add(i);
			}
			
			fontSizeComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
			fontSizeComboBox.Enabled = false;
			
			fontListComboBox.Enabled = false;
			fontListComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
			fontListComboBox.SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
			
			Font currentFont = FontSelectionPanel.ParseFont(properties.FontContainer.DefaultFont.ToString());
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
		
		bool IsClearTypeEnabled {
			get {
				return SystemInformation.IsFontSmoothingEnabled && SystemInformation.FontSmoothingType >= 2;
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			helper.UpdateFontPreviewLabel(ControlDictionary["fontPreviewLabel"]);
		}
		
		public override bool StorePanelContents()
		{
			SharpDevelopTextEditorProperties properties = SharpDevelopTextEditorProperties.Instance;
			
			if (((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Enabled) {
				properties.TextRenderingHint = ((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked
					? TextRenderingHint.ClearTypeGridFit : TextRenderingHint.SystemDefault;
			} else {
				properties.TextRenderingHint = TextRenderingHint.SystemDefault;
			}
			properties.MouseWheelTextZoom = ((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked;
			//((Properties)CustomizationObject).Set("EnableCodeCompletion", ((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked);
			properties.EnableFolding = ((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked;
			Font currentFont = CurrentFont;
			if (currentFont != null) {
				properties.Font = currentFont;
			}
			properties.EncodingCodePage = CharacterEncodings.GetCodePageByIndex(((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex);
			properties.ShowQuickClassBrowserPanel = ((CheckBox)ControlDictionary["showQuickClassBrowserCheckBox"]).Checked;
			
			IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (activeViewContent is ITextEditorControlProvider) {
				TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
				textarea.OptionsChanged();
			}
			return true;
		}
	}
}
