// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
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
				Font font = CurrentFont;
				if (font != null)
					return font.ToString();
				else
					return null;
			}
			set {
				CurrentFont = FontSelectionPanel.ParseFont(value);
			}
		}
		
		public Font CurrentFont {
			get {
				if (helper == null)
					return null;
				return helper.GetSelectedFont();
			}
			set {
				if (helper == null) {
					helper = new FontSelectionPanelHelper((ComboBox)ControlDictionary["fontSizeComboBox"], (ComboBox)ControlDictionary["fontListComboBox"], value);
					helper.StartThread();
					((ComboBox)ControlDictionary["fontListComboBox"]).MeasureItem += helper.MeasureComboBoxItem;
					((ComboBox)ControlDictionary["fontListComboBox"]).DrawItem += helper.ComboBoxDrawItem;
				} else {
					int index = 0;
					for (int i = 0; i < ((ComboBox)ControlDictionary["fontListComboBox"]).Items.Count; ++i) {
						FontSelectionPanelHelper.FontDescriptor descriptor = (FontSelectionPanelHelper.FontDescriptor)((ComboBox)ControlDictionary["fontListComboBox"]).Items[i];
						if (descriptor.Name == value.Name) {
							index = i;
						}
					}
					((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndex = index;
				}
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Text = value.Size.ToString();
			}
		}
		
		public FontSelectionPanel()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.FontSelectionPanel.xfrm"));

			for (int i = 6; i <= 24; ++i) {
				((ComboBox)ControlDictionary["fontSizeComboBox"]).Items.Add(i);
			}
			((ComboBox)ControlDictionary["fontSizeComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontSizeComboBox"]).Enabled = false;
			((ComboBox)ControlDictionary["fontListComboBox"]).Enabled = false;
			
			((ComboBox)ControlDictionary["fontListComboBox"]).TextChanged += new EventHandler(UpdateFontPreviewLabel);
			((ComboBox)ControlDictionary["fontListComboBox"]).SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
		}
		
		FontSelectionPanelHelper helper;
		
		public static Font ParseFont(string font)
		{
			try {
				string[] descr = font.Split(new char[]{',', '='});
				return new Font(descr[1], Single.Parse(descr[3]));
			} catch (Exception ex) {
				LoggingService.Warn(ex);
				return WinFormsResourceService.DefaultMonospacedFont;
			}
		}
		
		void UpdateFontPreviewLabel(object sender, EventArgs e)
		{
			helper.UpdateFontPreviewLabel(ControlDictionary["fontPreviewLabel"]);
		}
	}
	
	class FontSelectionPanelHelper
	{
		ComboBox fontSizeComboBox, fontListComboBox;
		Font defaultFont;
		
		public FontSelectionPanelHelper(ComboBox fontSizeComboBox, ComboBox fontListComboBox, Font defaultFont)
		{
			this.fontSizeComboBox = fontSizeComboBox;
			this.fontListComboBox = fontListComboBox;
			this.defaultFont = defaultFont;
			boldComboBoxFont = new Font(fontListComboBox.Font, FontStyle.Bold);
		}
		
		public void StartThread()
		{
			Thread thread = new Thread(DetectMonospacedThread);
			thread.IsBackground = true;
			thread.Start();
		}
		
		void DetectMonospacedThread()
		{
			Thread.Sleep(0); // first allow UI thread to do some work
			DebugTimer.Start();
			InstalledFontCollection installedFontCollection = new InstalledFontCollection();
			Font currentFont = defaultFont;
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
					fontSizeComboBox.Enabled = true;
					fontListComboBox.Enabled = true;
					fontListComboBox.SelectedIndex = index;
					fontSizeComboBox.Text = currentFont.Size.ToString();
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
		
		internal void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
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
		Font         boldComboBoxFont;
		
		internal void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			ComboBox comboBox = (ComboBox)sender;
			e.DrawBackground();
			
			Rectangle drawingRect = new Rectangle(e.Bounds.X,
			                                      e.Bounds.Y,
			                                      e.Bounds.Width,
			                                      e.Bounds.Height);
			
			Brush drawItemBrush = SystemBrushes.WindowText;
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
				drawItemBrush = SystemBrushes.HighlightText;
			}
			
			if (comboBox.Enabled == false) {
				e.Graphics.DrawString(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode"),
				                      comboBox.Font,
				                      drawItemBrush,
				                      drawingRect,
				                      drawStringFormat);
			} else if (e.Index >= 0) {
				FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
				e.Graphics.DrawString(fontDescriptor.Name,
				                      fontDescriptor.IsMonospaced ? boldComboBoxFont : comboBox.Font,
				                      drawItemBrush,
				                      drawingRect,
				                      drawStringFormat);
			}
			e.DrawFocusRectangle();
		}
		
		public Font GetSelectedFont()
		{
			if (!fontListComboBox.Enabled)
				return null;
			float fontSize = 10f;
			try {
				fontSize = Math.Max(6, Single.Parse(fontSizeComboBox.Text));
			} catch (Exception) {}
			
			FontDescriptor fontDescriptor = (FontDescriptor)fontListComboBox.Items[fontListComboBox.SelectedIndex];
			
			return new Font(fontDescriptor.Name,
			                fontSize);
		}
		
		public void UpdateFontPreviewLabel(Control fontPreviewLabel)
		{
			Font currentFont = GetSelectedFont();
			fontPreviewLabel.Visible = currentFont != null;
			if (currentFont != null) {
				fontPreviewLabel.Font = currentFont;
			}
		}
		
		public class FontDescriptor
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
	}
}
