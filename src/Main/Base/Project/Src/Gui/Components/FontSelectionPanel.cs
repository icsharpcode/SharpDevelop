using System;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Description of FontSelectionPanel.
	/// </summary>
	public class FontSelectionPanel : BaseSharpDevelopUserControl
	{
		public string CurrentFontString {
			get {
				return CurrentFont.ToString();
			}
			set {
				CurrentFont = FontSelectionPanel.ParseFont(value);
			}
		}
		
		public Font CurrentFont {
			get {
				int fontSize = 10;
				try {
					fontSize = Math.Max(6, Int32.Parse(ControlDictionary["fontSizeComboBox"].Text));
				} catch (Exception) {}
				
				int index = ((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex;
				if (index < 0) {
					return Font;
				}
				FontDescriptor fontDescriptor = (FontDescriptor)((ComboBox)ControlDictionary["fontListComboBox"]).Items[index];
				
				return new Font(fontDescriptor.Name,
				                fontSize);
			}
			set {
				int index = 0;
				for (int i = 0; i < ((ComboBox)ControlDictionary["fontListComboBox"]).Items.Count; ++i) {
					FontDescriptor descriptor = (FontDescriptor)((ComboBox)ControlDictionary["fontListComboBox"]).Items[i];
					if (descriptor.Name == value.Name) {
						index = i;
					}
				}
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Text = value.Size.ToString();
				((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex = index;
				UpdateFontPreviewLabel(this, EventArgs.Empty);
			}
		}
		
		public FontSelectionPanel()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.FontSelectionPanel.xfrm"));

			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			
			for (int i = 6; i <= 24; ++i) {
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Items.Add(i);
			}
			((ComboBox)ControlDictionary["fontSizeComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			foreach (FontFamily fontFamily in installedFontCollection.Families) {
				if (fontFamily.IsStyleAvailable(FontStyle.Regular) && fontFamily.IsStyleAvailable(FontStyle.Bold) && fontFamily.IsStyleAvailable(FontStyle.Italic)) {
					((ComboBox)ControlDictionary["fontListComboBox"]).Items.Add(new FontDescriptor(fontFamily));
				}
			}
			
			((ComboBox)ControlDictionary["fontListComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
			((ComboBox)ControlDictionary["fontListComboBox"]).DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
			
			boldComboBoxFont = new Font(ControlDictionary["fontListComboBox"].Font, FontStyle.Bold);
			
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
		
	
		public static Font ParseFont(string font)
		{
			try {
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			} catch (Exception) {
				return new Font("Courier New", 10);
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
		
		class FontDescriptor
		{
			FontFamily fontFamily;
			bool       isMonospaced = false;
			bool       initializedMonospace = false;
			public string Name {
				get {
					return fontFamily.Name;
				}
			}
			
			public bool IsMonospaced {
				get {
					if (!initializedMonospace) {
						isMonospaced = GetIsMonospaced(fontFamily);
					}
					return isMonospaced;
				}
			}
			
			bool GetIsMonospaced(FontFamily fontFamily)
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
			
			public FontDescriptor(FontFamily fontFamily)
			{
				this.fontFamily = fontFamily;
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			ControlDictionary["fontPreviewLabel"].Font = CurrentFont;
		}
	}
}
