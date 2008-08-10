// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HexEditor.View
{
	public class HexEditOptionsPanel : AbstractOptionPanel
	{
		ComboBox cmbForeColor;
		ListBox lstElements;
		CheckBox cbBold;
		CheckBox cbItalic;
		CheckBox cbUnderline;
		CheckBox cbFitToWidth;
		Label lblOffsetPreview;
		Label lblDataPreview;
		Button btnSelectFont;
		FontDialog fdSelectFont;
		TextBox txtExtensions;
		
		NumericUpDown nUDBytesPerLine;
		DomainUpDown dUDViewModes;
		
		List<Color> Colors;
		Color customFore = Color.Transparent;
		Color customBack = Color.Transparent;
		
		bool fcmanualchange = false;
		
		// New values
		Color OffsetForeColor;
		Color DataForeColor;
		
		bool OffsetBold, OffsetItalic, OffsetUnderline;
		bool DataBold, DataItalic, DataUnderline;
		
		float FontSize = 9.75f;
		
		string FontName;
		
		public HexEditOptionsPanel()
		{
			Colors = new List<Color>();
			
			ListColors();
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("HexEditor.Resources.HexEditOptions.xfrm"));
			this.InitializeComponents();
			string configpath = Path.GetDirectoryName(typeof(Editor).Assembly.Location) + Path.DirectorySeparatorChar + "config.xml";
			
			if (!File.Exists(configpath)) return;
			
			XmlDocument file = new XmlDocument();
			file.Load(configpath);
			
			foreach (XmlElement el in file.GetElementsByTagName("Setting")) {
				switch(el.GetAttribute("Name")) {
					case "OffsetFore" :
						OffsetForeColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
						break;
					case "DataFore" :
						DataForeColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
						break;
					case "OffsetStyle" :
						OffsetBold = bool.Parse(el.GetAttribute("Bold"));
						OffsetItalic = bool.Parse(el.GetAttribute("Italic"));
						OffsetUnderline = bool.Parse(el.GetAttribute("Underline"));
						break;
					case "DataStyle" :
						DataBold = bool.Parse(el.GetAttribute("Bold"));
						DataItalic = bool.Parse(el.GetAttribute("Italic"));
						DataUnderline = bool.Parse(el.GetAttribute("Underline"));
						break;
					case "Font" :
						FontName = el.GetAttribute("FontName");
						FontSize = float.Parse(el.GetAttribute("FontSize"));
						break;
					case "TextDisplay":
						this.cbFitToWidth.Checked = bool.Parse(el.GetAttribute("FitToWidth"));
						this.nUDBytesPerLine.Value = int.Parse(el.GetAttribute("BytesPerLine"));
						this.dUDViewModes.SelectedItem = ((HexEditor.Util.ViewMode)HexEditor.Util.ViewMode.Parse(typeof(HexEditor.Util.ViewMode),el.GetAttribute("ViewMode"))).ToString();
						break;
					case "FileTypes":
						txtExtensions.Text = el.GetAttribute("FileTypes");
						break;
				}
			}
			
			lblOffsetPreview.Font = new Font(FontName, FontSize);
			lblDataPreview.Font = new Font(FontName, FontSize);
			
			fdSelectFont.Font = new Font(FontName, FontSize);
			
			if (OffsetBold) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Bold);
			if (OffsetItalic) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Italic);
			if (OffsetUnderline) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Underline);
			
			if (DataBold) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Bold);
			if (DataItalic) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Italic);
			if (DataUnderline) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Underline);
			
			if (IsNamedColor(OffsetForeColor)) {
				OffsetForeColor = Color.FromName(GetColorName(OffsetForeColor));
			} else {
				customFore = OffsetForeColor;
			}
			
			if (IsNamedColor(DataForeColor)) {
				DataForeColor = Color.FromName(GetColorName(DataForeColor));
			} else {
				customFore = DataForeColor;
			}
			
			if (!OffsetForeColor.IsNamedColor) {
				cmbForeColor.SelectedIndex = 0;
			} else {
				cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(OffsetForeColor.Name);
			}
			
			this.cbBold.Checked = OffsetBold;
			this.cbItalic.Checked = OffsetItalic;
			this.cbUnderline.Checked = OffsetUnderline;
		}

		public override bool StorePanelContents()
		{
			string configpath = Path.GetDirectoryName(typeof(Editor).Assembly.Location) + Path.DirectorySeparatorChar + "config.xml";
			XmlDocument file = new XmlDocument();
			file.LoadXml("<Config></Config>");
			
			XmlElement el = file.CreateElement("Setting");
			el.SetAttribute("Name", "OffsetFore");
			el.SetAttribute("R", OffsetForeColor.R.ToString());
			el.SetAttribute("G", OffsetForeColor.G.ToString());
			el.SetAttribute("B", OffsetForeColor.B.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "DataFore");
			el.SetAttribute("R", DataForeColor.R.ToString());
			el.SetAttribute("G", DataForeColor.G.ToString());
			el.SetAttribute("B", DataForeColor.B.ToString());
			file.FirstChild.AppendChild(el);

			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "OffsetStyle");
			el.SetAttribute("Bold", OffsetBold.ToString());
			el.SetAttribute("Italic", OffsetItalic.ToString());
			el.SetAttribute("Underline", OffsetUnderline.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "DataStyle");
			el.SetAttribute("Bold", DataBold.ToString());
			el.SetAttribute("Italic", DataItalic.ToString());
			el.SetAttribute("Underline", DataUnderline.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "Font");
			el.SetAttribute("FontName", FontName);
			el.SetAttribute("FontSize", FontSize.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "TextDisplay");
			el.SetAttribute("FitToWidth", this.cbFitToWidth.Checked.ToString());
			el.SetAttribute("BytesPerLine", this.nUDBytesPerLine.Value.ToString());
			if (this.dUDViewModes.SelectedIndex == -1)
				el.SetAttribute("ViewMode", HexEditor.Util.ViewMode.Hexadecimal.ToString());
			else
				el.SetAttribute("ViewMode", this.dUDViewModes.SelectedItem.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "FileTypes");
			el.SetAttribute("FileTypes", txtExtensions.Text);
			file.FirstChild.AppendChild(el);
			
			file.Save(configpath);
			return true;
		}

		void InitializeComponents()
		{
			cmbForeColor = (ComboBox)ControlDictionary["cmbForeColor"];
			lstElements = (ListBox)ControlDictionary["lstElements"];
			cbBold = (CheckBox)ControlDictionary["cbBold"];
			cbItalic = (CheckBox)ControlDictionary["cbItalic"];
			cbUnderline = (CheckBox)ControlDictionary["cbUnderline"];
			lblOffsetPreview = (Label)ControlDictionary["lblOffsetPreview"];
			lblDataPreview = (Label)ControlDictionary["lblDataPreview"];
			btnSelectFont = (Button)ControlDictionary["btnSelectFont"];
			
			nUDBytesPerLine = (NumericUpDown)ControlDictionary["nUDBytesPerLine"];
			dUDViewModes = (DomainUpDown)ControlDictionary["dUDViewModes"];
			cbFitToWidth = (CheckBox)ControlDictionary["cbFitToWidth"];
			
			txtExtensions = (TextBox)ControlDictionary["txtExtensions"];
			
			fdSelectFont = new FontDialog();
			
			// Initialize FontDialog
			fdSelectFont.FontMustExist = true;
			fdSelectFont.FixedPitchOnly = true;
			fdSelectFont.ShowEffects = false;
			fdSelectFont.ShowColor = false;
			
			cmbForeColor.Items.Add("Custom");
			
			foreach (Color c in Colors) {
				cmbForeColor.Items.Add(c.Name);
			}
			
			lstElements.Items.Add("Offset");
			lstElements.Items.Add("Data");

			lstElements.SetSelected(0, true);
			
			foreach (string s in HexEditor.Util.ViewMode.GetNames(typeof(HexEditor.Util.ViewMode)))
			{
				dUDViewModes.Items.Add(s);
			}
			
			btnSelectFont.Click += new EventHandler(this.btnSelectFontClick);
			cmbForeColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbForeColorDrawItem);
			cmbForeColor.SelectedValueChanged += new EventHandler(this.cmbForeColorSelectedValueChanged);
			
			cmbForeColor.DropDown += cmbForeColorDropDown;
			
			cbBold.CheckedChanged += new EventHandler(this.cbBoldCheckedChanged);
			cbItalic.CheckedChanged += new EventHandler(this.cbItalicCheckedChanged);
			cbUnderline.CheckedChanged += new EventHandler(this.cbUnderlineCheckedChanged);
			
			lstElements.SelectedValueChanged += new EventHandler(this.lstElementsSelectedValueChanged);
		}
		
		void ListColors()
		{
			foreach (Color c in System.ComponentModel.TypeDescriptor.GetConverter(typeof (Color)).GetStandardValues()) {
				Colors.Add(c);
			}
		}
		
		void btnSelectFontClick(object sender, EventArgs e)
		{
			if (fdSelectFont.ShowDialog() == DialogResult.OK) {
				lblOffsetPreview.Font = new Font(fdSelectFont.Font, FontStyle.Regular);
				lblDataPreview.Font = new Font(fdSelectFont.Font, FontStyle.Regular);
				
				FontName = fdSelectFont.Font.Name;
				FontSize = fdSelectFont.Font.Size;
				
				if (OffsetBold) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Bold);
				if (OffsetItalic) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Italic);
				if (OffsetUnderline) lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Underline);
				
				if (DataBold) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Bold);
				if (DataItalic) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Italic);
				if (DataUnderline) lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Underline);
			}
		}
		
		void cmbForeColorSelectedValueChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				if (cmbForeColor.SelectedIndex == 0) {
					if (fcmanualchange) {
						ColorDialog cdColor = new ColorDialog();
						if (cdColor.ShowDialog() == DialogResult.OK) {
							customFore = cdColor.Color;
							switch (lstElements.SelectedIndex) {
								case 0 :
									OffsetForeColor = customFore;
									break;
								case 1 :
									DataForeColor = customFore;
									break;
							}
						}
					}
				} else {
					if (cmbForeColor.SelectedIndex == -1) cmbForeColor.SelectedIndex = 0;
					switch (lstElements.SelectedIndex) {
						case 0 :
							OffsetForeColor = Color.FromName(cmbForeColor.Items[cmbForeColor.SelectedIndex].ToString());
							break;
						case 1 :
							DataForeColor = Color.FromName(cmbForeColor.Items[cmbForeColor.SelectedIndex].ToString());
							break;
					}
				}
				
				lblOffsetPreview.ForeColor = OffsetForeColor;
				
				lblDataPreview.ForeColor = DataForeColor;
			} else {
				MessageService.ShowError("Please select an element first!");
			}
			fcmanualchange = false;
		}
		
		void cmbForeColorDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			Rectangle rc = new Rectangle(e.Bounds.X, e.Bounds.Y,
			                             e.Bounds.Width, e.Bounds.Height);
			Rectangle rc2 = new Rectangle(e.Bounds.X + 20, e.Bounds.Y,
			                              e.Bounds.Width, e.Bounds.Height);
			Rectangle rc3 = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, 10, 10);
			
			string str;
			Color color;
			if (e.Index != -1) {
				str = (string)cmbForeColor.Items[e.Index];
			} else {
				str = (string)cmbForeColor.Items[0];
			}
			
			if (str == "Custom") {
				color = customFore;
			} else {
				color = Color.FromName(str);
			}

			if ( e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect)) {
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight) , rc);
				e.Graphics.DrawString( str , cmbForeColor.Font, new SolidBrush(Color.White), rc2);
				e.Graphics.FillRectangle(new SolidBrush(color), rc3);
				e.Graphics.DrawRectangle(Pens.White, rc3);
			} else {
				e.Graphics.FillRectangle(new SolidBrush(Color.White) , rc);
				e.Graphics.DrawString( str , cmbForeColor.Font, new SolidBrush(Color.Black), rc2);
				e.Graphics.FillRectangle(new SolidBrush(color), rc3);
				e.Graphics.DrawRectangle(Pens.Black, rc3);
			}
		}
		
		void cbBoldCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						OffsetBold = cbBold.Checked;
						if ((cbBold.Checked & !lblOffsetPreview.Font.Bold) ^ (!cbBold.Checked & lblOffsetPreview.Font.Bold))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Bold);
						break;
					case 1 :
						DataBold = cbBold.Checked;
						if ((cbBold.Checked & !lblDataPreview.Font.Bold) ^ (!cbBold.Checked & lblDataPreview.Font.Bold))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Bold);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void cbItalicCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						OffsetItalic = cbItalic.Checked;
						if ((cbItalic.Checked & !lblOffsetPreview.Font.Italic) || (!cbItalic.Checked & lblOffsetPreview.Font.Italic))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Italic);
						break;
					case 1 :
						DataItalic = cbItalic.Checked;
						if ((cbItalic.Checked & !lblDataPreview.Font.Italic) || (!cbItalic.Checked & lblDataPreview.Font.Italic))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Italic);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void cbUnderlineCheckedChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				switch (lstElements.SelectedIndex) {
					case 0 :
						OffsetUnderline = cbUnderline.Checked;
						if ((cbUnderline.Checked & !lblOffsetPreview.Font.Underline) || (!cbUnderline.Checked & lblOffsetPreview.Font.Underline))
							lblOffsetPreview.Font = new Font(lblOffsetPreview.Font, lblOffsetPreview.Font.Style ^ FontStyle.Underline);
						break;
					case 1 :
						DataUnderline = cbUnderline.Checked;
						if ((cbUnderline.Checked & !lblDataPreview.Font.Underline) || (!cbUnderline.Checked & lblDataPreview.Font.Underline))
							lblDataPreview.Font = new Font(lblDataPreview.Font, lblDataPreview.Font.Style ^ FontStyle.Underline);
						break;
				}
			} else {
				MessageService.ShowError("Please select an element first!");
			}
		}
		
		void lstElementsSelectedValueChanged(object sender, EventArgs e)
		{
			switch (lstElements.SelectedIndex) {
				case 0 :
					cbBold.Checked = OffsetBold;
					cbItalic.Checked = OffsetItalic;
					cbUnderline.Checked = OffsetUnderline;

					if (OffsetForeColor == customFore) {
						cmbForeColor.SelectedIndex = 0;
					} else {
						cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(OffsetForeColor.Name);
					}
					break;
				case 1 :
					cbBold.Checked = DataBold;
					cbItalic.Checked = DataItalic;
					cbUnderline.Checked = DataUnderline;

					if (DataForeColor == customFore) {
						cmbForeColor.SelectedIndex = 0;
					} else {
						cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(DataForeColor.Name);
					}
					break;
			}
		}
		
		void cmbForeColorDropDown(object sender, EventArgs e)
		{
			fcmanualchange = true;
		}
		
		bool IsNamedColor(Color color)
		{
			foreach (Color c in Colors) {
				if (c.A == color.A) {
					if (c.R == color.R) {
						if (c.G == color.G) {
							if (c.B == color.B) {
								return true;
							}
						}
					}
				}
			}
			
			return false;
		}
		
		string GetColorName(Color color)
		{
			if (IsNamedColor(color)) {
				foreach (Color c in Colors) {
					if (c.A == color.A) {
						if (c.R == color.R) {
							if (c.G == color.G) {
								if (c.B == color.B) {
									return c.Name;
								}
							}
						}
					}
				}
			}
			return String.Empty;
		}
	}
}
