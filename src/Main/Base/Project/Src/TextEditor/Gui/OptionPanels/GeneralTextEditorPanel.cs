// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// General texteditor options panel.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{
		int encoding = Encoding.UTF8.CodePage;
		
		class FontDescriptor
		{
			string name;
			bool   isMonospaced;
			
			public string Name {
				get {
					return name;
				}
				set {
					name = value;
				}
			}
			
			public bool IsMonospaced {
				get {
					return isMonospaced;
				}
				set {
					isMonospaced = value;
				}
			}
			
			public FontDescriptor(string name, bool isMonospaced)
			{
				this.name = name;
				this.isMonospaced = isMonospaced;
			}
			
		}
		
		bool IsMonospaced(FontFamily fontFamily)
		{
			using (Bitmap newBitmap = new Bitmap(1, 1)) {
				using (Graphics g  = Graphics.FromImage(newBitmap)) {
					using (Font f = new Font(fontFamily, 10)) {
						// determine if the length of i == m because I see no other way of
						// getting if a font is monospaced or not.
						int w1 = (int)g.MeasureString("i.", f).Width;
						int w2 = (int)g.MeasureString("mw", f).Width;
						return w1 == w2;
					}
				}
			}
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.GeneralTextEditorPanel.xfrm"));
			
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
			
			Font currentFont = ParseFont(((Properties)CustomizationObject).Get("DefaultFont", ResourceService.CourierNew10.ToString()).ToString());
			
			for (int i = 6; i <= 24; ++i) {
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Items.Add(i);
			}
			((ComboBox)ControlDictionary["fontSizeComboBox"]).Text = currentFont.Size.ToString();
			((ComboBox)ControlDictionary["fontSizeComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			
			
			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			
			int index = 0;
			foreach (FontFamily fontFamily in installedFontCollection.Families) {
				if (fontFamily.IsStyleAvailable(FontStyle.Regular) && fontFamily.IsStyleAvailable(FontStyle.Bold)  && fontFamily.IsStyleAvailable(FontStyle.Italic)) {
					if (fontFamily.Name == currentFont.Name) {
						index = ((ComboBox)ControlDictionary["fontListComboBox"]).Items.Count;
					}
					((ComboBox)ControlDictionary["fontListComboBox"]).Items.Add(new FontDescriptor(fontFamily.Name, IsMonospaced(fontFamily)));
				}
			}
			
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex = index;
			((ComboBox)ControlDictionary["fontListComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			((ComboBox)ControlDictionary["fontListComboBox"]).DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			
			boldComboBoxFont = new Font(ControlDictionary["fontListComboBox"].Font, FontStyle.Bold);
			
//			GeneralTextEditorPanel.selectedFont = ParseFont(ControlDictionary["fontNameDisplayTextBox"].Text);
//
//			ControlDictionary["browseButton"].Click += new EventHandler(SelectFontEvent);
			UpdateFontPreviewLabel(this, EventArgs.Empty);
		}
		
		void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			if (e.Index >= 0) {
				FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
				SizeF size = e.Graphics.MeasureString(fontDescriptor.Name, comboBox.Font);
				e.ItemWidth  = (int)size.Width;
				e.ItemHeight = (int)comboBox.Font.Height;
			}
		}
		
		static StringFormat drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);
		static Font         boldComboBoxFont;
		
		void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			e.DrawBackground();
			if (e.Index >= 0) {
				FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
				Rectangle drawingRect = new Rectangle(e.Bounds.X,
				                                      e.Bounds.Y,
				                                      e.Bounds.Width,
				                                      e.Bounds.Height);
				
				Brush drawItemBrush = SystemBrushes.WindowText;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
					drawItemBrush = SystemBrushes.HighlightText;
				}
				
				e.Graphics.DrawString(fontDescriptor.Name,
				                      fontDescriptor.IsMonospaced ? boldComboBoxFont : comboBox.Font,
				                      drawItemBrush,
				                      drawingRect,
				                      drawStringFormat);
			}
			e.DrawFocusRectangle();
		}
		
		Font CurrentFont {
			get {
				int fontSize = 10;
				try {
					fontSize = Math.Max(6, Int32.Parse(ControlDictionary["fontSizeComboBox"].Text));
				} catch (Exception) {}
				
				FontDescriptor fontDescriptor = (FontDescriptor)((ComboBox)ControlDictionary["fontListComboBox"]).Items[((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex];
				
				return new Font(fontDescriptor.Name,
				                fontSize);
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			ControlDictionary["fontPreviewLabel"].Font = CurrentFont;
		}
		
		public override bool StorePanelContents()
		{
			((Properties)CustomizationObject).Set("DoubleBuffer",         ((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("UseAntiAliasFont",     ((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("MouseWheelTextZoom",   ((CheckBox)ControlDictionary["mouseWheelZoomCheckBox"]).Checked);
			//((Properties)CustomizationObject).Set("EnableCodeCompletion", ((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("EnableFolding",        ((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked);
			((Properties)CustomizationObject).Set("DefaultFont",          CurrentFont.ToString());
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
				return ResourceService.CourierNew10;
			}
		}
	}
}
