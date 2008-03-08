// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace HexEditor.View
{
	public class HexEditOptionsPanel : AbstractOptionPanel
	{
		ComboBox cmbForeColor;
		ComboBox cmbBackColor;
		ListBox lstElements;
		CheckBox cbBold;
		CheckBox cbItalic;
		CheckBox cbUnderline;
		Label lblOffsetPreview;
		Label lblDataPreview;
		Button btnSelectFont;
		FontDialog fdSelectFont;
		
		List<Color> Colors;
		Color customFore = Color.Transparent;
		Color customBack = Color.Transparent;
		
		bool fcmanualchange = false;
		bool bcmanualchange = false;
		
		// New values
		Color OffsetForeColor, OffsetBackColor;
		Color DataForeColor, DataBackColor;
		
		bool OffsetBold, OffsetItalic, OffsetUnderline;
		bool DataBold, DataItalic, DataUnderline;
		
		float FontSize = 10.0f;
		
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
			string configpath = Path.GetDirectoryName(typeof(HexEditControl).Assembly.Location) + Path.DirectorySeparatorChar + "config.xml";
			
			if (!File.Exists(configpath)) return;
			
			XmlDocument file = new XmlDocument();
			file.Load(configpath);
			
			foreach (XmlElement el in file.GetElementsByTagName("Setting")) {
				switch(el.GetAttribute("Name")) {
					case "OffsetFore" :
						OffsetForeColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
						break;
					case "OffsetBack" :
						OffsetBackColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
						break;
					case "DataFore" :
						DataForeColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
						break;
					case "DataBack" :
						DataBackColor = Color.FromArgb(int.Parse(el.GetAttribute("R")), int.Parse(el.GetAttribute("G")), int.Parse(el.GetAttribute("B")));
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
			
			if (IsNamedColor(OffsetBackColor)) {
				OffsetBackColor = Color.FromName(GetColorName(OffsetBackColor));
			} else {
				customBack = OffsetBackColor;
			}
			
			if (IsNamedColor(DataForeColor)) {
				DataForeColor = Color.FromName(GetColorName(DataForeColor));
			} else {
				customFore = DataForeColor;
			}
			
			if (IsNamedColor(DataBackColor)) {
				DataBackColor = Color.FromName(GetColorName(DataBackColor));
			} else {
				customBack = DataBackColor;
			}
			
			if (!OffsetForeColor.IsNamedColor) {
				cmbForeColor.SelectedIndex = 0;
			} else {
				cmbForeColor.SelectedIndex = cmbForeColor.Items.IndexOf(OffsetForeColor.Name);
			}
			
			if (OffsetBackColor.IsNamedColor) {
				cmbBackColor.SelectedIndex = 0;
			} else {
				cmbBackColor.SelectedIndex = cmbBackColor.Items.IndexOf(OffsetBackColor.Name);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Single.ToString")]
		public override bool StorePanelContents()
		{
			string configpath = Path.GetDirectoryName(typeof(HexEditControl).Assembly.Location) + Path.DirectorySeparatorChar + "config.xml";
			XmlDocument file = new XmlDocument();
			file.LoadXml("<Config></Config>");
			
			XmlElement el = file.CreateElement("Setting");
			el.SetAttribute("Name", "OffsetFore");
			el.SetAttribute("R", OffsetForeColor.R.ToString());
			el.SetAttribute("G", OffsetForeColor.G.ToString());
			el.SetAttribute("B", OffsetForeColor.B.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "OffsetBack");
			el.SetAttribute("R", OffsetBackColor.R.ToString());
			el.SetAttribute("G", OffsetBackColor.G.ToString());
			el.SetAttribute("B", OffsetBackColor.B.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "DataFore");
			el.SetAttribute("R", DataForeColor.R.ToString());
			el.SetAttribute("G", DataForeColor.G.ToString());
			el.SetAttribute("B", DataForeColor.B.ToString());
			file.FirstChild.AppendChild(el);
			
			el = file.CreateElement("Setting");
			el.SetAttribute("Name", "DataBack");
			el.SetAttribute("R", DataBackColor.R.ToString());
			el.SetAttribute("G", DataBackColor.G.ToString());
			el.SetAttribute("B", DataBackColor.B.ToString());
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
			
			file.Save(configpath);
			return true;
		}

		void InitializeComponents()
		{
			cmbForeColor = (ComboBox)ControlDictionary["cmbForeColor"];
			cmbBackColor = (ComboBox)ControlDictionary["cmbBackColor"];
			lstElements = (ListBox)ControlDictionary["lstElements"];
			cbBold = (CheckBox)ControlDictionary["cbBold"];
			cbItalic = (CheckBox)ControlDictionary["cbItalic"];
			cbUnderline = (CheckBox)ControlDictionary["cbUnderline"];
			lblOffsetPreview = (Label)ControlDictionary["lblOffsetPreview"];
			lblDataPreview = (Label)ControlDictionary["lblDataPreview"];
			btnSelectFont = (Button)ControlDictionary["btnSelectFont"];
			fdSelectFont = new FontDialog();
			
			// Initialize FontDialog
			fdSelectFont.FontMustExist = true;
			fdSelectFont.FixedPitchOnly = true;
			fdSelectFont.ShowEffects = false;
			fdSelectFont.ShowColor = false;
			
			cmbForeColor.Items.Add("Custom");
			cmbBackColor.Items.Add("Custom");
			
			foreach (Color c in Colors) {
				cmbForeColor.Items.Add(c.Name);
				cmbBackColor.Items.Add(c.Name);
			}
			
			lstElements.Items.Add("Offset");
			lstElements.Items.Add("Data");

			lstElements.SetSelected(0, true);
			
			btnSelectFont.Click += new EventHandler(this.btnSelectFontClick);
			cmbForeColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbForeColorDrawItem);
			cmbForeColor.SelectedValueChanged += new EventHandler(this.cmbForeColorSelectedValueChanged);
			
			cmbForeColor.DropDown += cmbForeColorDropDown;
			cmbBackColor.DropDown += cmbBackColorDropDown;
			
			cmbBackColor.DrawItem += new DrawItemEventHandler(this.cmbBackColorDrawItem);
			cmbBackColor.SelectedValueChanged += new EventHandler(this.cmbBackColorSelectedValueChanged);
			
			
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
				lblOffsetPreview.BackColor = OffsetBackColor;
				
				lblDataPreview.ForeColor = DataForeColor;
				lblDataPreview.BackColor = DataBackColor;
			} else {
				MessageService.ShowError("Please select an element first!");
			}
			fcmanualchange = false;
		}
		
		void cmbBackColorSelectedValueChanged(object sender, EventArgs e)
		{
			if (lstElements.SelectedIndex != -1) {
				if (cmbBackColor.SelectedIndex == 0) {
					if (bcmanualchange) {
						ColorDialog cdColor = new ColorDialog();
						if (cdColor.ShowDialog() == DialogResult.OK) {
							customBack = cdColor.Color;
							switch (lstElements.SelectedIndex)
							{
								case 0 :
									OffsetBackColor = customBack;
									break;
								case 1 :
									DataBackColor = customBack;
									break;
							}
						}
					}
				} else {
					if (cmbBackColor.SelectedIndex == -1) cmbBackColor.SelectedIndex = 0;
					switch (lstElements.SelectedIndex) {
						case 0 :
							OffsetBackColor = Color.FromName(cmbBackColor.Items[cmbBackColor.SelectedIndex].ToString());
							break;
						case 1 :
							DataBackColor = Color.FromName(cmbBackColor.Items[cmbBackColor.SelectedIndex].ToString());
							break;
					}
				}
				
				lblOffsetPreview.ForeColor = OffsetForeColor;
				lblOffsetPreview.BackColor = OffsetBackColor;
				
				lblDataPreview.ForeColor = DataForeColor;
				lblDataPreview.BackColor = DataBackColor;
			} else {
				MessageService.ShowError("Please select an element first!");
			}
			bcmanualchange = false;
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
		
		void cmbBackColorDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			Rectangle rc = new Rectangle(e.Bounds.X, e.Bounds.Y,
			                             e.Bounds.Width, e.Bounds.Height);
			Rectangle rc2 = new Rectangle(e.Bounds.X + 20, e.Bounds.Y,
			                              e.Bounds.Width, e.Bounds.Height);
			Rectangle rc3 = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, 10, 10);
			
			string str;
			Color color;
			if (e.Index != -1) {
				str = (string)cmbBackColor.Items[e.Index];
			} else {
				str = (string)cmbBackColor.Items[0];
			}
			
			if (str == "Custom") {
				color = customBack;
			} else {
				color = Color.FromName(str);
			}
			
			if ( e.State == (DrawItemState.Selected | DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect)) {
				e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight) , rc);
				e.Graphics.DrawString(str , cmbBackColor.Font, new SolidBrush(Color.White), rc2);
				e.Graphics.FillRectangle(new SolidBrush(color), rc3);
				e.Graphics.DrawRectangle(Pens.White, rc3);
			} else {
				e.Graphics.FillRectangle(new SolidBrush(Color.White), rc);
				e.Graphics.DrawString(str , cmbBackColor.Font, new SolidBrush(Color.Black), rc2);
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
					if (OffsetBackColor == customBack) {
						cmbBackColor.SelectedIndex = 0;
					} else {
						cmbBackColor.SelectedIndex = cmbBackColor.Items.IndexOf(OffsetBackColor.Name);
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
					if (DataBackColor == customBack) {
						cmbBackColor.SelectedIndex = 0;
					} else {
						cmbBackColor.SelectedIndex = cmbBackColor.Items.IndexOf(DataBackColor.Name);
					}
					break;
			}
		}
		
		void cmbForeColorDropDown(object sender, EventArgs e)
		{
			fcmanualchange = true;
		}
		
		void cmbBackColorDropDown(object sender, EventArgs e)
		{
			bcmanualchange = true;
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
