// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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
			FontFamily fontFamily;
			internal string Name;
			internal bool IsMonospaced;
			
			public FontDescriptor(FontFamily fontFamily)
			{
				this.fontFamily = fontFamily;
				this.Name = fontFamily.Name;
			}
			
			internal void DetectMonospaced(Graphics g)
			{
				this.IsMonospaced = DetectMonospaced(g, fontFamily);
			}
			
			static bool DetectMonospaced(Graphics g, FontFamily fontFamily)
			{
				using (Font f = new Font(fontFamily, 10)) {
					// determine if the length of i == m because I see no other way of
					// getting if a font is monospaced or not.
					int w1 = TextRenderer.MeasureText("i.", f).Width;
					int w2 = TextRenderer.MeasureText("mw", f).Width;
					return w1 == w2;
				}
			}
		}
		
		ComboBox fontListComboBox, fontSizeComboBox;
		
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
			fontListComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			fontListComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			
			boldComboBoxFont = new Font(ControlDictionary["fontListComboBox"].Font, FontStyle.Bold);
			
//			GeneralTextEditorPanel.selectedFont = ParseFont(ControlDictionary["fontNameDisplayTextBox"].Text);
//
//			ControlDictionary["browseButton"].Click += new EventHandler(SelectFontEvent);
			UpdateFontPreviewLabel(null, null);
			
			Thread thread = new Thread(DetectMonospacedThread);
			thread.IsBackground = true;
			thread.Start();
		}
		
		void DetectMonospacedThread()
		{
			DebugTimer.Start();
			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			Font currentFont = ParseFont(((Properties)CustomizationObject).Get("DefaultFont", ResourceService.DefaultMonospacedFont.ToString()).ToString());
			List<FontDescriptor> fonts = new List<FontDescriptor>();
			
			int index = 0;
			foreach (FontFamily fontFamily in installedFontCollection.Families) {
				if (fontFamily.IsStyleAvailable(FontStyle.Regular) && fontFamily.IsStyleAvailable(FontStyle.Bold)  && fontFamily.IsStyleAvailable(FontStyle.Italic)) {
					if (fontFamily.Name == currentFont.Name) {
						index = fonts.Count;
					}
					fonts.Add(new FontDescriptor(fontFamily));
				}
			}
			DebugTimer.Stop("Getting installed fonts");
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					fontListComboBox.Items.AddRange(fonts.ToArray());
					fontListComboBox.SelectedIndex = index;
					fontListComboBox.Enabled = true;
					fontSizeComboBox.Text = currentFont.Size.ToString();
					fontSizeComboBox.Enabled = true;
				});
			DebugTimer.Start();
			using (Bitmap newBitmap = new Bitmap(1, 1)) {
				using (Graphics g  = Graphics.FromImage(newBitmap)) {
					foreach (FontDescriptor fd in fonts) {
						fd.DetectMonospaced(g);
					}
				}
			}
			DebugTimer.Stop("Detect Monospaced");
			fontListComboBox.Invalidate();
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
				if (!fontListComboBox.Enabled)
					return null;
				int fontSize = 10;
				try {
					fontSize = Math.Max(6, Int32.Parse(ControlDictionary["fontSizeComboBox"].Text));
				} catch (Exception) {}
				
				FontDescriptor fontDescriptor = (FontDescriptor)fontListComboBox.Items[fontListComboBox.SelectedIndex];
				
				return new Font(fontDescriptor.Name,
				                fontSize);
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			Font currentFont = CurrentFont;
			ControlDictionary["fontPreviewLabel"].Visible = currentFont != null;
			if (currentFont != null) {
				ControlDictionary["fontPreviewLabel"].Font = currentFont;
			}
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
